using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaymodeTool : MonoBehaviour
{
#if UNITY_EDITOR

    private void Start()
    {
        Time.timeScale = 1f;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            Debug.Break();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(!G.IsPaused)
            G.currentTimeScale = Time.timeScale == 0.25f ? 1F : 0.25F;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (!G.IsPaused)
                G.currentTimeScale = Time.timeScale == 2f ? 1F : 2F;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            G.SetPaused(!G.IsPaused);
        }
    }
#endif
}
