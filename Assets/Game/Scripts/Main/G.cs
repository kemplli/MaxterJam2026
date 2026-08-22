using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Editor;
using System;

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

    public static CardsArea area;
    public static Sack sack;
    public static Lever lever;
    public static CameraShaker cameraShaker;
    public static LevelManager levelManager;

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
}

public class GameStaticDataManager
{
    private List<CardData> cards = new List<CardData>();
    private List<EnemyData> enemies = new List<EnemyData>();

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

    private void Initialize()
    {
        cards = Resources.LoadAll<CardData>("Data/Static/Cards").ToList();
        enemies = Resources.LoadAll<EnemyData>("Data/Static/Enemies").ToList();
    }
}

public class GameDynamicDataManager
{
    private Transform parent;

    public GameDynamicDataManager()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameObject parent = new GameObject("DynamicData");
        this.parent = parent.transform;

        //List<GameObject> data = Resources.LoadAll<GameObject>("Data/Dynamic/Resources").ToList();
        //CreateDynamicDataObjects<ResourceData>(data,ref resourceData);
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
}
