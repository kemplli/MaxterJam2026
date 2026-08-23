using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private void Awake()
    {
        G.ui = this;
    }

    public GameObject dialogueWithManager;

    public TMP_Text actionsLeft;

    public Slider healthSlider;
    public TMP_Text healthText;

    public GameObject floatingMessagePrefab;

    public void SpawnFloatingMessage(string text,Color color)
    {
        var inst = GameObject.Instantiate(floatingMessagePrefab,floatingMessagePrefab.transform.position, Quaternion.identity);
        inst.transform.parent = transform;
        var textComp = inst.GetComponent<TMP_Text>();   
        textComp.text = text;
        textComp.color = color;
    }

}
