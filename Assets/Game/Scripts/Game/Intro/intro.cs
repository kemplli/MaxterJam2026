using Unity.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

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
        G.audio.SetLoopAndPlay("magazin_game_mus_04", 0f);
        G.audio.FadeInLoop(fadeDuration, musicVolume);

        CustomCoroutine.WaitThenExecute(framesSwitchSpeed, () => { PlaySequence(); });
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
}
