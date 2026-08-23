using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueWithManager : TypingText
{
    protected override void ManagedInitialize()
    {
        base.ManagedInitialize();
        G.ui.dialogueWithManager.transform.localScale = Vector3.zero;
        G.events.onWin += Appear;
        G.typing.onEnding += MoveToManager;
    }

    private void MoveToManager()
    {
        G.transition.TransitioN(() => { G.SetPaused(false); SceneManager.LoadScene(1); });
    }

    private void Appear()
    {
        G.SetPaused(true);

        G.ui.dialogueWithManager.transform.DOScale(1, 0.5f).SetUpdate(true).SetEase(Ease.OutBack).OnComplete(() => StartTyping());
    }

    private void OnDestroy()
    {
        G.events.onWin -= Appear;
        G.typing.onEnding -= MoveToManager;
    }
}