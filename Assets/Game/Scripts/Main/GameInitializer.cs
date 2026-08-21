using UnityEngine;

public static class GameInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitGame()
    {
        var systemObject = new GameObject("[Main]");
        Object.DontDestroyOnLoad(systemObject);

        var mainComponent = systemObject.AddComponent<Main>();
        G.main = mainComponent;

        G.GC = new Destroyer(mainComponent);
        G.d_data = new GameDynamicDataManager();
        G.s_data = new GameStaticDataManager();
        G.feel = new Feel();

        G.localization = new LocalizationManager();
    }
}
