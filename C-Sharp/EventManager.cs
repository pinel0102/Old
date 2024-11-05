using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EventManager : SingletonMono<EventManager>
{
    [Header("★ [Parameter] Current Event : Special (Debug)")]
    public SpecialEventType debugEvent = SpecialEventType.NONE;
    public bool debugRefresh;

    [Header("★ [Parameter] Settings")]
    //public Vector2 gameModeViewportOffsetMax_Default = new Vector2(-360, 0);
    //public Vector2 gameModeViewportOffsetMax_Big = Vector2.zero;
    public Vector2 gameModeButtonSize_Default;
    public Vector2 gameModeButtonSize_Big;
    
    [Header("★ [Parameter] Current Event")]
    public int generalEventCount;
    public int specialEventCount;
    public bool onSpecialEvent;
    
    ///<Summary>
    ///<para>오늘의 목표 세부 정보.</para>
    ///<para>[0] 일일 도전 1</para>
    ///<para>[1] 이벤트 레벨 7</para>
    ///<para>[2] 리워드 레벨 2 (일반 레벨)</para>
    ///<para>[3] 하드 레벨 1 (일반 레벨)</para>
    ///<para>[4] 부스터 사용 1</para>
    ///<para>[5] * 모든 레벨 12</para>
    ///<para>[6] * 비밀 레벨 1</para>
    ///</Summary>
    public List<int> clearedGoalsList = new List<int>();

    [Header("★ [Parameter] Old Info")]
    public List<int> cachedClearCount = new List<int>();
    public bool cachedGoalsCleared;
    public bool cachedCanPlaySecret;
    public float cachedCalenderPercent;

    [Header("★ [Parameter] Current Event : Special")]
    public SpecialEventType currentEvent = SpecialEventType.NONE;

    [Header("★ [Parameter] Calendar Info")]
    public int selectedYear;
    public int selectedMonth;
    public int selectedDay;
    public bool isDailyChallengeShortcut;
    public bool isDailyChallenge { get { return (eventInfo_Daily != null) && 
                                    (eventInfo_Daily.year == selectedYear) && 
                                    (eventInfo_Daily.month == selectedMonth) && 
                                    (eventInfo_Daily.day == selectedDay);}}

    [Header("★ [Parameter] Event Info")]
    public List<CalendarLevels> calendarLevels;
    public EventInfo_Goals eventInfo_Goals;
    public EventInfo_Daily eventInfo_Daily;
    public EventInfo_Secret eventInfo_Secret;
    public EventInfo_Jigsaw eventInfo_Jigsaw;
    public EventInfo_Adventure eventInfo_Adventure;

    [Header("★ [Reference] UI")]
    //public RectTransform gameModeViewPort;
    public CalendarManager calendarManager;
    public RectTransform gameModeButtonDaily;
    public GameObject gameModeButtonJigsaw;
    public GameObject gameModeButtonAdventure;
    public ToggleController footerToggle;
    public GoalsSetter mainGoalsSetter;

    [Header("★ [Parameter] Event Item")]
    private EventItem_Calendar eventItem_Calendar;
    private EventItem_Goals eventItem_Goals;
    private EventItem_Daily eventItem_Daily;
    private EventItem_Secret eventItem_Secret;
    private EventItem_Jigsaw eventItem_Jigsaw;
    private EventItem_Adventure eventItem_Adventure;

    private UserData userData { get { return UserData.Instance; } }
    private RandomManager randomManager { get { return RandomManager.Instance; } }
    private TutorialManager tutorialManager { get { return TutorialManager.Instance; } }
    private GameMode gameMode { get {return GameMode.Instance;}}
    private EventTime eventTime { get {return EventTime.Instance;}}
    private JigsawManager jigsawManager { get { return JigsawManager.Instance; } }
    private AdventureManager adventureManager { get { return AdventureManager.Instance; } }


#region Initialize

    private void Awake()
    {
        gameModeButtonJigsaw.SetActive(false);
        gameModeButtonAdventure.SetActive(false);
    }

    public void Initialize()
    {
        gameModeButtonJigsaw.SetActive(false);
        gameModeButtonAdventure.SetActive(false);

        StartSpecialEvent();
        StartGeneralEvent();

        RefreshClearCountCache();
        mainGoalsSetter.RefreshButtons();

        eventTime.Initialize();
    }

    private void StartGeneralEvent()
    {
        CheckCurrent_Calendar();
        CheckCurrent_GeneralEvent();
    }

    private void StartSpecialEvent()
    {
        specialEventCount = userData.LoadData(EventDefine.key_eventInfo_Count_Special, 0);
        debugRefresh = false;
        
        switch(debugEvent)
        {
            case SpecialEventType.JIGSAW_PUZZLE:
                Remove_SpecialEvent();
                if (randomManager.CanCreateJigsawSet())
                {
                    gameModeButtonDaily.sizeDelta = gameModeButtonSize_Default;
                    CreateEventInfo_Jigsaw(EventDefine.Jigsaw_Day);
                    debugRefresh = true;
                }
                break;
            case SpecialEventType.ADVENTURE:
                Remove_SpecialEvent();
                if (randomManager.CanCreateAdventureSet())
                {
                    gameModeButtonDaily.sizeDelta = gameModeButtonSize_Default;
                    CreateEventInfo_Adventure(EventDefine.Adventure_Day);
                    debugRefresh = true;
                }
                break;
            case SpecialEventType.DEBUG_RESET:
                Remove_SpecialEvent();
                debugRefresh = true;
                break;
            default:
                CheckCurrent_SpecialEvent();
                break;
        }
        
        if (!onSpecialEvent)
            CreateNew_SpecialEvent();
    }

    private void CheckCurrent_Calendar()
    {
        List<string> dailySummary = userData.LoadListString(EventDefine.key_eventInfo_Daily_Summary);

        if (dailySummary.Count == 0)
        {
            CreateCalendarInfo();
        }
        else
        {
            LoadCalendarInfo(dailySummary);
        }
    }

    private void CheckCurrent_GeneralEvent()
    {
        generalEventCount = userData.LoadData(EventDefine.key_eventInfo_Count_General, 0);

        if (generalEventCount == 0)
        {
            CreateEventInfo_General(0);
        }
        else
        {
            LoadEventInfo_General();
        }
    }

    private void CheckCurrent_SpecialEvent()
    {
        //specialEventCount = userData.LoadData(EventDefine.key_eventInfo_Count_Special, 0);
        int lastSpecialEvent = userData.LoadData(EventDefine.key_eventInfo_Type, 0);        

        switch(lastSpecialEvent)
        {
            case (int)SpecialEventType.JIGSAW_PUZZLE:
                LoadEventInfo_Jigsaw();
                break;
            case (int)SpecialEventType.ADVENTURE:
                LoadEventInfo_Adventure();
                break;
            default:
                Remove_SpecialEvent();
                break;
        }
    }

    private void CreateNew_SpecialEvent()
    {
        if (randomManager.CanCreateJigsawSet())
        {
            //gameModeViewPort.offsetMax = gameModeViewportOffsetMax_Default;
            gameModeButtonDaily.sizeDelta = gameModeButtonSize_Default;
            CreateEventInfo_Jigsaw(EventDefine.Jigsaw_Day);
        }
        else if (randomManager.CanCreateAdventureSet())
        {
            //gameModeViewPort.offsetMax = gameModeViewportOffsetMax_Default;
            gameModeButtonDaily.sizeDelta = gameModeButtonSize_Default;
            CreateEventInfo_Adventure(EventDefine.Adventure_Day);
        }
        else
        {
            Debug.Log(CodeManager.GetMethodName() + "Can Not Create Special Event");
            
            //gameModeViewPort.offsetMax = gameModeViewportOffsetMax_Big;
            gameModeButtonDaily.sizeDelta = gameModeButtonSize_Big;
            currentEvent = SpecialEventType.NONE;
            onSpecialEvent = false;
        }
    }

#endregion Initialize


#region [General Event] Calendar

    private void CreateCalendarInfo()
    {
        Debug.Log(CodeManager.GetMethodName());

        DateTime now = DateTime.Now;
        int year = now.Year;
        int month = now.Month;
        int days = DateTime.DaysInMonth(year, month);
        
        eventItem_Calendar = new EventItem_Calendar();
        eventItem_Calendar.CreateEvent(year, month, days);
    }

    private void LoadCalendarInfo(List<string> dailySummary)
    {
        Debug.Log(CodeManager.GetMethodName());

        eventItem_Calendar = new EventItem_Calendar();
        eventItem_Calendar.LoadEvent(dailySummary);
    }

    public void SaveEventInfo_Calendar(CalendarLevels calendar)
    {
        if (eventItem_Calendar != null)
            eventItem_Calendar.SaveEvent(calendar);
    }

    public int GetSelectedCalandarLevel()
    {
        CalendarLevels calendar = GetSelectedCalendar();

        if (calendar != null)
            return calendar.GetLevelNumber(selectedDay);
        else
            return -1;
    }

    public int GetYear(string inline)
    {
        return int.Parse(inline.Substring(0, 4));
    }

    public int GetMonth(string inline)
    {
        return int.Parse(inline.Substring(4, 2));
    }

    public void SelectCalendarToday()
    {
        SelectCalendar(DateTime.Now);
    }

    public void SelectCalendar(DateTime dateTime)
    {
        //Debug.Log(CodeManager.GetMethodName() + dateTime.Day);

        selectedYear = dateTime.Year;
        selectedMonth = dateTime.Month;
        selectedDay = dateTime.Day;
    }

    public void SelectCalendar(int year, int month, int day)
    {
        //Debug.Log(CodeManager.GetMethodName() + day);

        selectedYear = year;
        selectedMonth = month;
        selectedDay = day;
    }

    public CalendarLevels GetSelectedCalendar()
    {
        return GetCalendar(selectedYear, selectedMonth);
    }

    public CalendarLevels GetCalendar(int year, int month)
    {
        int index = calendarLevels.FindIndex(item => item.year == year && item.month == month);
        if (index > -1)
            return calendarLevels[index];
        else
            return null;
    }

#endregion [General Event] Calendar


#region [General Event] Goals / Daily / Secret

    private void CreateEventInfo_General(int days)
    {
        Debug.Log(CodeManager.GetMethodName());

        generalEventCount++;

        eventItem_Goals = new EventItem_Goals();
        eventItem_Goals.CreateEvent(days);

        eventItem_Daily = new EventItem_Daily();
        eventItem_Daily.CreateEvent(days);

        eventItem_Secret = new EventItem_Secret();
        eventItem_Secret.CreateEvent(days);
    }

    private void LoadEventInfo_General()
    {
        Debug.Log(CodeManager.GetMethodName());

        List<string> goalsInfo = userData.LoadListString(EventDefine.key_eventInfo_Goals);
        if (goalsInfo.Count > 0 && !IsExpired(goalsInfo[1]))
        {
            eventItem_Goals = new EventItem_Goals();
            eventItem_Goals.LoadEvent(goalsInfo);

            List<string> dailyInfo = userData.LoadListString(EventDefine.key_eventInfo_Daily);
            eventItem_Daily = new EventItem_Daily();
            if (dailyInfo.Count > 0)
                eventItem_Daily.LoadEvent(dailyInfo);
            else
                eventItem_Daily.CreateEvent(0);
            
            List<string> secretInfo = userData.LoadListString(EventDefine.key_eventInfo_Secret);
            eventItem_Secret = new EventItem_Secret();
            if (secretInfo.Count > 0)
                eventItem_Secret.LoadEvent(secretInfo);
            else
                eventItem_Secret.CreateEvent(0);
        }
        else
        {
            Remove_GeneralEvent();
            CreateEventInfo_General(0);
        }
    }

    public void SaveEventInfo_Goals()
    {
        if (eventItem_Goals != null)
            eventItem_Goals.SaveEvent();
    }

    public void SaveEventInfo_Daily()
    {
        if (eventItem_Daily != null)
            eventItem_Daily.SaveEvent();
    }

    public void SaveEventInfo_Secret()
    {
        if (eventItem_Secret != null)
            eventItem_Secret.SaveEvent();
    }    

#endregion [General Event] Goals / Daily / Secret


#region [Special Event] Jigsaw Puzzle

    private void CreateEventInfo_Jigsaw(int days)
    {   
        Debug.Log(CodeManager.GetMethodName());

        if (specialEventCount == 0)
        {
            randomManager.currentJigsawTheme = ServerDefine.JigsawThemes[0];
            randomManager.currentJigsawThemeIndex = 0;
            randomManager.currentJigsawTheme_Localized = gameMode.localizedText_JigsawThemes[0];
        }

        specialEventCount++;

        eventItem_Jigsaw = new EventItem_Jigsaw();
        eventItem_Jigsaw.CreateEvent(days);
    }

    private void LoadEventInfo_Jigsaw()
    {
        Debug.Log(CodeManager.GetMethodName());

        List<string> eventInfo = userData.LoadListString(EventDefine.key_eventInfo_Jigsaw);

        if (!IsExpired(eventInfo[1]))
        {
            eventItem_Jigsaw = new EventItem_Jigsaw();
            eventItem_Jigsaw.LoadEvent(eventInfo);
        }
        else
        {
            Remove_SpecialEvent();
        }
    }

    public void SaveEventInfo_Jigsaw()
    {
        if (eventItem_Jigsaw != null)
            eventItem_Jigsaw.SaveEvent(eventInfo_Jigsaw.currentPage);
    }

    public void SaveEventInfo_Jigsaw(int jigsawIndex)
    {
        if (eventItem_Jigsaw != null)
            eventItem_Jigsaw.SaveEvent(jigsawIndex);
    }

#endregion [Special Event] Jigsaw Puzzle


#region [Special Event] Adventure

    private void CreateEventInfo_Adventure(int days)
    {   
        Debug.Log(CodeManager.GetMethodName());

        specialEventCount++;

        eventItem_Adventure = new EventItem_Adventure();
        eventItem_Adventure.CreateEvent(days);
    }

    private void LoadEventInfo_Adventure()
    {
        Debug.Log(CodeManager.GetMethodName());

        List<string> eventInfo = userData.LoadListString(EventDefine.key_eventInfo_Adventure);

        if (!IsExpired(eventInfo[1]))
        {
            eventItem_Adventure = new EventItem_Adventure();
            eventItem_Adventure.LoadEvent(eventInfo);
        }
        else
        {
            Remove_SpecialEvent();
        }
    }

    public void SaveEventInfo_Adventure()
    {
        if (eventItem_Adventure != null)
            eventItem_Adventure.SaveEvent();
    }

#endregion [Special Event] Adventure


#region General

    public void AddClearedGoals_UsedItem()
    {
        if(!clearedGoalsList.Contains(4))
            clearedGoalsList.Add(4);
    }

    public void AddClearedGoals(GameType gameType, bool rewardLevel = false, bool hardLevel = false)
    {
        if(!clearedGoalsList.Contains(5))
            clearedGoalsList.Add(5);

        switch(gameType)
        {
            case GameType.Normal:
                if (rewardLevel)
                {
                    if(!clearedGoalsList.Contains(2))
                        clearedGoalsList.Add(2);
                }
                if (hardLevel)
                {
                    if(!clearedGoalsList.Contains(3))
                        clearedGoalsList.Add(3);
                }
                break;
            case GameType.Calendar:
            case GameType.Daily:
                if (isDailyChallenge)
                {
                    if(!clearedGoalsList.Contains(0))
                        clearedGoalsList.Add(0);
                }              
                break;
            case GameType.Collections:
                // Do Nothing
                break;
            case GameType.Secret:
                if(!clearedGoalsList.Contains(6))
                    clearedGoalsList.Add(6);
                break;
            case GameType.Jigsaw:                
            case GameType.Adventure:
                if(!clearedGoalsList.Contains(1))
                    clearedGoalsList.Add(1);
                break;
        }
    }

    public void OnBackToMain()
    {
        if (isDailyChallengeShortcut)
        {
            footerToggle._togglesArray[1].isOn = true;
            isDailyChallengeShortcut = false;
        }

        switch(gameMode.currentType)
        {
            case GameType.Secret:
                gameMode.ChangeMode(GameType.Normal);
                break;
            case GameType.Daily:
                gameMode.ChangeMode(GameType.Calendar);
                break;
        }

        mainGoalsSetter.RefreshButtons();
    }

    public void RefreshCache()
    {
        RefreshClearCountCache();
        clearedGoalsList.Clear();
    }

    public void RefreshClearCountCache()
    {
        //Debug.Log(CodeManager.GetMethodCall());

        cachedClearCount.Clear();

        for (int i=0; i < eventInfo_Goals.goals.Count; i++)
        {
            cachedClearCount.Add(eventInfo_Goals.goals[i].clearCount);
        }

        cachedGoalsCleared = eventInfo_Goals.cleared;
        cachedCanPlaySecret = eventInfo_Goals.CanPlaySecret();
    }

    private bool IsExpired(string endTime)
    {
        return DateTime.Now.Subtract(DateTime.Parse(endTime)).Ticks >= 0;
    }

    private void Remove_CalendarEvent()
    {
        List<string> dailySummary = userData.LoadListString(EventDefine.key_eventInfo_Daily_Summary);

        for(int i=0; i < dailySummary.Count; i++)
        {
            int year = GetYear(dailySummary[i]);
            int month = GetMonth(dailySummary[i]);
            userData.DeleteData(string.Format(EventDefine.key_eventInfo_Daily_Level, year, month));
            userData.DeleteData(string.Format(EventDefine.key_eventInfo_Daily_Clear, year, month));
        }

        userData.DeleteData(EventDefine.key_eventInfo_Daily_Summary);

        eventItem_Calendar = null;
    }

    private void Remove_GeneralEvent()
    {
        userData.DeleteData(EventDefine.key_eventInfo_Goals);
        userData.DeleteData(EventDefine.key_eventInfo_Daily);
        userData.DeleteData(EventDefine.key_eventInfo_Secret);

        eventItem_Goals = null;
        eventItem_Daily = null;
        eventItem_Secret = null;
    }

    private void Remove_SpecialEvent()
    {
        userData.DeleteData(EventDefine.key_eventInfo_Jigsaw);
        for (int i=0; i < EventDefine.Jigsaw_ImageCount; i++)
        {
            userData.DeleteData(string.Format(EventDefine.key_eventInfo_Jigsaw_Level, i));
            userData.DeleteData(string.Format(EventDefine.key_eventInfo_Jigsaw_Clear, i));
        }

        userData.DeleteData(EventDefine.key_eventInfo_Adventure);
        userData.DeleteData(EventDefine.key_eventInfo_Adventure_Level);

        userData.SaveData(EventDefine.key_eventInfo_Type, 0);

        eventItem_Jigsaw = null;
        eventItem_Adventure = null;

        currentEvent = SpecialEventType.NONE;
        onSpecialEvent = false;
    }

#endregion General


#region Buttons

    public void OnClick_Play_Daily()
    {
        if (!userData.loadComplete) return;

        if (eventInfo_Daily.cleared)
        {
            footerToggle._togglesArray[1].isOn = true;
        }
        else
        {
            SelectCalendarToday();
            isDailyChallengeShortcut = true;            
            cachedCalenderPercent = GetSelectedCalendar().clearPercent;

            tutorialManager.CheckTutorialAndPlayGame(eventInfo_Daily.level);
        }
    }

    public void OnClick_Play_Calendar()
    {
        if (!userData.loadComplete) return;

        int level = GetSelectedCalandarLevel();

        if (level > 0)
        {
            if (isDailyChallenge)
                gameMode.ChangeMode(GameType.Daily);
            
            cachedCalenderPercent = GetSelectedCalendar().clearPercent;
            
            tutorialManager.CheckTutorialAndPlayGame(level);
        }
    }

    public void OnClick_ResetEventInfo()
    {
        Debug.Log(CodeManager.GetMethodName() + string.Format("<color=yellow>[Debug] Reset EventInfo</color>"));
        
        Remove_CalendarEvent();
        Remove_GeneralEvent();
        Remove_SpecialEvent();

        userData.SaveData(EventDefine.key_eventInfo_Count_General, 0);
        userData.SaveData(EventDefine.key_eventInfo_Count_Special, 0);

        UnityEngine.SceneManagement.SceneManager.LoadScene(GlobalDefine.SCENE_GAME);
    }

    public void OnClick_SetSpecialEvent(int index)
    {
        if (gameMode.currentScene != SceneType.Main) return;

        switch(index)
        {
            case 0: // Jigsaw
                Debug.Log(CodeManager.GetMethodName() + string.Format("<color=yellow>[Debug] New Event : Jigsaw Puzzle</color>"));
                debugEvent = SpecialEventType.JIGSAW_PUZZLE;
                break;
            case 1: // Adventure
                Debug.Log(CodeManager.GetMethodName() + string.Format("<color=yellow>[Debug] New Event : Adventure</color>"));
                debugEvent = SpecialEventType.ADVENTURE;
                break;
            case 2: // Auto
                Debug.Log(CodeManager.GetMethodName() + string.Format("<color=yellow>[Debug] New Event : Auto</color>"));
                debugEvent = SpecialEventType.DEBUG_RESET;
                break;
        }

        Initialize();
    }

    public void OnClick_EventClear()
    {
        if (gameMode.currentScene != SceneType.Event) return;

        switch(currentEvent)
        {
            case SpecialEventType.JIGSAW_PUZZLE:
                jigsawManager.OnClickDebug_ClearPiece();
                break;
            case SpecialEventType.ADVENTURE:
                adventureManager.OnClickDebug_ClearLevel();
                break;
        }
    }

#endregion Buttons

}