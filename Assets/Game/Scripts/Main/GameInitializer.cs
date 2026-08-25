using UnityEngine;

public static class GameInitializer
{
    public static bool failed;

    public static bool wasAtManager;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitGame()
    {
        G.d_data = new GameDynamicDataManager();
        G.s_data = new GameStaticDataManager();
        G.feel = new Feel();
        G.events = new Events();

        G.localization = new LocalizationManager();

        var systemObject = new GameObject("[Main]");
        Object.DontDestroyOnLoad(systemObject);
        var mainComponent = systemObject.AddComponent<Main>();
        G.main = mainComponent;

        G.GC = new Destroyer(mainComponent);
    }
}
