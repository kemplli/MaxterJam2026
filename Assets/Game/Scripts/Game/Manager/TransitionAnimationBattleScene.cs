using UnityEngine;

public class TransitionAnimationBattleScene : ManagedBehaviour
{
    private LevelDynamicData levelDynamicData;
    protected override void ManagedInitialize()
    {
        levelDynamicData = G.d_data.GetLevelDynamicData();
        G.transition.FadeOut();
        G.levelManager.LoadLevel(levelDynamicData.currentLevel);

        G.audio.SetLoopAndPlay("BattleMusic", 0f,sourceIndex: 1);
        G.audio.FadeInLoop(0.5f, 0.6f,1);
    }


    private void OnDestroy()
    {
        G.audio.FadeOutLoop(0.5f, 1);
    }
}