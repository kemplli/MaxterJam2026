using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionAnimation : ManagedBehaviour
{
    protected override void ManagedInitialize()
    {
        G.transition.FadeOut();

        G.audio.SetLoopAndPlay("manager", 0f);
        G.audio.FadeInLoop(1f, 1f, 0);
    }

    public override void ManagedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            G.transition.TransitioN(() => SceneManager.LoadScene(0));
            G.audio.FadeOutLoop(1f, 0);
        }
    }
}
