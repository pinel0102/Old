using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System;
using uLink;

/// <summary>
/// 클라이언트의 요청에 의해 시간 경과에 따른 유저의 행동력 변화를 갱신하는 클래스.
/// </summary>
public class Receiver_EnergyTime : uLink.MonoBehaviour
{
	bool showJsonLog = false;

	public uLinkNetworkView view;

	public double lastTime = -1;
	public DateTime refreshTime;

	public int MaxEnergy = 999;
	public int EnergyTime = 360000;
	public int cutLine = 50;

	public bool isEnergyUp = false;
	public int finalEnergy = -1;

	void Awake()
	{
		view = GetComponent<uLinkNetworkView>();
	}
	
	/// <summary>
	/// 에너지 변경으로 인한 시간 계산을 시작하거나 끝낸다.
	/// </summary>
	/// <param name="AUID"></param>
	/// <param name="isLevelup"></param>
	/// <param name="userLevel"></param>
	/// <param name="oldEnergy"></param>
	/// <param name="newEnergy"></param>
	public void EnergyChanged(int AUID, bool isLevelup, int userLevel, int oldEnergy, ref int newEnergy)
	{
		cutLine = userLevel + 49;		

		//SQLManager.DBOpen();

		string SQL = string.Format("select * from Setting");
		SqlDataReader reader = SQLManager.ExecuteReader(SQL);

		if (reader.HasRows)
		{
			reader.Read();
			MaxEnergy = int.Parse(reader["MaxEnergy"].ToString());
			EnergyTime = int.Parse(reader["EnergyTime"].ToString());
		}

		reader.Close();

		SQL = string.Format("select * from UserEnergyTime where AUID='{0}'", AUID);
		reader = SQLManager.ExecuteReader(SQL);

		if (reader.HasRows)
		{
			reader.Read();
			lastTime = double.Parse(reader["LastTime"].ToString());
		}

		reader.Close();

		if (oldEnergy > newEnergy)
		{
			// 에너지가 감소했다.
			if (showJsonLog)
				Debug.Log(CodeManager.GetMethodName() + "에너지가 감소했다.");

			if (isLevelup)
			{
				// 레벨업을 했는데 에너지가 커트라인보다 낮으면 커트라인까지 회복시킨다.
				if (newEnergy < cutLine)
					newEnergy = cutLine;

				// 레벨업을 했는데 초기 에너지가 커트라인보다 높으면 유지시킨다.
				if (oldEnergy > cutLine)
					newEnergy = oldEnergy;
			}

			if (newEnergy >= cutLine)
			{
				// 최종 에너지가 커트라인 이상이면 계산을 중지한다.
				// 레벨업을 했으면 여기로 온다.
				lastTime = -1;
			}
			else
			{
				if (oldEnergy >= cutLine)
				{
					// 이제부터 계산을 시작한다.
					lastTime = EnergyTime;

					SQL = string.Format("update UserEnergyTime set LastTime='{0}', RefreshTime=convert(varchar(30), getdate(), 121) where AUID='{1}'", lastTime, AUID);
					SQLManager.ExecuteNonQuery(SQL);

					if (showJsonLog)
						Debug.Log(CodeManager.GetMethodName() + oldEnergy + " -> " + newEnergy + " / lastTime : " + lastTime);
				}
			}
		}
		else if (oldEnergy < newEnergy)
		{
			// 에너지가 증가했다.
			if (showJsonLog)
				Debug.Log(CodeManager.GetMethodName() + "에너지가 증가했다.");

			if (newEnergy > MaxEnergy)
				newEnergy = MaxEnergy;

			if (newEnergy >= cutLine)
			{
				// 최종 에너지가 커트라인 이상이면 계산을 중지한다.
				lastTime = -1;

				SQL = string.Format("update UserEnergyTime set LastTime='{0}' where AUID='{1}'", lastTime, AUID);
				SQLManager.ExecuteNonQuery(SQL);

				if (showJsonLog)
					Debug.Log(CodeManager.GetMethodName() + oldEnergy + " -> " + newEnergy + " / lastTime : " + lastTime);
			}
		}

		//SQLManager.DBClose();		
	}


	/// <summary>
	/// 에너지의 자연 회복을 갱신한다.
	/// </summary>
	/// <param name="AUID"></param>
	public void RefreshEnergy(int AUID)
	{
		isEnergyUp = false;
		finalEnergy = -1;

		int userLevel = -1;
		int userEnergy = -1;

		string SQL = string.Format("select * from UserInfo where AUID='{0}'", AUID);
		SqlDataReader reader = SQLManager.ExecuteReader(SQL);

		if (reader.HasRows)
		{
			reader.Read();

			userLevel = int.Parse(reader["Level"].ToString());
			userEnergy = int.Parse(reader["Energy"].ToString());
		}

		reader.Close();

		cutLine = userLevel + 49;

		SQL = string.Format("select * from Setting");
		reader = SQLManager.ExecuteReader(SQL);

		if (reader.HasRows)
		{
			reader.Read();
			MaxEnergy = int.Parse(reader["MaxEnergy"].ToString());
			EnergyTime = int.Parse(reader["EnergyTime"].ToString());
		}

		reader.Close();

		SQL = "select convert(varchar(30), getdate(), 121) as currentTime";
		reader = SQLManager.ExecuteReader(SQL);
		reader.Read();

		// 현재 시각.
		DateTime cTime = Convert.ToDateTime(reader["currentTime"].ToString());

		reader.Close();

		SQL = string.Format("select * from UserEnergyTime where AUID='{0}'", AUID);
		reader = SQLManager.ExecuteReader(SQL);

		if (reader.HasRows)
		{
			reader.Read();

			// 1 회복까지 남은 시간.
			lastTime = double.Parse(reader["LastTime"].ToString());

			// 마지막으로 에너지가 변경된 시각.
			refreshTime = Convert.ToDateTime(reader["RefreshTime"].ToString());

			reader.Close();

			// 현재 시각 - 마지막으로 에너지가 변경된 시각 : 회복을 적용해야 하는 시간.
			TimeSpan ss = cTime.Subtract(refreshTime);
			double ts = ss.TotalMilliseconds;

			if (showJsonLog)
				Debug.Log(CodeManager.GetMethodName() + AUID + " : " + ts / 1000 + " / refreshTime : " + refreshTime);

			if (ts < EnergyTime)
			{
				// 회복은 없다.
				if (userEnergy < userLevel + 49)
				{
					lastTime = EnergyTime - ts;

					SQL = string.Format("update UserEnergyTime set LastTime='{0}' where AUID='{1}'", lastTime - lastTime % 1, AUID);
					SQLManager.ExecuteNonQuery(SQL);

					if (showJsonLog)
						Debug.Log(CodeManager.GetMethodName() + AUID + " : " + "1 회복까지 앞으로 " + ((lastTime - lastTime % 1) / 1000) + " 초.");
				}
			}
			else
			{
				// 에너지를 회복시킨다.
				bool isIncreaseEnergy = false;

				while (ts >= EnergyTime)
				{
					ts -= EnergyTime;

					if (userEnergy < userLevel + 49)
					{
						userEnergy++;

						isIncreaseEnergy = true;
						isEnergyUp = true;

						if (showJsonLog)
							Debug.Log(CodeManager.GetMethodName() + AUID + " : " + "에너지 1 회복 : " + userEnergy + " / " + ts / 1000);
					}

					if (userEnergy >= userLevel + 49)
					{
						ts = -1;

						break;
					}
				}

				if (isIncreaseEnergy)
				{
					SQL = string.Format("update UserInfo set Energy='{0}' where AUID='{1}'", userEnergy, AUID);
					SQLManager.ExecuteNonQuery(SQL);
					
					if (ts > -1)
					{
						lastTime = EnergyTime - ts;

						if (showJsonLog)
							Debug.Log(CodeManager.GetMethodName() + AUID + " : " + "1 회복까지 앞으로 " + ((lastTime - lastTime % 1) / 1000) + " 초.");
					}
					else
					{
						lastTime = -1;

						if (showJsonLog)
							Debug.Log(CodeManager.GetMethodName() + AUID + " : " + "에너지가 만땅이 되었다!!");
					}
					//getdate() - (EnergyTime - lastTime)
					// DATEADD(millisecond, {1}, getdate())
					// RefreshTime 은 현재 시각이 아니라 마지막으로 회복이 되는 시점을 넣어야 한다.
					SQL = string.Format("update UserEnergyTime set LastTime='{0}', RefreshTime=convert(varchar(30), DATEADD(millisecond, {1}, getdate()), 121) where AUID='{2}'", lastTime - lastTime % 1, -(EnergyTime-lastTime) , AUID);
					//SQL = string.Format("update UserEnergyTime set LastTime='{0}', RefreshTime=convert(varchar(30), getdate(), 121) where AUID='{1}'", lastTime - lastTime % 1, AUID);
					SQLManager.ExecuteNonQuery(SQL);
				}
				else
				{
					// 이미 만땅이라서 회복이 되지 않았다.
					SQL = string.Format("update UserEnergyTime set LastTime='{0}' where AUID='{1}'", -1, AUID);
					SQLManager.ExecuteNonQuery(SQL);

					if (showJsonLog)
						Debug.Log(CodeManager.GetMethodName() + AUID + " : " + "이미 만땅이라서 회복이 되지 않았다.");
				}
			}
		}

		finalEnergy = userEnergy;

		reader.Close();
	}

	[RPC]
	void GetEnergyTime(string[] data, string GUID, uLink.NetworkMessageInfo info)
	{
		if (GetComponent<Receiver_Account>().DuplicateLogin(data[0], GUID, info))
			return;

		int AUID = int.Parse(data[0]);		

		//타입을 설정합니다.
		GetEnergyTimeRes res = new GetEnergyTimeRes();
		ProtocolType type = ProtocolType.GetEnergyTime;

		if (int.Parse(data[0]) <= 0)
		{
			res.ret = GetEnergyTimeStatus.NO_AUID;
			uLink.BitStream bitstream_noauid = new uLink.BitStream(false);
			bitstream_noauid.Write(res);
			if (info.sender.isConnected)
				view.RPC("ReceiveResponse", info.sender, type, bitstream_noauid);

			return;
		}



		SQLManager.DBOpen();
		
		RefreshEnergy(AUID);

		res.isEnergyUp = isEnergyUp;
		res.userEnergy = finalEnergy;
		res.MaxEnergy = MaxEnergy;
		res.EnergyTime = EnergyTime;
		res.cutLine = cutLine;		
		res.lastTime = (int) lastTime;

		res.ret = GetEnergyTimeStatus.SUCCESS;


		/////////////////////////////////////////////////////////////////////////////

		// 이하는 동일합니다.
		if (showJsonLog)
			Debug.Log(CodeManager.GetFunctionName() + "AUID: " + AUID + " (" + res.ret + ")");

		uLink.BitStream bitstream = new uLink.BitStream(false);
		bitstream.Write(res);
		if (info.sender.isConnected)
			view.RPC("ReceiveResponse", info.sender, type, bitstream);

		SQLManager.DBClose();
	}

}