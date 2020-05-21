using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Play_UpdateScene : MonoBehaviour {

	public float _timer = 0.03f;
	public float _StartTimer = 0.001f;
	//private float _timerSaveForReal;
	public bool stop = false;

	//int speed_count = 0;

	Play_LoadMap loadmap;
	Play_EnemyBehave enemypattern;
	Play_DrawEffect effect;
	Play_DrawJelly drawjelly;
	Play_ResultCalculate resultcalc;
	Play_DrawResult drawresult;
	Play_DrawBase drawbase;
	Play_GetStar getstar;
	Play_DrawStart drawstart;
	Play_DrawTutor drawtutor;


	// Use this for initialization
	void Start () {
		loadmap = GameObject.FindWithTag ("PLAY").GetComponent<Play_LoadMap> ();
		enemypattern = GameObject.FindWithTag ("PLAY").GetComponent<Play_EnemyBehave> ();
		effect =  GameObject.FindWithTag ("PLAY").transform.FindChild("Objects/Effect").GetComponent<Play_DrawEffect> ();
		drawjelly = GameObject.FindWithTag ("PLAY").transform.FindChild("Objects/Jelly").GetComponent<Play_DrawJelly> ();
		resultcalc = GameObject.FindWithTag ("PLAY").GetComponent<Play_ResultCalculate> ();
		drawresult = GameObject.FindWithTag ("RESULT").GetComponent<Play_DrawResult> ();
		drawbase = GameObject.FindWithTag ("PLAY").transform.FindChild("Objects/Base").GetComponent<Play_DrawBase> ();
		getstar = GameObject.FindWithTag ("PLAY").transform.FindChild("Up/UI_Star").GetComponent<Play_GetStar> ();
		drawstart = GameObject.FindWithTag ("START").GetComponent<Play_DrawStart> ();
		drawtutor = GameObject.FindWithTag ("PLAY").transform.FindChild("Up/UI_Tutor").GetComponent<Play_DrawTutor> ();


		InvokeRepeating("OnTimerExpired", _StartTimer, _timer);

	}

	/*void Update()
	{
		OnTimerExpired ();
	}*/

	void OnTimerExpired()
	{
		//Debug.Log (JWGlobal.Instance.global_count);

		//루프 타임 스탭 확인을 위한 디버그 영역.
		//var TempTime = Time.realtimeSinceStartup; ;
		//Debug.Log(TempTime - _timerSaveForReal);
		//_timerSaveForReal = Time.realtimeSinceStartup;
		//루프 타임 스탭 확인을 위한 디버그 영역 끝.

		if(GameObject.FindWithTag("PLAY").transform.localPosition.x == 0)
		{
			if(JWGlobal.Instance.start_stage == true)
			{
				JWGlobal.Instance.start_count++;
				drawstart.DrawStart();
			}
			else
			{
				if(stop == false)
				{
					if(JWGlobal.Instance.state == JWDefine.PLAY)
					{
						JWGlobal.Instance.global_count++;

						FinishStage(JWGlobal.Instance.win_type);

						UpdateScene();

					}
					else if(JWGlobal.Instance.result_state != 0 && JWGlobal.Instance.state == JWDefine.PAUSE)
					{
						if(JWGlobal.Instance.global_count < 10)
						{
							JWGlobal.Instance.global_count++;
							UpdateScene();
						}
						else
						{
							drawbase.CancelAllSelect();
							drawresult.DrawResult();
							stop = true;
						}
					}
				}
			}
		}
	}



	/// <summary>
	/// 업데이트씬 함수. 괴랄맞게 거대합니다.
	/// (100%)
	/// </summary>
	void UpdateScene () 
	{
		//Debug.Log (JWGlobal.Instance.total_jelly + " / " + JWGlobal.Instance.total_jelly_e);
	
		// 0. GetTIme
		if(JWGlobal.Instance.result_count_win == 0 && JWGlobal.Instance.result_count_lose == 0)
			JWGlobal.Instance.GetTime();

		// 1. Update Animation Count (100%)
		UpdateAnimation ();


		// 2. Collision Check (100%)
		CollisionCheck ();
		drawjelly.UpdateSprite ();
		////drawbase.UpdateBase ();

		// 3. Call Skill (100%)
		ContinueSkill();
		enemypattern.EnemySkillUse ();


		// 4. Generate Jelly in Bases (100%)
		GenerateJelly ();


		// 5. Others
		//Mana Update
		if(JWGlobal.Instance.global_count % (int)((3000f / (float)InitSkillValue.Instance.P_Mana[HandleUserData.Instance.skill_level[5]])/(float)HandleUserData.Instance.SPEED) == 0)//2900f
		{	if(JWGlobal.Instance.mana < 100)	JWGlobal.Instance.mana++;	}


		//Skill Condition Update
		if(JWGlobal.Instance.what_skill_on != JWDefine.NO_SKILL)
		{
			if(JWGlobal.Instance.skill_cool[JWGlobal.Instance.what_skill_on - 4001] == 0 && JWGlobal.Instance.mana >= JWGlobal.Instance.cost)
			{	
				JWGlobal.Instance.skill_ready = true;			
			}
			else if(JWGlobal.Instance.skill_ready == true)
			{	JWGlobal.Instance.skill_ready = false;	}
		}
		else if(JWGlobal.Instance.skill_ready == true)
		{	JWGlobal.Instance.skill_ready = false;	}



		//Call Enemy's Action Pattern
		if(JWGlobal.Instance.realm_num == 1 && JWGlobal.Instance.stage_num == 1 && JWGlobal.Instance.mission_num == 1){}
		else
		{
			if(JWGlobal.Instance.global_count >= (144/HandleUserData.Instance.SPEED))
			{
				if(JWGlobal.Instance.global_count%(288/HandleUserData.Instance.SPEED) == 0)
				{
					//Debug.Log("------------------------------------------------------------ Regular Action");
					enemypattern.EnemyBehave();
				}
				else if(JWGlobal.Instance.global_count%(36/HandleUserData.Instance.SPEED) == 0)
				{
					if(Random.Range(0, 24) < (HandleUserData.Instance.difficulty-751))
					{
						//Debug.Log("------------------------------------------------------------ Irregular Action");
						enemypattern.EnemyBehave();
					}
				}
			}
		}

		//13. Tutorial
		if(JWGlobal.Instance.tutor_bind == true)
		{
			if(JWGlobal.Instance.global_count <= JWGlobal.Instance.tutorial_end)
			{
				if(JWGlobal.Instance.mission_num == 1) drawtutor.Tutorial1();
				else if(JWGlobal.Instance.mission_num == 2) drawtutor.Tutorial2();
				else if(JWGlobal.Instance.mission_num == 3) drawtutor.Tutorial3();
			}			
		}
	}


	/// <summary>
	/// 건물에서 젤리 생산.
	/// (100%)
	/// </summary>
	void GenerateJelly()
	{
		// 3-1) Generate Player's Jelly in Bases

		for (int it = 0; it < loadmap.Player_Base.Count; it++) //for (List<Base>.Enumerator it = loadmap.Player_Base.GetEnumerator(); it.MoveNext(); )
		{
			Base temp = loadmap.Player_Base[it];
						
			if(temp.num < (temp.max * InitSkillValue.Instance.P_Max[HandleUserData.Instance.skill_level[3]])/100)
			{
				switch(temp.type)
				{
				case JWDefine.CASTLE:
					if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_CASTLE/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[HandleUserData.Instance.skill_level[4]]) == 0)
						temp.num += 1;
					break;
				case JWDefine.HOUSE:
					if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_HOUSE/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[HandleUserData.Instance.skill_level[4]]) == 0)
						temp.num += 1;
					break;
				case JWDefine.FARM:
					if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_FARM/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[HandleUserData.Instance.skill_level[4]]) == 0)
						temp.num += 1;
					break;
				case JWDefine.FORTRESS:
					if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_FORTRESS/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[HandleUserData.Instance.skill_level[4]]) == 0)
						temp.num += 1;
					break;
				case JWDefine.BARRACKS_1:
					if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_SWORD/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[HandleUserData.Instance.skill_level[4]]) == 0)
						temp.num += 1;
					break;
				case JWDefine.BARRACKS_2:
					if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_AXE/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[HandleUserData.Instance.skill_level[4]]) == 0)
						temp.num += 1;
					break;
				}
			}
			else if(temp.num > (temp.max * InitSkillValue.Instance.P_Max[HandleUserData.Instance.skill_level[3]])/100) //The number of Jelly is more than maximum of Base
			{
				if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_DECREASE/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[HandleUserData.Instance.skill_level[4]]) == 0)
				{
					temp.num -= 1;
					if(JWGlobal.Instance.score_get > (HandleUserData.Instance.difficulty-750) * 15)
						JWGlobal.Instance.score_get -= (HandleUserData.Instance.difficulty-750) * 15;
					else JWGlobal.Instance.score_get = 0;
				}
			}
			
			if(temp.type == JWDefine.CASTLE)
			{
				temp.hero_out = false;

				for(int i=0; i<drawjelly.Player_Jelly.Count; i++)				
				{
					if(drawjelly.Player_Jelly[i].level == JWDefine.LEVEL_3)					
					{
						if(temp.tag == drawjelly.Player_Jelly[i].hero_tag)						
						{						
							temp.hero_out = true;
							break;
						}
					}
				}
			}

			JWGlobal.Instance.total_jelly += temp.num;

			loadmap.Player_Base[it] = temp;
		}



		// 3-2) Generate Enemy's Jelly in Bases

		JWGlobal.Instance.enemy_base = 0;

		for (int it = 0; it < loadmap.Enemy_Base.Count; it++) //for (List<Base>.Enumerator it = loadmap.Enemy_Base.GetEnumerator(); it.MoveNext(); )
		{
			Base temp = loadmap.Enemy_Base[it];

			if(temp.num < (temp.max * InitSkillValue.Instance.P_Max[InitSkillValue.Instance.skill_level_e[0]])/100)
			{
				switch(temp.type)
				{
				case JWDefine.CASTLE:
					if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_CASTLE/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[InitSkillValue.Instance.skill_level_e[1]]) == 0)
						temp.num += 1;
					break;
				case JWDefine.HOUSE:
					if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_HOUSE/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[InitSkillValue.Instance.skill_level_e[1]]) == 0)
						temp.num += 1;
					break;
				case JWDefine.FARM:
					if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_FARM/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[InitSkillValue.Instance.skill_level_e[1]]) == 0)
						temp.num += 1;
					break;
				case JWDefine.FORTRESS:
					if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_FORTRESS/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[InitSkillValue.Instance.skill_level_e[1]]) == 0)
						temp.num += 1;
					break;
				case JWDefine.BARRACKS_1:
					if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_SWORD/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[InitSkillValue.Instance.skill_level_e[1]]) == 0)
						temp.num += 1;
					break;
				case JWDefine.BARRACKS_2:
					if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_AXE/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[InitSkillValue.Instance.skill_level_e[1]]) == 0)
						temp.num += 1;
					break;
				}
			}
			else if(temp.num > (temp.max * InitSkillValue.Instance.P_Max[InitSkillValue.Instance.skill_level_e[0]])/100) //The number of Jelly is more than maximum of Base
			{
				if(JWGlobal.Instance.global_count % ((JWDefine.RATE_OF_DECREASE/HandleUserData.Instance.SPEED) * 100 / InitSkillValue.Instance.P_Gen[InitSkillValue.Instance.skill_level_e[1]]) == 0)
					temp.num -= 1;
			}
			
			if(temp.type == JWDefine.CASTLE)
			{
				temp.hero_out = false;

				for(int i=0; i<drawjelly.Enemy_Jelly.Count; i++)
				{
					if(drawjelly.Enemy_Jelly[i].level == JWDefine.LEVEL_3)
					{
						if(temp.tag == drawjelly.Enemy_Jelly[i].hero_tag)
						{						
							temp.hero_out = true;
							break;
						}
					}
				}
			}

			JWGlobal.Instance.enemy_base++;
			JWGlobal.Instance.total_jelly_e += temp.num;

			loadmap.Enemy_Base[it] = temp;
		}
		//Debug.Log(JWGlobal.Instance.total_jelly + " / " + JWGlobal.Instance.total_jelly_e);
	}


	/// <summary>
	/// Check Collisions.
	/// (100%)
	/// </summary>
	void CollisionCheck()
	{
		// 2. Collision Check
		
		// 2-0) Collision Check for Star
		if(JWGlobal.Instance.star_get < 3)
		{
			for(int temp = 0; temp < loadmap.Nature_Star.Count; temp++)
			{
				Star it = loadmap.Nature_Star[(temp)];
				
				if(it.num == 1)
				{
					CLRECT rect_star = new CLRECT(it.position.x-16f, it.position.y-16f, 32f, 32f);
					{
						for(int i=0; i<loadmap.Player_Base.Count; i++)						
						{
							CLRECT rect_player_b = new CLRECT(loadmap.Player_Base[i].position.x-32f, loadmap.Player_Base[i].position.y-32f, 64f, 64f);
							{						
								if(CLCollisionChkRwithR(rect_star, rect_player_b) == true) //Collision happened
								{
									JWGlobal.Instance.star_get++;
									JWGlobal.Instance.score_get+= (HandleUserData.Instance.difficulty-750) * 6000;
									it.num = 0;
									getstar.UIStarUpdate();
									loadmap.Nature_Star[(temp)] = it;
									
									break;
								}
							}
						}

						for(int i=0; i<drawjelly.Player_Jelly.Count; i++)						
						{
							CLRECT rect_player_j = new CLRECT(drawjelly.Player_Jelly[i].position.x-25f, drawjelly.Player_Jelly[i].position.y-25f, 50f, 50f);
							{
								if(CLCollisionChkRwithR(rect_star, rect_player_j) == true) //Collision happened
								{
									JWGlobal.Instance.star_get++;
									JWGlobal.Instance.score_get+= (HandleUserData.Instance.difficulty-750) * 6000;
									it.num = 0;
									getstar.UIStarUpdate();
									loadmap.Nature_Star[(temp)] = it;
									
									break;
								}
							}
						}
					}
				}
			}
		}

		// 2-1) Collision Check for Player

		JWGlobal.Instance.total_jelly = 0;

		CollisionCheckJelly (JWDefine.PLAYER);

		// 2-2) Collision Check for Enemy

		JWGlobal.Instance.total_jelly_e = 0;

		CollisionCheckJellyE (JWDefine.ENEMY);


	}


	/// <summary>
	/// Collisions Check for Jelly Player.
	/// </summary>
	void CollisionCheckJelly (int PLAYER)
	{
		for(int temp = 0; temp < drawjelly.Player_Jelly.Count; temp++)		
		{
			Jelly it = drawjelly.Player_Jelly[temp];

			if(it.affect_count > 0)
			{
				it.affect_count -= HandleUserData.Instance.SPEED;
				if(it.affect_count <= 0)
				{
					it.affect_count = 0;
					it.affect = false;
					it.slow = 1.0f;
				}
			}
			else if(it.slow != 1.0f)
			{
				it.affect_count = 0;
				it.affect = false;
				it.slow = 1.0f;
			}
			
			int ct_p = 1;
			int ct_e = 1;			
			bool collision = false;

			CLRECT rect_player_j = new CLRECT(it.position.x-25f, it.position.y-25f, 50f, 50f);
			{			
				if(PLAYER == JWDefine.PLAYER)
				{
					// 2-1-0) Check with Enemy's Jelly
					if(it.state != JWDefine.BATTLE)
					{
						for(int e_temp = 0; e_temp < drawjelly.Enemy_Jelly.Count; e_temp++)
						{
							Jelly e_it = drawjelly.Enemy_Jelly[e_temp];

							CLRECT rect_enemy_j = new CLRECT(e_it.position.x-25f, e_it.position.y-25f, 50f, 50f);
							{							
								if(it.state != JWDefine.BATTLE && e_it.state != JWDefine.BATTLE) //Can't Check Collision Both Jellies in Battle
								{
									if(CLCollisionChkRwithR(rect_player_j, rect_enemy_j) == true) //Collision happened
									{								
										if(Random.Range(0, 100) < InitSkillValue.Instance.P_Critical[HandleUserData.Instance.skill_level[8]] + it.critical)
										{	ct_p = 2;	}
										else ct_p = 1;
										if(Random.Range(0, 100) < InitSkillValue.Instance.P_Critical[InitSkillValue.Instance.skill_level_e[4]] + e_it.critical)
										{	ct_e = 2;	}
										else ct_e = 1;
										if(e_it.affect == true) ct_e = 2;

										it.eit_pos.x = e_it.position.x;
										it.eit_pos.y = e_it.position.y;

										if(ct_p == 2)
										{
											//Critical Effect
											Effect critical = new Effect();
											critical.position = new CLVEC2(it.position.x, it.position.y + 40f);
											critical.anicount = 0;
											critical.count = 0;
											
											//Set Sprite Tag
											critical.tag = effect.index_critical;

											effect.Critical_Effect.Add(critical);
											effect.DrawNewEffect(critical, "Critical", effect.Critical_Effect.Count-1);
										}
										if(ct_e == 2)
										{
											//Critical Effect
											Effect critical = new Effect();
											critical.position = new CLVEC2(e_it.position.x, e_it.position.y + 40f);
											critical.anicount = 0;
											critical.count = 0;
											
											//Set Sprite Tag
											critical.tag = effect.index_critical;
											
											effect.Critical_Effect.Add(critical);
											effect.DrawNewEffect(critical, "Critical", effect.Critical_Effect.Count-1);
										}

										//Set state
										it.state = JWDefine.BATTLE;
										e_it.state = JWDefine.BATTLE;
										it.anicount = 0;
										it.aninum = 0;
										e_it.anicount = 0;
										e_it.aninum = 0;


										//Attack Effect
										Effect attack = new Effect();
										attack.position = new CLVEC2((it.position.x + it.eit_pos.x) / 2, (it.position.y + it.eit_pos.y) / 2);
										attack.anicount = 0;
										attack.count = 0;
										
										//Set Sprite Tag
										attack.tag = effect.index_attack;

										effect.Attack_Effect.Add(attack);
										effect.DrawNewEffect(attack, "Attack", effect.Attack_Effect.Count-1);
										
										AudioManager.Instance.SFXPlay("SFX_ATTACK");


										//Decide the Jelly which wins in Battle
										if((int)((float)it.num * (InitSkillValue.Instance.P_Power[HandleUserData.Instance.skill_level[6]] + it.power) * (float)ct_p) > (int)((float)e_it.num * (InitSkillValue.Instance.P_Power[InitSkillValue.Instance.skill_level_e[2]] + e_it.power) * (float)ct_e)) 
										{
											//Player Win
											int killed = (int)((float)e_it.num * ((InitSkillValue.Instance.P_Power[InitSkillValue.Instance.skill_level_e[2]] + e_it.power) * (float)ct_e) / ((InitSkillValue.Instance.P_Power[HandleUserData.Instance.skill_level[6]] + it.power) * (float)ct_p));;
											JWGlobal.Instance.score_get += e_it.num * (HandleUserData.Instance.difficulty-750) * 120;
											JWGlobal.Instance.score_get += (HandleUserData.Instance.difficulty-750) * 1200;
											it.num -= killed;
											e_it.num = 0;
										}
										else if((int)((float)it.num * (InitSkillValue.Instance.P_Power[HandleUserData.Instance.skill_level[6]] + it.power) * (float)ct_p) < (int)((float)e_it.num * (InitSkillValue.Instance.P_Power[InitSkillValue.Instance.skill_level_e[2]] + e_it.power) * (float)ct_e)) 
										{
											//Enemy WIn
											int killed = (int)((float)it.num * ((InitSkillValue.Instance.P_Power[HandleUserData.Instance.skill_level[6]] + it.power) * (float)ct_p) / ((InitSkillValue.Instance.P_Power[InitSkillValue.Instance.skill_level_e[2]] + e_it.power) * (float)ct_e));
											JWGlobal.Instance.score_get += killed * (HandleUserData.Instance.difficulty-750) * 120;
											e_it.num -= killed;
											it.num = 0;
										}
										else 
										{
											//Both Dead
											JWGlobal.Instance.score_get += e_it.num * (HandleUserData.Instance.difficulty-750) * 120;
											it.num = 0;
											e_it.num = 0;
										}
										
										it.JellyWin = 0;
										e_it.JellyWin = -5;

										drawjelly.Enemy_Jelly[e_temp] = e_it;

										collision = true; //Collision happened
										
										break;
									}
								}
							}
						}
					}
				}
				
				
				//2-2-1) Check with Rival's Base.
				if(collision == false && it.state != JWDefine.BATTLE)
				{
					for(int e_temp = 0; e_temp < loadmap.Enemy_Base.Count; e_temp++)
					{
						Base e_it = loadmap.Enemy_Base[(e_temp)];

						CLRECT rect_enemy_b = new CLRECT(e_it.position.x - e_it.size.x / 2, e_it.position.y - e_it.size.y / 2, e_it.size.x, e_it.size.y);
						{
							if(CLCollisionChkRwithR(rect_player_j, rect_enemy_b) == true) //Collision happened
							{
								//skill_level[8]
								if(Random.Range(0, 100) < InitSkillValue.Instance.P_Critical[HandleUserData.Instance.skill_level[8]] + it.critical)
								{	ct_p = 2;	}
								else ct_p = 1;
								
								if(PLAYER == JWDefine.ENEMY) // 4. Snow Fortress Buff
								{
									if(it.affect == true) ct_p = 2;
								}

								it.eit_pos.x = e_it.position.x;
								it.eit_pos.y = e_it.position.y;
								it.eit_type = e_it.type;
								it.eit_owner = e_it.owner;
								it.eit_tag = e_it.tag;
								
								if(ct_p == 2)
								{
									//Critical Effect
									Effect critical = new Effect();
									critical.position = new CLVEC2(it.position.x, it.position.y + 40f);
									critical.anicount = 0;
									critical.count = 0;
									
									//Set Sprite Tag
									critical.tag = effect.index_critical;

									effect.Critical_Effect.Add(critical);
									effect.DrawNewEffect(critical, "Critical", effect.Critical_Effect.Count-1);
								}
								
								//Set state
								it.state = JWDefine.BATTLE;
								it.anicount = 0;
								it.aninum = 0;
								
								//Decide the Jelly which wins in Battle
								int n = 1;
								switch(e_it.type)			// 건물 젤리 1마리를 잡기 위한 공격 젤리수 (Power : Normal : 1 / Hero : 2).
								{
								case JWDefine.CASTLE:		
								case JWDefine.FORTRESS:
								case JWDefine.BARRACKS_2:	// 200%
									n = 2/it.ignore;		// Normal: 2.0 , Ignore: 1.0 , Hero: 1.0/2
									break;
								default:					// 100%
									break;
								}


								//Attack Effect
								Effect attack = new Effect();
								attack.position = new CLVEC2((it.position.x + it.eit_pos.x) / 2, (it.position.y + it.eit_pos.y) / 2);
								attack.anicount = 0;
								attack.count = 0;
								
								//Set Sprite Tag		
								attack.tag = effect.index_attack;

								effect.Attack_Effect.Add(attack);
								effect.DrawNewEffect(attack, "Attack", effect.Attack_Effect.Count-1);

								
								//Jelly Win
								if((int)((float)it.num * (InitSkillValue.Instance.P_Power[HandleUserData.Instance.skill_level[6]]+it.power)) * ct_p > e_it.num * n)
								{							
									AudioManager.Instance.SFXPlay ("SFX_OCCUPY");

									//Occupy Effect
									Effect occupy = new Effect();
									occupy.position = it.eit_pos;
									occupy.anicount = 0;
									occupy.count = 0;
									
									//Set Sprite Tag		
									occupy.tag = effect.index_occupy;

									effect.Occupy_Effect.Add(occupy);
									effect.DrawNewEffect(occupy, "Occupy", effect.Occupy_Effect.Count-1);


									int killed = (int)(((float)e_it.num * (float)n) / ((InitSkillValue.Instance.P_Power[HandleUserData.Instance.skill_level[6]]+it.power) * (float)ct_p));
									
									//Set Score
									
									if(PLAYER == JWDefine.PLAYER)
									{
										JWGlobal.Instance.score_get += e_it.num * (HandleUserData.Instance.difficulty-750) * 72;
										JWGlobal.Instance.score_get += (HandleUserData.Instance.difficulty-750) * 720;
									}
									else
									{
										JWGlobal.Instance.score_get += killed * (HandleUserData.Instance.difficulty-750) * 72;
										if(JWGlobal.Instance.score_get > (HandleUserData.Instance.difficulty-750) * 720)
											JWGlobal.Instance.score_get -= (HandleUserData.Instance.difficulty-750) * 720;
										else JWGlobal.Instance.score_get = 0;
									}
									
									// Set Final Number
									e_it.num = it.num - killed;
									if(e_it.num < 0) e_it.num = 0;
									it.num = 0;

									
									//Change owner of base
									loadmap.Player_Base.Add(drawbase.MakeBase(e_it, JWDefine.PLAYER));
									loadmap.Enemy_Base.RemoveAt(e_temp);
									drawbase.ChangeBase(PLAYER, it.eit_owner, it.eit_type, it.eit_tag);

									it.JellyWin = 1;
								}
								else
								{
									//Base Win

									AudioManager.Instance.SFXPlay("SFX_ATTACK");


									int killed = (int)(((float)it.num / (float)n) * ((InitSkillValue.Instance.P_Power[HandleUserData.Instance.skill_level[6]]+it.power) * (float)ct_p));
									
									if(PLAYER == JWDefine.PLAYER)
									{	
										JWGlobal.Instance.score_get += killed * (HandleUserData.Instance.difficulty-750) * 72;							
									}
									else
									{
										JWGlobal.Instance.score_get += it.num * (HandleUserData.Instance.difficulty-750) * 72;							
									}
									
									e_it.num -= killed;
									if(e_it.num < 0) e_it.num = 0;
									it.num = 0;
									
									loadmap.Enemy_Base[(e_temp)] = e_it;
									
									it.JellyWin = 2;
								}

								if(loadmap.drag_it.tag == e_it.tag)
								{
									loadmap.drag_it = e_it;
								}

								//drawbase.UpdateBase ();
								collision = true; //Collision happened
								break;
							}
						}
					}
				}
				
				// 2-2-2) Check with Nature's Base
				if(collision == false && it.state != JWDefine.BATTLE)
				{
					for(int n_temp = 0; n_temp < loadmap.Nature_Base.Count; n_temp++)
					{
						Base e_it = loadmap.Nature_Base[(n_temp)];

						CLRECT rect_enemy_b = new CLRECT(e_it.position.x - e_it.size.x / 2, e_it.position.y - e_it.size.y / 2, e_it.size.x, e_it.size.y);
						{
							if(CLCollisionChkRwithR(rect_player_j, rect_enemy_b) == true) //Collision happened
							{
								if(Random.Range(0, 100) < InitSkillValue.Instance.P_Critical[HandleUserData.Instance.skill_level[8]] + it.critical)
								{	ct_p = 2;	}
								else ct_p = 1;
								
								if(PLAYER == JWDefine.ENEMY) // 4. Snow Fortress Buff
								{
									if(it.affect == true) ct_p = 2;
								}

								it.eit_pos = new CLVEC2(e_it.position);
								it.eit_type = e_it.type;
								it.eit_owner = e_it.owner;
								it.eit_tag = e_it.tag;
								
								if(ct_p == 2)
								{
									//Critical Effect
									Effect critical = new Effect();
									critical.position = new CLVEC2(it.position.x, it.position.y + 40f);
									critical.anicount = 0;
									critical.count = 0;
									
									//Set Sprite Tag
									critical.tag = effect.index_critical;

									effect.Critical_Effect.Add(critical);
									effect.DrawNewEffect(critical, "Critical", effect.Critical_Effect.Count-1);
								}
								
								//Set state
								it.state = JWDefine.BATTLE;
								it.anicount = 0;
								it.aninum = 0;
								
								//Decide the Jelly which wins in Battle
								int n = 1;
								switch(e_it.type)			// 건물 젤리 1마리를 잡기 위한 공격 젤리수 (Power : Normal : 1 / Hero : 2).
								{
								case JWDefine.CASTLE:		
								case JWDefine.FORTRESS:
								case JWDefine.BARRACKS_2:	// 200%
									n = 2/it.ignore;		// Normal: 2.0 , Ignore: 1.0 , Hero: 1.0/2
									break;
								default:					// 100%
									break;
								}


								//Attack Effect
								Effect attack = new Effect();
								attack.position = new CLVEC2((it.position.x + it.eit_pos.x) / 2, (it.position.y + it.eit_pos.y) / 2);
								attack.anicount = 0;
								attack.count = 0;
								
								//Set Sprite Tag	
								attack.tag = effect.index_attack;

								effect.Attack_Effect.Add(attack);
								effect.DrawNewEffect(attack, "Attack", effect.Attack_Effect.Count-1);


								//Jelly Win		
								if((int)((float)it.num * (InitSkillValue.Instance.P_Power[HandleUserData.Instance.skill_level[6]]+it.power)) * ct_p > e_it.num * n)
								{
									AudioManager.Instance.SFXPlay ("SFX_OCCUPY");
									
									//Occupy Effect
									Effect occupy = new Effect();
									occupy.position = it.eit_pos;
									occupy.anicount = 0;
									occupy.count = 0;
									
									//Set Sprite Tag			
									occupy.tag = effect.index_occupy;

									effect.Occupy_Effect.Add(occupy);
									effect.DrawNewEffect(occupy, "Occupy", effect.Occupy_Effect.Count-1);


									int killed = (int)(((float)e_it.num * (float)n) / ((InitSkillValue.Instance.P_Power[HandleUserData.Instance.skill_level[6]]+it.power) * (float)ct_p));
									
									if(PLAYER == JWDefine.PLAYER)
									{
										JWGlobal.Instance.score_get += e_it.num * (HandleUserData.Instance.difficulty-750) * 72;
										JWGlobal.Instance.score_get += (HandleUserData.Instance.difficulty-750) * 720;
									}
									
									// Set Final Number
									e_it.num = it.num - killed;
									if(e_it.num < 0) e_it.num = 0;
									it.num = 0;
									
									//Change owner of base
									loadmap.Player_Base.Add(drawbase.MakeBase(e_it, JWDefine.PLAYER));
									loadmap.Nature_Base.RemoveAt(n_temp);
									drawbase.ChangeBase(PLAYER, it.eit_owner, it.eit_type, it.eit_tag);

									it.JellyWin = 1;
								}
								else
								{
									//Base Win
									
									AudioManager.Instance.SFXPlay("SFX_ATTACK");


									int killed = (int)(((float)it.num / (float)n) * ((InitSkillValue.Instance.P_Power[HandleUserData.Instance.skill_level[6]]+it.power) * (float)ct_p));							             
									
									if(PLAYER == JWDefine.PLAYER)
									{
										JWGlobal.Instance.score_get += killed * (HandleUserData.Instance.difficulty-750) * 72;
									}
									
									e_it.num -= killed;
									
									if(e_it.num < 0) e_it.num = 0;
									it.num = 0;
									
									loadmap.Nature_Base[(n_temp)] = e_it;
									
									it.JellyWin = 2;
								}

								if(loadmap.drag_it.tag == e_it.tag)
								{
									loadmap.drag_it = e_it;
								}

								//drawbase.UpdateBase ();
								collision = true; //Collision happened
								break;
							}
						}
					}
				}
				
				
				// 2-2-3) Check with My Base - No Battle
				if(collision == false && it.state != JWDefine.BATTLE)
				{
					for(int e_temp = 0; e_temp < loadmap.Player_Base.Count; e_temp++)
					{
						Base e_it = loadmap.Player_Base[(e_temp)];

						CLRECT rect_player_b = new CLRECT(e_it.position.x - e_it.size.x / 2, e_it.position.y - e_it.size.y / 2, e_it.size.x, e_it.size.y);
						{
							if(CLCollisionChkRwithR(rect_player_j, rect_player_b) == true) //Collision happened
							{
								if(it.seq_of_target >= it.num_of_move && it.num_of_move > 1) // it is a correct target base
								{
									AudioManager.Instance.SFXPlay("SFX_MAKE");
									
									//Make Effect
									Effect make = new Effect();
									make.position = new CLVEC2(it.position.x, it.position.y + 40f);
									make.count = 0;
									make.anicount = 0;
									
									//Set Sprite Tag	
									make.tag = effect.index_make;

									effect.Make_Effect.Add(make);
									effect.DrawNewEffect(make, "Make", effect.Make_Effect.Count-1);
									
									e_it.num += it.num; //Add Jellies to Player's Base
									it.num = 0;
									it.JellyWin = -1;

									loadmap.Player_Base[(e_temp)] = e_it;

									if(loadmap.drag_it.tag == e_it.tag)
									{
										loadmap.drag_it = e_it;
									}

									//drawbase.UpdateBase ();
									collision = true; //Collision happened
								}
								break;
							}
						}
					}
					if(collision == true)
					{
						it.state = JWDefine.PEACE;
						//Debug.Log ("Help : Remove List : " + it.tag);
					}
				}
			}
			
			//Update
			if(it.state == JWDefine.MOVE)
			{
				CLRECT pathpos = new CLRECT(it.pointfortarget[it.seq_of_target].x-(4*HandleUserData.Instance.SPEED), it.pointfortarget[it.seq_of_target].y-(4*HandleUserData.Instance.SPEED), 8*HandleUserData.Instance.SPEED, 8*HandleUserData.Instance.SPEED);
				{
					//Change Force Via Paths ([1]부터 시작: 시작 지점 제외).
					if(CLCollisionChkRwithV(pathpos, it.position))
					{
						//Add the Sequence of Target
						it.seq_of_target = it.seq_of_target+1;
						
						if(it.seq_of_target < it.num_of_move)
						{
							//Calculate Jelly's Forces( x, y )
							float px = it.pointfortarget[it.seq_of_target].x - it.position.x;
							float py = it.pointfortarget[it.seq_of_target].y - it.position.y;
							float length = Mathf.Sqrt(px*px + py*py);
							it.x_force = px / length;
							it.y_force = py / length;
							
							//Decide the direction of Jelly
							if(it.x_force > 0 && it.y_force > 0)
							{	it.direction = JWDefine.DIRECTION_86;	}
							else if(it.x_force < 0 && it.y_force > 0)
							{	it.direction = JWDefine.DIRECTION_84;	}
							else if(it.x_force < 0 && it.y_force < 0)
							{	it.direction = JWDefine.DIRECTION_24;	}
							else if(it.x_force > 0 && it.y_force < 0)
							{	it.direction = JWDefine.DIRECTION_26;	}
							else
							{
								if(it.x_force > 0)
								{	it.direction = JWDefine.DIRECTION_26+it.ran;	}
								else if(it.x_force < 0)
								{	it.direction = JWDefine.DIRECTION_84+it.ran;	}
								else if(it.y_force > 0)
								{	it.direction = JWDefine.DIRECTION_86+it.ran;	}
								else if(it.y_force < 0)
								{	it.direction = JWDefine.DIRECTION_24+it.ran;	}
							}
						}
					}
				}
				
				//Adjust Jelly's Speed
				if(JWGlobal.Instance.obstacles > 0)
				{
					for (List<Obstacle>.Enumerator o_it = loadmap.Nature_Obstacle.GetEnumerator(); o_it.MoveNext(); )
					{
						CLRECT rect_obstacle = new CLRECT(o_it.Current.start.x, o_it.Current.start.y, o_it.Current.end.x - o_it.Current.start.x, o_it.Current.end.y - o_it.Current.start.y);
						{
							if(CLCollisionChkRwithV(rect_obstacle, it.position))
							{
								if(it.y_force > 0)		it.adjust_speed = o_it.Current.up/100;
								else if(it.y_force < 0)	it.adjust_speed = o_it.Current.dn/100;
								else 					it.adjust_speed = o_it.Current.zero/100;
								break;
							}
							else it.adjust_speed = 1.0f;
						}
					}
				}
				
				//Update Jelly's Position				
				it.position.x += it.x_force * HandleUserData.Instance.SPEED * (InitSkillValue.Instance.P_Speed[HandleUserData.Instance.skill_level[7]]+it.speed) * it.adjust_speed * it.slow;
				it.position.y += it.y_force * HandleUserData.Instance.SPEED * (InitSkillValue.Instance.P_Speed[HandleUserData.Instance.skill_level[7]]+it.speed) * it.adjust_speed * it.slow;
				it.pathcount += HandleUserData.Instance.SPEED;
				
				//Update Animation Count
				it.anicount++;
				if(it.anicount % 4 == 0) //Control Animation Speed
				{	it.aninum = (it.aninum + 1) % 4;	}
				
			}
			else if(it.state == JWDefine.BATTLE)
			{
				//Debug.Log (it.tag + " : " + it.aninum);
				//Update Animation Count
				it.anicount++;
				if(it.anicount % 2 == 0)
				{
					it.aninum = (it.aninum) + 1;
					//Debug.Log (it.aninum);
					if(it.aninum >= 8) //The end of Battle Animation
					{
						it.aninum = 0;
						
						if(it.num > 0)
						{
							//Set State of Jelly
							it.state = JWDefine.MOVE;
						}
						else
						{
							it.state = JWDefine.PEACE;
						}
					}
				}
			}
			
			if(PLAYER == JWDefine.PLAYER)
				JWGlobal.Instance.total_jelly += it.num;
			else
				JWGlobal.Instance.total_jelly_e += it.num;


			drawjelly.Player_Jelly[(temp)] = it;

		}//End of Collision Check

		//drawjelly.Player_Jelly.RemoveAll(item => item.state == JWDefine.PEACE && item.num == 0);
		//drawjelly.SortJelly ();

	}







	/// <summary>
	/// Collisions Check for Jelly Enemy.
	/// </summary>
	void CollisionCheckJellyE (int PLAYER)
	{
		for(int temp = 0; temp < drawjelly.Enemy_Jelly.Count; temp++)
		{
			Jelly it = drawjelly.Enemy_Jelly[(temp)];

			if(it.affect_count > 0)
			{
				it.affect_count -= HandleUserData.Instance.SPEED;
				if(it.affect_count <= 0)
				{
					it.affect_count = 0;
					it.affect = false;
					it.fast = 1.0f;
				}
			}
			else if(it.fast != 1.0f)
			{
				it.affect_count = 0;
				it.affect = false;
				it.fast = 1.0f;
			}

			int ct_p = 1;
			bool collision = false;
			CLRECT rect_player = new CLRECT (it.position.x-25f, it.position.y-25f, 50f, 50f);


			//2-2-1) Check with Rival's Base.
			if(collision == false && it.state != JWDefine.BATTLE)
			{
				for(int e_temp = 0; e_temp < loadmap.Player_Base.Count; e_temp++)
				{
					Base e_it = loadmap.Player_Base[(e_temp)];

					CLRECT rect_enemy = new CLRECT (e_it.position.x - e_it.size.x / 2, e_it.position.y - e_it.size.y / 2, e_it.size.x, e_it.size.y);
					
					if(CLCollisionChkRwithR(rect_player, rect_enemy) == true) //Collision happened
					{
																						//skill_level[8]
						if(Random.Range(0, 100) < InitSkillValue.Instance.P_Critical[InitSkillValue.Instance.skill_level_e[4]] + it.critical)
						{	ct_p = 2;	}
						else ct_p = 1;

						if(PLAYER == JWDefine.ENEMY) // 4. Snow Fortress Buff
						{
							if(it.affect == true) ct_p = 2;
						}

						it.eit_pos = new CLVEC2(e_it.position);
						it.eit_type = e_it.type;
						it.eit_owner = e_it.owner;
						it.eit_tag = e_it.tag;

						if(ct_p == 2)
						{
							//Critical Effect
							Effect critical = new Effect();
							critical.position = new CLVEC2(it.position.x, it.position.y + 40);
							critical.anicount = 0;
							critical.count = 0;

							//Set Sprite Tag	
							critical.tag = effect.index_critical;

							effect.Critical_Effect.Add(critical);
							effect.DrawNewEffect(critical, "Critical", effect.Critical_Effect.Count-1);
						}
						
						//Set state
						it.state = JWDefine.BATTLE;
						it.anicount = 0;
						it.aninum = 0;
						
						//Decide the Jelly which wins in Battle
						int n = 1;
						switch(e_it.type)			// 건물 젤리 1마리를 잡기 위한 공격 젤리수 (Power : Normal : 1 / Hero : 2).
						{
						case JWDefine.CASTLE:		
						case JWDefine.FORTRESS:
						case JWDefine.BARRACKS_2:	// 200%
							n = 2/it.ignore;		// Normal: 2.0 , Ignore: 1.0 , Hero: 1.0/2
							break;
						default:					// 100%
							break;
						}


						//Attack Effect
						Effect attack = new Effect();
						attack.position = new CLVEC2((it.position.x + it.eit_pos.x) / 2, (it.position.y + it.eit_pos.y) / 2);
						attack.anicount = 0;
						attack.count = 0;
						
						//Set Sprite Tag	
						attack.tag = effect.index_attack;

						effect.Attack_Effect.Add(attack);
						effect.DrawNewEffect(attack, "Attack", effect.Attack_Effect.Count-1);


						//Jelly Win
						if((int)((float)it.num * (InitSkillValue.Instance.P_Power[InitSkillValue.Instance.skill_level_e[2]]+it.power)) * ct_p > e_it.num * n)
						{
							AudioManager.Instance.SFXPlay ("SFX_OCCUPY");
							
							//Occupy Effect
							Effect occupy = new Effect();
							occupy.position = it.eit_pos;
							occupy.anicount = 0;
							occupy.count = 0;
							
							//Set Sprite Tag	
							occupy.tag = effect.index_occupy;

							effect.Occupy_Effect.Add(occupy);
							effect.DrawNewEffect(occupy, "Occupy", effect.Occupy_Effect.Count-1);


							int killed = (int)(((float)e_it.num * (float)n) / ((InitSkillValue.Instance.P_Power[InitSkillValue.Instance.skill_level_e[2]]+it.power) * (float)ct_p));

							//Set Score

							if(PLAYER == JWDefine.PLAYER)
							{
								JWGlobal.Instance.score_get += e_it.num * (HandleUserData.Instance.difficulty-750) * 72;
								JWGlobal.Instance.score_get += (HandleUserData.Instance.difficulty-750) * 720;
							}
							else
							{
								JWGlobal.Instance.score_get += killed * (HandleUserData.Instance.difficulty-750) * 72;
								if(JWGlobal.Instance.score_get > (HandleUserData.Instance.difficulty-750) * 720)
									JWGlobal.Instance.score_get -= (HandleUserData.Instance.difficulty-750) * 720;
								else JWGlobal.Instance.score_get = 0;
							}
							
							// Set Final Number
							e_it.num = it.num - killed;
							if(e_it.num < 0) e_it.num = 0;
							it.num = 0;
							

							//drag_it fix
							if(e_it.tag == loadmap.drag_it.tag)
							{
								for (List<Base>.Enumerator p_it = loadmap.Player_Base.GetEnumerator(); p_it.MoveNext(); )
								{
									if(p_it.Current.selected == true && p_it.Current.tag != e_it.tag)
									{
										loadmap.drag_it = loadmap.Player_Base[loadmap.Player_Base.IndexOf(p_it.Current)];
										break;
									}
								}
							}

							
							//Change owner of base
							loadmap.Enemy_Base.Add(drawbase.MakeBase(e_it, JWDefine.ENEMY));
							loadmap.Player_Base.RemoveAt(e_temp);
							drawbase.ChangeBase(PLAYER, it.eit_owner, it.eit_type, it.eit_tag);

							it.JellyWin = 1;
						}
						else
						{
							//Base Win
														
							AudioManager.Instance.SFXPlay("SFX_ATTACK");


							int killed = (int)(((float)it.num / (float)n) * ((InitSkillValue.Instance.P_Power[InitSkillValue.Instance.skill_level_e[2]]+it.power) * (float)ct_p));

							if(PLAYER == JWDefine.PLAYER)
							{	
								JWGlobal.Instance.score_get += killed * (HandleUserData.Instance.difficulty-750) * 72;							
							}
							else
							{
								JWGlobal.Instance.score_get += it.num * (HandleUserData.Instance.difficulty-750) * 72;							
							}

							e_it.num -= killed;
							if(e_it.num < 0) e_it.num = 0;
							it.num = 0;
							
							loadmap.Player_Base[(e_temp)] = e_it;

							it.JellyWin = 2;
						}

						if(loadmap.drag_it.tag == e_it.tag)
						{
							loadmap.drag_it = e_it;
						}

						//drawbase.UpdateBase ();
						collision = true; //Collision happened
						break;
					}
				}
			}

			// 2-2-2) Check with Nature's Base
			if(collision == false && it.state != JWDefine.BATTLE)
			{
				for(int n_temp = 0; n_temp < loadmap.Nature_Base.Count; n_temp++)
				{
					Base n_it = loadmap.Nature_Base[(n_temp)];

					CLRECT rect_nature = new CLRECT (n_it.position.x - n_it.size.x / 2, n_it.position.y - n_it.size.y / 2, n_it.size.x, n_it.size.y);
					
					if(CLCollisionChkRwithR(rect_player, rect_nature) == true) //Collision happened
					{
						if(Random.Range(0, 100) < InitSkillValue.Instance.P_Critical[InitSkillValue.Instance.skill_level_e[4]] + it.critical)
						{	ct_p = 2;	}
						else ct_p = 1;

						if(PLAYER == JWDefine.ENEMY) // 4. Snow Fortress Buff
						{
							if(it.affect == true) ct_p = 2;
						}

						it.eit_pos = new CLVEC2(n_it.position);
						it.eit_type = n_it.type;
						it.eit_owner = n_it.owner;
						it.eit_tag = n_it.tag;

						if(ct_p == 2)
						{
							//Critical Effect
							Effect critical = new Effect();
							critical.position = new CLVEC2 (it.position.x, it.position.y + 40);
							critical.anicount = 0;
							critical.count = 0;

							//Set Sprite Tag
							critical.tag = effect.index_critical;

							effect.Critical_Effect.Add(critical);
							effect.DrawNewEffect(critical, "Critical", effect.Critical_Effect.Count-1);
						}

						//Set state
						it.state = JWDefine.BATTLE;
						it.anicount = 0;
						it.aninum = 0;
						
						//Decide the Jelly which wins in Battle
						int n = 1;
						switch(n_it.type)			// 건물 젤리 1마리를 잡기 위한 공격 젤리수 (Power : Normal : 1 / Hero : 2).
						{
						case JWDefine.CASTLE:		
						case JWDefine.FORTRESS:
						case JWDefine.BARRACKS_2:	// 200%
							n = 2/it.ignore;		// Normal: 2.0 , Ignore: 1.0 , Hero: 1.0/2
							break;
						default:					// 100%
							break;
						}


						//Attack Effect
						Effect attack = new Effect();
						attack.position = new CLVEC2((it.position.x + it.eit_pos.x) / 2, (it.position.y + it.eit_pos.y) / 2);
						attack.anicount = 0;
						attack.count = 0;
						
						//Set Sprite Tag		
						attack.tag = effect.index_attack;

						effect.Attack_Effect.Add(attack);
						effect.DrawNewEffect(attack, "Attack", effect.Attack_Effect.Count-1);


						//Jelly Win		
						if((int)((float)it.num * (InitSkillValue.Instance.P_Power[InitSkillValue.Instance.skill_level_e[2]]+it.power)) * ct_p > n_it.num * n)
						{
							AudioManager.Instance.SFXPlay ("SFX_OCCUPY");
							
							//Occupy Effect
							Effect occupy = new Effect();
							occupy.position = it.eit_pos;
							occupy.anicount = 0;
							occupy.count = 0;
							
							//Set Sprite Tag	
							occupy.tag = effect.index_occupy;

							effect.Occupy_Effect.Add(occupy);
							effect.DrawNewEffect(occupy, "Occupy", effect.Occupy_Effect.Count-1);


							int killed = (int)(((float)n_it.num * (float)n) / ((InitSkillValue.Instance.P_Power[InitSkillValue.Instance.skill_level_e[2]]+it.power) * (float)ct_p));

							if(PLAYER == JWDefine.PLAYER)
							{
								JWGlobal.Instance.score_get += n_it.num * (HandleUserData.Instance.difficulty-750) * 72;
								JWGlobal.Instance.score_get += (HandleUserData.Instance.difficulty-750) * 720;
							}

							// Set Final Number
							n_it.num = it.num - killed;
							if(n_it.num < 0) n_it.num = 0;
							it.num = 0;

							//Change owner of base
							loadmap.Enemy_Base.Add(drawbase.MakeBase(n_it, JWDefine.ENEMY));
							loadmap.Nature_Base.RemoveAt(n_temp);
							drawbase.ChangeBase(PLAYER, it.eit_owner, it.eit_type, it.eit_tag);

							it.JellyWin = 1;
						}
						else
						{
							//Base Win
							
							AudioManager.Instance.SFXPlay("SFX_ATTACK");


							int killed = (int)(((float)it.num / (float)n) * ((InitSkillValue.Instance.P_Power[InitSkillValue.Instance.skill_level_e[2]]+it.power) * (float)ct_p));							             

							if(PLAYER == JWDefine.PLAYER)
							{
								JWGlobal.Instance.score_get += killed * (HandleUserData.Instance.difficulty-750) * 72;
							}

							n_it.num -= killed;

							if(n_it.num < 0) n_it.num = 0;
							it.num = 0;
							
							loadmap.Nature_Base[(n_temp)] = n_it;

							it.JellyWin = 2;
						}

						if(loadmap.drag_it.tag == n_it.tag)
						{
							loadmap.drag_it = n_it;
						}

						//drawbase.UpdateBase ();
						collision = true; //Collision happened
						break;
					}
				}
			}


			// 2-2-3) Check with My Base - No Battle
			if(collision == false && it.state != JWDefine.BATTLE)
			{
				for(int e_temp = 0; e_temp < loadmap.Enemy_Base.Count; e_temp++)
				{
					Base e_it = loadmap.Enemy_Base[(e_temp)];
					
					CLRECT rect_player_2 = new CLRECT (e_it.position.x - e_it.size.x / 2, e_it.position.y - e_it.size.y / 2, e_it.size.x, e_it.size.y);
					
					if(CLCollisionChkRwithR(rect_player, rect_player_2) == true) //Collision happened
					{
						if(it.seq_of_target >= it.num_of_move && it.num_of_move > 1) // it is a correct target base
						{
							AudioManager.Instance.SFXPlay("SFX_MAKE");
							
							//Make Effect
							Effect make = new Effect();
							make.position = new CLVEC2 (it.position.x, it.position.y + 40);
							make.count = 0;
							make.anicount = 0;

							//Set Sprite Tag		
							make.tag = effect.index_make;

							effect.Make_Effect.Add(make);
							effect.DrawNewEffect(make, "Make", effect.Make_Effect.Count-1);
							
							e_it.num += it.num; //Add Jellies to Player's Base
							it.num = 0;
							it.JellyWin = -1;
							
							loadmap.Enemy_Base[(e_temp)] = e_it;

							if(loadmap.drag_it.tag == e_it.tag)
							{
								loadmap.drag_it = e_it;
							}

							//drawbase.UpdateBase ();
							collision = true; //Collision happened
						}
						break;
					}
				}
				if(collision == true)
				{
					it.state = JWDefine.PEACE;
				}
			}

			
			//Update
			if(it.state == JWDefine.MOVE)
			{
				//Change Force Via Paths ([1]부터 시작: 시작 지점 제외).
				if(CLCollisionChkRwithV(new CLRECT(it.pointfortarget[it.seq_of_target].x-(4*HandleUserData.Instance.SPEED), it.pointfortarget[it.seq_of_target].y-(4*HandleUserData.Instance.SPEED), 8*HandleUserData.Instance.SPEED, 8*HandleUserData.Instance.SPEED), it.position))
				{
					//Add the Sequence of Target
					it.seq_of_target = it.seq_of_target+1;
					
					if(it.seq_of_target < it.num_of_move)
					{
						//Calculate Jelly's Forces( x, y )
						float px = it.pointfortarget[it.seq_of_target].x - it.position.x;
						float py = it.pointfortarget[it.seq_of_target].y - it.position.y;
						float length = Mathf.Sqrt(px*px + py*py);
						it.x_force = px / length;
						it.y_force = py / length;
						
						//Decide the direction of Jelly
						if(it.x_force > 0 && it.y_force > 0)
						{	it.direction = JWDefine.DIRECTION_86;	}
						else if(it.x_force < 0 && it.y_force > 0)
						{	it.direction = JWDefine.DIRECTION_84;	}
						else if(it.x_force < 0 && it.y_force < 0)
						{	it.direction = JWDefine.DIRECTION_24;	}
						else if(it.x_force > 0 && it.y_force < 0)
						{	it.direction = JWDefine.DIRECTION_26;	}
						else
						{
							if(it.x_force > 0)
							{	it.direction = JWDefine.DIRECTION_26+it.ran;	}
							else if(it.x_force < 0)
							{	it.direction = JWDefine.DIRECTION_84+it.ran;	}
							else if(it.y_force > 0)
							{	it.direction = JWDefine.DIRECTION_86+it.ran;	}
							else if(it.y_force < 0)
							{	it.direction = JWDefine.DIRECTION_24+it.ran;	}
						}
					}
				}
								
				//Adjust Jelly's Speed
				if(JWGlobal.Instance.obstacles > 0)
				{
					for (List<Obstacle>.Enumerator o_it = loadmap.Nature_Obstacle.GetEnumerator(); o_it.MoveNext(); )
					{
						CLRECT rect_obstacle = new CLRECT (o_it.Current.start.x, o_it.Current.start.y, o_it.Current.end.x - o_it.Current.start.x, o_it.Current.end.y - o_it.Current.start.y);
						
						if(CLCollisionChkRwithV(new CLRECT(rect_obstacle), new CLVEC2(it.position.x, it.position.y)))
						{
							if(it.y_force > 0)		it.adjust_speed = o_it.Current.up/100;
							else if(it.y_force < 0)	it.adjust_speed = o_it.Current.dn/100;
							else 					it.adjust_speed = o_it.Current.zero/100;
							break;
						}
						else it.adjust_speed = 1.0f;
					}
				}
				
				//Update Jelly's Position				
				it.position.x += it.x_force * HandleUserData.Instance.SPEED * (InitSkillValue.Instance.P_Speed[InitSkillValue.Instance.skill_level_e[3]]+it.speed) * it.adjust_speed * it.slow * it.fast;
				it.position.y += it.y_force * HandleUserData.Instance.SPEED * (InitSkillValue.Instance.P_Speed[InitSkillValue.Instance.skill_level_e[3]]+it.speed) * it.adjust_speed * it.slow * it.fast;
				it.pathcount += HandleUserData.Instance.SPEED;
				
				//Update Animation Count
				it.anicount++;
				if(it.anicount % 4 == 0) //Control Animation Speed
				{	it.aninum = (it.aninum + 1) % 4;	}

			}
			else if(it.state == JWDefine.BATTLE)
			{
				//Update Animation Count
				it.anicount++;
				if(it.anicount % 2 == 0)
				{
					it.aninum = (it.aninum) + 1;

					if(it.aninum >= 8) //The end of Battle Animation
					{
						it.aninum = 0;

						if(it.num > 0)
						{
							//Set State of Jelly
							it.state = JWDefine.MOVE;
						}
						else
						{
							it.state = JWDefine.PEACE;
						}
					}
				}
			}

			if(PLAYER == JWDefine.PLAYER)
				JWGlobal.Instance.total_jelly += it.num;
			else
				JWGlobal.Instance.total_jelly_e += it.num;


			drawjelly.Enemy_Jelly[(temp)] = it;

		}//End of Collision Check

		//drawjelly.Enemy_Jelly.RemoveAll(item => item.num == 0 && item.state == JWDefine.PEACE);
		//drawjelly.SortJelly ();
	}















	/// <summary>
	/// 애니메이션 이펙트 업데이트.
	/// (100%)
	/// </summary>
	void UpdateAnimation()
	{
		JWGlobal.Instance.particle = (JWGlobal.Instance.global_count%72)*5;

		//Star Effect
		for (int e = 0; e < loadmap.Nature_Star.Count; e++)
		{
			Star temp = loadmap.Nature_Star[e];
			
			temp.anicount++;
			if(temp.anicount % 30 == 0) //Control Animation Speed
			{	
				temp.aninum = (temp.aninum + 1) % 2;	
			}
			if(temp.anicount == 300)
				temp.anicount = 0;

			loadmap.Nature_Star[e] = temp;
			
			effect.UpdateE_Star (temp, e);
		}

		//Lockon Effect
		for (int e = 0; e < effect.Lockon_Effect.Count; e++)
		{
			Lockon temp = effect.Lockon_Effect[(e)];
			
			temp.count += (3*HandleUserData.Instance.SPEED);
			
			effect.Lockon_Effect[(e)] = temp;
			
			effect.UpdateE_Lockon (temp);
		}
		effect.Lockon_Effect.RemoveAll(item => item.count > 108);
		//SortLockonEffect (effect.Lockon_Effect, effect.pLockon);

		//Attack Effect
		for (int e = 0; e < effect.Attack_Effect.Count; e++)
		{
			Effect temp = effect.Attack_Effect[(e)];
			
			temp.anicount++;
			if(temp.anicount%2 == 0)
			{	temp.count++;	}
			
			effect.Attack_Effect[(e)] = temp;
			
			effect.UpdateE_Attack (temp);
		}
		effect.Attack_Effect.RemoveAll(item => item.anicount%2 == 1 && item.count == 4);
		//SortEffect (effect.Attack_Effect, effect.pAttack);

		//Occupy Effect
		for (int e = 0; e < effect.Occupy_Effect.Count; e++)
		{
			Effect temp = effect.Occupy_Effect[(e)];
			
			temp.anicount++;
			if(temp.anicount%2 == 0)
			{	temp.count++;	}
			
			effect.Occupy_Effect[(e)] = temp;
			
			effect.UpdateE_Occupy (temp);
		}
		effect.Occupy_Effect.RemoveAll(item => item.anicount%3 == 2 && item.count == 8);
		//SortEffect (effect.Occupy_Effect, effect.pOccupy);

		//Make Effect
		for (int e = 0; e < effect.Make_Effect.Count; e++)
		{
			Effect temp = effect.Make_Effect[(e)];
			
			temp.anicount++;
			if(temp.anicount > 1)
			{	temp.count++;	}
			
			effect.Make_Effect[(e)] = temp;
			
			effect.UpdateE_Make (temp);
		}
		effect.Make_Effect.RemoveAll(item => item.count == 4);
		//SortEffect (effect.Make_Effect, effect.pMake);

		//Critical Effect
		for (int e = 0; e < effect.Critical_Effect.Count; e++)
		{
			Effect temp = effect.Critical_Effect[(e)];
			
			temp.anicount++;
			if(temp.anicount%3 == 0)
			{	temp.count++;	}
			
			effect.Critical_Effect[(e)] = temp;
			
			effect.UpdateE_Critical (temp);
		}
		effect.Critical_Effect.RemoveAll(item => item.anicount%3 == 2 && item.count == 4);
		//SortEffect (effect.Critical_Effect, effect.pCritical);




		//Fire Effect
		for (int e = 0; e < effect.Fire_Effect.Count; e++)
		{
			Effect temp = effect.Fire_Effect[(e)];
			
			temp.anicount++;
			if(temp.anicount%2 == 0)
			{	temp.count++;	}
			
			effect.Fire_Effect[(e)] = temp;

			effect.UpdateSE_Fire (temp);
		}
		effect.Fire_Effect.RemoveAll(item => item.anicount%2 == 1 && item.count == 4);

		//Lightning Effect
		for (int e = 0; e < effect.Lightning_Effect.Count; e++)
		{
			Effect temp = effect.Lightning_Effect[(e)];
			
			temp.anicount++;
			if(temp.anicount%2 == 0)
			{	temp.count++;	}
			
			effect.Lightning_Effect[(e)] = temp;

			effect.UpdateSE_Lightning (temp);
		}
		effect.Lightning_Effect.RemoveAll(item => item.anicount%2 == 1 && item.count == 4);

		//Poison Effect
		for (int e = 0; e < effect.Poison_Effect.Count; e++)
		{
			Effect temp = effect.Poison_Effect[(e)];
			
			temp.anicount++;
			if(temp.anicount%5 == 0)
			{	temp.count++;	}
			
			if(JWGlobal.Instance.skill_cool[2] < (JWDefine.COOL_POISON-5))
			{
				temp.delete = true;
			}
			
			effect.Poison_Effect[(e)] = temp;

			effect.UpdateSE_Poison (temp);
		}
		effect.Poison_Effect.RemoveAll (item => item.delete == true);
		
		
		
		

		//Enemy Skill Effect
		for (int e = 0; e < loadmap.EnemySkill_Effect.Count; e++)
		{
			EnemySkill temp = loadmap.EnemySkill_Effect[(e)];
			
			if(JWGlobal.Instance.global_count<16)
			{
				if(JWGlobal.Instance.global_count == 1)
				{
					effect.pEnemySkill[e].transform.localPosition = new Vector2(temp.position.x-512f, temp.position.y);
					temp.test = true;
				}
				temp.anicount++;
				if(temp.anicount%4 == 0)
				{
					temp.count++;
					temp.speed += HandleUserData.Instance.SPEED;
				}
			}
			else if(temp.test)
			{
				temp.test = false;
				temp.anicount = 0;
				temp.count = 0;
				temp.speed = 0;
				effect.pEnemySkill[e].transform.localPosition = new Vector2(temp.position.x, temp.position.y);
			}
			
			if(temp.use == false)
			{
				if(JWGlobal.Instance.result_state == 0 && JWGlobal.Instance.global_count%(temp.interval/HandleUserData.Instance.SPEED) == 0)
				{	temp.use = true;	}
			}
			else
			{
				if(temp.anicount%4 == 0 && temp.speed >= 12*(HandleUserData.Instance.difficulty-749))
				{
					temp.use = false;
					temp.anicount = 0;
					temp.count = 0;
					temp.speed = 0;
				}
				else
				{
					temp.anicount++;
					if(temp.anicount%4 == 0)
					{
						temp.count++;
						temp.speed += HandleUserData.Instance.SPEED;
					}
				}
			}
			
			loadmap.EnemySkill_Effect[(e)] = temp;

			effect.UpdateE_EnemySkill(temp, e);
		}
	}


	/// <summary>
	/// 지속형 스킬 (포이즌) 제어 함수. 
	/// (100%)
	/// </summary>
	void ContinueSkill()
	{
		bool delete_poison = false;

		for(int temp_it = 0; temp_it < effect.Player_Skill.Count; temp_it++ )
		{
			Skill it = effect.Player_Skill[temp_it];

			if(it.type == JWDefine.ACTIVE_SKILL_POISON)
			{
				//With Enemy's Jellies
				for(int e_it = 0; e_it < drawjelly.Enemy_Jelly.Count; e_it++)
				{
					Jelly temp = drawjelly.Enemy_Jelly[(e_it)];

					if(CLCollisionChkCwithV(it.position, it.range, temp.position) == true) //Collision happened
					{
						if(JWGlobal.Instance.skill_cool[2] >= (JWDefine.COOL_POISON-5)) temp.slow = InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].slow;
						else temp.slow = 1.0f;
						
						//Poison
						if(JWGlobal.Instance.poisonattack == true)
						{
							if(temp.num > InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].damage)
							{
								JWGlobal.Instance.score_get += InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].damage * (HandleUserData.Instance.difficulty-750) * 60;
								temp.num -= InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].damage;
							}
							else if(temp.num > 1)
							{
								JWGlobal.Instance.score_get += (temp.num-1) * (HandleUserData.Instance.difficulty-750) * 60;
								temp.num = 1;
							}
						}
					}
					else 
					{
						temp.slow = 1.0f;
					}

					drawjelly.Enemy_Jelly[e_it] = temp;
				}

				//With Enemy's Bases
				for(int e_it = 0; e_it < loadmap.Enemy_Base.Count; e_it++)
				{
					Base temp = loadmap.Enemy_Base[(e_it)];

					if(CLCollisionChkCwithV(it.position, it.range, temp.position) == true) //Collision happened
					{
						if(JWGlobal.Instance.poisonattack == true)
						{
							if(temp.num > InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].damage)
							{
								JWGlobal.Instance.score_get += InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].damage * (HandleUserData.Instance.difficulty-750) * 36;
								temp.num -= InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].damage;
							}
							else if(temp.num > 1)
							{
								JWGlobal.Instance.score_get += (temp.num-1) * (HandleUserData.Instance.difficulty-750) * 36;
								temp.num = 1;
							}
						}
					}

					loadmap.Enemy_Base[(e_it)] = temp;
				}

				//With Nature's Bases
				for(int e_it = 0; e_it < loadmap.Nature_Base.Count; e_it++)
				{
					Base temp = loadmap.Nature_Base[(e_it)];

					if(CLCollisionChkCwithV(it.position, it.range, temp.position) == true) //Collision happened
					{
						//Debug.Log ("Collision");
						if(JWGlobal.Instance.poisonattack == true)
						{
							if(temp.num > InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].damage)
							{
								JWGlobal.Instance.score_get += InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].damage * (HandleUserData.Instance.difficulty-750) * 48;
								temp.num -= InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].damage;
							}
							else if(temp.num > 1)
							{
								JWGlobal.Instance.score_get += (temp.num-1) * (HandleUserData.Instance.difficulty-750) * 48;
								temp.num = 1;
							}
						}
					}

					loadmap.Nature_Base[(e_it)] = temp;
				}
			}


			it.count++;
			effect.Player_Skill[(temp_it)] = it;

			JWGlobal.Instance.poisonattack = false;

			//Delete Continue Skill
			switch(it.type)
			{
			case JWDefine.ACTIVE_SKILL_POISON:
				if(JWGlobal.Instance.skill_cool[2] < (JWDefine.COOL_POISON-5))
				{
					delete_poison = true;
				}
				break;
			default:
				break;
			}
		}

		if(delete_poison)
		{
			effect.Player_Skill.RemoveAll(item => item.type == JWDefine.ACTIVE_SKILL_POISON);
		}
	}

	/// <summary>
	/// 스테이지 끝내기 판별 함수 (100%)
	/// </summary>
	/// <param name="win_type">Win_type.</param>
	void FinishStage(int win_type)
	{
		switch(win_type)
		{
		case JWDefine.WIN_NORMAL:

			if(loadmap.Enemy_Base.Count == 0 && drawjelly.Enemy_Jelly.Count == 0)
			{
				JWGlobal.Instance.result_count_win++;
				if(JWGlobal.Instance.result_count_win == 10)	JWGlobal.Instance.score_get += JWGlobal.Instance.score_time;
				if(JWGlobal.Instance.result_count_win == 20)	JWGlobal.Instance.result_state = JWDefine.WIN;
			}
			if(loadmap.Player_Base.Count == 0 && drawjelly.Player_Jelly.Count == 0)
			{
				JWGlobal.Instance.result_count_lose++;
				if(JWGlobal.Instance.result_count_lose == 20)	JWGlobal.Instance.result_state = JWDefine.LOSE;
			}
			break;

		case JWDefine.WIN_TIMEATTACK:

			if(loadmap.Enemy_Base.Count == 0 && drawjelly.Enemy_Jelly.Count == 0)
			{
				JWGlobal.Instance.result_count_win++;
				if(JWGlobal.Instance.result_count_win == 10)	JWGlobal.Instance.score_get += JWGlobal.Instance.score_time;
				if(JWGlobal.Instance.result_count_win == 20)	JWGlobal.Instance.result_state = JWDefine.WIN;
			}
			if(JWGlobal.Instance.timeattack == 0 || (loadmap.Player_Base.Count == 0 && drawjelly.Player_Jelly.Count == 0))
			{
				JWGlobal.Instance.result_count_lose++;
				if(JWGlobal.Instance.result_count_lose == 20)	JWGlobal.Instance.result_state = JWDefine.LOSE;
			}
			break;

		case JWDefine.WIN_TIMEDEFENCE:

			if(JWGlobal.Instance.timeattack == 0 || (loadmap.Enemy_Base.Count == 0 && drawjelly.Enemy_Jelly.Count == 0))
			{
				JWGlobal.Instance.result_count_win++;
				if(JWGlobal.Instance.result_count_win == 10)	JWGlobal.Instance.score_get += JWGlobal.Instance.score_time;
				if(JWGlobal.Instance.result_count_win == 20)	JWGlobal.Instance.result_state = JWDefine.WIN;
			}
			if(loadmap.Player_Base.Count == 0 && drawjelly.Player_Jelly.Count == 0)
			{
				JWGlobal.Instance.result_count_lose++;
				if(JWGlobal.Instance.result_count_lose == 20)	JWGlobal.Instance.result_state = JWDefine.LOSE;
			}
			break;
		default:
			break;
		}
		
		if(JWGlobal.Instance.result_state == JWDefine.WIN)
		{
			Debug.Log("Perform : Finish Stage : Win");

			LoadResult();
			resultcalc.ResultCalculate();
		}
		else if(JWGlobal.Instance.result_state == JWDefine.LOSE)
		{
			Debug.Log("Perform : Finish Stage : Lose");

			LoadResult();
		}
	}

	void LoadResult()
	{
		JWGlobal.Instance.state = JWDefine.PAUSE;
		JWGlobal.Instance.global_count = 0;
		JWGlobal.Instance.what_skill_on = JWDefine.NO_SKILL;
		JWGlobal.Instance.skill_ready = false;
	}

	private static int CompareListByTag(Effect i1, Effect i2)
	{
		return i1.tag.CompareTo(i2.tag); 
	}
	private static int CompareListByTagL(Lockon i1, Lockon i2)
	{
		return i1.tag.CompareTo(i2.tag); 
	}
	private static int CompareListBySpriteTag(Transform i1, Transform i2)
	{
		return i1.GetComponent<PrefabSprite>().sprite_tag.CompareTo(i2.GetComponent<PrefabSprite>().sprite_tag); 
	}

	void SortEffect(List<Effect> effect, List<Transform> sprite)
	{
		if(effect.Count > 0)
		{
			effect.Sort (CompareListByTag);
			sprite.Sort (CompareListBySpriteTag);
		}
	}

	void SortLockonEffect(List<Lockon> effect, List<Transform> sprite)
	{
		if(effect.Count > 0)
		{
			effect.Sort (CompareListByTagL);
			sprite.Sort (CompareListBySpriteTag);
		}
	}

	static bool CLCollisionChkRwithR(CLRECT r1, CLRECT r2)
	{
		return ((r1.x >= r2.x && r1.x <= r2.x + r2.w && r1.y >= r2.y && r1.y <= r2.y + r2.h) || (r1.x + r1.w >= r2.x && r1.x + r1.w <= r2.x + r2.w && r1.y >= r2.y && r1.y <= r2.y + r2.h) || (r1.x >= r2.x && r1.x <= r2.x + r2.w && r1.y + r1.h >= r2.y && r1.y + r1.h <= r2.y + r2.h) || (r1.x + r1.w >= r2.x && r1.x + r1.w <= r2.x + r2.w && r1.y + r1.h >= r2.y && r1.y + r1.h <= r2.y + r2.h) || (r2.x >= r1.x && r2.x <= r1.x + r1.w && r2.y >= r1.y && r2.y <= r1.y + r1.h) || (r2.x + r2.w >= r1.x && r2.x + r2.w <= r1.x + r1.w && r2.y >= r1.y && r2.y <= r1.y + r1.h) || (r2.x >= r1.x && r2.x <= r1.x + r1.w && r2.y + r2.h >= r1.y && r2.y + r2.h <= r1.y + r1.h) || (r2.x + r2.w >= r1.x && r2.x + r2.w <= r1.x + r1.w && r2.y + r2.h >= r1.y && r2.y + r2.h <= r1.y + r1.h));
	}
	static bool CLCollisionChkRwithV(CLRECT r, CLVEC2 v)
	{
		return (v.x >= r.x && v.x <= r.x + r.w && v.y >= r.y && v.y <= r.y + r.h);
	}
	static bool CLCollisionChkCwithV(CLVEC2 v1, float r, CLVEC2 v2)
	{
		return ((v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y) <= r * r);
	}

}
