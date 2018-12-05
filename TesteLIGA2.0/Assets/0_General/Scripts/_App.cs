using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class _App : MonoBehaviour
{
    public static _App Instance { get; private set; }
    public static _SceneSwapManager SceneSwapManager { get; private set; }
    public static _SoundManager SoundManager { get; private set; }

//#if UNITY_EDITOR
//    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//    public static void PreloadScene()
//    {
//        SceneManager.LoadScene(0);
//    }
//#endif
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Object.Destroy(gameObject);
        
        Object.DontDestroyOnLoad(gameObject);
        
        SceneSwapManager = GetComponent<_SceneSwapManager>();
        SoundManager = GetComponent<_SoundManager>();
    }

    public IEnumerator Sleep(int seconds)
    {
        Time.timeScale = 0.0001f;
        float pauseEndTime = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        Time.timeScale = 1;
    }
}