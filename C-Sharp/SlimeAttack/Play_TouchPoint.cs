using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct DragBox
{
	public CLVEC2 position;
	public CLRECT rect;

	public float px;
	public float py;
	public float x;
	public float y;
	public float w;
	public float h;
};

public class Play_TouchPoint : MonoBehaviour
{

	#region Variables

	public bool isUseAllAttack = false;
	
	Play_LoadMap loadmap;
	Play_DrawBase drawbase;
	Play_DrawJelly drawjelly;
	Play_DrawEffect effect;

	public Skill skill = new Skill();

	float clickdelay = 0.2f;
	float[] clicktime = new float[2];
	public bool tap_select1 = false;
	public int tap_select2 = 0;
	public int drag_select = 0;

	GameObject SkillRange = null;

	static CLRECT ICON_FIRE = new CLRECT(110f,0f,64f,64f);
	static CLRECT ICON_LIGHTNING = new CLRECT(180f,0f,64f,64f);
	static CLRECT ICON_POISON = new CLRECT(250f,0f,64f,64f);

	public DragBox Drag_box = new DragBox ();
	GameObject GoDragBox = null;
	static Vector2 dragboxini = new Vector2(-256f, 600f);

	public Vector2 NewPoint = Vector2.zero;
	public Vector2 MovePoint = Vector2.zero;
	public Vector2 ReleasePoint = Vector2.zero;
	Vector2 SavePoint = new Vector2 (-1000f , -1000f);

	/// <summary>
	/// The Point that Touch Started.
	/// If a Skill is activated, this changes to the Skill's Using Point.
	/// </summary>
	public CLVEC2 TouchPoint = new CLVEC2(0,0);

	bool isPress = false;
	bool isMoving = false;

	float margin_x = 0;
	float margin_y = 0;

	#endregion

	void Start () 
	{
		loadmap = GameObject.FindWithTag ("PLAY").GetComponent<Play_LoadMap> ();
		drawbase = GameObject.FindWithTag ("PLAY").transform.FindChild("Objects/Base").GetComponent<Play_DrawBase> ();
		drawjelly = GameObject.FindWithTag ("PLAY").transform.FindChild("Objects/Jelly").GetComponent<Play_DrawJelly> ();
		effect =  GameObject.FindWithTag ("PLAY").transform.FindChild("Objects/Effect").GetComponent<Play_DrawEffect> ();
		SkillRange = GameObject.FindWithTag ("Range");
		GoDragBox = GameObject.FindWithTag ("DRAGBOX");

		float targetaspect = 480.0f / 800.0f;	
		
		// determine the game window's current aspect ratio
		float width = (float)Screen.width;
		float height = (float)Screen.height;
		float windowaspect = width / height;
		
		// current viewport height should be scaled by this amount
		float scaleheight = windowaspect / targetaspect;		
		
		// if scaled height is less than current height, add letterbox
		if (scaleheight < 1.0f)
		{  					
			margin_y = (1.0f - scaleheight) / 2.0f;
			margin_y *= (float)Screen.height;
		} 
		else 
		{ 				
			float scalewidth = 1.0f / scaleheight;			
			margin_x = (1.0f - scalewidth) / 2.0f;
			margin_x *= (float)Screen.width;
		}

		isMoving = false;

		clicktime [0] = -10f;


		if (isUseAllAttack)
			JWGlobal.Instance.isAllAttack = true;
		else
			JWGlobal.Instance.isAllAttack = false;
	}

	public void Initialize()
	{
		Drag_box.rect = new CLRECT (0, 0, 0, 0);
		Drag_box.w = 0;
		Drag_box.h = 0;

		try{
			GoDragBox.GetComponent<UISprite>().width = 0;
			GoDragBox.GetComponent<UISprite>().height = 0;
		}catch{}
	}

	void Update()
	{
		if(JWGlobal.Instance.state == JWDefine.PLAY && JWGlobal.Instance.drag_lock == false)
		{
			if (isMoving) isMoving = false;

			if (!isPress && ((Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.touchCount == 0 && Input.GetMouseButtonDown(0))))
			{
				isPress = true;
				Press();
			}
			else if (isPress && ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || (Input.touchCount == 0 && Input.GetMouseButton(0))))
			{
				if(Input.touchCount > 0)
				{
					MovePoint.x = (Input.GetTouch(0).position.x-margin_x) * (480f / ((float)Screen.width-(2*margin_x))) ;
					MovePoint.y = (Input.GetTouch(0).position.y-margin_y) * (800f / ((float)Screen.height-(2*margin_y))) ;
				}
				else
				{
					MovePoint.x = (Input.mousePosition.x-margin_x) * (480f / ((float)Screen.width-(2*margin_x))) ;
					MovePoint.y = (Input.mousePosition.y-margin_y) * (800f / ((float)Screen.height-(2*margin_y))) ;
				}

				isMoving = true;
			}
			else if (isPress && ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || (Input.touchCount == 0 && Input.GetMouseButtonUp(0))))
			{
				isPress = false;
				Release();
			}

			if(isMoving && (Mathf.Abs (NewPoint.x-MovePoint.x) > 10f || Mathf.Abs (NewPoint.y-MovePoint.y) > 10f))
			{
				OnTouchMoved(MovePoint);
			}
		}
	}
	
	public void Press()
	{
		if(JWGlobal.Instance.state == JWDefine.PLAY && JWGlobal.Instance.drag_lock == false)
		{
			clicktime[1] = Time.time;

			if(Input.touchCount == 1)
			{
				NewPoint.x = (Input.GetTouch(0).position.x-margin_x) * (480f / ((float)Screen.width-(2*margin_x))) ;
				NewPoint.y = (Input.GetTouch(0).position.y-margin_y) * (800f / ((float)Screen.height-(2*margin_y))) ;
			}
			else
			{
				NewPoint.x = (Input.mousePosition.x-margin_x) * (480f / ((float)Screen.width-(2*margin_x))) ;
				NewPoint.y = (Input.mousePosition.y-margin_y) * (800f / ((float)Screen.height-(2*margin_y))) ;
			}

			if((HandleUserData.Instance.double_onoff == 1) && (clicktime[1] - clicktime[0] < clickdelay) && (Mathf.Abs(NewPoint.x - SavePoint.x) < 50 && Mathf.Abs(NewPoint.y - SavePoint.y) < 50))
			{
				if(isUseAllAttack && JWGlobal.Instance.isAllAttack)
				{
					//
				}
				else
				{
					OnTouchDoublePressed(NewPoint);
				}

				clicktime[0] = -10f;
			}
			else
			{
				OnTouchPressed(NewPoint);

				clicktime[0] = clicktime[1];
			}

			SavePoint = NewPoint;
			ReleasePoint = NewPoint;			
		}
	}

	public void Release()
	{
		if(JWGlobal.Instance.state == JWDefine.PLAY && JWGlobal.Instance.drag_lock == false)
		{
			if(Input.touchCount > 0)
			{
				ReleasePoint.x = (Input.GetTouch(0).position.x-margin_x) * (480f / ((float)Screen.width-(2*margin_x))) ;
				ReleasePoint.y = (Input.GetTouch(0).position.y-margin_y) * (800f / ((float)Screen.height-(2*margin_y))) ;			
			}
			else
			{
				ReleasePoint.x = (Input.mousePosition.x-margin_x) * (480f / ((float)Screen.width-(2*margin_x))) ;
				ReleasePoint.y = (Input.mousePosition.y-margin_y) * (800f / ((float)Screen.height-(2*margin_y))) ;			
			}

			OnTouchReleased(ReleasePoint);

			//Debug.Log (SavePoint + " -> " +ReleasePoint);
		}

		NewPoint.x = 0;
		NewPoint.y = 0;
		MovePoint.x = 0;
		MovePoint.y = 0;
		TouchPoint.x = 0;
		TouchPoint.y = 0;
	}
	
	void OnTouchDoublePressed(Vector2 currentPosition) 
	{	
		TouchPoint.x = currentPosition.x;
		TouchPoint.y = currentPosition.y;

		////////////// TOUCH_DOUBLE /////////////
		if (HandleUserData.Instance.double_onoff == 1)
		{
			#region [OnTouchDoublePressed] 모든 건물 선택.
			if (JWGlobal.Instance.start_count == 0)
			{
				if(JWGlobal.Instance.state == JWDefine.PLAY && JWGlobal.Instance.start_count == 0)
				{
					for(int temp = 0; temp < loadmap.Player_Base.Count; temp++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
					{
						Base it = loadmap.Player_Base[temp];

						if(CLCollisionChkRwithV(new CLRECT(it.position.x-45f,it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, currentPosition.y)))
						{
							tap_select2 = 1;
							drag_select = 0;
							loadmap.drag_it = it;

							for(int temp_2 = 0; temp_2 < loadmap.Player_Base.Count; temp_2++)// for (list<Base>::iterator it_2 = Player_Base.begin(); it_2 != Player_Base.end(); ++it_2)
							{
								Base it_2 = loadmap.Player_Base[temp_2];
								it_2.selected = true;
								drag_select++;

								loadmap.Player_Base[temp_2] = it_2;
							}
							break;
						}
					}
					
					if(JWGlobal.Instance.what_skill_on != JWDefine.NO_SKILL)
					{
						JWGlobal.Instance.what_skill_on = JWDefine.NO_SKILL;
						JWGlobal.Instance.skill_ready = false;
						JWGlobal.Instance.skill_activate = false;
						JWGlobal.Instance.skill_fire = false;
						JWGlobal.Instance.skill_lightning = false;
						JWGlobal.Instance.skill_poison = false;
						JWGlobal.Instance.level = 0;
						JWGlobal.Instance.cost = 0;
					}
				}
			}
			#endregion
		}
		////////////// TOUCH_DOUBLE /////////////		
	}
		
	void OnTouchPressed(Vector2 currentPosition)
	{
		if(JWGlobal.Instance.start_count == 0)
		{
			TouchPoint.x = currentPosition.x;
			TouchPoint.y = currentPosition.y;

			#region [OnTouchPressed] 건물 선택 시작.

			if (JWGlobal.Instance.what_skill_on == JWDefine.NO_SKILL && currentPosition.y > 90f)
			{
				if(HandleUserData.Instance.touch_type == 0)//Touch:Tap
				{
					#region [OnTouchPressed] control type : Tap
					for (int temp = 0; temp < loadmap.Player_Base.Count; temp++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
					{
						Base it = loadmap.Player_Base[temp];

						if(CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, currentPosition.y)))
						{
							if(it.selected == false)
							{
								if(loadmap.drag_it.owner == JWDefine.ENEMY) 
								{
									for(int temp_e = 0; temp_e < loadmap.Enemy_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
									{
										if(loadmap.Enemy_Base[temp_e].tag == loadmap.drag_it.tag)
										{
											loadmap.drag_it = loadmap.Enemy_Base[temp_e];
											loadmap.drag_it.selected = false;
											loadmap.Enemy_Base[temp_e] = loadmap.drag_it;
											break;
										}
									}

									tap_select2 = 1;
								}
								else if(loadmap.drag_it.owner == JWDefine.NATURE)
								{
									for(int temp_e = 0; temp_e < loadmap.Nature_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
									{
										if(loadmap.Nature_Base[temp_e].tag == loadmap.drag_it.tag)
										{
											loadmap.drag_it = loadmap.Nature_Base[temp_e];
											loadmap.drag_it.selected = false;
											loadmap.Nature_Base[temp_e] = loadmap.drag_it;
											break;
										}
									}

									tap_select2 = 1;
								}
								else
								{
									tap_select2++;
								}
								
								it.selected = true;

								loadmap.Player_Base[temp] = it;
							}
							else tap_select2 = 2;
							
							tap_select1 = true;
							loadmap.drag_it = it;
							break;
						}
					}
					if(tap_select1 == false)
					{
						for(int temp = 0; temp < loadmap.Nature_Base.Count; temp++)//for (list<Base>::iterator it = Nature_Base.begin(); it != Nature_Base.end(); ++it)
						{
							Base it = loadmap.Nature_Base[temp];

							if(CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, currentPosition.y)))
							{
								if(it.selected == false)
								{
									if(loadmap.drag_it.owner == JWDefine.ENEMY) 
									{										
										for(int temp_e = 0; temp_e < loadmap.Enemy_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
										{
											if(loadmap.Enemy_Base[temp_e].tag == loadmap.drag_it.tag)
											{
												loadmap.drag_it = loadmap.Enemy_Base[temp_e];
												loadmap.drag_it.selected = false;
												loadmap.Enemy_Base[temp_e] = loadmap.drag_it;
												break;
											}
										}
										
										tap_select2 = 1;
									}
									else if(loadmap.drag_it.owner == JWDefine.NATURE)
									{										
										for(int temp_e = 0; temp_e < loadmap.Nature_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
										{
											if(loadmap.Nature_Base[temp_e].tag == loadmap.drag_it.tag)
											{
												loadmap.drag_it = loadmap.Nature_Base[temp_e];
												loadmap.drag_it.selected = false;
												loadmap.Nature_Base[temp_e] = loadmap.drag_it;
												break;
											}
										}
										
										tap_select2 = 1;
									}
									else
									{
										tap_select2++;
									}
									
									it.selected = true;

									loadmap.Nature_Base[temp] = it;
								}
								else tap_select2 = 0;
								
								tap_select1 = true;
								loadmap.drag_it = it;
								break;
							}
						}
					}
					if(tap_select1 == false)
					{
						for(int temp = 0; temp < loadmap.Enemy_Base.Count; temp++)//for (list<Base>::iterator it = Enemy_Base.begin(); it != Enemy_Base.end(); ++it)
						{
							Base it = loadmap.Enemy_Base[temp];

							if(CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, currentPosition.y)))
							{
								if(it.selected == false)
								{
									if(loadmap.drag_it.owner == JWDefine.ENEMY) 
									{										
										for(int temp_e = 0; temp_e < loadmap.Enemy_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
										{
											if(loadmap.Enemy_Base[temp_e].tag == loadmap.drag_it.tag)
											{
												loadmap.drag_it = loadmap.Enemy_Base[temp_e];
												loadmap.drag_it.selected = false;
												loadmap.Enemy_Base[temp_e] = loadmap.drag_it;
												break;
											}
										}
										
										tap_select2 = 1;
									}
									else if(loadmap.drag_it.owner == JWDefine.NATURE)
									{										
										for(int temp_e = 0; temp_e < loadmap.Nature_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
										{
											if(loadmap.Nature_Base[temp_e].tag == loadmap.drag_it.tag)
											{
												loadmap.drag_it = loadmap.Nature_Base[temp_e];
												loadmap.drag_it.selected = false;
												loadmap.Nature_Base[temp_e] = loadmap.drag_it;
												break;
											}
										}
										
										tap_select2 = 1;
									}
									else
									{
										tap_select2++;
									}
									
									it.selected = true;

									loadmap.Enemy_Base[temp] = it;
								}
								else tap_select2 = 0;
								
								tap_select1 = true;
								loadmap.drag_it = it;
								break;
							}
						}
					}
					if(tap_select1 == false)
					{	tap_select2 = 0; }




					#endregion
				}
				else//Touch:Drag
				{
					#region [OnTouchPressed] control type : Drag
					for (int temp = 0; temp < loadmap.Player_Base.Count; temp++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
					{
						Base it = loadmap.Player_Base[temp];

						if(CLCollisionChkRwithV(new CLRECT(it.position.x-45, it.position.y-45, 90, 90), new CLVEC2(currentPosition.x, currentPosition.y)))
						{
							if(it.selected == false)
							{
								if(HandleUserData.Instance.touch_type == 0)
								{
									if(loadmap.drag_it.owner == JWDefine.ENEMY) 
									{										
										for(int temp_e = 0; temp_e < loadmap.Enemy_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
										{
											if(loadmap.Enemy_Base[temp_e].tag == loadmap.drag_it.tag)
											{
												loadmap.drag_it = loadmap.Enemy_Base[temp_e];
												loadmap.drag_it.selected = false;
												loadmap.Enemy_Base[temp_e] = loadmap.drag_it;
												break;
											}
										}
									}
									else if(loadmap.drag_it.owner == JWDefine.NATURE)
									{										
										for(int temp_e = 0; temp_e < loadmap.Nature_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
										{
											if(loadmap.Nature_Base[temp_e].tag == loadmap.drag_it.tag)
											{
												loadmap.drag_it = loadmap.Nature_Base[temp_e];
												loadmap.drag_it.selected = false;
												loadmap.Nature_Base[temp_e] = loadmap.drag_it;
												break;
											}
										}
									}
								}

								it.selected = true;

								loadmap.Player_Base[temp] = it;
								loadmap.drag_it = it;
								drag_select = 1;
								break;
							}
						}
					}

					if(drag_select == 0)
					{
						for(int temp = 0; temp < loadmap.Nature_Base.Count; temp++)//for (list<Base>::iterator it = Nature_Base.begin(); it != Nature_Base.end(); ++it)
						{
							Base it = loadmap.Nature_Base[temp];

							if(CLCollisionChkRwithV(new CLRECT(it.position.x-45, it.position.y-45, 90, 90), new CLVEC2(currentPosition.x, currentPosition.y)))
							{
								if(HandleUserData.Instance.touch_type == 0)
								{
									if(loadmap.drag_it.owner == JWDefine.ENEMY) 
									{										
										for(int temp_e = 0; temp_e < loadmap.Enemy_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
										{
											if(loadmap.Enemy_Base[temp_e].tag == loadmap.drag_it.tag)
											{
												loadmap.drag_it = loadmap.Enemy_Base[temp_e];
												loadmap.drag_it.selected = false;
												loadmap.Enemy_Base[temp_e] = loadmap.drag_it;
												break;
											}
										}
									}
									else if(loadmap.drag_it.owner == JWDefine.NATURE)
									{										
										for(int temp_e = 0; temp_e < loadmap.Nature_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
										{
											if(loadmap.Nature_Base[temp_e].tag == loadmap.drag_it.tag)
											{
												loadmap.drag_it = loadmap.Nature_Base[temp_e];
												loadmap.drag_it.selected = false;
												loadmap.Nature_Base[temp_e] = loadmap.drag_it;
												break;
											}
										}
									}
								}
										
								it.selected = true;

								loadmap.Nature_Base[temp] = it;
								loadmap.drag_it = it;
								drag_select = -1;
								break;
							}
						}
					}
					if(drag_select == 0)
					{
						for(int temp = 0; temp < loadmap.Enemy_Base.Count; temp++)//for (list<Base>::iterator it = Enemy_Base.begin(); it != Enemy_Base.end(); ++it)
						{
							Base it = loadmap.Enemy_Base[temp];

							if(CLCollisionChkRwithV(new CLRECT(it.position.x-45, it.position.y-45, 90, 90), new CLVEC2(currentPosition.x, currentPosition.y)))
							{
								if(HandleUserData.Instance.touch_type == 0)
								{
									if(loadmap.drag_it.owner == JWDefine.ENEMY) 
									{										
										for(int temp_e = 0; temp_e < loadmap.Enemy_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
										{
											if(loadmap.Enemy_Base[temp_e].tag == loadmap.drag_it.tag)
											{
												loadmap.drag_it = loadmap.Enemy_Base[temp_e];
												loadmap.drag_it.selected = false;
												loadmap.Enemy_Base[temp_e] = loadmap.drag_it;
												break;
											}
										}
									}
									else if(loadmap.drag_it.owner == JWDefine.NATURE)
									{										
										for(int temp_e = 0; temp_e < loadmap.Nature_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
										{
											if(loadmap.Nature_Base[temp_e].tag == loadmap.drag_it.tag)
											{
												loadmap.drag_it = loadmap.Nature_Base[temp_e];
												loadmap.drag_it.selected = false;
												loadmap.Nature_Base[temp_e] = loadmap.drag_it;
												break;
											}
										}
									}
								}
																
								it.selected = true;

								loadmap.Enemy_Base[temp] = it;
								loadmap.drag_it = it;
								drag_select = -2;
								break;
							}
						}
					}
					#endregion
				}
			}
			else tap_select2 = 0;

			#endregion

			#region [OnTouchPressed] 스킬 선택 시작.

			//Press Skill Icon
			if (isUseAllAttack && JWGlobal.Instance.isAllAttack)
			{
				#region [OnTouchPressed] 혼합 방식.

				if (currentPosition.y <= 64f)
				{
					JWGlobal.Instance.what_skill_on = JWDefine.NO_SKILL;
					JWGlobal.Instance.skill_ready = false;

					if (HandleUserData.Instance.skill_level[0] > 0 && CLCollisionChkRwithV(ICON_FIRE, new CLVEC2(currentPosition.x, currentPosition.y))) //Fire Skill
					{
						if (JWGlobal.Instance.skill_fire == true)
						{
							JWGlobal.Instance.level = 0;
							JWGlobal.Instance.cost = 0;
							JWGlobal.Instance.skill_fire = false;
							JWGlobal.Instance.skill_activate = false;
							JWGlobal.Instance.skill_move0 = false;
						}
						else
						{
							JWGlobal.Instance.cost = InitSkillValue.Instance.A_Fire[HandleUserData.Instance.skill_level[0]].cost;
							JWGlobal.Instance.skill_push = true;

							if (JWGlobal.Instance.skill_cool[0] == 0)
							{
								JWGlobal.Instance.what_skill_on = JWDefine.ACTIVE_SKILL_FIRE;
								TouchPoint.x = 142f;
								TouchPoint.y = 32f;
								skill.range = 50;
								JWGlobal.Instance.level = HandleUserData.Instance.skill_level[0];
								JWGlobal.Instance.skill_fire = true;
								JWGlobal.Instance.skill_lightning = false;
								JWGlobal.Instance.skill_poison = false;
								JWGlobal.Instance.skill_move0 = true;
							}
							else
							{
								JWGlobal.Instance.skill_move0 = false;
							}
						}
					}
					else if (HandleUserData.Instance.skill_level[1] > 0 && CLCollisionChkRwithV(ICON_LIGHTNING, new CLVEC2(currentPosition.x, currentPosition.y)))
					{
						if (JWGlobal.Instance.skill_lightning == true)
						{
							JWGlobal.Instance.level = 0;
							JWGlobal.Instance.cost = 0;
							JWGlobal.Instance.skill_lightning = false;
							JWGlobal.Instance.skill_activate = false;
							JWGlobal.Instance.skill_move0 = false;
						}
						else
						{
							JWGlobal.Instance.cost = InitSkillValue.Instance.A_Lightning[HandleUserData.Instance.skill_level[1]].cost;
							JWGlobal.Instance.skill_push = true;

							if (JWGlobal.Instance.skill_cool[1] == 0)
							{
								JWGlobal.Instance.what_skill_on = JWDefine.ACTIVE_SKILL_LIGHTNING;
								TouchPoint.x = 212f;
								TouchPoint.y = 32f;
								skill.range = 100;
								JWGlobal.Instance.level = HandleUserData.Instance.skill_level[1];
								JWGlobal.Instance.skill_fire = false;
								JWGlobal.Instance.skill_lightning = true;
								JWGlobal.Instance.skill_poison = false;
								JWGlobal.Instance.skill_move0 = true;
							}
							else
							{
								JWGlobal.Instance.skill_move0 = false;
							}
						}
					}
					else if (HandleUserData.Instance.skill_level[2] > 0 && CLCollisionChkRwithV(ICON_POISON, new CLVEC2(currentPosition.x, currentPosition.y)))
					{
						if (JWGlobal.Instance.skill_poison == true)
						{
							JWGlobal.Instance.level = 0;
							JWGlobal.Instance.cost = 0;
							JWGlobal.Instance.skill_poison = false;
							JWGlobal.Instance.skill_activate = false;
							JWGlobal.Instance.skill_move0 = false;
						}
						else
						{
							JWGlobal.Instance.cost = InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].cost;
							JWGlobal.Instance.skill_push = true;

							if (JWGlobal.Instance.skill_cool[2] == 0)
							{
								JWGlobal.Instance.what_skill_on = JWDefine.ACTIVE_SKILL_POISON;
								TouchPoint.x = 282f;
								TouchPoint.y = 32f;
								skill.range = 100;
								JWGlobal.Instance.level = HandleUserData.Instance.skill_level[2];
								JWGlobal.Instance.skill_fire = false;
								JWGlobal.Instance.skill_lightning = false;
								JWGlobal.Instance.skill_poison = true;
								JWGlobal.Instance.skill_move0 = true;
							}
							else
							{
								JWGlobal.Instance.skill_move0 = false;
							}
						}
					}
					else
					{
						JWGlobal.Instance.skill_activate = false;
						JWGlobal.Instance.skill_fire = false;
						JWGlobal.Instance.skill_lightning = false;
						JWGlobal.Instance.skill_poison = false;
						JWGlobal.Instance.level = 0;
						JWGlobal.Instance.cost = 0;

						JWGlobal.Instance.skill_move0 = false;
						JWGlobal.Instance.skill_move = false;
					}
				}

				if (JWGlobal.Instance.skill_activate == true)
				{
					if (currentPosition.y > 64f)
					{
						JWGlobal.Instance.skill_move0 = true;
						JWGlobal.Instance.skill_activate = false;
						JWGlobal.Instance.tap_x = currentPosition.x;
						JWGlobal.Instance.tap_y = currentPosition.y;
					}
				}

				#endregion
			}
			else
			{
				#region [OnTouchPressed] 기존 방식.

				if (HandleUserData.Instance.skill_type == 0)//Skill:Tap
				{
					#region [OnTouchPressed] skill type : Tap
					if (currentPosition.y <= 64f)
					{
						JWGlobal.Instance.what_skill_on = JWDefine.NO_SKILL;
						JWGlobal.Instance.skill_ready = false;

						if (HandleUserData.Instance.skill_level[0] > 0 && CLCollisionChkRwithV(ICON_FIRE, new CLVEC2(currentPosition.x, currentPosition.y)))
						{
							if (JWGlobal.Instance.skill_fire == true)
							{
								JWGlobal.Instance.level = 0;
								JWGlobal.Instance.cost = 0;
								JWGlobal.Instance.skill_fire = false;
								JWGlobal.Instance.skill_activate = false;
							}
							else
							{
								JWGlobal.Instance.cost = InitSkillValue.Instance.A_Fire[HandleUserData.Instance.skill_level[0]].cost;
								JWGlobal.Instance.skill_push = true;

								if (JWGlobal.Instance.skill_cool[0] == 0)
								{
									JWGlobal.Instance.what_skill_on = JWDefine.ACTIVE_SKILL_FIRE;
									TouchPoint.x = 142f;
									TouchPoint.y = 32f;
									skill.range = 50;
									JWGlobal.Instance.level = HandleUserData.Instance.skill_level[0];
									JWGlobal.Instance.skill_fire = true;
									JWGlobal.Instance.skill_lightning = false;
									JWGlobal.Instance.skill_poison = false;
								}
							}
						}
						else if (HandleUserData.Instance.skill_level[1] > 0 && CLCollisionChkRwithV(ICON_LIGHTNING, new CLVEC2(currentPosition.x, currentPosition.y)))
						{
							if (JWGlobal.Instance.skill_lightning == true)
							{
								JWGlobal.Instance.level = 0;
								JWGlobal.Instance.cost = 0;
								JWGlobal.Instance.skill_lightning = false;
								JWGlobal.Instance.skill_activate = false;
							}
							else
							{
								JWGlobal.Instance.cost = InitSkillValue.Instance.A_Lightning[HandleUserData.Instance.skill_level[1]].cost;
								JWGlobal.Instance.skill_push = true;

								if (JWGlobal.Instance.skill_cool[1] == 0)
								{
									JWGlobal.Instance.what_skill_on = JWDefine.ACTIVE_SKILL_LIGHTNING;
									TouchPoint.x = 212f;
									TouchPoint.y = 32f;
									skill.range = 100;
									JWGlobal.Instance.level = HandleUserData.Instance.skill_level[1];
									JWGlobal.Instance.skill_fire = false;
									JWGlobal.Instance.skill_lightning = true;
									JWGlobal.Instance.skill_poison = false;
								}
							}
						}
						else if (HandleUserData.Instance.skill_level[2] > 0 && CLCollisionChkRwithV(ICON_POISON, new CLVEC2(currentPosition.x, currentPosition.y)))
						{
							if (JWGlobal.Instance.skill_poison == true)
							{
								JWGlobal.Instance.level = 0;
								JWGlobal.Instance.cost = 0;
								JWGlobal.Instance.skill_poison = false;
								JWGlobal.Instance.skill_activate = false;
							}
							else
							{
								JWGlobal.Instance.cost = InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].cost;
								JWGlobal.Instance.skill_push = true;

								if (JWGlobal.Instance.skill_cool[2] == 0)
								{
									JWGlobal.Instance.what_skill_on = JWDefine.ACTIVE_SKILL_POISON;
									TouchPoint.x = 282f;
									TouchPoint.y = 32f;
									skill.range = 100;
									JWGlobal.Instance.level = HandleUserData.Instance.skill_level[2];
									JWGlobal.Instance.skill_fire = false;
									JWGlobal.Instance.skill_lightning = false;
									JWGlobal.Instance.skill_poison = true;
								}
							}
						}
						else
						{
							JWGlobal.Instance.skill_activate = false;
							JWGlobal.Instance.skill_fire = false;
							JWGlobal.Instance.skill_lightning = false;
							JWGlobal.Instance.skill_poison = false;
							JWGlobal.Instance.level = 0;
							JWGlobal.Instance.cost = 0;
						}


					}

					if (JWGlobal.Instance.skill_activate == true)
					{
						if (currentPosition.y > 64f)
						{
							JWGlobal.Instance.skill_move0 = true;
							JWGlobal.Instance.skill_activate = false;
							JWGlobal.Instance.tap_x = currentPosition.x;
							JWGlobal.Instance.tap_y = currentPosition.y;
						}
					}
					#endregion
				}
				else//Skill:Drag
				{
					#region [OnTouchPressed] skill type : Drag
					if (HandleUserData.Instance.skill_level[0] > 0 && CLCollisionChkRwithV(ICON_FIRE, new CLVEC2(currentPosition.x, currentPosition.y))) //Fire Skill
					{
						JWGlobal.Instance.what_skill_on = JWDefine.ACTIVE_SKILL_FIRE;
						TouchPoint.x = 142f;
						TouchPoint.y = 32f;
						skill.range = 50;
						JWGlobal.Instance.level = HandleUserData.Instance.skill_level[0];
						JWGlobal.Instance.cost = InitSkillValue.Instance.A_Fire[HandleUserData.Instance.skill_level[0]].cost;
						JWGlobal.Instance.skill_fire = true;
						JWGlobal.Instance.skill_move0 = true;


					}
					if (HandleUserData.Instance.skill_level[1] > 0 && CLCollisionChkRwithV(ICON_LIGHTNING, new CLVEC2(currentPosition.x, currentPosition.y))) //Ligthning Skill
					{
						JWGlobal.Instance.what_skill_on = JWDefine.ACTIVE_SKILL_LIGHTNING;
						TouchPoint.x = 212f;
						TouchPoint.y = 32f;
						skill.range = 100;
						JWGlobal.Instance.level = HandleUserData.Instance.skill_level[1];
						JWGlobal.Instance.cost = InitSkillValue.Instance.A_Lightning[HandleUserData.Instance.skill_level[1]].cost;
						JWGlobal.Instance.skill_lightning = true;
						JWGlobal.Instance.skill_move0 = true;


					}
					if (HandleUserData.Instance.skill_level[2] > 0 && CLCollisionChkRwithV(ICON_POISON, new CLVEC2(currentPosition.x, currentPosition.y))) //Poison skill
					{
						JWGlobal.Instance.what_skill_on = JWDefine.ACTIVE_SKILL_POISON;
						TouchPoint.x = 282f;
						TouchPoint.y = 32f;
						skill.range = 100;
						JWGlobal.Instance.level = HandleUserData.Instance.skill_level[2];
						JWGlobal.Instance.cost = InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].cost;
						JWGlobal.Instance.skill_poison = true;
						JWGlobal.Instance.skill_move0 = true;


					}
					#endregion
				}

				#endregion
			}

			if (JWGlobal.Instance.what_skill_on != JWDefine.NO_SKILL)
			{
				#region [OnTouchPressed] 건물 선택 해제 / 스킬 아이콘 업데이트.
				SkillRange.GetComponent<PrefabSprite>().List[0].SetActive(false);
				SkillRange.GetComponent<PrefabSprite>().List[1].SetActive(false);
				SkillRange.GetComponent<PrefabSprite>().List[2].SetActive(false);
				SkillRange.GetComponent<PrefabSprite>().List[3].SetActive(false);

				drawbase.CancelAllSelect();
				UpdateRangePosition();
				#endregion
			}

			#endregion
		}
	}

	void OnTouchMoved(Vector2 currentPosition) 
	{
		if(JWGlobal.Instance.start_count == 0)
		{
			#region [OnTouchMoved] 화면 드래그 진행.

			if (isUseAllAttack && JWGlobal.Instance.isAllAttack)
			{
				#region [OnTouchMoved] 전체 방식.

				if (JWGlobal.Instance.skill_move0 == true)
				{
					//TODO: SKILL
					#region [OnTouchMoved] 스킬 사용 지점 이동.

					//Debug.Log ("**********************************************************Skill Move");					

					TouchPoint.x = currentPosition.x;
					TouchPoint.y = currentPosition.y;


					if (JWGlobal.Instance.skill_move == false)
					{
						if (Mathf.Abs(TouchPoint.x - JWGlobal.Instance.tap_x) > 16f || Mathf.Abs(TouchPoint.y - JWGlobal.Instance.tap_y) > 16f)
						{
							JWGlobal.Instance.skill_push = false;
							JWGlobal.Instance.skill_move = true;							
						}
						else
						{
							JWGlobal.Instance.skill_push = true;
							JWGlobal.Instance.skill_move = false;
						}

						/*
						if (HandleUserData.Instance.skill_type == 0)//Tap
						{
							if (Mathf.Abs(TouchPoint.x - JWGlobal.Instance.tap_x) > 16f || Mathf.Abs(TouchPoint.y - JWGlobal.Instance.tap_y) > 16f)
							{
								JWGlobal.Instance.skill_move = true;								
							}
						}
						else//Drag
						{
							JWGlobal.Instance.skill_move = true;							
						}*/												
					}

					TouchPoint.y += skill.range * (HandleUserData.Instance.range_adjust - 1);
					if (TouchPoint.y > 800f) TouchPoint.y = 800f;
					if (TouchPoint.y < 64f) TouchPoint.y = 64f;

					UpdateRangePosition();
					
					
					#endregion			
				}
				else if (JWGlobal.Instance.what_skill_on == JWDefine.NO_SKILL)
				{				
					if (HandleUserData.Instance.touch_type == 0)
					{
						#region [OnTouchMoved] control type : Tap
						//TODO: drag와 같은 방식.
						if ((currentPosition.y) > 90f)
						{
							bool chosen = false;

							for (int temp = 0; temp < loadmap.Player_Base.Count; temp++)
							{
								Base it = loadmap.Player_Base[temp];

								if (it.selected == false && CLCollisionChkRwithV(new CLRECT(it.position.x - 45, it.position.y - 45, 90, 90), new CLVEC2(currentPosition.x, (currentPosition.y))))
								{
									chosen = true;
									loadmap.drag_it.selected = false;

									DragItUpdate(loadmap.drag_it);

									it.selected = true;
									loadmap.drag_it = it;
									tap_select1 = true;
									tap_select2 = 1;

									loadmap.Player_Base[temp] = it;
									break;
								}
								if (it.selected == true && (CLCollisionChkRwithV(new CLRECT(it.position.x - 45f, it.position.y - 45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y)))) == false)
								{
									it.selected = false;
									tap_select1 = false;
									tap_select2 = 0;

									loadmap.Player_Base[temp] = it;
									break;
								}
							}
							if (chosen == false)
							{
								for (int temp = 0; temp < loadmap.Nature_Base.Count; temp++)
								{
									Base it = loadmap.Nature_Base[temp];

									if (it.selected == false && CLCollisionChkRwithV(new CLRECT(it.position.x - 45f, it.position.y - 45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y))))
									{
										chosen = true;
										loadmap.drag_it.selected = false;

										DragItUpdate(loadmap.drag_it);

										it.selected = true;
										loadmap.drag_it = it;
										tap_select1 = true;
										tap_select2 = 1;

										loadmap.Nature_Base[temp] = it;
										break;
									}
									if (it.selected == true && (CLCollisionChkRwithV(new CLRECT(it.position.x - 45f, it.position.y - 45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y)))) == false)
									{
										it.selected = false;
										tap_select1 = false;
										tap_select2 = 0;

										loadmap.Nature_Base[temp] = it;
										break;
									}
								}
							}
							if (chosen == false)
							{
								for (int temp = 0; temp < loadmap.Enemy_Base.Count; temp++)
								{
									Base it = loadmap.Enemy_Base[temp];

									if (it.selected == false && CLCollisionChkRwithV(new CLRECT(it.position.x - 45f, it.position.y - 45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y))))
									{
										chosen = true;
										loadmap.drag_it.selected = false;

										DragItUpdate(loadmap.drag_it);

										it.selected = true;
										loadmap.drag_it = it;
										tap_select1 = true;
										tap_select2 = 1;

										loadmap.Enemy_Base[temp] = it;
										break;
									}
									if (it.selected == true && (CLCollisionChkRwithV(new CLRECT(it.position.x - 45f, it.position.y - 45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y)))) == false)
									{
										it.selected = false;
										tap_select1 = false;
										tap_select2 = 0;

										loadmap.Enemy_Base[temp] = it;
										break;
									}
								}
							}
						}
						#endregion
					}
					else
					{
						#region [OnTouchMoved] control type : Drag
						if ((currentPosition.y) > 90f)
						{							
							bool chosen = false;
								
							for (int temp = 0; temp < loadmap.Player_Base.Count; temp++)							
							{
								Base it = loadmap.Player_Base[temp];
									
								if(it.selected == false && CLCollisionChkRwithV(new CLRECT(it.position.x-45, it.position.y-45, 90, 90), new CLVEC2(currentPosition.x, (currentPosition.y))))
								{
									chosen = true;
									loadmap.drag_it.selected = false;
										
									DragItUpdate(loadmap.drag_it);
										
									it.selected = true;
									loadmap.drag_it = it;
									drag_select = 1;
										
									loadmap.Player_Base[temp] = it;
									break;
								}
								if (it.selected == true && (CLCollisionChkRwithV(new CLRECT(it.position.x - 45f, it.position.y - 45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y)))) == false)
								{
									it.selected = false;
									drag_select = 0;

									loadmap.Player_Base[temp] = it;
									break;
								}
							}
							if(chosen == false)
							{
								for (int temp = 0; temp < loadmap.Nature_Base.Count; temp++)							
								{
									Base it = loadmap.Nature_Base[temp];
										
									if(it.selected == false && CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y))))
									{
										chosen = true;
										loadmap.drag_it.selected = false;
											
										DragItUpdate(loadmap.drag_it);
											
										it.selected = true;
										loadmap.drag_it = it;
										drag_select = -1;
											
										loadmap.Nature_Base[temp] = it;
										break;
									}
									if(it.selected == true && (CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y)))) == false)
									{
										it.selected = false;
										drag_select = 0;
											
										loadmap.Nature_Base[temp] = it;
										break;
									}
								}
							}
							if(chosen == false)
							{
								for (int temp = 0; temp < loadmap.Enemy_Base.Count; temp++)							
								{
									Base it = loadmap.Enemy_Base[temp];
										
									if(it.selected == false && CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y))))
									{
										chosen = true;
										loadmap.drag_it.selected = false;
											
										DragItUpdate(loadmap.drag_it);
											
										it.selected = true;
										loadmap.drag_it = it;
										drag_select = -2;
											
										loadmap.Enemy_Base[temp] = it;
										break;
									}
									if(it.selected == true && (CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y)))) == false)
									{
										it.selected = false;
										drag_select = 0;
											
										loadmap.Enemy_Base[temp] = it;
										break;
									}
								}
							}
						}
						#endregion
					}
				}

				#endregion
			}
			else
			{
				#region [OnTouchMoved] 기존 방식.

				if (JWGlobal.Instance.skill_move0 == true)
				{
					#region [OnTouchMoved] 스킬 사용 지점 이동.

					//Debug.Log ("**********************************************************Skill Move");

					TouchPoint.x = currentPosition.x;
					TouchPoint.y = currentPosition.y;


					if(JWGlobal.Instance.skill_move == false)
					{
						if(HandleUserData.Instance.skill_type == 0)
						{
							if(Mathf.Abs(TouchPoint.x-JWGlobal.Instance.tap_x) > 16f || Mathf.Abs(TouchPoint.y-JWGlobal.Instance.tap_y) > 16f)
								JWGlobal.Instance.skill_move = true;
						}
						else 
						{
							JWGlobal.Instance.skill_move = true;
						}
					}

					TouchPoint.y += skill.range*(HandleUserData.Instance.range_adjust-1);
					if(TouchPoint.y > 800f)	TouchPoint.y = 800f;
					if(TouchPoint.y < 64f)	TouchPoint.y = 64f;

					UpdateRangePosition();

					#endregion			
				}
				else if (JWGlobal.Instance.what_skill_on == JWDefine.NO_SKILL)
				{
					if (HandleUserData.Instance.touch_type == 0)
					{
						#region [OnTouchMoved] control type : Tap
						if (TouchPoint.x != currentPosition.x && TouchPoint.y != (currentPosition.y))
						{

							if(TouchPoint.x < currentPosition.x)
							{
								Drag_box.x = TouchPoint.x;
								Drag_box.w = currentPosition.x - TouchPoint.x;

								Drag_box.px = currentPosition.x - (Drag_box.w/2f);
							}
							else
							{
								Drag_box.x = currentPosition.x;
								Drag_box.w = TouchPoint.x - currentPosition.x;

								Drag_box.px = currentPosition.x + Drag_box.w/2f;
							}

							if(TouchPoint.y < (currentPosition.y))
							{
								Drag_box.y = TouchPoint.y;
								if(TouchPoint.y < 90f) TouchPoint.y = 90f;
								Drag_box.h = (currentPosition.y) - TouchPoint.y;

								Drag_box.py = TouchPoint.y + Drag_box.h/2f;
							}
							else
							{
								Drag_box.y = ((currentPosition.y));
								if((currentPosition.y) < 90f)
									Drag_box.h = TouchPoint.y - 90f;
								else
									Drag_box.h = TouchPoint.y - (currentPosition.y);

								Drag_box.py = TouchPoint.y - Drag_box.h/2f;
							}

							if(Drag_box.py < 90f) Drag_box.py = 90f;

							Drag_box.rect = new CLRECT(Drag_box.x, Drag_box.y, Drag_box.w, Drag_box.h);
							Drag_box.position = new CLVEC2(Drag_box.px, Drag_box.py);
							
							if(Drag_box.w >= 32f || Drag_box.h >= 32f)
							{
								drag_select = 0;
								if(loadmap.drag_it.owner != JWDefine.PLAYER)
								{
									for (int temp = 0; temp < loadmap.Enemy_Base.Count; temp++)
									{
										Base e_it = loadmap.Enemy_Base[temp];
										//Freeze Fix
										if(e_it.tag == loadmap.drag_it.tag)
										{
											e_it.selected = false;
											loadmap.drag_it = e_it;
										}
									}
									for (int temp = 0; temp < loadmap.Nature_Base.Count; temp++)
									{
										Base e_it = loadmap.Nature_Base[temp];
										//Freeze Fix
										if(e_it.tag == loadmap.drag_it.tag)
										{
											e_it.selected = false;
											loadmap.drag_it = e_it;
										}
									}

									DragItUpdate(loadmap.drag_it);
								}

								for (int temp = 0; temp < loadmap.Player_Base.Count; temp++)
								{
									Base it = loadmap.Player_Base[temp];

									if(CLCollisionChkRwithV(Drag_box.rect, it.position) == true) //Collision happened
									{
										it.selected = true;
										loadmap.drag_it = it;
										drag_select++;
									}
									else it.selected = false;

									loadmap.Player_Base[temp] = it;
								}


								if(drag_select > 0) tap_select2 = 1;

							}

							//Draw DragBox
							GoDragBox.transform.localPosition = new Vector2(Drag_box.position.x, Drag_box.position.y);
							GoDragBox.GetComponent<UISprite>().width = (int)Drag_box.w;
							GoDragBox.GetComponent<UISprite>().height = (int)Drag_box.h;

							if(GoDragBox.GetComponent<UISprite>().width > 2 && GoDragBox.GetComponent<UISprite>().height > 2)
								GoDragBox.SetActive(true);
							else
								GoDragBox.SetActive(false);
						}
						#endregion
					}
					else
					{
						#region [OnTouchMoved] control type : Drag
						if ((currentPosition.y) > 90f)
						{
							if(drag_select > 0)//Target moving
							{
								for (int temp = 0; temp < loadmap.Player_Base.Count; temp++)							
								{
									Base it = loadmap.Player_Base[temp];

									if(CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, currentPosition.y)))
									{
										if(it.selected == false)
										{
											it.selected = true;
											loadmap.drag_it = it;
											drag_select++;

											loadmap.Player_Base[temp] = it;
											break;
										}
										else
										{
											loadmap.drag_it = it;

											loadmap.Player_Base[temp] = it;
											break;
										}
									}
								}

								for (int temp = 0; temp < loadmap.Nature_Base.Count; temp++)							
								{
									Base it = loadmap.Nature_Base[temp];
								
									if(it.selected == false && CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y))))
									{
										if(loadmap.drag_it.owner != JWDefine.PLAYER)
										{
											loadmap.drag_it.selected = false;
											
											DragItUpdate(loadmap.drag_it);									
										}
										it.selected = true;
										loadmap.drag_it = it;

										loadmap.Nature_Base[temp] = it;
										break;
									}
									if(it.selected == true && (CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y)))) == false)
									{
										it.selected = false;

										loadmap.Nature_Base[temp] = it;
										break;
									}
								}
								for (int temp = 0; temp < loadmap.Enemy_Base.Count; temp++)							
								{
									Base it = loadmap.Enemy_Base[temp];

									if(it.selected == false && CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y))))
									{
										if(loadmap.drag_it.owner != JWDefine.PLAYER)
										{
											loadmap.drag_it.selected = false;
											
											DragItUpdate(loadmap.drag_it);
										}
										it.selected = true;
										loadmap.drag_it = it;

										loadmap.Enemy_Base[temp] = it;
										break;
									}
									if(it.selected == true && (CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y)))) == false)
									{
										it.selected = false;

										loadmap.Enemy_Base[temp] = it;
										break;
									}
								}
								
							}
							else//Select Moving
							{
								bool chosen = false;
								
								for (int temp = 0; temp < loadmap.Player_Base.Count; temp++)							
								{
									Base it = loadmap.Player_Base[temp];
								
									if(it.selected == false && CLCollisionChkRwithV(new CLRECT(it.position.x-45, it.position.y-45, 90, 90), new CLVEC2(currentPosition.x, (currentPosition.y))))
									{
										chosen = true;
										loadmap.drag_it.selected = false;
										
										DragItUpdate(loadmap.drag_it);

										it.selected = true;
										loadmap.drag_it = it;
										drag_select = 1;

										loadmap.Player_Base[temp] = it;
										break;
									}
								}
								if(chosen == false)
								{
									for (int temp = 0; temp < loadmap.Nature_Base.Count; temp++)							
									{
										Base it = loadmap.Nature_Base[temp];

										if(it.selected == false && CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y))))
										{
											chosen = true;
											loadmap.drag_it.selected = false;
											
											DragItUpdate(loadmap.drag_it);

											it.selected = true;
											loadmap.drag_it = it;
											drag_select = -1;

											loadmap.Nature_Base[temp] = it;
											break;
										}
										if(it.selected == true && (CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y)))) == false)
										{
											it.selected = false;
											drag_select = 0;

											loadmap.Nature_Base[temp] = it;
											break;
										}
									}
								}
								if(chosen == false)
								{
									for (int temp = 0; temp < loadmap.Enemy_Base.Count; temp++)							
									{
										Base it = loadmap.Enemy_Base[temp];
									
										if(it.selected == false && CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y))))
										{
											chosen = true;
											loadmap.drag_it.selected = false;
											
											DragItUpdate(loadmap.drag_it);

											it.selected = true;
											loadmap.drag_it = it;
											drag_select = -2;

											loadmap.Enemy_Base[temp] = it;
											break;
										}
										if(it.selected == true && (CLCollisionChkRwithV(new CLRECT(it.position.x-45f, it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, (currentPosition.y)))) == false)
										{
											it.selected = false;
											drag_select = 0;

											loadmap.Enemy_Base[temp] = it;
											break;
										}
									}
								}
							}
						}
						#endregion
					}
				}


				#endregion
			}			

			#endregion
		}
	}
	
	void OnTouchReleased(Vector2 currentPosition)
	{
		////////////////////////////// TOUCH_RELEASED: PLAY: START //////////////////////////////

		#region [OnTouchReleased] 건물 선택 끝남.

		if (isUseAllAttack && JWGlobal.Instance.isAllAttack)
		{
			#region [OnTouchReleased] 전체 방식.

			if (HandleUserData.Instance.touch_type == 0)
			{
				#region [OnTouchReleased] touch type : Tap
				//Debug.Log ("[OnTouchReleased] tap_select2 : " + tap_select2);
				
				if(tap_select2 != 0)
				{
					if(tap_select2 == 1)
					{
						//////////////////////////////////////
						//Debug.Log ("[OnTouchReleased] AllAttack");
						
						SelectAllPlayerBases();
						//////////////////////////////////////
						
						int move_num = 0;
						for(int temp = 0; temp < loadmap.Player_Base.Count; temp++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
						{
							Base p_it = loadmap.Player_Base[temp];
							
							if(p_it.selected && p_it.tag != loadmap.drag_it.tag)
							{
								if(p_it.num >= JWDefine.MIN_JELLY_OUT)
								{
									Jelly jelly;
									
									jelly = drawjelly.MakeJelly(p_it, loadmap.drag_it, temp);
									
									if(jelly.num_of_move > 1)
									{
										drawjelly.Player_Jelly.Add(jelly);
										drawjelly.DrawNewJelly(jelly, JWDefine.PLAYER);
										//Debug.Log ("jelly.num_of_move : " + jelly.num_of_move);
										
										if(move_num == 0)
										{
											Lockon lockon = new Lockon();
											lockon.position = loadmap.drag_it.position;
											lockon.type = loadmap.drag_it.type;
											lockon.count = 0;
											lockon.lockon = JWDefine.LOCK_BY_PLAYER;
											
											//Set Sprite Tag	
											lockon.tag = effect.index_lockon;
											
											effect.Lockon_Effect.Add(lockon);
											effect.DrawNewLockOn(lockon, JWDefine.PLAYER, effect.Lockon_Effect.Count-1);
											
											DragItNew();
											loadmap.drag_it.selected = false;
											DragItUpdate(loadmap.drag_it);
											//Debug.Log("************************************Player O");
										}
										move_num++;
									}
								}
							}
							
							p_it = loadmap.Player_Base[temp];
							p_it.selected = false;
							loadmap.Player_Base[temp] = p_it;
							
							//Freeze Fix
							if(p_it.tag == loadmap.drag_it.tag)
								loadmap.drag_it = p_it;
						}
						
						//////////////////////////////////////
						if(move_num == 0)
						{
							DragItNew();
							loadmap.drag_it.selected = false;
							DragItUpdate(loadmap.drag_it);
							tap_select2 = 0;
						}
						else
						{
							tap_select2 = 0;
						}
						
						//////////////////////////////////////
					}
					else
					{
						tap_select2 = 0;
					}
				}
				
				tap_select1 = false;
				
				Initialize();

				#endregion
			}
			else
			{
				#region [OnTouchReleased] touch type : Drag
				//Debug.Log ("[OnTouchReleased] tap_select2 : " + tap_select2);

				if (drag_select != 0)
				{
					//////////////////////////////////////
					//Debug.Log ("[OnTouchReleased] AllAttack");
						
					SelectAllPlayerBases();
					//////////////////////////////////////
						
					int move_num = 0;
					for(int temp = 0; temp < loadmap.Player_Base.Count; temp++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
					{
						Base p_it = loadmap.Player_Base[temp];
							
						if(p_it.selected && p_it.tag != loadmap.drag_it.tag)
						{
							if(p_it.num >= JWDefine.MIN_JELLY_OUT)
							{
								Jelly jelly;
									
								jelly = drawjelly.MakeJelly(p_it, loadmap.drag_it, temp);
									
								if(jelly.num_of_move > 1)
								{
									drawjelly.Player_Jelly.Add(jelly);
									drawjelly.DrawNewJelly(jelly, JWDefine.PLAYER);
									//Debug.Log ("jelly.num_of_move : " + jelly.num_of_move);
										
									if(move_num == 0)
									{
										Lockon lockon = new Lockon();
										lockon.position = loadmap.drag_it.position;
										lockon.type = loadmap.drag_it.type;
										lockon.count = 0;
										lockon.lockon = JWDefine.LOCK_BY_PLAYER;
											
										//Set Sprite Tag	
										lockon.tag = effect.index_lockon;
											
										effect.Lockon_Effect.Add(lockon);
										effect.DrawNewLockOn(lockon, JWDefine.PLAYER, effect.Lockon_Effect.Count-1);
											
										DragItNew();
										loadmap.drag_it.selected = false;
										DragItUpdate(loadmap.drag_it);
										//Debug.Log("************************************Player O");
									}
									move_num++;
								}
							}
						}
							
						p_it = loadmap.Player_Base[temp];
						p_it.selected = false;
						loadmap.Player_Base[temp] = p_it;
							
						//Freeze Fix
						if(p_it.tag == loadmap.drag_it.tag)
							loadmap.drag_it = p_it;
					}
						
					//////////////////////////////////////
					if(move_num == 0)
					{
						DragItNew();
						loadmap.drag_it.selected = false;
						DragItUpdate(loadmap.drag_it);
						drag_select = 0;
					}
					else
					{
						drag_select = 0;
					}
						
					//////////////////////////////////////
				}
				#endregion
			}

			#endregion
		}
		else
		{
			#region [OnTouchReleased] 기존 방식.

			if (HandleUserData.Instance.touch_type == 0)
			{				
				#region [OnTouchReleased] touch type : Tap
				if (tap_select2 != 1)
				{
					if(tap_select2 == 2)
					{
						int move_num = 0;
						for(int temp = 0; temp < loadmap.Player_Base.Count; temp++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
						{
							Base p_it = loadmap.Player_Base[temp];

							if(p_it.selected && p_it.tag != loadmap.drag_it.tag)
							{
								if(p_it.num >= JWDefine.MIN_JELLY_OUT)
								{
									Jelly jelly;
									
									jelly = drawjelly.MakeJelly(p_it, loadmap.drag_it, temp);
									
									if(jelly.num_of_move > 1)
									{
										drawjelly.Player_Jelly.Add(jelly);
										drawjelly.DrawNewJelly(jelly, JWDefine.PLAYER);
										//Debug.Log ("jelly.num_of_move : " + jelly.num_of_move);
										
										if(move_num == 0)
										{
											Lockon lockon = new Lockon();
											lockon.position = loadmap.drag_it.position;
											lockon.type = loadmap.drag_it.type;
											lockon.count = 0;
											lockon.lockon = JWDefine.LOCK_BY_PLAYER;
											
											//Set Sprite Tag	
											lockon.tag = effect.index_lockon;

											effect.Lockon_Effect.Add(lockon);
											effect.DrawNewLockOn(lockon, JWDefine.PLAYER, effect.Lockon_Effect.Count-1);

											DragItNew();
											loadmap.drag_it.selected = false;
											DragItUpdate(loadmap.drag_it);
											//Debug.Log("************************************Player O");
										}
										move_num++;
									}
								}
							}

							p_it = loadmap.Player_Base[temp];
							p_it.selected = false;
							loadmap.Player_Base[temp] = p_it;

							//Freeze Fix
							if(p_it.tag == loadmap.drag_it.tag)
							loadmap.drag_it = p_it;
						}


						if(loadmap.drag_it.owner == JWDefine.PLAYER && move_num == 0)
						{
							DragItNew();
							loadmap.drag_it.selected = true;
							DragItUpdate(loadmap.drag_it);
							tap_select2 = 1;
						}
						else 
						{
							tap_select2 = 0;
						}
					}
					else // tap_select2 == 0
					{
						for(int temp = 0; temp < loadmap.Player_Base.Count; temp++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
						{
							Base it = loadmap.Player_Base[temp];
							it.selected = false;
							loadmap.Player_Base[temp] = it;

							//Freeze Fix
							if(it.tag == loadmap.drag_it.tag)
								loadmap.drag_it = it;
						}

						DragItNew();
						loadmap.drag_it.selected = false;
						DragItUpdate(loadmap.drag_it);

						tap_select2 = 0;
					}
				}
				else // tap_select2 == 1
				{

				}

				tap_select1 = false;
				
				Initialize();

				#endregion
			}
			else
			{			
				#region [OnTouchReleased] touch type : Drag
				if (drag_select > 0)
				{
					if(CLCollisionChkRwithV(new CLRECT(loadmap.drag_it.position.x-45f, loadmap.drag_it.position.y-45f, 90f, 90f), new CLVEC2(currentPosition.x, currentPosition.y)))
					{
						int move_num = 0;
						for(int temp = 0; temp < loadmap.Player_Base.Count; temp++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
						{
							Base p_it = loadmap.Player_Base[temp];
						
							if(p_it.selected && p_it.tag != loadmap.drag_it.tag)
							{
								if(p_it.num >= JWDefine.MIN_JELLY_OUT)
								{
									Jelly jelly;
									
									jelly = drawjelly.MakeJelly(p_it, loadmap.drag_it, temp);
									
									if(jelly.num_of_move > 1)
									{
										drawjelly.Player_Jelly.Add(jelly);
										drawjelly.DrawNewJelly(jelly, JWDefine.PLAYER);
										//Debug.Log ("jelly.num_of_move : " + jelly.num_of_move);
										
										if(move_num == 0)
										{
											Lockon lockon = new Lockon();
											lockon.position = loadmap.drag_it.position;
											lockon.type = loadmap.drag_it.type;
											lockon.count = 0;
											lockon.lockon = JWDefine.LOCK_BY_PLAYER;
											
											//Set Sprite Tag
											lockon.tag = effect.index_lockon;
																					
											effect.Lockon_Effect.Add(lockon);
											effect.DrawNewLockOn(lockon, JWDefine.PLAYER, effect.Lockon_Effect.Count-1);
											//Debug.Log("************************************Player O");
										}
										move_num++;
									}
								}
							}

							p_it = loadmap.Player_Base[temp];
							p_it.selected = false;
							loadmap.Player_Base[temp] = p_it;

							//Freeze Fix
							if(p_it.tag == loadmap.drag_it.tag)
								loadmap.drag_it = p_it;
						}
					}
					else
					{
						for(int temp = 0; temp < loadmap.Player_Base.Count; temp++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
						{
							Base it = loadmap.Player_Base[temp];
							it.selected = false;
							loadmap.Player_Base[temp] = it;

							//Freeze Fix
							if(it.tag == loadmap.drag_it.tag)
								loadmap.drag_it = it;
						}
					}
				}

				if(loadmap.drag_it.owner != JWDefine.PLAYER)
				{
					for (int temp = 0; temp < loadmap.Enemy_Base.Count; temp++)
					{
						Base e_it = loadmap.Enemy_Base[temp];
						//Freeze Fix
						if(e_it.tag == loadmap.drag_it.tag)
						{
							e_it.selected = false;
							loadmap.drag_it = e_it;
						}
					}
					for (int temp = 0; temp < loadmap.Nature_Base.Count; temp++)
					{
						Base e_it = loadmap.Nature_Base[temp];
						//Freeze Fix
						if(e_it.tag == loadmap.drag_it.tag)
						{
							e_it.selected = false;
							loadmap.drag_it = e_it;
						}
					}
				}
				else
				{
					DragItNew();
					loadmap.drag_it.selected = false;
				}

				DragItUpdate(loadmap.drag_it);

				#endregion
			}

			#endregion
		}

		#endregion
		
		
		SkillRelease (currentPosition);
		
		if(JWGlobal.Instance.start_count != 0)
			JWGlobal.Instance.start_count = 0;

		drag_select = 0;

		DragBoxOff ();

		////////////////////////////// TOUCH_RELEASED: PLAY: END ///////////////////////////////// 
	}

	void SelectAllPlayerBases()
	{
		for (int temp_2 = 0; temp_2 < loadmap.Player_Base.Count; temp_2++)// for (list<Base>::iterator it_2 = Player_Base.begin(); it_2 != Player_Base.end(); ++it_2)
		{
			Base it_2 = loadmap.Player_Base[temp_2];
			it_2.selected = true;

			loadmap.Player_Base[temp_2] = it_2;
		}
	}

	public void DragBoxOff()
	{
		GoDragBox.transform.localPosition = dragboxini;
		GoDragBox.SetActive(false);
	}

	public void SkillRelease(Vector2 currentPosition)
	{
		//Release Skill Icon		

		if (isUseAllAttack && JWGlobal.Instance.isAllAttack)
		{
			#region [SkillRelease] 혼합 방식.

			if (JWGlobal.Instance.skill_push == true)//Skill:Tap only
			{
				if (JWGlobal.Instance.skill_fire == true && CLCollisionChkRwithV(ICON_FIRE, new CLVEC2(currentPosition.x, currentPosition.y)))
				{ JWGlobal.Instance.skill_activate = true; }
				else if (JWGlobal.Instance.skill_lightning == true && CLCollisionChkRwithV(ICON_LIGHTNING, new CLVEC2(currentPosition.x, currentPosition.y)))
				{ JWGlobal.Instance.skill_activate = true; }
				else if (JWGlobal.Instance.skill_poison == true && CLCollisionChkRwithV(ICON_POISON, new CLVEC2(currentPosition.x, currentPosition.y)))
				{ JWGlobal.Instance.skill_activate = true; }
				else
				{
					JWGlobal.Instance.what_skill_on = JWDefine.NO_SKILL;
					JWGlobal.Instance.skill_ready = false;
					JWGlobal.Instance.skill_activate = false;
					JWGlobal.Instance.skill_fire = false;
					JWGlobal.Instance.skill_lightning = false;
					JWGlobal.Instance.skill_poison = false;
					JWGlobal.Instance.level = 0;
					JWGlobal.Instance.cost = 0;
				}
				JWGlobal.Instance.skill_push = false;
				
			}
			else
			{
				if (JWGlobal.Instance.what_skill_on != JWDefine.NO_SKILL)
				{
					if (currentPosition.y > 64f)
					{
						switch (JWGlobal.Instance.what_skill_on)
						{
							case JWDefine.ACTIVE_SKILL_FIRE:
								JWGlobal.Instance.skill_fire = false;
								skill.type = JWDefine.ACTIVE_SKILL_FIRE;
								skill.damage = InitSkillValue.Instance.A_Fire[HandleUserData.Instance.skill_level[0]].damage;
								skill.cooltime = JWDefine.COOL_FIRE;

								if (JWGlobal.Instance.skill_move == true) skill.position = new CLVEC2(currentPosition.x, currentPosition.y + skill.range * (HandleUserData.Instance.range_adjust - 1));
								else skill.position = new CLVEC2(currentPosition.x, currentPosition.y);
								break;
							case JWDefine.ACTIVE_SKILL_LIGHTNING:
								JWGlobal.Instance.skill_lightning = false;
								skill.type = JWDefine.ACTIVE_SKILL_LIGHTNING;
								skill.damage = InitSkillValue.Instance.A_Lightning[HandleUserData.Instance.skill_level[1]].damage;
								skill.cooltime = JWDefine.COOL_LIGHTNING;

								if (JWGlobal.Instance.skill_move == true) skill.position = new CLVEC2(currentPosition.x, currentPosition.y + skill.range * (HandleUserData.Instance.range_adjust - 1));
								else skill.position = new CLVEC2(currentPosition.x, currentPosition.y);
								break;
							case JWDefine.ACTIVE_SKILL_POISON:
								JWGlobal.Instance.skill_poison = false;
								skill.type = JWDefine.ACTIVE_SKILL_POISON;
								skill.damage = InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].damage;
								skill.cooltime = JWDefine.COOL_POISON;

								if (JWGlobal.Instance.skill_move == true) skill.position = new CLVEC2(currentPosition.x, currentPosition.y + skill.range * (HandleUserData.Instance.range_adjust - 1));
								else skill.position = new CLVEC2(currentPosition.x, currentPosition.y);
								break;
							default:
								break;
						}

						if (skill.position.y > 800f) skill.position.y = 800f;
						if (skill.position.y < 64f) skill.position.y = 64f;

						if (JWGlobal.Instance.skill_ready == true)
						{
							JWGlobal.Instance.mana -= JWGlobal.Instance.cost;
							JWGlobal.Instance.skill_cool[JWGlobal.Instance.what_skill_on - 4001] = skill.cooltime;
							JWGlobal.Instance.ctick0[JWGlobal.Instance.what_skill_on - 4001] = JWGlobal.Instance.GetTicks();

							//Debug.Log ("******************************************************** SKILL");
							UsingSkill(skill);
						}
					}
					else
					{
						JWGlobal.Instance.skill_fire = false;
						JWGlobal.Instance.skill_lightning = false;
						JWGlobal.Instance.skill_poison = false;
					}

					JWGlobal.Instance.what_skill_on = JWDefine.NO_SKILL;
					JWGlobal.Instance.skill_ready = false;
					JWGlobal.Instance.skill_move0 = false;
					JWGlobal.Instance.skill_move = false;
					JWGlobal.Instance.level = 0;
					JWGlobal.Instance.cost = 0;

					//JWGlobal.Instance.skill_activate = false;


				}
			}

			#endregion
		}
		else
		{
			#region [SkillRelease] 기존 방식.

			if (JWGlobal.Instance.skill_push == true)//Skill:Tap only
			{
				if (JWGlobal.Instance.skill_fire == true && CLCollisionChkRwithV(ICON_FIRE, new CLVEC2(currentPosition.x, currentPosition.y)))
				{ JWGlobal.Instance.skill_activate = true; }
				else if (JWGlobal.Instance.skill_lightning == true && CLCollisionChkRwithV(ICON_LIGHTNING, new CLVEC2(currentPosition.x, currentPosition.y)))
				{ JWGlobal.Instance.skill_activate = true; }
				else if (JWGlobal.Instance.skill_poison == true && CLCollisionChkRwithV(ICON_POISON, new CLVEC2(currentPosition.x, currentPosition.y)))
				{ JWGlobal.Instance.skill_activate = true; }
				else
				{
					JWGlobal.Instance.what_skill_on = JWDefine.NO_SKILL;
					JWGlobal.Instance.skill_ready = false;
					JWGlobal.Instance.skill_activate = false;
					JWGlobal.Instance.skill_fire = false;
					JWGlobal.Instance.skill_lightning = false;
					JWGlobal.Instance.skill_poison = false;
					JWGlobal.Instance.level = 0;
					JWGlobal.Instance.cost = 0;
				}
				JWGlobal.Instance.skill_push = false;


			}
			else
			{
				if (JWGlobal.Instance.what_skill_on != JWDefine.NO_SKILL)
				{
					if (currentPosition.y > 64f)
					{
						switch (JWGlobal.Instance.what_skill_on)
						{
							case JWDefine.ACTIVE_SKILL_FIRE:
								JWGlobal.Instance.skill_fire = false;
								skill.type = JWDefine.ACTIVE_SKILL_FIRE;
								skill.damage = InitSkillValue.Instance.A_Fire[HandleUserData.Instance.skill_level[0]].damage;
								skill.cooltime = JWDefine.COOL_FIRE;

								if (JWGlobal.Instance.skill_move == true) skill.position = new CLVEC2(currentPosition.x, currentPosition.y + skill.range * (HandleUserData.Instance.range_adjust - 1));
								else skill.position = new CLVEC2(currentPosition.x, currentPosition.y);
								break;
							case JWDefine.ACTIVE_SKILL_LIGHTNING:
								JWGlobal.Instance.skill_lightning = false;
								skill.type = JWDefine.ACTIVE_SKILL_LIGHTNING;
								skill.damage = InitSkillValue.Instance.A_Lightning[HandleUserData.Instance.skill_level[1]].damage;
								skill.cooltime = JWDefine.COOL_LIGHTNING;

								if (JWGlobal.Instance.skill_move == true) skill.position = new CLVEC2(currentPosition.x, currentPosition.y + skill.range * (HandleUserData.Instance.range_adjust - 1));
								else skill.position = new CLVEC2(currentPosition.x, currentPosition.y);
								break;
							case JWDefine.ACTIVE_SKILL_POISON:
								JWGlobal.Instance.skill_poison = false;
								skill.type = JWDefine.ACTIVE_SKILL_POISON;
								skill.damage = InitSkillValue.Instance.A_Poison[HandleUserData.Instance.skill_level[2]].damage;
								skill.cooltime = JWDefine.COOL_POISON;

								if (JWGlobal.Instance.skill_move == true) skill.position = new CLVEC2(currentPosition.x, currentPosition.y + skill.range * (HandleUserData.Instance.range_adjust - 1));
								else skill.position = new CLVEC2(currentPosition.x, currentPosition.y);
								break;
							default:
								break;
						}

						if (skill.position.y > 800f) skill.position.y = 800f;
						if (skill.position.y < 64f) skill.position.y = 64f;

						if (JWGlobal.Instance.skill_ready == true)
						{
							JWGlobal.Instance.mana -= JWGlobal.Instance.cost;
							JWGlobal.Instance.skill_cool[JWGlobal.Instance.what_skill_on - 4001] = skill.cooltime;
							JWGlobal.Instance.ctick0[JWGlobal.Instance.what_skill_on - 4001] = JWGlobal.Instance.GetTicks();

							//Debug.Log ("******************************************************** SKILL");
							UsingSkill(skill);
						}
					}
					else
					{
						JWGlobal.Instance.skill_fire = false;
						JWGlobal.Instance.skill_lightning = false;
						JWGlobal.Instance.skill_poison = false;
					}

					JWGlobal.Instance.what_skill_on = JWDefine.NO_SKILL;
					JWGlobal.Instance.skill_ready = false;
					JWGlobal.Instance.skill_move0 = false;
					JWGlobal.Instance.skill_move = false;
					JWGlobal.Instance.level = 0;
					JWGlobal.Instance.cost = 0;



				}
			}

			#endregion
		}	
	}

	/// <summary>
	/// Drag_it이 가리키는 건물의 최신 상태를 받아옴.
	/// </summary>
	public void DragItNew()
	{
		bool find = false;

		for(int temp_e = 0; temp_e < loadmap.Player_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
		{
			if(loadmap.Player_Base[temp_e].tag == loadmap.drag_it.tag)
			{
				loadmap.drag_it = loadmap.Player_Base[temp_e];
				find = true;
				break;
			}
		}
		if(!find)
		{
			for(int temp_e = 0; temp_e < loadmap.Nature_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
			{
				if(loadmap.Nature_Base[temp_e].tag == loadmap.drag_it.tag)
				{
					loadmap.drag_it = loadmap.Nature_Base[temp_e];
					find = true;
					break;
				}
			}
		}
		if(!find)
		{
			for(int temp_e = 0; temp_e < loadmap.Enemy_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
			{
				if(loadmap.Enemy_Base[temp_e].tag == loadmap.drag_it.tag)
				{
					loadmap.drag_it = loadmap.Enemy_Base[temp_e];
					find = true;
					break;
				}
			}	
		}
	}

	/// <summary>
	/// Drag_it의 상태를 실제 건물에 적용.
	/// </summary>
	/// <param name="drag_it">Drag_it.</param>
	public void DragItUpdate(Base drag_it)
	{
		if(drag_it.owner == JWDefine.PLAYER) 
		{
			for(int temp_e = 0; temp_e < loadmap.Player_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
			{
				if(loadmap.Player_Base[temp_e].tag == drag_it.tag)
				{
					loadmap.Player_Base[temp_e] = drag_it;
					break;
				}
			}
		}
		else if(drag_it.owner == JWDefine.NATURE)
		{			
			for(int temp_e = 0; temp_e < loadmap.Nature_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
			{
				if(loadmap.Nature_Base[temp_e].tag == drag_it.tag)
				{
					loadmap.Nature_Base[temp_e] = drag_it;
					break;
				}
			}
		}
		else if(drag_it.owner == JWDefine.ENEMY) 
		{
			for(int temp_e = 0; temp_e < loadmap.Enemy_Base.Count; temp_e++)	//for (list<Base>::iterator it = Player_Base.begin(); it != Player_Base.end(); ++it)
			{
				if(loadmap.Enemy_Base[temp_e].tag == drag_it.tag)
				{
					loadmap.Enemy_Base[temp_e] = drag_it;
					break;
				}
			}
		}
	}

	public void UpdateRangePosition()
	{
		//15. Skill Range
		switch(JWGlobal.Instance.what_skill_on)
		{
		case JWDefine.ACTIVE_SKILL_FIRE:
			if(JWGlobal.Instance.skill_cool[0] == 0)
			{
				SkillRange.transform.localPosition = new Vector2(TouchPoint.x, TouchPoint.y);
			}	
			break;
		case JWDefine.ACTIVE_SKILL_LIGHTNING:
			if(JWGlobal.Instance.skill_cool[1] == 0)
			{
				SkillRange.transform.localPosition = new Vector2(TouchPoint.x, TouchPoint.y);
			}	
			break;
		case JWDefine.ACTIVE_SKILL_POISON:
			if(JWGlobal.Instance.skill_cool[2] == 0)
			{
				SkillRange.transform.localPosition = new Vector2(TouchPoint.x, TouchPoint.y);
			}	
			break;
		default:	

			break;
		}
	}

	void UsingSkill(Skill skill)
	{
		Effect skill_effect = new Effect();

		switch(skill.type)
		{
		case JWDefine.ACTIVE_SKILL_FIRE:
		case JWDefine.ACTIVE_SKILL_LIGHTNING:
			if(skill.type == JWDefine.ACTIVE_SKILL_FIRE)
			{
				AudioManager.Instance.SFXPlay("SFX_FIRE");
				
				//Animation Effect
				skill_effect.position = skill.position;
				skill_effect.count = 0;
				skill_effect.anicount = 0;
				effect.Fire_Effect.Add(skill_effect);
				effect.DrawNewEffect(skill_effect, "Fire", effect.Fire_Effect.Count-1);
			}
			else if(skill.type == JWDefine.ACTIVE_SKILL_LIGHTNING)
			{
				AudioManager.Instance.SFXPlay("SFX_LIGHTNING");
				
				//Animation Effect
				skill_effect.position = skill.position;
				skill_effect.count = 0;
				skill_effect.anicount = 0;
				effect.Lightning_Effect.Add(skill_effect);
				effect.DrawNewEffect(skill_effect, "Lightning", effect.Lightning_Effect.Count-1);
			}
			
			//With Enemy's Jellies
			for(int temp = 0; temp < drawjelly.Enemy_Jelly.Count; temp++)//foreach(Base e_it in loadmap.Nature_Base)			
			{
				Jelly e_it = drawjelly.Enemy_Jelly[(temp)];
				
				if(CLCollisionChkCwithV(skill.position, skill.range, e_it.position) == true) //Collision happened
				{
					if(e_it.num > skill.damage)
					{
						JWGlobal.Instance.score_get += skill.damage * (HandleUserData.Instance.difficulty-750) * 60;
						e_it.num -= skill.damage;
					}
					else
					{
						//Debug.Log ("Kill");
						JWGlobal.Instance.score_get += e_it.num * (HandleUserData.Instance.difficulty-750) * 60;
						JWGlobal.Instance.score_get += (HandleUserData.Instance.difficulty-750) * 600;
						e_it.num = 0;
						e_it.state = JWDefine.PEACE;
					}

					drawjelly.Enemy_Jelly[(temp)] = e_it;
				}
			}
			//drawjelly.Enemy_Jelly.RemoveAll(item => item.num == 0 && item.state == JWDefine.PEACE);
			//drawjelly.SortJelly ();

			//With Enemy's Bases
			for(int temp = 0; temp < loadmap.Enemy_Base.Count; temp++)//foreach(Base e_it in loadmap.Nature_Base)			
			{
				Base e_it = loadmap.Enemy_Base[(temp)];
				
				if(CLCollisionChkCwithV(skill.position, skill.range, e_it.position) == true) //Collision happened
				{
					if(e_it.num > skill.damage)
					{
						JWGlobal.Instance.score_get += skill.damage * (HandleUserData.Instance.difficulty-750) * 36;
						e_it.num -= skill.damage;
					}
					else
					{
						JWGlobal.Instance.score_get += e_it.num * (HandleUserData.Instance.difficulty-750) * 36;
						e_it.num = 0;
					}
					
					loadmap.Enemy_Base[(temp)] = e_it;
				}
			}
			//With Nature's Bases
			for(int temp = 0; temp < loadmap.Nature_Base.Count; temp++)//foreach(Base e_it in loadmap.Nature_Base)			
			{
				Base e_it = loadmap.Nature_Base[(temp)];
			
				if(CLCollisionChkCwithV(skill.position, skill.range, e_it.position) == true) //Collision happened
				{
					if(e_it.num > skill.damage)
					{
						JWGlobal.Instance.score_get += skill.damage * (HandleUserData.Instance.difficulty-750) * 48;
						e_it.num -= skill.damage;
					}
					else
					{
						JWGlobal.Instance.score_get += e_it.num * (HandleUserData.Instance.difficulty-750) * 48;
						e_it.num = 0;
					}

					loadmap.Nature_Base[(temp)] = e_it;
				}
			}
			break;

		case JWDefine.ACTIVE_SKILL_POISON:
			
			AudioManager.Instance.SFXPlay("SFX_POISON");
			
			//Animation Effect
			skill_effect.position = skill.position;
			skill_effect.count = 0;
			skill_effect.anicount = 0;
			skill.count = 0;
			effect.Poison_Effect.Add(skill_effect);
			effect.Player_Skill.Add(skill);
			effect.DrawNewEffect(skill_effect, "Poison", effect.Poison_Effect.Count-1);
			
			break;
		}
	}

	public void DragLock()
	{
		JWGlobal.Instance.drag_lock = true;
		
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
	}

	public void DragRelease()
	{
		JWGlobal.Instance.drag_lock = false;
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
