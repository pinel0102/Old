using UnityEngine;
using System.Collections;

public class JWObjectManager : SingletonMono<JWObjectManager> 
{

#region PARAMETERS

	static Vector2 iniPos = new Vector2(512, 0);
	static Vector2 iniPosM = new Vector2(-512, 0);
	bool exit_repeat = true;

#endregion



	public IEnumerator LoadScene(string SceneName)
	{
		if(JWGlobal.Instance.state != JWDefine.LOADING_START)
		{
			//Debug.Log ("======================== Loading Start ========================");

			JWGlobal.Instance.state = JWDefine.LOADING_START;

			Application.LoadLevelAdditiveAsync ("99.Scene.Loading");

			AsyncOperation async = Application.LoadLevelAsync ( SceneName );

			JWGlobal.Instance.lineprogress = 0;

			while(async.isDone == false)
			{
				JWGlobal.Instance.lineprogress = async.progress;		

				//Debug.Log ((JWGlobal.Instance.lineprogress*100f) + "%");

				yield return true;
			}
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if(JWGlobal.Instance.popup_state != 0)
			{
				if(JWGlobal.Instance.popup_state == JWDefine.SHOWHELP)
				{
					GameObject.Find("Popup_Text").transform.localPosition = Vector2.zero;
					JWGlobal.Instance.popup_state = JWDefine.MENU;
					GameObject.Find("DrawPopup").GetComponent<PopupDraw> ().releasePrefabfromOuter();
				}
				else if(Application.loadedLevelName == "20.Scene.Title")
				{
					if(exit_repeat)
					{
						// Close Popup

						GameObject.Find("Buttons_Moving").transform.localPosition = Vector2.zero;
						GameObject.Find("Popup_Text").transform.localPosition = Vector2.zero;
						GameObject.Find("DrawPopup").transform.localPosition = iniPos;
						GameObject.FindWithTag("POPUP").GetComponent<PopupDraw>().Initialize();

						JWGlobal.Instance.popup_state = 0;
						
						try{
							GameObject.Find("Shop").GetComponent<BoxCollider>().enabled = true;
						}catch{}
							
						GameObject.Find ("Play").GetComponent<TweenAlpha>().ResetToBeginning();
						GameObject.Find ("Play").GetComponent<TweenAlpha>().Play(true);
												
						JWGlobal.Instance.state = JWGlobal.Instance.before_state;
						GameObject.Find("DrawPopup").GetComponent<PopupDraw> ().releasePrefabfromOuter();

						foreach(GameObject ga in GameObject.Find("DrawPopup/Menu_Sub").GetComponent<PopupSubManager>().ViewObjectsSub)
						{
							ga.SetActive(false);
						}
					}
				}
				else
				{
					// Close Popup

					GameObject.FindWithTag("POPUP").GetComponent<PopupDraw>().Initialize();
					GameObject.Find("Buttons_Moving").transform.localPosition = Vector2.zero;
					GameObject.Find("Popup_Text").transform.localPosition = Vector2.zero;
					GameObject.Find("DrawPopup").transform.localPosition = iniPos;

					JWGlobal.Instance.popup_state = 0;

					if(Application.loadedLevelName != "60.Scene.Play")
					{
						try{
							GameObject.Find("Shop").GetComponent<BoxCollider>().enabled = true;
						}catch{}
					}

					JWGlobal.Instance.state = JWGlobal.Instance.before_state;
					GameObject.Find("DrawPopup").GetComponent<PopupDraw> ().releasePrefabfromOuter();

					foreach(GameObject ga in GameObject.Find("DrawPopup/Menu_Sub").GetComponent<PopupSubManager>().ViewObjectsSub)
					{
						ga.SetActive(false);
					}
				}
			}
			else
			{
				switch(Application.loadedLevelName)
				{
				case "20.Scene.Title":

						//팝업.
						GameObject.Find("Buttons_Moving").transform.localPosition = iniPos;
						GameObject.Find("DrawPopup").transform.localPosition = Vector2.zero;
						JWGlobal.Instance.popup_state = JWDefine.MENU;
						JWGlobal.Instance.state = JWDefine.PAUSE;

						GameObject.Find("Shop").GetComponent<BoxCollider>().enabled = false;

						//EXIT 활성.
						JWGlobal.Instance.popup_state = JWDefine.EXIT;
						
						PopupDraw popupdraw = GameObject.Find("DrawPopup").GetComponent<PopupDraw> ();
						popupdraw.DrawSub (JWGlobal.Instance.popup_state);
						
						PopupSubManager popupsub = GameObject.Find("Menu_Sub").GetComponent<PopupSubManager> ();
						popupsub.SetSubMenu();
					

					break;

				case "60.Scene.Play":

					if(JWGlobal.Instance.start_stage == false)
					{
						if(JWGlobal.Instance.result_state == 0)
						{
							GameObject.Find("Buttons_Moving").transform.localPosition = iniPos;
							GameObject.Find("DrawPopup").transform.localPosition = Vector2.zero;
							
							if(JWGlobal.Instance.state == JWDefine.PLAY)
							{
								if(JWGlobal.Instance.what_skill_on != JWDefine.NO_SKILL)
								{
									JWGlobal.Instance.what_skill_on = JWDefine.NO_SKILL;
									JWGlobal.Instance.skill_fire = false;
									JWGlobal.Instance.skill_lightning = false;
									JWGlobal.Instance.skill_poison = false;
									JWGlobal.Instance.skill_ready = false;
									JWGlobal.Instance.skill_activate = false;
									JWGlobal.Instance.level = 0;
									JWGlobal.Instance.cost = 0;	
									JWGlobal.Instance.skill_push = false;
									JWGlobal.Instance.skill_move = false;
									JWGlobal.Instance.skill_move0 = false;
									
									GameObject.FindWithTag ("PLAY").transform.FindChild("Down/UI_InterfaceBar").GetComponent<Play_Update_InterfaceBar>().UpdateSkillIcon();
									GameObject.FindWithTag ("PLAY").transform.FindChild("Down/UI_InterfaceBar/CostArea").GetComponent<Play_DrawCostArea> ().DrawCost();
									GameObject.FindWithTag ("PLAY").transform.FindChild("Touch").GetComponent<Play_TouchPoint> ().UpdateRangePosition();									
								}
								
								GameObject.FindWithTag ("PLAY").transform.FindChild("Objects/Base").GetComponent<Play_DrawBase> ().CancelAllSelect();
								GameObject.FindWithTag ("PLAY").transform.FindChild("Objects/Base").GetComponent<Play_DrawBase> ().UpdateBase();
								
								JWGlobal.Instance.ptick0 = JWGlobal.Instance.GetTicks();
							}
							else
							{
								try{
									GameObject.Find("Shop").GetComponent<BoxCollider>().enabled = false;
								}catch{}
							}
							
							JWGlobal.Instance.before_state = JWGlobal.Instance.state;
							JWGlobal.Instance.state = JWDefine.PAUSE;
							JWGlobal.Instance.popup_state = JWDefine.MENU;


							//EXIT 활성.
							JWGlobal.Instance.popup_state = JWDefine.EXIT;
							
							GameObject.Find("DrawPopup").GetComponent<PopupDraw> ().DrawSub (JWGlobal.Instance.popup_state);
							GameObject.Find("Menu_Sub").GetComponent<PopupSubManager> ().SetSubMenu();
						}
						else
						{
							if(JWGlobal.Instance.state == JWDefine.SKILLSHOP)
							{
								JWGlobal.Instance.state = JWGlobal.Instance.before_state;

								GameObject.FindWithTag("PLAY").transform.localPosition = Vector3.zero;
								GameObject.FindWithTag("RESULT").transform.localPosition = Vector3.zero;

								Destroy(GameObject.FindWithTag("SHOP"));

								GameObject.FindWithTag("PLAY").transform.FindChild("Down/UI_InterfaceBar").GetComponent<Play_Update_InterfaceBar>().SetSkillIcon();
								
								AudioManager.Instance.ChangeBGM("b_game_loc"+JWGlobal.Instance.realm_num); 
							}
							else
							{
								JWGlobal.Instance.play_select = true;
								JWGlobal.Instance.cheat = false;
								//JWGlobal.Instance.stage_num = 0;
								//JWGlobal.Instance.mission_num = 0;
								JWGlobal.Instance.result_state = 0;
								
								if(JWGlobal.Instance.realm_num == 5)
									AudioManager.Instance.ChangeBGM("b_select_ex");
								else
									AudioManager.Instance.ChangeBGM("b_select");
								
								JWGlobal.Instance.state = JWDefine.SELECT;

								GameObject.FindWithTag ("RESULT").transform.localPosition = iniPosM;

								try{
									GameObject.FindWithTag("TOUCHLOCK").transform.localPosition = Vector2.zero;
								}
								catch{}
								
								JWObjectManager.Instance.StartCoroutine ("LoadScene", "50.Scene.Select");
							}
						}
					}
					else
					{
						GameObject.FindWithTag ("START").GetComponent<Play_DrawStart>().GameStart();
					}
					break;
				case "30.Scene.Shop":
					//이전 씬.
					try{
						GameObject.FindWithTag("TOUCHLOCK").transform.localPosition = Vector2.zero;
					}
					catch{}

					switch(PlayerPrefs.GetString("LastLevelLoaded"))
					{
					case "20.Scene.Title":
						JWGlobal.Instance.state = JWDefine.TITLE;
						AudioManager.Instance.ChangeBGM("b_worldmap"); break;
					case "40.Scene.Main": 
						JWGlobal.Instance.state = JWDefine.MAIN;
						AudioManager.Instance.ChangeBGM("b_worldmap"); break;
					case "50.Scene.Select": 
						JWGlobal.Instance.state = JWDefine.SELECT;
						AudioManager.Instance.ChangeBGM("b_select"); break;
					case "60.Scene.Play": 
						JWGlobal.Instance.state = JWDefine.PAUSE;
						AudioManager.Instance.ChangeBGM("b_game_loc"+JWGlobal.Instance.realm_num); break;
						
					default: AudioManager.Instance.ChangeBGM("b_nosound"); break;
					}
					
					Application.LoadLevel(PlayerPrefs.GetString("LastLevelLoaded"));

					break;

				case "40.Scene.Main":
					//타이틀로.
					try{
						GameObject.FindWithTag("TOUCHLOCK").transform.localPosition = Vector2.zero;
					}
					catch{}

					JWGlobal.Instance.state = JWDefine.TITLE;
					Application.LoadLevel("20.Scene.Title");

					break;

				case "50.Scene.Select":
					//메인으로.
					try{
						GameObject.FindWithTag("TOUCHLOCK").transform.localPosition = Vector2.zero;
					}
					catch{}
					if(JWGlobal.Instance.realm_num > 0)
					{
						HandleUserData.Instance.SaveClearTime (JWGlobal.Instance.realm_num);
						HandleUserData.Instance.SaveClearScore (JWGlobal.Instance.realm_num);
						
						switch(JWGlobal.Instance.realm_num)
						{		
						case 1: HandleUserData.Instance.BackupUserData ("/user_clear_1.ini"); break;
						case 2: HandleUserData.Instance.BackupUserData ("/user_clear_2.ini"); break;
						case 3: HandleUserData.Instance.BackupUserData ("/user_clear_3.ini"); break;
						case 4: HandleUserData.Instance.BackupUserData ("/user_clear_4.ini"); break;
						case 5: HandleUserData.Instance.BackupUserData ("/user_clear_ex.ini"); break;
						default: break;
						}
					}
					JWGlobal.Instance.realm_num = 0;
					//JWGlobal.Instance.stage_num = 0;
					AudioManager.Instance.ChangeBGM("b_worldmap");

					JWGlobal.Instance.state = JWDefine.MAIN;
					Application.LoadLevel("40.Scene.Main");
					break;				

				default:
					Debug.Log (Application.loadedLevelName);
					break;
				}
			}
		}
	}
}
