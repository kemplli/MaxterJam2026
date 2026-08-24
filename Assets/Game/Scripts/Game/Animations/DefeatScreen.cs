using System;
using UnityEngine;

public class DefeatScreen : ManagedBehaviour
{
    public override void ManagedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameInitializer.failed = true;
            G.transition.TransitioN(() =>
            {
                G.d_data.ResetAndRestartGame();
                Destroy(G.main.gameObject);
                GameInitializer.InitGame();
            });
        }
    }
}
