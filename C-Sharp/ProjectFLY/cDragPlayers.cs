using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class cDragPlayers : MonoBehaviour {

	public bool showLogs = true;

	private Camera touchCamera;

	public cGameState gameState;

	public Transform scrollingObjects;	
	public Transform dragLeft;
	public Transform dragRight;			
	public Transform Player1;
	public Transform Player2;
	public Transform Player1_UI;
	public Transform Player2_UI;
	public Transform Player1_Model;
	public Transform Player2_Model;

	public bool selectedPlayer1 = false;
	public bool selectedPlayer2 = false;

	public float LIMIT_BORDER = 300f;
	public float LIMIT_PLAYERS_DISTANCE = 600f;

	[Range(0, 90)]
	public float minimumPixelToMove = 2f;
	[Range(0, 90)]
	public float minimumPixelToTilt = 10f;
	[Range(0, 90)]
	public float tiltAngle = 45f;
	[Range(0, 3)]
	public float holdTime = 0.5f;


	float keyboardDirectionPlayer1 = 0;
	float keyboardDirectionPlayer2 = 0;
	Vector3 keyboardVectorPlayer1 = Vector3.zero;
	Vector3 keyboardVectorPlayer2 = Vector3.zero;
	

	/// <summary>
	/// 합체한 상태이다.
	/// </summary>
	public bool isCouple = false;

	/// <summary>
	/// 합체 가능 여부.
	/// </summary>
	//public bool canCouple = false;

	/// <summary>
	/// 현자 타임 여부.
	/// </summary>
	public bool isWiseman = false;

	/// <summary>
	/// 현자 타임 유지 시간.
	/// </summary>
	public float wisemanTime = 3f;


	public bool coupleBreakTouch1 = false;
	public bool coupleBreakTouch2 = false;
	public int coupleBreakTouch1_ID = -1;
	public int coupleBreakTouch2_ID = -1;
	public float coupleBreakTouch1_x = 0;
	public float coupleBreakTouch2_x = 0;

	public int Touch1_ID = -1;
	public int Touch2_ID = -1;

	public float playersDistance = 0f;
	public float coupleStartDistance = 70f;
	public float coupleBreakDistance = 300f;	

	/// <summary>
	/// 1P의 드래그 영역들.
	/// </summary>
	Vector3 player1DragVectorCenter = new Vector3(170f, 320f, 0f);
	Vector3 player1DragVectorSize = new Vector3(340f, 640f, 0f);
	Vector3 coupleDragVectorCenter = new Vector3(170f, 320f, 0f);
	Vector3 coupleDragVectorSize = new Vector3(1100f, 640f, 0f);


	float Player1LimitOuter = 60f; // -300f
	float Player1LimitInner = 340f; // 합체시 = Player2LimitOuter - coupleStartDistance
	float Player2LimitInner = 380f;
	float Player2LimitOuter = 1220f; // 1020f

	Vector3 Player1IniPos = new Vector3(0, 0, 0);
	Vector3 Player2IniPos = new Vector3(0, 0, 0);


	Vector3 Player1_ModelInitRotation = Vector3.zero;
	Vector3 Player2_ModelInitRotation = Vector3.zero;
	Vector3 Player1_ModelRotationAxis = new Vector3(0, 1f, 0);
	Vector3 Player2_ModelRotationAxis = new Vector3(0, -1f, 0);

	float middleLimit = 0;

	public float destination1 = 0;
	public float destination2 = 0;

	void Start()
	{
		init();
	}

	//Maps touch id to drag info structs
	Dictionary<int, DragInfo> drags;
	struct DragInfo
	{		
		public Transform draggedTransform;
		public Transform anotherTransform;

		public Transform draggedUI;

		public float touchPos;		
		public float iniPos;	

		/// <summary>
		/// Left:-1 / Right:1
		/// </summary>
		public int direction;
		public float limitInner;
		public float limitOuter;
		
	}

	void init()
	{
		drags = new Dictionary<int, DragInfo>();
		touchCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
				
		selectedPlayer1 = false;
		selectedPlayer2 = false;

		isWiseman = false;

		Player1IniPos = Player1.localPosition;
		Player2IniPos = Player2.localPosition;
		Player1_ModelInitRotation = Player1_Model.localRotation.eulerAngles;
		Player2_ModelInitRotation = Player2_Model.localRotation.eulerAngles;

		middleLimit = 0;
		destination1 = 0;
		destination2 = 0;
	}

	DragInfo SelectPlayer(int num, Touch touch)
	{
		Debug.Log("[SelectPlayer] " + num);

		DragInfo info;

        if (num == 1)
		{
			info.draggedTransform = Player1;
			info.anotherTransform = Player2;
			info.draggedUI = Player1_UI;
			info.direction = -1;
			info.limitInner = Player1LimitInner;
			info.limitOuter = Player1LimitOuter;

			info.iniPos = info.draggedTransform.localPosition.x;
			info.touchPos = touch.position.x * 720f / Screen.width;

			info.draggedTransform.GetComponent<cPlayerStatus>().distance = info.draggedTransform.localPosition.x - info.touchPos;
			info.draggedTransform.GetComponent<cPlayerStatus>().previousPosition = info.draggedTransform.transform.localPosition;

			Touch1_ID = touch.fingerId;

			selectedPlayer1 = true;

			coupleBreakTouch1_ID = touch.fingerId;

			destination1 = info.touchPos;

			Player1IniPos = touch.position * 720f / Screen.width;

            Debug.Log("[Began] destination1 = " + destination1);

			if (isCouple)
			{
				info.limitInner = Player2LimitOuter - coupleStartDistance;
				coupleBreakTouch1 = true;
			}

			if (Mathf.Abs(info.draggedTransform.GetComponent<cPlayerStatus>().distance) > 30f)
			{
				info.draggedTransform.GetComponent<cPlayerStatus>().isTracking = true;
				SyncPosition(info.draggedTransform, info.draggedTransform.localPosition,
					new Vector3(info.touchPos, info.draggedTransform.localPosition.y, 0));

				if (isCouple && Player2)
				{
					SyncPosition(Player2, Player2.localPosition,
					new Vector3(info.touchPos + coupleStartDistance, Player2.localPosition.y, 0));
				}
			}

			if (showLogs) Debug.Log("[" + touch.fingerId + "] Add Player : " + info.draggedTransform.name);
		}
		else if (num == 2)
		{
			info.draggedTransform = Player2;
			info.anotherTransform = Player1;
			info.draggedUI = Player2_UI;
			info.direction = 1;
			info.limitInner = Player2LimitInner;
			info.limitOuter = Player2LimitOuter;

			info.iniPos = info.draggedTransform.localPosition.x;
			info.touchPos = touch.position.x * 720f / Screen.width;

			info.draggedTransform.GetComponent<cPlayerStatus>().distance = info.draggedTransform.localPosition.x - info.touchPos;
			info.draggedTransform.GetComponent<cPlayerStatus>().previousPosition = info.draggedTransform.transform.localPosition;

			Touch2_ID = touch.fingerId;

			selectedPlayer2 = true;

			destination2 = info.touchPos;

			Player2IniPos = touch.position * 720f / Screen.width;

			if (Mathf.Abs(info.draggedTransform.GetComponent<cPlayerStatus>().distance) > 30f)
			{
				info.draggedTransform.GetComponent<cPlayerStatus>().isTracking = true;
				SyncPosition(info.draggedTransform, info.draggedTransform.localPosition,
					new Vector3(info.touchPos, info.draggedTransform.localPosition.y, 0));
			}

			if (showLogs) Debug.Log("[" + touch.fingerId + "] Add Player : " + info.draggedTransform.name);

		}
		else
		{
			// dummy.
			info.draggedTransform = null;
			info.anotherTransform = null;
			info.draggedUI = null;
			info.direction = 1;
			info.limitInner = 0;
			info.limitOuter = 0;

			info.iniPos = 0;
			info.touchPos = 0;

			if (coupleBreakTouch2_ID == touch.fingerId)
			{
				info.iniPos = -10000f;
			}

			if (showLogs) Debug.Log("[" + touch.fingerId + "] ### Add Dummy : [" + touch.fingerId + "] ### ");
		}

		return info;
	}

	void DeselectPlayer(int num)
	{
		//DragInfo info;

		if (num == 1)
		{
			Touch1_ID = -1;
			selectedPlayer1 = false;
			coupleBreakTouch1 = false;

			if (Player1)
			{
				if (showLogs) Debug.Log("Del Player : " + Player1.name);

				Player1.GetComponent<cPlayerStatus>().StopCoroutine("HoldTimeCoroutine");
				Player1.GetComponent<cPlayerStatus>().holdTime = 0;
				Player1.GetComponent<cPlayerStatus>().tiltAngle = 0;

				if (isCouple)
				{
					Player2.GetComponent<cPlayerStatus>().StopCoroutine("HoldTimeCoroutine");
					Player2.GetComponent<cPlayerStatus>().holdTime = Player1.GetComponent<cPlayerStatus>().holdTime;
					Player2.GetComponent<cPlayerStatus>().tiltAngle = Player1.GetComponent<cPlayerStatus>().tiltAngle;
				}
			}

		}
		else if (num == 2)
		{
			Touch2_ID = -1;
			selectedPlayer2 = false;

			if (Player2)
			{
				if (showLogs) Debug.Log("Del Player : " + Player2.name);

				Player2.GetComponent<cPlayerStatus>().StopCoroutine("HoldTimeCoroutine");
				Player2.GetComponent<cPlayerStatus>().holdTime = 0;
				Player2.GetComponent<cPlayerStatus>().tiltAngle = 0;
			}
		}
		else
		{
			if (showLogs) Debug.Log("### Del Dummy ###");

			/*if (info.iniPos == -10000f)
			{
				coupleBreakTouch2 = false;
				Debug.Log("coupleBreakTouch2 = false");
			}*/
		}
	}


	void Update()
	{
		AGAIN:

		if (Input.touchCount > 0)
		{
			//if (!isWiseman)
			{
				if (!Player1 && selectedPlayer1)
					selectedPlayer1 = false;
				if (!Player2 && selectedPlayer2)
					selectedPlayer2 = false;


				

				foreach (Touch touch in Input.touches)
				{

					Ray ray = touchCamera.ScreenPointToRay(touch.position);
					RaycastHit hit;

					if (touch.phase == TouchPhase.Began && Physics.Raycast(ray, out hit) && !drags.ContainsKey(touch.fingerId))
					{
						//Start building up a profile for this specific drag
						DragInfo info = new DragInfo();

						if (hit.collider.name == dragLeft.name)
						{

							float tx = (touch.position.x * 720f / Screen.width);

							

							if (Player1 && Player2)
							{
								if(isCouple)
								{
									if (!selectedPlayer1)
									{
										// 1P 링크.
										info = SelectPlayer(1, touch);
                                    }
									else
									{

										if(Mathf.Abs(Player1.localPosition.x - tx) > coupleBreakDistance)
										{
											//분리 실행.
											StartCoroutine(CoupleModeBreak());

											DeselectPlayer(1);
											DeselectPlayer(2);

											goto AGAIN;
										}
                                    }
								}
								else
								{
									if (!selectedPlayer1 && !selectedPlayer2)
									{
										if (tx < (Player1.localPosition.x + Player2.localPosition.x) / 2f)
										{
											// 1P 링크.
											info = SelectPlayer(1, touch);
										}
										else
										{
											// 2P 링크.
											info = SelectPlayer(2, touch);
										}
									}
									else if (!selectedPlayer1 && selectedPlayer2)
									{
										if (tx < Player2.localPosition.x)
										{
											// 1P 링크.
											info = SelectPlayer(1, touch);
										}
									}
									else if (selectedPlayer1 && !selectedPlayer2)
									{
										if (tx > Player1.localPosition.x)
										{
											// 2P 링크.
											info = SelectPlayer(2, touch);
										}
                                    }									
								}
							}
							else if (Player1 && !Player2)
							{
								if (!selectedPlayer1)
								{
									// 1P 링크.
									info = SelectPlayer(1, touch);
								}
                            }
							else if (!Player1 && Player2)
							{
								if (!selectedPlayer2)
								{
									// 2P 링크.
									info = SelectPlayer(2, touch);
								}
							}


							// dummy 링크.
							if(!info.draggedTransform)
								info = SelectPlayer(0, touch);





							/*if (isCouple && Player1 && selectedPlayer1 && (hit.collider.name == dragLeft.name || hit.collider.name == Player1.name || hit.collider.name == Player2.name))
							{
								Debug.Log("[isCouple]");

								//float Touch1CurrentPos = Input.touches[coupleBreakTouch1_ID].position.x * 720f / Screen.width;

								//Debug.Log("Touch1CurrentPos = " + Touch1CurrentPos + " / touch = " + touch.position.x * 720f / Screen.width);

								//if (Mathf.Abs(Touch1CurrentPos - (touch.position.x * 720f / Screen.width)) < 300f)
								//{									
								//	coupleBreakTouch2_ID = touch.fingerId;

								//	coupleBreakTouch2 = true;

								//	Debug.Log("coupleBreakTouch2 = true / " + Mathf.Abs(Touch1CurrentPos - (touch.position.x * 720f / Screen.width))  + " < " + 300f);
								//	Debug.Log("coupleBreakTouch1_ID = " + coupleBreakTouch1_ID + " / coupleBreakTouch2_ID = " + coupleBreakTouch2_ID);
								//}
							}*/

							/*if (Player1 && !selectedPlayer1 && (hit.collider.name == dragLeft.name || hit.collider.name == Player1.name))
							{
								info.draggedTransform = Player1;
								info.anotherTransform = Player2;
								info.draggedUI = Player1_UI;
								info.direction = -1;
								info.limitInner = Player1LimitInner;
								info.limitOuter = Player1LimitOuter;

								info.iniPos = info.draggedTransform.localPosition.x;
								info.touchPos = touch.position.x * 720f / Screen.width;

								info.draggedTransform.GetComponent<cPlayerStatus>().distance = info.draggedTransform.localPosition.x - info.touchPos;
								info.draggedTransform.GetComponent<cPlayerStatus>().previousPosition = info.draggedTransform.transform.localPosition;

								Touch1_ID = touch.fingerId;

								selectedPlayer1 = true;

								coupleBreakTouch1_ID = touch.fingerId;

								destination1 = info.touchPos;
								Debug.Log("[Began] destination1 = " + destination1);

								if (isCouple)
								{
									info.limitInner = Player2LimitOuter - coupleStartDistance;
									coupleBreakTouch1 = true;
								}

								if (Mathf.Abs(info.draggedTransform.GetComponent<cPlayerStatus>().distance) > 30f)
								{
									info.draggedTransform.GetComponent<cPlayerStatus>().isTracking = true;
									SyncPosition(info.draggedTransform, info.draggedTransform.localPosition,
										new Vector3(info.touchPos, info.draggedTransform.localPosition.y, 0));

									if (isCouple && Player2)
									{
										SyncPosition(Player2, Player2.localPosition,
										new Vector3(info.touchPos + coupleStartDistance, Player2.localPosition.y, 0));
									}
								}

								if (showLogs) Debug.Log("[" + touch.fingerId + "] Add Player : " + info.draggedTransform.name + " / " + hit.collider.name);
							}
							else if (!isCouple && Player2 && !selectedPlayer2 && (hit.collider.name == dragRight.name || hit.collider.name == Player2.name))
							{
								info.draggedTransform = Player2;
								info.anotherTransform = Player1;
								info.draggedUI = Player2_UI;
								info.direction = 1;
								info.limitInner = Player2LimitInner;
								info.limitOuter = Player2LimitOuter;

								info.iniPos = info.draggedTransform.localPosition.x;
								info.touchPos = touch.position.x * 720f / Screen.width;

								info.draggedTransform.GetComponent<cPlayerStatus>().distance = info.draggedTransform.localPosition.x - info.touchPos;
								info.draggedTransform.GetComponent<cPlayerStatus>().previousPosition = info.draggedTransform.transform.localPosition;

								Touch2_ID = touch.fingerId;

								selectedPlayer2 = true;

								destination2 = info.touchPos;


								if (Mathf.Abs(info.draggedTransform.GetComponent<cPlayerStatus>().distance) > 30f)
								{
									info.draggedTransform.GetComponent<cPlayerStatus>().isTracking = true;
									SyncPosition(info.draggedTransform, info.draggedTransform.localPosition,
										new Vector3(info.touchPos, info.draggedTransform.localPosition.y, 0));
								}

								if (showLogs) Debug.Log("[" + touch.fingerId + "] Add Player : " + info.draggedTransform.name + " / " + hit.collider.name);

							}
							else
							{
								// dummy.
								info.draggedTransform = null;
								info.anotherTransform = null;
								info.draggedUI = null;
								info.direction = 1;
								info.limitInner = 0;
								info.limitOuter = 0;

								info.iniPos = 0;
								info.touchPos = 0;

								if (coupleBreakTouch2_ID == touch.fingerId)
								{
									info.iniPos = -10000f;
								}

								if (showLogs) Debug.Log("[" + touch.fingerId + "] ### Add Dummy : [" + touch.fingerId + "] ### (" + hit.collider.name + ")");
							}*/

							//Add to the dictionary
							drags.Add(touch.fingerId, info);

						}
					}



					if (touch.phase == TouchPhase.Ended && drags.ContainsKey(touch.fingerId))
					{
						DragInfo info = drags[touch.fingerId];

						if (info.draggedTransform != null)
						{
							Debug.Log("info.draggedTransform : " + info.draggedTransform.name);						
                        }

						if (info.draggedTransform == Player1)
						{
							DeselectPlayer(1);
                        }
						else if (info.draggedTransform == Player2)
						{
							DeselectPlayer(2);
						}
						else
						{
							DeselectPlayer(0);
						}

							/*if (info.draggedTransform == Player1)
							{
								Touch1_ID = -1;
								selectedPlayer1 = false;
								coupleBreakTouch1 = false;

								if (info.draggedTransform)
								{
									if (showLogs) Debug.Log("[" + touch.fingerId + "] Del Player : " + info.draggedTransform.name);

									info.draggedTransform.GetComponent<cPlayerStatus>().StopCoroutine("HoldTimeCoroutine");
									info.draggedTransform.GetComponent<cPlayerStatus>().holdTime = 0;
									info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle = 0;

									if (isCouple)
									{
										Player2.GetComponent<cPlayerStatus>().StopCoroutine("HoldTimeCoroutine");
										Player2.GetComponent<cPlayerStatus>().holdTime = info.draggedTransform.GetComponent<cPlayerStatus>().holdTime;
										Player2.GetComponent<cPlayerStatus>().tiltAngle = info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle;																		
									}
								}

							}
							else if (info.draggedTransform == Player2)
							{
								Touch2_ID = -1;
								selectedPlayer2 = false;							

								if (info.draggedTransform)
								{
									if (showLogs) Debug.Log("[" + touch.fingerId + "] Del Player : " + info.draggedTransform.name);

									info.draggedTransform.GetComponent<cPlayerStatus>().StopCoroutine("HoldTimeCoroutine");
									info.draggedTransform.GetComponent<cPlayerStatus>().holdTime = 0;
									info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle = 0;
								}
							}
							else
							{
								if (showLogs) Debug.Log("[" + touch.fingerId + "] ### Del Dummy : " + "[" + touch.fingerId + "] ###");

								if (info.iniPos == -10000f)
								{
									coupleBreakTouch2 = false;
									Debug.Log("coupleBreakTouch2 = false");
								}
							}*/

						//Remove from drags dictionary
						drags.Remove(touch.fingerId);
					}
				}

				/*if(isCouple)
				{
					if (coupleBreakTouch1 && coupleBreakTouch2)
					{
						//Debug.Log("coupleBreakTouch1 && coupleBreakTouch2");
						//Debug.Log("coupleBreakTouch1_ID : " + coupleBreakTouch1_ID + " / coupleBreakTouch2_ID : " + coupleBreakTouch2_ID);
						coupleBreakTouch1_x = Input.touches[coupleBreakTouch1_ID].position.x * 720f / Screen.width;
						coupleBreakTouch2_x = Input.touches[coupleBreakTouch2_ID].position.x * 720f / Screen.width;
						
						if (Mathf.Abs(coupleBreakTouch2_x - coupleBreakTouch1_x) >= coupleBreakDistance)
						{
							//분리 실행.
							StartCoroutine(CoupleModeBreak());
						}
					}
				}*/
			}
		}
	}

	void FixedUpdate()
	{				
		playersDistance = Player2.localPosition.x - Player1.localPosition.x;

		// 합체 시작.
		if (!isCouple && !isWiseman && playersDistance <= coupleStartDistance)
		{
			CoupleModeStart();
		}

		if (Input.touchCount > 0) 
		{
			// 현자타임이 아닐 때 이동한다.
			if (!isWiseman)
			{
				int index = 0;
				foreach (var entry in drags.OrderBy(dic => dic.Key))
				{
					//if (showLogs) Debug.Log("count: " + index + " / " + entry.Key + " / " + entry.value.draggedTransform.name); // index[Max] = entry.Key

					Touch touch = Input.GetTouch(index);
					DragInfo info = entry.Value;

					//Drag
					if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
					{
						//Simple Move
						//info.draggedTransform.localPosition = new Vector3((touch.position.x * 720f / Screen.width) + info.draggedTransform.GetComponent<cPlayerStatus>().distance, info.draggedTransform.localPosition.y, 0);
						//info.draggedTransform.localPosition = new Vector3((touch.position.x * 720f / Screen.width), info.draggedTransform.localPosition.y, 0);


						if (info.draggedTransform)
						{
							//이전 좌표를 저장.
							info.draggedTransform.GetComponent<cPlayerStatus>().previousPosition = info.draggedTransform.transform.localPosition;

							if (isCouple)
							{
								if (info.draggedTransform == Player1)
								{
									if(!coupleBreakTouch1)
										coupleBreakTouch1 = true;

									//Debug.Log("info.limitInner = " + info.limitInner);
								}
								if (info.draggedTransform == Player2)
								{
									// 합체 상태에서의 2p의 상태는 변화가 완료된 1p 상태를 따른다.								
									continue;
								}
							}


							
							

							//새로운 좌표.
							float lx = 0;
							//lx = (touch.position.x * 720f / Screen.width) + info.draggedTransform.GetComponent<cPlayerStatus>().distance;

							// 목표 지점.
							lx = (touch.position.x * 720f / Screen.width);



							//itween 이동하는 도중.
							if (info.draggedTransform.GetComponent<cPlayerStatus>().isTracking)
							{
								//Debug.Log("[Began] destination1 = " + destination1);

								float d = 0;

								if (info.draggedTransform == Player1)
								{
									d = destination1;
								}
								else
								{
									d = destination2;
								}

								// 이동하는 도중에 터치 지점이 움직였다.
								if (Mathf.Abs(d - lx) > 30f)
								{									
									if (info.draggedTransform == Player1)
									{
										destination1 = lx;
									}
									else
									{
										destination2 = lx;
									}


									SyncPosition(info.draggedTransform, info.draggedTransform.localPosition,
										new Vector3(lx, info.draggedTransform.localPosition.y, 0));

									if (isCouple)
									{
										SyncPosition(Player2, Player2.localPosition,
										new Vector3(lx + coupleStartDistance, Player2.localPosition.y, 0));
									}
								}

								if (Mathf.Abs(info.draggedTransform.localPosition.x - lx) <= 30f)
								{
									info.draggedTransform.GetComponent<cPlayerStatus>().isTracking = false;
								}

							}
							//else



							//scrollingObjects.GetComponent<cScrollHorizontal>().CheckLimit();

							// [폐기] 최종 좌표가 화면 중간을 넘어서지 않는다.														
							//if (lx * info.direction > (scrollingObjects.localPosition.x + info.limitInner + middleLimit) * info.direction)

							// 플레이어 사이의 거리가 합체 거리 이상이면 움직일 수 있다.
							//if(playersDistance >= coupleStartDistance)
							{
								// [폐기] 화면 횡스크롤.
								/*if (scrollingObjects.GetComponent<cScrollHorizontal>().isScrolling)
								{
									// 화면 끝으로 가면 스크롤 오브젝트를 활성화시킨다. 유닛은 스크롤 오브젝트 내에서만 움직인다.
									if (lx * info.direction > (scrollingObjects.localPosition.x + 360f + (LIMIT_BORDER * info.direction)) * info.direction)
									{
										if (lx > scrollingObjects.GetComponent<cScrollHorizontal>().limitRight.x + 360f + (LIMIT_BORDER * info.direction))
										{
											lx = scrollingObjects.GetComponent<cScrollHorizontal>().limitRight.x + 360f + (LIMIT_BORDER * info.direction);
											scrollingObjects.GetComponent<cScrollHorizontal>().dirX = 0;
										}
										else if (lx < scrollingObjects.GetComponent<cScrollHorizontal>().limitLeft.x + 360f + (LIMIT_BORDER * info.direction))
										{
											lx = scrollingObjects.GetComponent<cScrollHorizontal>().limitLeft.x + 360f + (LIMIT_BORDER * info.direction);
											scrollingObjects.GetComponent<cScrollHorizontal>().dirX = 0;
										}
										else
										{
											lx = scrollingObjects.localPosition.x + 360f + (LIMIT_BORDER * info.direction);
											scrollingObjects.GetComponent<cScrollHorizontal>().dirX = info.direction;
										}

										scrollingObjects.GetComponent<cScrollHorizontal>().CheckLimit();
									}
								}*/
															

								if (info.anotherTransform != null)
								{
									//두 플레이어간의 최대 거리를 제한한다.
									if ((lx - info.anotherTransform.localPosition.x) * info.direction > LIMIT_PLAYERS_DISTANCE)
										lx = info.anotherTransform.localPosition.x + (LIMIT_PLAYERS_DISTANCE * info.direction);

									
								}


								//최소값 또는 최대값 보정. LIMIT_BORDER = 300
								if (isCouple)
								{
									// lx는 600 오른쪽으로 가지 않는다.
									if (lx > scrollingObjects.localPosition.x + 360f + LIMIT_BORDER - coupleStartDistance)
										lx = scrollingObjects.localPosition.x + 360f + LIMIT_BORDER - coupleStartDistance;
								}
								else
								{
									// lx는 660 오른쪽으로 가지 않는다.
									if (lx > scrollingObjects.localPosition.x + 360f + LIMIT_BORDER)
										lx = scrollingObjects.localPosition.x + 360f + LIMIT_BORDER;
								}
								// lx는 60 왼쪽으로 가지 않는다.
								if (lx < scrollingObjects.localPosition.x + 360f - LIMIT_BORDER)
									lx = scrollingObjects.localPosition.x + 360f - LIMIT_BORDER;

								/*
								// lx는 1020 오른쪽으로 가지 않는다.
								if (lx > 720f + LIMIT_BORDER)
									lx = 720f + LIMIT_BORDER;
								// lx는 -300 왼쪽으로 가지 않는다.
								if (lx < -LIMIT_BORDER)
									lx = -LIMIT_BORDER;*/


								// 최종 좌표.
								if (!info.draggedTransform.GetComponent<cPlayerStatus>().isTracking)
								{
									if (Mathf.Abs(lx - info.draggedTransform.GetComponent<cPlayerStatus>().previousPosition.x) >= minimumPixelToMove)
									{
										info.draggedTransform.localPosition = new Vector3(lx, info.draggedTransform.localPosition.y, 0);

										if (info.draggedTransform == Player1)
											Player1IniPos = touch.position * 720f / Screen.width;
										if (info.draggedTransform == Player2)
											Player2IniPos = touch.position * 720f / Screen.width;

										if (isCouple)
										{
											// 위치.
											Player2.localPosition = new Vector3(Player1.localPosition.x + coupleStartDistance, Player2.localPosition.y, 0);

											Player2IniPos = touch.position * 720f / Screen.width;
										}
									}
								}

								// 기울이기.
								if (Mathf.Abs(lx - info.draggedTransform.GetComponent<cPlayerStatus>().previousPosition.x) >= minimumPixelToTilt)
								{
									if (lx > info.draggedTransform.GetComponent<cPlayerStatus>().previousPosition.x)
									{
										//오른쪽으로 기울인다.
										if (info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle > -tiltAngle)
											info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle = -tiltAngle;

										//if (showLogs) Debug.Log("//////////////// tilt Right : " + info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle);

										info.draggedTransform.GetComponent<cPlayerStatus>().holdTime = holdTime;
										info.draggedTransform.GetComponent<cPlayerStatus>().HoldTimeUpdate();

										if(isCouple)
										{
											Player2.GetComponent<cPlayerStatus>().tiltAngle = info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle;
											Player2.GetComponent<cPlayerStatus>().holdTime = info.draggedTransform.GetComponent<cPlayerStatus>().holdTime;
											Player2.GetComponent<cPlayerStatus>().HoldTimeUpdate();
										}

									}
									else if (lx < info.draggedTransform.GetComponent<cPlayerStatus>().previousPosition.x)
									{
										//왼쪽으로 기울인다.								
										if (info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle < tiltAngle)
											info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle = tiltAngle;

										//if (showLogs) Debug.Log("//////////////// tilt Left : " + info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle);
										
										info.draggedTransform.GetComponent<cPlayerStatus>().holdTime = holdTime;
										info.draggedTransform.GetComponent<cPlayerStatus>().HoldTimeUpdate();

										if (isCouple)
										{
											Player2.GetComponent<cPlayerStatus>().tiltAngle = info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle;
											Player2.GetComponent<cPlayerStatus>().holdTime = info.draggedTransform.GetComponent<cPlayerStatus>().holdTime;
											Player2.GetComponent<cPlayerStatus>().HoldTimeUpdate();
										}
									}
									else
									{
										SetTiltZero(info);
									}
								}
								else
								{
									SetTiltZero(info);
								}
							}
							/*else
							{
								// 플레이어 사이의 거리가 합체 거리 이하이면 움직일 수 없다.

								//Debug.Log("--- : - " + lx + " < - ( " + scrollingObjects.localPosition.x + " + " + info.limitInner + " )");
								SetTiltZero(info);

								// [폐기] 화면 중간에서 멈춘다.
								//lx = scrollingObjects.localPosition.x + info.limitInner;
								//info.draggedTransform.localPosition = new Vector3(lx, info.draggedTransform.localPosition.y, 0);
							}*/

							SetDistance(info, touch);
						}
					}

					index++;
				}

				//if (isCouple)
				//{
				//	// 위치.
				//	Player2.localPosition = new Vector3(Player1.localPosition.x + coupleStartDistance, Player2.localPosition.y, 0);

				//	// 기울이기.
				//	Player2.GetComponent<cPlayerStatus>().tiltAngle = Player1.GetComponent<cPlayerStatus>().tiltAngle;
				//	Player2.GetComponent<cPlayerStatus>().holdTime = Player1.GetComponent<cPlayerStatus>().holdTime;				
				//}
			}			
		}

		if (SystemInfo.deviceType == DeviceType.Desktop && Input.touchCount == 0)
			KeyboardMove();



		
	}

	void SyncPosition(Transform target, Vector3 from, Vector3 to, float time = 0.2f)
	{
		Debug.Log("[SyncPosition] from " + from + " to " + to);
		iTween.Stop(target.gameObject, false);
		

		iTween.MoveTo(target.gameObject, iTween.Hash(
			"path", new Vector3[] { from, to },
			"easetype", iTween.EaseType.linear,
			"looptype", iTween.LoopType.none,
			"movetopath", false,
			"islocal", true,
			"oncompletetarget", gameObject,
            "oncomplete", "SyncEnd",
			"oncompleteparams", target.name,

            "time", time
			));
	}
	
	public void SyncEnd(object target)
	{
		Debug.Log(CodeManager.GetMethodName());
		string T = target as string;

		if (T == "Player1")
		{
			Player1.GetComponent<cPlayerStatus>().isTracking = false;
		}
		else
		{
			Player2.GetComponent<cPlayerStatus>().isTracking = false;
		}
		
	}

	/// <summary>
	/// holdTime이 0이면 기울이기 각도를 0으로 고정한다.
	/// </summary>
	/// <param name="info"></param>
	void SetTiltZero(DragInfo info)
	{
		if (info.draggedTransform.GetComponent<cPlayerStatus>().holdTime == 0)
		{
			info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle = 0;

			//if (showLogs) Debug.Log("//////////////// tilt Stop : " + info.draggedTransform.GetComponent<cPlayerStatus>().tiltAngle);

			if (isCouple)
			{
				Player2.GetComponent<cPlayerStatus>().tiltAngle = 0;
			}
		}
	}

	/// <summary>
	/// 터치점과 기체의 거리가 달라지면 거리를 재설정한다.
	/// </summary>
	/// <param name="info"></param>
	/// <param name="touch"></param>
	void SetDistance(DragInfo info, Touch touch)
	{		
		if (info.draggedTransform.GetComponent<cPlayerStatus>().distance != info.draggedTransform.localPosition.x - (touch.position.x * 720f / Screen.width))
		{
			info.draggedTransform.GetComponent<cPlayerStatus>().distance = info.draggedTransform.localPosition.x - (touch.position.x * 720f / Screen.width);
			//if (showLogs) Debug.Log((info.draggedTransform.GetComponent<cPlayerStatus>().distance) + " = " + (info.draggedTransform.localPosition.x - (touch.position.x * 720f / Screen.width)));
		}
	}

	void KeyboardMove()
	{
		keyboardDirectionPlayer1 = 0;
		keyboardDirectionPlayer2 = 0;

		if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
		{
			keyboardDirectionPlayer1 = -1f;

			if (isCouple)
				keyboardDirectionPlayer2 = keyboardDirectionPlayer1;
		}

		if (!Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
		{
			keyboardDirectionPlayer1 = 1f;

			if (isCouple)
				keyboardDirectionPlayer2 = keyboardDirectionPlayer1;
		}

		if (!isCouple)
		{
			if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
				keyboardDirectionPlayer2 = -1f;

			if (!Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))
				keyboardDirectionPlayer2 = 1f;
		}
		else
		{
			if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.RightArrow))
			{
				//분리 실행.
				StartCoroutine(CoupleModeBreak());
			}
		}

		keyboardVectorPlayer1 = new Vector3(keyboardDirectionPlayer1, 0, 0);
		keyboardVectorPlayer2 = new Vector3(keyboardDirectionPlayer2, 0, 0);		

		if (Player1 != null)
		{
			if (isWiseman)
			{
				Player1.GetComponent<Rigidbody>().velocity = Vector3.zero;
				Player1.GetComponent<cPlayerStatus>().tiltAngle = 0;
			}
			else
			{
				Player1.GetComponent<Rigidbody>().velocity = keyboardVectorPlayer1 * 5f;
				Player1.GetComponent<cPlayerStatus>().tiltAngle = keyboardVectorPlayer1.normalized.x * -tiltAngle;
			}
		}
		if (Player2 != null)
		{
			if (!isCouple)
			{
				if (isWiseman)
				{
					Player2.GetComponent<Rigidbody>().velocity = Vector3.zero;
					Player2.GetComponent<cPlayerStatus>().tiltAngle = 0;
				}
				else
				{
					Player2.GetComponent<Rigidbody>().velocity = keyboardVectorPlayer2 * 5f;
					Player2.GetComponent<cPlayerStatus>().tiltAngle = keyboardVectorPlayer2.normalized.x * -tiltAngle;
				}
			}
			else
			{
				Player2.localPosition = new Vector3(Player1.localPosition.x + coupleStartDistance, Player2.localPosition.y, 0);
				Player2.GetComponent<Rigidbody>().velocity = Player1.GetComponent<Rigidbody>().velocity;
				Player2.GetComponent<cPlayerStatus>().tiltAngle = Player1.GetComponent<cPlayerStatus>().tiltAngle;
			}
		}	
	}

	void CoupleModeStart()
	{
		Debug.Log(CodeManager.GetMethodName());

		isCouple = true;

		//dragRight.GetComponent<BoxCollider>().enabled = false;


		foreach (var entry in drags.OrderBy(dic => dic.Key))
		{
			DragInfo info = entry.Value;

			if (info.draggedTransform == Player1)
			{				
				info.limitInner = Player2LimitOuter - coupleStartDistance;
				middleLimit = Player2LimitOuter - coupleStartDistance;
				Debug.Log(CodeManager.GetMethodName() + " Found Player1 limitInner : " + info.limitInner);
			}
			/*else if (info.draggedTransform == Player2)
			{
				Debug.Log(CodeManager.GetMethodName() + " Set Player2 info to dummy");
				
				// dummy.
				info.draggedTransform = null;
				info.anotherTransform = null;
				info.draggedUI = null;
				info.direction = 1;
				info.limitInner = 0;
				info.limitOuter = 0;

				info.iniPos = 0;
				info.touchPos = 0;
			}*/
		}
		
		if (selectedPlayer2)
		{			
			Debug.Log(CodeManager.GetMethodName() + " Delete : Player 2 drag info : Touch2_ID = " + Touch2_ID);

			selectedPlayer2 = false;
			coupleBreakTouch2 = false;

			drags.Remove(Touch2_ID);			
		}


		//dragLeft.GetComponent<BoxCollider>().center = coupleDragVectorCenter;
		//dragLeft.GetComponent<BoxCollider>().size = coupleDragVectorSize;
		

		//touch - model 동기화.
	}

	IEnumerator CoupleModeBreak()
	{
		Debug.Log("[CoupleModeBreak] isWiseman = true");

		isCouple = false;
		coupleBreakTouch1 = false;
		coupleBreakTouch2 = false;
		coupleBreakTouch1_ID = -1;
		coupleBreakTouch2_ID = -1;
		coupleBreakTouch1_x = 0;
		coupleBreakTouch2_x = 0;
		middleLimit = 0;

		isWiseman = true;

		//dragLeft.GetComponent<BoxCollider>().center = player1DragVectorCenter;
		//dragLeft.GetComponent<BoxCollider>().size = player1DragVectorSize;
		//dragRight.GetComponent<BoxCollider>().enabled = true;

		foreach (var entry in drags.OrderBy(dic => dic.Key))
		{
			DragInfo info = entry.Value;

			if (info.draggedTransform == Player1)
			{
				info.limitInner = Player1LimitInner;
			}
			else continue;
		}




		
		/*
		foreach (Touch touch in Input.touches)
		{
			Ray ray = touchCamera.ScreenPointToRay(touch.position);
			RaycastHit hit;


			if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && 
				Physics.Raycast(ray, out hit) && drags.ContainsKey(touch.fingerId))
			{
				DragInfo info = new DragInfo();
				
				if (hit.collider.name == dragLeft.name)
				{
					float tx = (touch.position.x * 720f / Screen.width);
					
					if (Player1 && Player2)
					{

						{
							if (!selectedPlayer1 && !selectedPlayer2)
							{
								if (tx < (Player1.localPosition.x + Player2.localPosition.x) / 2f)
								{
									// 1P 링크.
									info = SelectPlayer(1, touch);
								}
								else
								{
									// 2P 링크.
									info = SelectPlayer(2, touch);
								}
							}
							else if (!selectedPlayer1 && selectedPlayer2)
							{
								if (tx < Player2.localPosition.x)
								{
									// 1P 링크.
									info = SelectPlayer(1, touch);
								}
							}
							else if (selectedPlayer1 && !selectedPlayer2)
							{
								if (tx > Player1.localPosition.x)
								{
									// 2P 링크.
									info = SelectPlayer(2, touch);
								}
							}
						}
					}
					else if (Player1 && !Player2)
					{
						if (!selectedPlayer1)
						{
							// 1P 링크.
							info = SelectPlayer(1, touch);
						}
					}
					else if (!Player1 && Player2)
					{
						if (!selectedPlayer2)
						{
							// 2P 링크.
							info = SelectPlayer(2, touch);
						}
					}


					// dummy 링크.
					if (!info.draggedTransform)
						info = SelectPlayer(0, touch);
				}

				drags.Remove(touch.fingerId);
				drags.Add(touch.fingerId, info);
			}
		}*/

		//if (Player1.localPosition != Player1IniPos)
		{
			iTween.MoveTo(Player1.gameObject, iTween.Hash(
				"path", new Vector3[] { Player1.localPosition, Player1IniPos },
				/*"easetype", iTween.EaseType.linear,*/
				"looptype", iTween.LoopType.none,
				"movetopath", false,
				"islocal", true,
				"time", wisemanTime));
		}
		//if (Player2.localPosition != Player2IniPos)
		{
			iTween.MoveTo(Player2.gameObject, iTween.Hash(
				"path", new Vector3[] { Player2.localPosition, Player2IniPos },
				/*"easetype", iTween.EaseType.linear,*/
				"looptype", iTween.LoopType.none,
				"movetopath", false,
				"islocal", true,
				"time", wisemanTime));
		}





		CoupleModeBreakRotateStart();

		yield return new WaitForSeconds(wisemanTime);

		CoupleModeBreakRotateStop();		
	}

	void CoupleModeBreakRotateStart()
	{
		Debug.Log(CodeManager.GetMethodName());

		Player1.GetComponent<cPlayerStatus>().StopCoroutine("HoldTimeCoroutine");
		Player1.GetComponent<cPlayerStatus>().holdTime = 0;
		Player1.GetComponent<cPlayerStatus>().tiltAngle = 0;
		Player2.GetComponent<cPlayerStatus>().StopCoroutine("HoldTimeCoroutine");
		Player2.GetComponent<cPlayerStatus>().holdTime = 0;
		Player2.GetComponent<cPlayerStatus>().tiltAngle = 0;


		iTween.RotateBy(Player1_Model.gameObject, iTween.Hash(
				"amount", Player1_ModelRotationAxis,
				"easetype", iTween.EaseType.linear,
				"looptype", iTween.LoopType.none,
				"time", wisemanTime));
		iTween.RotateBy(Player2_Model.gameObject, iTween.Hash(
				"amount", Player2_ModelRotationAxis,
				"easetype", iTween.EaseType.linear,
				"looptype", iTween.LoopType.none,
				"time", wisemanTime));


	}

	void CoupleModeBreakRotateStop()
	{
		Debug.Log(CodeManager.GetMethodName() + " isWiseman = false");

		isWiseman = false;

		if (Player1_Model.localRotation.eulerAngles != Player1_ModelInitRotation)
		{
			Debug.Log(Player1_Model.localRotation.eulerAngles + " != " + Player1_ModelInitRotation);
			iTween.RotateTo(Player1_Model.gameObject, iTween.Hash(
				"rotation", Player1_ModelInitRotation,
				"islocal", true,
                "easetype", iTween.EaseType.linear,
				"looptype", iTween.LoopType.none,
				"time", 0.5f));
		}
		if (Player2_Model.localRotation.eulerAngles != Player2_ModelInitRotation)
		{
			Debug.Log(Player2_Model.localRotation.eulerAngles + " != " + Player2_ModelInitRotation);
			iTween.RotateTo(Player2_Model.gameObject, iTween.Hash(
				"rotation", Player2_ModelInitRotation,
				"islocal", true,
				"easetype", iTween.EaseType.linear,
				"looptype", iTween.LoopType.none,
				"time", 0.5f));
		}
	}


	
}
