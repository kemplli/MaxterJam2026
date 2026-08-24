using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionAnimation : TypingText
{
    protected override void ManagedInitialize()
    {
        G.transition.FadeOut();

        G.audio.SetLoopAndPlay("manager", 0f);
        G.audio.FadeInLoop(1f, 1f, 0);

        G.typing.onEnding += End;

        StartCoroutine(MainSequence());
    }

    private void End()
    {
        G.transition.TransitioN(() => SceneManager.LoadScene(0));
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
