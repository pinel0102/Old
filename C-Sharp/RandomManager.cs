using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

public class RandomManager : SingletonMono<RandomManager>
{
    [Header("★ [Parameter] Pixel Levels Total")]
    public List<int> pixelLevels = new List<int>();
    
    [Header("★ [Parameter] Playable Levels")]
    public int playableLevelCount_Total;
    public int playableLevelCount_Pixel;
    public int playableLevelCount_Normal;

    [Header("★ [Parameter] Jigsaw Theme")]
    public int currentJigsawThemeIndex;
    public string currentJigsawTheme = ServerDefine.JigsawThemes[0];
    public string currentJigsawTheme_Localized = "Hobby";

    private bool isInitialized = false;
    private WaitForSecondsRealtime waitDelay = new WaitForSecondsRealtime(0.5f);
    
    private GameManager gameManager { get { return GameManager.Instance; } }
    private ServerClient serverClient { get { return ServerClient.Instance; } }
    private EventManager eventManager { get { return EventManager.Instance; } }
    private UserData userData { get { return UserData.Instance; } }
    private GameMode gameMode { get { return GameMode.Instance; } }
    private int levelCount { get { return gameManager.levelCount; } }
    private List<string> playableThemes { get { return GetPlayableThemes(); } }
    private HashSet<int> excludeLevel { get { return new HashSet<int>(userData.usedinfo_LevelSummary); } }
    private HashSet<int> excludeJigsaw { get { return new HashSet<int>(userData.usedinfo_JigsawSummary); } }
    private HashSet<int> excludeTheme { get { return new HashSet<int>(userData.usedinfo_ThemeSummary); } }


#region Initialize

    public void Initialize()
    {
        Debug.Log(CodeManager.GetMethodName());
        
        pixelLevels = GetPixelLevels();
        RefreshPlayableLevels();

        isInitialized = true;
    }

    public void RefreshSeed()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    public void RefreshPlayableLevels()
    {
        playableLevelCount_Total = levelCount - excludeLevel.Count;
        playableLevelCount_Pixel = Enumerable.Range(1, levelCount).Where(i => !excludeLevel.Contains(i) && pixelLevels.Contains(i)).Count();
        playableLevelCount_Normal = playableLevelCount_Total - playableLevelCount_Pixel;
    }

    public bool IsPixelLevel(int level)
    {
        if (!isInitialized)
            return false;

        return pixelLevels.Contains(level);
    }

    public void RefreshLocalizedText()
    {
        currentJigsawTheme_Localized = gameMode.localizedText_JigsawThemes[currentJigsawThemeIndex];
    }

    private List<int> GetPixelLevels()
    {
        List<LevelSummary> pixelSummary = gameManager.levelSummaries.FindAll(ls => ls.isPixelLevel == true);
        
        List<int> result = new List<int>();
        for (int i=0; i < pixelSummary.Count; i++)
        {
            result.Add(pixelSummary[i].level);
        }

        return result;
    }

#endregion Initialize


#region Random Level

    ///<Summary><para>UsedInfo가 존재하지 않는 레벨 중에서 랜덤하게 뽑아낸다.</para>
    ///<para>레벨 수가 부족할 경우 UsedInfo가 존재하는 일반 레벨 중에서 추가로 뽑아낸다.</para></Summary>
    public int GetRandomLevel()
    {
        return GetRandomLevelList(1)[0];
    }

    ///<Summary><para>UsedInfo가 존재하지 않는 레벨 중에서 랜덤하게 뽑아낸다.</para>
    ///<para>레벨 수가 부족할 경우 UsedInfo가 존재하는 일반 레벨 중에서 추가로 뽑아낸다.</para></Summary>
    public List<int> GetRandomLevelList(int count)
    {
        RefreshSeed();

        List<int> result = Enumerable.Range(1, levelCount)
                        .Where(i => !excludeLevel.Contains(i))
                        .OrderBy(g => System.Guid.NewGuid())
                        .Take(count).ToList();
        
        if (result.Count < count)
        {
            result.AddRange(GetRandomLevelList_Used_NormalOnly(result, count - result.Count));
        }
        
        return result.OrderBy(g => System.Guid.NewGuid()).ToList();
    }

    ///<Summary><para>UsedInfo가 존재하지 않는 레벨 중에서 랜덤하게 뽑아낸다.</para>
    ///<para>레벨 수가 부족할 경우 UsedInfo가 존재하는 일반 레벨 중에서 추가로 뽑아낸다.</para></Summary>
    public List<int> GetRandomLevelList(int count, int pixelCount)
    {
        RefreshSeed();

        List<int> result = GetRandomLevelList_NormalOnly(count - pixelCount);
        result.AddRange(GetRandomLevelList_PixelOnly(pixelCount));

        if (result.Count < count)
        {
            result.AddRange(GetRandomLevelList_Used_NormalOnly(result, count - result.Count));
        }

        return result.OrderBy(g => System.Guid.NewGuid()).ToList();
    }

    private List<int> GetRandomLevelList_NormalOnly(int count)
    {
        var result = Enumerable.Range(1, levelCount)
                    .Where(i => !excludeLevel.Contains(i) && !pixelLevels.Contains(i))
                    .OrderBy(g => System.Guid.NewGuid())
                    .Take(count);
        
        return result.ToList();
    }

    private List<int> GetRandomLevelList_PixelOnly(int count)
    {
        var result = Enumerable.Range(1, levelCount)
                    .Where(i => !excludeLevel.Contains(i) && pixelLevels.Contains(i))
                    .OrderBy(g => System.Guid.NewGuid())
                    .Take(count);
        
        return result.ToList();
    }

    ///<Summary>UsedInfo가 존재하는 일반 레벨 중에서 랜덤하게 뽑아낸다.</Summary>
    private List<int> GetRandomLevelList_Used_NormalOnly(List<int> excludeList, int addCount)
    {
        var result = excludeLevel.Where(i => !excludeList.Contains(i) && !pixelLevels.Contains(i))
                    .OrderBy(g => System.Guid.NewGuid())
                    .Take(addCount);

        return result.ToList();
    }

#endregion Random Level


#region Random Jigsaw

    ///<Summary>직소 이벤트 생성 가능 여부.</Summary>
    public bool CanCreateJigsawSet()
    {
        RefreshPlayableLevels();

        // usedInfo가 없는 레벨이 105개 이상 존재.
        bool canSetLevel = playableLevelCount_Total >= EventDefine.Jigsaw_LevelCount_Total;
        bool canSetPixelLevel = playableLevelCount_Pixel >= EventDefine.Jigsaw_LevelCount_Pixel;
        bool canSetNormalLevel = playableLevelCount_Normal >= EventDefine.Jigsaw_LevelCount_Total - EventDefine.Jigsaw_LevelCount_Pixel;

        // usedInfo가 없는 직소 이미지가 3개 이상 있는 테마가 존재.
        bool canSetJigsaw = playableThemes.Count > 0;

        return canSetLevel && canSetNormalLevel && canSetPixelLevel && canSetJigsaw;
    }

    ///<Summary>랜덤한 Theme 1개와 UsedInfo가 존재하지 않는 이미지 3개를 랜덤하게 뽑아낸다.</Summary>
    public List<int> GetRandomJigsawList()
    {
        RefreshSeed();

        currentJigsawTheme = GetRandomJigsawTheme();
        currentJigsawThemeIndex = GetThemeIndex(currentJigsawTheme);
        currentJigsawTheme_Localized = gameMode.localizedText_JigsawThemes[currentJigsawThemeIndex];

        List<int> imageList = GetImageList(currentJigsawTheme);

        /*for (int i=0; i < imageList.Count; i++)
        {   Debug.Log(CodeManager.GetMethodName() + string.Format("Selected Image : {0}/{1}", currentTheme, imageList[i])); }*/
        
        return imageList;
    }

    ///<Summary>Theme 안의 이미지를 랜덤하게 뽑아낸다.</Summary>
    private List<int> GetImageList(string theme)
    {
        var themeImageList = serverClient.metaInfo.FindAll(item => item[serverClient.metaCategory[1]].ToString().Equals(theme) && 
                                                        !excludeJigsaw.Contains(int.Parse(item[serverClient.metaCategory[0]].ToString())));
        
        List<int> themeIndexList = new List<int>();
        for (int i=0; i < themeImageList.Count; i++)
        {
            themeIndexList.Add(int.Parse(themeImageList[i][serverClient.metaCategory[0]].ToString()));
        }

        /*for (int i=0; i < themeIndexList.Count; i++)
        {   Debug.Log(CodeManager.GetMethodName() + string.Format("Playable Image : {0}/{1}", theme, themeIndexList[i])); }*/

        return themeIndexList.OrderBy(g => System.Guid.NewGuid())
                            .Take(EventDefine.Jigsaw_ImageCount).ToList();
    }

    ///<Summary>Playable 테마 리스트에서 하나의 테마를 랜덤하게 뽑아낸다.</Summary>
    private string GetRandomJigsawTheme()
    {
        /*for (int i=0; i < playableThemes.Count; i++)
        {   Debug.Log(CodeManager.GetMethodName() + string.Format("Playable Theme : {0}", playableThemes[i])); }*/

        return playableThemes.OrderBy(g => System.Guid.NewGuid())
                            .Take(1).ToList()[0];
    }

    ///<Summary>Playable 테마 리스트를 뽑아낸다.</Summary>
    private List<string> GetPlayableThemes()
    {
        List<string> playble = new List<string>();

        for (int i = 0; i < ServerDefine.JigsawThemes.Length; i++)
        {
            if (!excludeTheme.Contains(i))
            {
                if (serverClient.metaInfo.FindAll(item => item[serverClient.metaCategory[1]].ToString().Equals(ServerDefine.JigsawThemes[i]) && 
                                                !excludeJigsaw.Contains(int.Parse(item[serverClient.metaCategory[0]].ToString()))).Count >= EventDefine.Jigsaw_ImageCount)
                {
                    playble.Add(ServerDefine.JigsawThemes[i]);
                }
            }
        }

        return playble;
    }
    
    public int GetThemeIndex(string theme)
    {
        for (int i=0; i < ServerDefine.JigsawThemes.Length; i++)
        {
            if (ServerDefine.JigsawThemes[i].Equals(theme.ToLower()))
                return i;
        }
        return 0;
    }

#endregion Random Jigsaw


#region Random Adventure

    ///<Summary>어드벤처 이벤트 생성 가능 여부.</Summary>
    public bool CanCreateAdventureSet()
    {
        RefreshPlayableLevels();

        // usedInfo가 없는 레벨이 50개 이상 존재. (픽셀은 부족해도 가능)
        bool canSetLevel = playableLevelCount_Total >= EventDefine.Adventure_LevelCount_Total;
        bool canSetPixelLevel = playableLevelCount_Pixel >= EventDefine.Adventure_LevelCount_Pixel;
        bool canSetNormalLevel = playableLevelCount_Normal >= EventDefine.Adventure_LevelCount_Total - Mathf.Min(playableLevelCount_Pixel, EventDefine.Adventure_LevelCount_Pixel);

        return canSetLevel && canSetNormalLevel;
    }

    ///<Summary>UsedInfo가 존재하지 않는 레벨 중에서 랜덤하게 뽑아낸다.</Summary>
    public List<int> GetRandomAdventureList()
    {
        return GetRandomLevelList(EventDefine.Adventure_LevelCount_Total, Mathf.Min(playableLevelCount_Pixel, EventDefine.Adventure_LevelCount_Pixel));
    }

#endregion Random Adventure


#region ETC

    public List<int> GetRandomIndexList(int count, int pick)
    {
        RefreshSeed();

        var result = Enumerable.Range(0, count)
                    .OrderBy(g => System.Guid.NewGuid())
                    .Take(pick);
        
        return result.ToList();
    }

    public List<int> GetRandomGoalsList(int count, int pick, int checkCount = 30)
    {
        RefreshSeed();

        int startLevel = ObscuredPrefs.GetInt("CLEAR_LEVEL", 0) + 1;
        int endLevel = Mathf.Min(startLevel - 1 + checkCount, levelCount);

        List<int> excludeList = new List<int>();
        
        if (eventManager.onSpecialEvent)
        {
            switch (eventManager.currentEvent)
            {
                case SpecialEventType.JIGSAW_PUZZLE:
                    if (eventManager.eventInfo_Jigsaw.jigsawPlate[0].clearCount + eventManager.eventInfo_Jigsaw.jigsawPlate[1].clearCount + eventManager.eventInfo_Jigsaw.jigsawPlate[2].clearCount > EventDefine.Jigsaw_LevelCount_Total - EventDefine.Goals_GoalCount[1])
                    {
                        excludeList.Add(1);
                    }
                    break;
                case SpecialEventType.ADVENTURE:
                    if (eventManager.eventInfo_Adventure.clearedLevel > EventDefine.Adventure_LevelCount_Total - EventDefine.Goals_GoalCount[1])
                    {
                        excludeList.Add(1);
                    }
                    break;
            }
        }
        else
        {
            excludeList.Add(1);
        }

        List<LevelSummary> checkSummary = gameManager.levelSummaries.FindAll(ls => ls.level >= startLevel && ls.level <= endLevel);
        if (checkSummary.Count > 0)
        {
            List<LevelSummary> rewardSummary = checkSummary.FindAll(ls => ls.rewardCoin != 0 || !ls.rewardItem.Equals(GlobalDefine.itemDefaultFormat));
            
            //for(int i=0; i < rewardSummary.Count; i++)
            //{ Debug.Log(CodeManager.GetMethodName() + string.Format("[Reward Level] {0}", rewardSummary[i].level)); }
            
            if (rewardSummary.Count < EventDefine.Goals_GoalCount[2])
            {
                excludeList.Add(2);
            }

            List<LevelSummary> hardSummary = checkSummary.FindAll(ls => ls.difficulty != 0);

            //for(int i=0; i < hardSummary.Count; i++)
            //{ Debug.Log(CodeManager.GetMethodName() + string.Format("[Hard Level] {0} ({1})", hardSummary[i].level, hardSummary[i].difficulty)); }

            if (hardSummary.Count < EventDefine.Goals_GoalCount[3])
            {
                excludeList.Add(3);
            }
        }
        else
        {
            excludeList.Add(2);
            excludeList.Add(3);
        }

        var result = Enumerable.Range(0, count)
                    .Where(i => !excludeList.Contains(i))
                    .OrderBy(g => System.Guid.NewGuid())
                    .Take(pick);
        
        return result.ToList();
    }

#endregion ETC
}
