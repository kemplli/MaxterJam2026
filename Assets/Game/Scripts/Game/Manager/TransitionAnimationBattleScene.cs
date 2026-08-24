using UnityEngine;

public class TransitionAnimationBattleScene : ManagedBehaviour
{
    private LevelDynamicData levelDynamicData;
    protected override void ManagedInitialize()
    {
        levelDynamicData = G.d_data.GetLevelDynamicData();
        if (levelDynamicData.currentLevel != 0 || GameInitializer.failed)
        {
            G.transition.FadeOut();
            G.levelManager.LoadLevel(levelDynamicData.currentLevel);
        }
    }

}