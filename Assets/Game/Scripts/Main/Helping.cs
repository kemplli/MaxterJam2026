using UnityEngine;
using UnityEngine.SceneManagement;

public class Helping : ManagedBehaviour
{
    [SerializeField]
    private GameObject helpLever;
    protected override void ManagedInitialize()
    {
        SceneManager.activeSceneChanged += OnLoadScene;
    }

    private void OnLoadScene(Scene arg0, Scene arg1)
    {
        helpLever.gameObject.SetActive(true);
    }
}