using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionAnimation : TypingText
{
    [SerializeField]
    private List<string> texts = new List<string>();

    private ManagerDataDynamic data;

    protected override void ManagedInitialize()
    {
        G.transition.FadeOut();

        G.audio.SetLoopAndPlay("manager", 0f,sourceIndex: 0);
        G.audio.FadeInLoop(1f, 1f, 0);
        G.audio.FadeOutLoop(0.1f, 1);

        data = G.d_data.GetManagerDynamicData();

        textContainers[data.index].Text = texts[data.index];

        GameInitializer.wasAtManager = true;

        G.typing.onEnding += End;

        StartCoroutine(MainSequence());
    }

    private void End()
    {
        G.transition.TransitioN(() => SceneManager.LoadScene(1));
        G.audio.FadeOutLoop(1f, 0);
    }

    public override void ManagedUpdate()
    {

    }

    private IEnumerator MainSequence()
    {
        yield return new WaitForSeconds(1f);
        StartTyping();
    }
}
