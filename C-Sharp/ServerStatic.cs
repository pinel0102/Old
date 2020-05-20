using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System;
using uLink;

/// <summary>
/// 서버용 static 클래스.
/// </summary>
static class ServerStatic
{
	static bool showLog = false;

	/// <summary>
	/// 클라이언트가 서버에 연결할때 서버로 보내는 디바이스 정보.
	/// </summary>
	/// <param name="playerName"></param>
	/// <returns></returns>
	static public string[] uLinkLoginDataSet(string playerName)
	{
		string[] loginData = new string[1];

		string GUID = null;

#if UNITY_STANDALONE || UNITY_EDITOR
		GUID = SystemInfo.deviceUniqueIdentifier + UnityEngine.Network.player.guid;
#else
		GUID = SystemInfo.deviceUniqueIdentifier;
#endif

		loginData[0] = GUID;
		
		return loginData;
	}

	

	/// <summary>
	/// 경험치를 얻고 레벨업을 합니다. 레벨은 <see cref="NetworkPacket.maxLevel"/> 까지 올라갑니다.
	/// </summary>
	/// <param name="AUID"></param>
	/// <param name="userLevel"></param>
	/// <param name="userExp"></param>
	/// <param name="getExp"></param>
	/// <returns></returns>
	static public bool GetExpAndLevelUp(int AUID, ref int userLevel, ref int userExp, int getExp)
	{
		if (userLevel == NetworkPacket.maxLevel)
			return false;
		
		// 경험치를 얻고.
		//int newLevel = userLevel;
		int newExp = userExp + getExp;

		Debug.Log(CodeManager.GetMethodName() + "AUID : " + AUID + " / Level : " + userLevel + " / Exp : " + userExp + " + " + getExp);

		CSVReader tbuser = new CSVReader("TbUser.txt");

		string[] arr2 = tbuser.FindJSONDataArray(null, null, true);

		bool levelup = false;

		// 레벨업을 하고.
		while (newExp >= int.Parse(ReadJSONData.ReadLine(arr2[userLevel - 1])[5]))
		{
			//Level UP
			newExp -= int.Parse(ReadJSONData.ReadLine(arr2[userLevel - 1])[5]);

			if (userLevel < NetworkPacket.maxLevel)
				userLevel++;

			levelup = true;

			Debug.Log(CodeManager.GetMethodName() + "AUID : " + AUID + " / Level : " + userLevel + " / Exp : " + newExp + " / Level UP !!");

			if (userLevel == NetworkPacket.maxLevel)
			{
				break;
			}
		}

		if (userLevel == NetworkPacket.maxLevel)
		{
			Debug.Log(CodeManager.GetMethodName() + "AUID : " + AUID + " / 만렙을 달성했다!!");
			userExp = 0;
		}
		else
		{
			userExp = newExp;
		}

		if (levelup)
		{
			//////////////////////////////////////////////////////////////
			// 업적을 갱신한다.
			UpdateAchievementWithNewValue(AUID, userLevel, "Achievement_00015", "Achievement_00016", "Achievement_00017", "Achievement_00018", "Achievement_00019");
		}

		return levelup;
	}

	

	/// <summary>
	/// 일일퀘스트를 갱신합니다.
	/// </summary>
	/// <param name="AUID"></param>
	/// <param name="QuestCode1"></param>
	/// <param name="QuestCode2">여러 개를 갱신할 때 입력.</param>
	/// <param name="QuestCode3">여러 개를 갱신할 때 입력.</param>
	static public void UpdateDailyQuest(int AUID, string QuestCode1, string QuestCode2 = null, string QuestCode3 = null)
	{
		int maxQuest = 3;

		string[] QuestCode = new string[maxQuest];
		int[] oldCount = new int[maxQuest];
		int[] newCount = new int[maxQuest];
		int[] cutLine = new int[maxQuest];
		int[] Complete = new int[maxQuest];
		int index = 0;
				
		string SQL = string.Format("select * from UserDailyQuest where AUID='{0}' and (QuestCode='{1}' or QuestCode='{2}' or QuestCode='{3}')", AUID, QuestCode1, QuestCode2, QuestCode3);
		SqlDataReader reader = SQLManager.ExecuteReader(SQL);

		CSVReader excel = null;
		
		if (reader.HasRows)
		{
			while (reader.Read())
			{
				QuestCode[index] = reader["QuestCode"].ToString();
				Complete[index] = int.Parse(reader["Complete"].ToString());

				if (Complete[index] == 0)
				{
					if(excel == null)
						excel = new CSVReader("TbQuest.txt");
					
					oldCount[index] = int.Parse(reader["Count"].ToString());
					newCount[index] = oldCount[index] + 1;
										
					cutLine[index] = int.Parse(ReadJSONData.ReadLine(excel.FindJSONData("QuestCode", QuestCode[index]))[3]);

					//Debug.Log(CodeManager.GetMethodName() + AUID + " : " + QuestCode[index]);
				}
								
				index++;				
			}			
		}
		reader.Close();

		for (int i = 0; i < index; i++)
		{
			if (Complete[i] == 0)
			{
				if (newCount[i] >= cutLine[i])
				{
					newCount[i] = cutLine[i];

					SQL = string.Format("update UserDailyQuest set Count='{0}', Complete='1' where AUID='{1}' and QuestCode='{2}'", newCount[i], AUID, QuestCode[i]);
					reader = SQLManager.ExecuteReader(SQL);

					reader.Close();

					SQL = string.Format("insert into UserDailyQuestCompleteNotice values ('{0}','{1}');", AUID, QuestCode[i]);
					reader = SQLManager.ExecuteReader(SQL);

					reader.Close();

					if (showLog) Debug.Log(CodeManager.GetMethodName() + AUID + " : " + QuestCode[i] + " 가 갱신되어서 커트라인을 달성했다.");
				}
				else
				{
					SQL = string.Format("update UserDailyQuest set Count='{0}' where AUID='{1}' and QuestCode='{2}'", newCount[i], AUID, QuestCode[i]);
					reader = SQLManager.ExecuteReader(SQL);

					reader.Close();

					if (showLog) Debug.Log(CodeManager.GetMethodName() + AUID + " : " + QuestCode[i] + " 가 갱신되었다.");
				}				
			}
			else
			{
				if (showLog) Debug.Log(CodeManager.GetMethodName() + AUID + " : " + QuestCode[i] + " 는 이미 커트라인을 달성했다.");
			}
		}
	}


	/// <summary>
	/// 업적을 갱신합니다. (실행할 때마다 count가 1개씩 증가).
	/// </summary>
	/// <param name="AUID"></param>
	/// <param name="AchievementCode1"></param>
	/// <param name="AchievementCode2">여러 개를 갱신할 때 입력.</param>
	/// <param name="AchievementCode3">여러 개를 갱신할 때 입력.</param>
	static public void UpdateAchievement(int AUID, string AchievementCode1, string AchievementCode2 = null, string AchievementCode3 = null, string AchievementCode4 = null, string AchievementCode5 = null)
	{
		int maxAchievement = 5;

		string[] AchievementCode = new string[maxAchievement];
		int[] oldCount = new int[maxAchievement];
		int[] newCount = new int[maxAchievement];
		int[] cutLine = new int[maxAchievement];
		int[] Complete = new int[maxAchievement];
		int index = 0;

		string SQL = string.Format("select * from UserAchievement where AUID='{0}' and (AchievementCode='{1}' or AchievementCode='{2}' or AchievementCode='{3}' or AchievementCode='{4}' or AchievementCode='{5}')", 
			AUID, AchievementCode1, AchievementCode2, AchievementCode3, AchievementCode4, AchievementCode5);
		SqlDataReader reader = SQLManager.ExecuteReader(SQL);

		CSVReader excel = null;

		if (reader.HasRows)
		{
			while (reader.Read())
			{
				AchievementCode[index] = reader["AchievementCode"].ToString();
				Complete[index] = int.Parse(reader["Complete"].ToString());

				if (Complete[index] == 0)
				{
					if (excel == null)
						excel = new CSVReader("TbAchievement.txt");

					oldCount[index] = int.Parse(reader["Count"].ToString());
					newCount[index] = oldCount[index] + 1;

					cutLine[index] = int.Parse(ReadJSONData.ReadLine(excel.FindJSONData("AchievementCode", AchievementCode[index]))[3]);

					Debug.Log(CodeManager.GetMethodName() + AUID + " : " + AchievementCode[index]);
				}

				index++;
			}
		}
		reader.Close();

		for (int i = 0; i < index; i++)
		{
			if (Complete[i] == 0)
			{
				if (newCount[i] >= cutLine[i])
				{
					newCount[i] = cutLine[i];

					SQL = string.Format("update UserAchievement set Count='{0}', Complete='1' where AUID='{1}' and AchievementCode='{2}'", newCount[i], AUID, AchievementCode[i]);
					reader = SQLManager.ExecuteReader(SQL);

					reader.Close();

					SQL = string.Format("insert into UserAchievementCompleteNotice values ('{0}','{1}');", AUID, AchievementCode[i]);
					reader = SQLManager.ExecuteReader(SQL);

					reader.Close();

					if (showLog) Debug.Log(CodeManager.GetMethodName() + AUID + " : " + AchievementCode[i] + " 가 갱신되어서 커트라인을 달성했다.");
				}
				else
				{
					SQL = string.Format("update UserAchievement set Count='{0}' where AUID='{1}' and AchievementCode='{2}'", newCount[i], AUID, AchievementCode[i]);
					reader = SQLManager.ExecuteReader(SQL);

					reader.Close();

					if (showLog) Debug.Log(CodeManager.GetMethodName() + AUID + " : " + AchievementCode[i] + " 가 갱신되었다.");
				}
			}
			else
			{
				if (showLog) Debug.Log(CodeManager.GetMethodName() + AUID + " : " + AchievementCode[i] + " 는 이미 커트라인을 달성했다.");
			}
		}
	}


	/// <summary>
	/// 업적을 갱신합니다. (현재 수치와 커트라인을 비교).
	/// </summary>
	/// <param name="AUID"></param>
	/// <param name="AchievementCode1"></param>
	/// <param name="AchievementCode2">여러 개를 갱신할 때 입력.</param>
	/// <param name="AchievementCode3">여러 개를 갱신할 때 입력.</param>
	static public void UpdateAchievementWithNewValue(int AUID, int newValue, string AchievementCode1, string AchievementCode2 = null, string AchievementCode3 = null, string AchievementCode4 = null, string AchievementCode5 = null)
	{
		int maxAchievement = 5;

		string[] AchievementCode = new string[maxAchievement];
		int[] oldCount = new int[maxAchievement];
		int[] newCount = new int[maxAchievement];
		int[] cutLine = new int[maxAchievement];
		int[] Complete = new int[maxAchievement];
		int index = 0;

		string SQL = string.Format("select * from UserAchievement where AUID='{0}' and (AchievementCode='{1}' or AchievementCode='{2}' or AchievementCode='{3}' or AchievementCode='{4}' or AchievementCode='{5}')",
			AUID, AchievementCode1, AchievementCode2, AchievementCode3, AchievementCode4, AchievementCode5);
		SqlDataReader reader = SQLManager.ExecuteReader(SQL);

		CSVReader excel = null;

		if (reader.HasRows)
		{
			while (reader.Read())
			{
				AchievementCode[index] = reader["AchievementCode"].ToString();
				Complete[index] = int.Parse(reader["Complete"].ToString());

				if (Complete[index] == 0)
				{
					if (excel == null)
						excel = new CSVReader("TbAchievement.txt");

					oldCount[index] = int.Parse(reader["Count"].ToString());
					newCount[index] = newValue;

					cutLine[index] = int.Parse(ReadJSONData.ReadLine(excel.FindJSONData("AchievementCode", AchievementCode[index]))[3]);

					Debug.Log(CodeManager.GetMethodName() + AUID + " : " + AchievementCode[index]);
				}

				index++;
			}
		}
		reader.Close();

		for (int i = 0; i < index; i++)
		{
			if (Complete[i] == 0)
			{
				if (newCount[i] >= cutLine[i])
				{
					newCount[i] = cutLine[i];

					SQL = string.Format("update UserAchievement set Count='{0}', Complete='1' where AUID='{1}' and AchievementCode='{2}'", newCount[i], AUID, AchievementCode[i]);
					reader = SQLManager.ExecuteReader(SQL);

					reader.Close();

					SQL = string.Format("insert into UserAchievementCompleteNotice values ('{0}','{1}');", AUID, AchievementCode[i]);
					reader = SQLManager.ExecuteReader(SQL);

					reader.Close();

					if (showLog) Debug.Log(CodeManager.GetMethodName() + AUID + " : " + AchievementCode[i] + " 가 갱신되어서 커트라인을 달성했다.");
				}
				else
				{
					SQL = string.Format("update UserAchievement set Count='{0}' where AUID='{1}' and AchievementCode='{2}'", newCount[i], AUID, AchievementCode[i]);
					reader = SQLManager.ExecuteReader(SQL);

					reader.Close();

					if (showLog) Debug.Log(CodeManager.GetMethodName() + AUID + " : " + AchievementCode[i] + " 가 갱신되었다.");
				}
			}
			else
			{
				if (showLog) Debug.Log(CodeManager.GetMethodName() + AUID + " : " + AchievementCode[i] + " 는 이미 커트라인을 달성했다.");
			}
		}
	}
}