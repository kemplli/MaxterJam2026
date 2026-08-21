using UnityEngine;

public class BlinkGameobjectActive : TimedBehaviour
{
    [SerializeField]
    private GameObject objectToBlink = default;

    [SerializeField]
    private string soundId = "";

    [SerializeField]
    private float volume = 1f;

    protected override void OnTimerReached()
    {
        objectToBlink.SetActive(!objectToBlink.activeSelf);
        G.audio.PlaySound(soundId, volume,transform.position);
    }
}
