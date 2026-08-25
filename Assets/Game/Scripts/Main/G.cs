using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public static class G 
{
    public static Main main;

    public static AudioController audio;
    public static VFXManager vfx;
    public static Transition transition;
    public static LocalizationManager localization;
    public static UI ui;
    public static TypingManager typing;
    public static Events events;
    public static GlobalVolumeController gVolCon;

    public static CardsArea area;
    public static Sack sack;
    public static Lever lever;
    public static CameraShaker cameraShaker;
    public static LevelManager levelManager;
    public static ActionCounter actions;
    public static Player player;
    public static Tooltip tooltip;
    public static SynergyManager synergy;

    public static GameDynamicDataManager d_data;
    public static GameStaticDataManager s_data;

    public static Feel feel;

    public static Enemy selectedEnemy;

    public static bool IsPaused => Time.timeScale == 0;
    private static float _currentTimeScale = 1f;

    public static float currentTimeScale
    {
        get
        {
            return _currentTimeScale; 
        }
        set
        {
            _currentTimeScale = value; 
            Time.timeScale = value;    
        }
    }

    public static void SetPaused(bool paused)
    {
        Time.timeScale = paused ? 0 : currentTimeScale;
    }

    public static Destroyer GC;

    public static Color darkGreen = new Color(0f, 0.3f, 0.15f, 1f);
    public static Color orange = new Color(1.0f, 0.647f, 0.0f);
}

public class GameStaticDataManager
{
    private List<CardData> cards = new List<CardData>();
    private List<EnemyData> enemies = new List<EnemyData>();
    private AllSynergiesData allSynergies;
    private List<SynergyData> synergies = new List<SynergyData>();

    public GameStaticDataManager()
    {
        Initialize();
    }

    public CardData GetCardData(Cards id)
    {
        return cards.Find(x => x.id == id);
    }

    public EnemyData GetEnemyData(Enemies id)
    {
        return enemies.Find(x => x.id == id);
    }

    public AllSynergiesData GetAllSynergiesData()
    {
        return allSynergies;
    }

    public SynergyData GetSynergyData(SynergyTypes id)
    {
        return synergies.Find(x => x.type == id);
    }

    private void Initialize()
    {
        cards = Resources.LoadAll<CardData>("Data/Static/Cards").ToList();
        enemies = Resources.LoadAll<EnemyData>("Data/Static/Enemies").ToList();
        allSynergies = Resources.Load<AllSynergiesData>("Data/Static/AllSynergies/AllSynergies");
        synergies = Resources.LoadAll<SynergyData>("Data/Static/Synergies").ToList();
    }
}

public class GameDynamicDataManager
{
    private Transform parent;

    private List<LevelDynamicData> levelDynamicData = new List<LevelDynamicData>();
    private List<PlayerDynamicData> playerDynamicData = new List<PlayerDynamicData>();
    private List<ManagerDataDynamic> managerDataDynamic = new List<ManagerDataDynamic>();

    public GameDynamicDataManager()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameObject parent = new GameObject("DynamicData");
        this.parent = parent.transform;
        GameObject.DontDestroyOnLoad(parent);

        List<GameObject> level = Resources.LoadAll<GameObject>("Data/Dynamic/Level").ToList();
        CreateDynamicDataObjects<LevelDynamicData>(level, ref levelDynamicData);

        List<GameObject> player = Resources.LoadAll<GameObject>("Data/Dynamic/Player").ToList();
        CreateDynamicDataObjects<PlayerDynamicData>(player, ref playerDynamicData);

        List<GameObject> manager = Resources.LoadAll<GameObject>("Data/Dynamic/Manager").ToList();
        CreateDynamicDataObjects<ManagerDataDynamic>(manager, ref managerDataDynamic);
    }

    private void CreateDynamicDataObjects<T>(List<GameObject> objects, ref List<T> dataComponents)
    {
        foreach (var obj in objects)
        {
            var inst = GameObject.Instantiate(obj);
            inst.transform.parent = parent;

            dataComponents.Add(inst.GetComponent<T>());
        }
    }

    //GetData(SomeType someId) - return someList.Find(x => x.someId == someId);}

    public LevelDynamicData GetLevelDynamicData()
    {
        return levelDynamicData.First();
    }

    public PlayerDynamicData GetPlayerDynamicData()
    {
        return playerDynamicData.First();
    }

    public ManagerDataDynamic GetManagerDynamicData()
    {
        return managerDataDynamic.First();
    }

    public void ResetAndRestartGame()
    {
        levelDynamicData.Clear();
        playerDynamicData.Clear();

        if (parent != null)
        {
            GameObject.Destroy(parent.gameObject);
        }

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

}
