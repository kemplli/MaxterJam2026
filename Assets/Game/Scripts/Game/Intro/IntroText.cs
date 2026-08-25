using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroText : TypingText
{
    void Start()
    {
        StartCoroutine(SceneCheck());
    }

    void Update()
    {
        
    }

    private IEnumerator SceneCheck()
    {
        var index = SceneManager.GetActiveScene().buildIndex;

        if (SceneManager.GetActiveScene().buildIndex == 4 || SceneManager.GetActiveScene().buildIndex == 3)
        {
            G.transition.FadeOut();

            if(SceneManager.GetActiveScene().buildIndex != 4)
            G.typing.onEnding += ToTitles;

            G.audio.FadeOutLoop(0.5f, 1);
        }

        yield return new WaitForSeconds(1f);
        StartTyping();
    }

    private void ToTitles()
    {
        G.transition.TransitioN(() => SceneManager.LoadScene(4));
    }
}
