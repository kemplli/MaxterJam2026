using UnityEngine;

public class MainTyping : TypingText
{
    public override void ManagedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartTyping();
        }
    }
}