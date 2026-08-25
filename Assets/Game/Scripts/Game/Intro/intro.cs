using Unity.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{

    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float musicVolume = 0.5f;

    [SerializeField] private Sprite[] frames;
    [SerializeField] private float framesSwitchSpeed;

    [SerializeField] private Image image1;
    [SerializeField] private Image image2;

    private int currentFrame = 0;
    private bool isFrame1Active = true;

    void Awake()
    {
        G.audio.SetLoopAndPlay("magazin_game_mus_04", 0f,sourceIndex: 2);
        G.audio.FadeInLoop(fadeDuration, musicVolume,2);
        G.audio.SetLoopVolume(0, 0, 1,cancelOtherFades: false);

        G.typing.onEnding += End;

        GameInitializer.wasAtManager = false;

        CustomCoroutine.WaitThenExecute(framesSwitchSpeed, () => { PlaySequence(); });
    }

    private void End()
    {
        G.transition.TransitioN(() => { SceneManager.LoadScene(1); G.audio.FadeOutLoop(0.5f); });
    }

    void PlaySequence()
    {
        if (currentFrame >= frames.Length) return;

        currentFrame++;

        if (!isFrame1Active)
        {
            image1.sprite = frames[currentFrame];
            image1.DOFade(1, framesSwitchSpeed);
            image2.DOFade(0, framesSwitchSpeed);
        }
        else
        {
            image2.sprite = frames[currentFrame];
            image2.DOFade(1, framesSwitchSpeed);
            image1.DOFade(0, framesSwitchSpeed);
        }

        isFrame1Active = !isFrame1Active;
        CustomCoroutine.WaitThenExecute(framesSwitchSpeed, () => { PlaySequence(); });
    }

    private void OnDestroy()
    {
        G.audio.FadeOutLoop(0.5f, 2);
        G.typing.onEnding -= End;
    }
}
