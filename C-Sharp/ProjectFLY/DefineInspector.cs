using UnityEngine;
using System.Collections;

/// <summary>
/// 전역 상수 모음.
/// </summary>
static class Define
{
	public const string S00_SCENE_MANAGER	= "00.Scene.Manager";
	public const string S00_SCENE_PROGRESS	= "00.Scene.Progress";
	public const string S10_SCENE_LOGO		= "10.Scene.Logo";
	public const string S11_SCENE_UPDATE	= "11.Scene.Update";
	public const string S12_SCENE_LOAD		= "12.Scene.Loading";
	public const string S13_SCENE_TITLE		= "13.Scene.Title";
	public const string S20_SCENE_REGISTER	= "20.Scene.Register";
	public const string S30_SCENE_LOBBY		= "30.Scene.Lobby";
	public const string S40_SCENE_INFINITE	= "40.Scene.Infinite";
	public const string S41_SCENE_SCENARIO	= "41.Scene.Scenario";
	public const string S42_SCENE_MATCHING	= "42.Scene.Matching";
	public const string S43_SCENE_SHOP		= "43.Scene.Shop";
	public const string S50_SCENE_PLAY		= "50.Scene.Play";
	public const string S60_SCENE_RESULT	= "60.Scene.Result";

	public const string MUSIC_NOSOUND		= "NoSound";
	public const string MUSIC_TITLE			= "bg01_Title";
	public const string MUSIC_LOBBY			= "bg02_Lobby";

	public const string SOUND_NOSOUND		= "NoSound";
	public const string SOUND_SELECT		= "sfx01_Select";
	public const string SOUND_ERROR			= "sfx02_Error";
	public const string SOUND_ENABLE		= "sfx03_Enable";
	public const string SOUND_BACK		= "sfx04_Disable";	

	public const string MSG_VERSION_CHECK		= "버전 정보를 확인 중입니다...";
	public const string MSG_VERSION_NEEDUPDATE	= "최신 버전으로 업데이트 해주세요";
	public const string MSG_UPDATE_CHECK		= "업데이트를 확인 중입니다...";
	public const string MSG_UPDATE_DOWNLOAD		= "업데이트를 받는 중입니다...";
	public const string MSG_UPDATE_COMPLETE		= "업데이트가 완료되었습니다.";
	public const string MSG_RESOURCE_CHECK		= "오빠를 깨우고 있습니다...";
	public const string MSG_RESOURCE_LOADING	= "오빠에게 베개를 던지고 있습니다...";
	public const string MSG_RESOURCE_COMPLETE	= "오빠의 준비가 끝났습니다.";
	public const string MSG_TOUCH_TO_START		= "화면을 터치하세요";
	public const string MSG_PLAY_LOADING		= "비행을 준비하고 있습니다...";

	public const int STATE_DEFAULT = 1000;
	public const int STATE_GAME_READY = 7000;
	public const int STATE_GAME_START = 7001;
	public const int STATE_LOADING_START = 9000;
	public const int STATE_LOADING_END = 9001;

	/// <summary>
	/// "user_data.sav"
	/// </summary>
	public const string FILE_USERDATA = "user_data.sav";

	/// <summary>
	/// "Home/user_data.txt"
	/// </summary>
	public const string FILE_USERDATA_REF = "Home/user_data.txt";


	public static Vector2 TouchLockOutPos = new Vector2(-720f, 0f);

	public const int STATUS_LIVE = 1;
	public const int STATUS_DEAD = 2;



	public const int CHAR_NUM_BOY = 1000;
	public const int CHAR_NUM_GIRL = 2000;
	public const int CHAR_NUM_BOSS = 3000;
	public const int CHAR_NUM_ZAKO_A = 4000;
	public const int CHAR_NUM_ZAKO_B = 5000;
	public const int CHAR_NUM_ZAKO_C = 6000;

	
	public const int SHOT = 0;
	public const int ROCKET = 0;


	public const int GRID_NUM = 10;	

	/// <summary>
	/// Random Seed 를 재설정.
	/// </summary>
	public static void RefreshRandomSeed()
	{
		Random.seed = System.DateTime.Now.Millisecond;
	}
}


/// <summary>
/// <para>Define 의 전역 상수를 인스펙터에 표시. (유니티 전용).</para>
/// <para>등록 : [ReadOnly] public string AAA = cDefine.AAA;</para>
/// <para>상수가 변경되었을 경우 인스펙터에서 [우클릭 - Reset] 또는 [Reload] 버튼을 누르면 갱신된다.</para>
/// <para>필요한 파일: </para>
/// <para>[ReadOnly] 속성을 사용하려면 <see cref="ReadOnlyAttribute"/>.cs 파일이 있어야 한다.</para>
/// <para>[ReadOnly] 속성을 인스펙터에 표시하려면 [Assets/Editor] 폴더에 <see cref="ReadOnlyDrawer"/>.cs 파일이 있어야 한다.</para>
/// <para>[Reload] 버튼을 사용하려면 [Assets/Editor] 폴더에 <see cref="cDefineInspectorGUI"/>.cs 파일이 있어야 한다.</para>
/// </summary>
public class DefineInspector : MonoBehaviour
{
	// Draw Inspector
	[ReadOnly]	public string S00_SCENE_MANAGER		= Define.S00_SCENE_MANAGER;
	[ReadOnly]	public string S00_SCENE_PROGRESS	= Define.S00_SCENE_PROGRESS;
	[ReadOnly]	public string S10_SCENE_LOGO		= Define.S10_SCENE_LOGO;
	[ReadOnly]	public string S11_SCENE_UPDATE		= Define.S11_SCENE_UPDATE;
	[ReadOnly]	public string S12_SCENE_LOAD		= Define.S12_SCENE_LOAD;
	[ReadOnly]	public string S13_SCENE_TITLE		= Define.S13_SCENE_TITLE;
	[ReadOnly]	public string S20_SCENE_REGISTER	= Define.S20_SCENE_REGISTER;
	[ReadOnly]	public string S30_SCENE_LOBBY		= Define.S30_SCENE_LOBBY;
	[ReadOnly]	public string S40_SCENE_INFINITE	= Define.S40_SCENE_INFINITE;
	[ReadOnly]	public string S41_SCENE_SCENARIO	= Define.S41_SCENE_SCENARIO;
	[ReadOnly]	public string S42_SCENE_MATCHING	= Define.S42_SCENE_MATCHING;
	[ReadOnly]	public string S43_SCENE_SHOP		= Define.S43_SCENE_SHOP;
	[ReadOnly]	public string S50_SCENE_PLAY		= Define.S50_SCENE_PLAY;
	[ReadOnly]	public string S60_SCENE_PLAY		= Define.S60_SCENE_RESULT;

	[ReadOnly]	public string FILE_USERDATA = Define.FILE_USERDATA;
	[ReadOnly]	public string FILE_USERDATA_REF = Define.FILE_USERDATA_REF;
	[ReadOnly]	public int STATUS_LIVE = Define.STATUS_LIVE;
	[ReadOnly]	public int STATUS_DEAD = Define.STATUS_DEAD;
	
}


