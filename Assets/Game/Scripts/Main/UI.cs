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
    public TMP_Text poisonText;

    public Image defeatImage;
    public TMP_Text WTo;
    public TMP_Text defeat;

    public GameObject floatingMessagePrefab;

    public void SpawnFloatingMessage(string text,Color color,float timeIn = 0.5f,bool withShake = false)
    {
        var inst = GameObject.Instantiate(floatingMessagePrefab,floatingMessagePrefab.transform.position, Quaternion.identity);
        inst.transform.parent = transform;
        var textComp = inst.GetComponent<TMP_Text>();
        var floating = inst.GetComponent<FloatingMessage>().withShake = withShake;
        if(timeIn != 0) inst.GetComponent<FloatingMessage>().timeIn = timeIn;
        textComp.text = text;
        textComp.text = textComp.text.Insert(0, "<link=\"shake\">");
        textComp.text += "</link>";
        textComp.color = color;
    }

}
