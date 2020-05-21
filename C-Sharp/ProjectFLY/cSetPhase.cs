using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class cSetPhase
{
	/// <summary>
	/// startHP에 대해서 역순으로 정렬을 한 후 startHP값이 다를 경우에 phaseNumber를 1씩 증가시킨다.
	/// </summary>
	/// <param name="shotCtrl"></param>
	public static void SetPhaseNumber(UbhShotCtrl shotCtrl)
	{
		Dictionary<int, float> dic = new Dictionary<int, float>();

		for (int i = 0; i < shotCtrl._ShotList.Count; i++)
		{
			dic.Add(i, 100f - shotCtrl._ShotList[i].startHP);
		}

		List<KeyValuePair<int, float>> list = dic.OrderBy(ph => ph.Value).ToList();

		int phaseNumber = 1;

		for (int i = 0; i < list.Count; i++)
		{
			if (i > 0 && list[i].Value == list[i - 1].Value)
			{
				phaseNumber--;
			}

			shotCtrl._ShotList[list[i].Key].phaseNumber = phaseNumber;

			//Debug.Log(list[i] + " / " + phaseNumber);

			phaseNumber++;
		}
	}

	/// <summary>
	/// HP 백분율에 따라 페이즈를 추가한다.
	/// </summary>
	/// <param name="shotCtrl"></param>
	/// <param name="isActive"></param>
	/// <param name="currentPhase"></param>
	/// <param name="HPCurrent"></param>
	/// <param name="HPMaximum"></param>
	/// <param name="objectName"></param>
	/// <returns>HP 백분율.</returns>
	public static float PhaseUpdate(UbhShotCtrl shotCtrl, bool isActive, int currentPhase, float HPCurrent, float HPMaximum, string objectName = null)
	{
		bool showLogs = false;

		//if (showLogs) Debug.Log(objectName + " : " + CodeManager.GetMethodCall(true));

		float currentPercent = (HPCurrent / HPMaximum) * 100f;
		int lastPhase = 0;

		foreach (UbhShotCtrl.ShotInfo sh in shotCtrl._ShotList.OrderBy(ph => ph.phaseNumber).ToList())
		{
			if (currentPercent <= sh.startHP && currentPercent > sh.endHP)
			{
				if (!sh.activePhase)
				{
					sh.activePhase = true;
					currentPhase = sh.phaseNumber;

					//if (showLogs) Debug.Log(objectName + " / " + currentPhase + " / " + lastPhase + " / " + isActive);

					if (lastPhase != currentPhase)
					{
						if (isActive)
						{
							if (objectName != null)
							{
								if (showLogs) Debug.Log(CodeManager.GetMethodName(true) + objectName + " Active Phase : " + currentPhase);
							}

							ChangePhase(shotCtrl);
						}
					}

					lastPhase = currentPhase;
				}
			}
			else
			{
				if (sh.activePhase)
				{
					sh.activePhase = false;
				}
			}
		}

		return currentPercent;
	}

	/// <summary>
	/// 샷 코루틴을 재실행.
	/// </summary>
	/// <param name="shotCtrl"></param>
	public static void ChangePhase(UbhShotCtrl shotCtrl)
	{
		shotCtrl.StopShotRoutine();
		shotCtrl.StartShotCoroutineIE();
	}

}
