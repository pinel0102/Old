using UnityEngine;
using System.Collections;

/// <summary>
/// 클라이언트에서 서버에 요청을 보내고 받은 응답을 처리하는 방법에 대한 예제.
/// </summary>
public class ServerPreset : MonoBehaviour
{
	NetworkClient Network;
		
	void Start () {
				
		Network = GameObject.Find("NetworkClient").GetComponent<NetworkClient>();
		
		StartCoroutine(GetItemInfo("ID", "7"));
		
	}
		
	/// <summary>
	/// 서버 요청 테스트 함수. 서버에 있는 csv 테이블에서 section 의 값이 value 인 행을 가져옵니다.
	/// <para>서버 응답 종류에 대해서는 <see cref="ProtocolType"/> 참조.</para>
	/// </summary>
	/// <param name="section">찾을 섹션 이름.</param>
	/// <param name="value">찾을 값.</param>
	/// <returns></returns>
	IEnumerator GetItemInfo(string section, string value)
	{		
		bool Success = false;
				
		// NetworkClient.cs 안의 함수를 실행시키면 해당 함수에서 서버에 요청합니다.
		Network.GetTbItemInfo(section, value);

		// 서버에서 응답이 올 때까지 대기합니다.
		while (true)
		{
			yield return null;
			
			// 타임아웃 수치. 요청을 보내면 자동으로 초기화 되므로 건드리지 않아도 됩니다.
			NetworkPacket.timeOut--;

			// 각 상황에 따라 분류합니다. 무한루프 방지를 위해 가장 마지막에 타임아웃 처리를 포함시킵니다.
			// xxxxRes 와 xxxxStatus 는 함수마다 다르니 변경에 주의해야 합니다.
			if (NetworkPacket.getTbItemInfoRes.ret == GetJSONDataFromCSVStatus.SUCCESS)
			{
				Success = true;

				Debug.Log(CodeManager.GetFunctionName() + "아이템을 찾았습니다: (" + NetworkPacket.getTbItemInfoRes.ret + ")");
				break;
			}
			else if (NetworkPacket.getTbItemInfoRes.ret == GetJSONDataFromCSVStatus.NOT_EXISTS)
			{
				Debug.LogWarning(CodeManager.GetFunctionName() + "해당 아이템은 존재하지 않습니다: (" + NetworkPacket.getTbItemInfoRes.ret + ")");
				break;
			}
			else if (NetworkPacket.timeOut < 0)
			{
				NetworkPacket.getTbItemInfoRes.ret = GetJSONDataFromCSVStatus.TIME_OUT;
				Debug.LogWarning(CodeManager.GetFunctionName() + "대기 시간이 초과되었습니다: (" + NetworkPacket.getTbItemInfoRes.ret + ")");
				break;
			}			
		}

		// 성공시 처리.
		if (Success)
		{
			Debug.Log(NetworkPacket.getTbItemInfoRes.jsonData);

			yield break;
		}		

		// 실패시 처리.
		yield return null;
	}
}
