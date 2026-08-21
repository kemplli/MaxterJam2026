using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private void Awake()
    {
        G.ui = this;
    }

    public Slider elixir;

    public List<GameObject> sprites;
}