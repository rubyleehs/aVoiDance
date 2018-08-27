using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalData : MonoBehaviour
{
    public string ImenuScene;
    public string IgameScene;
    public AudioClip testAudio;

    public static string menuScene;
    public static string gameScene;

    public static GlobalData instance;
    public static AudioClip selectedSong;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        menuScene = ImenuScene;
        gameScene = IgameScene;
        selectedSong = testAudio;
        //LoadGameScene();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) LoadGameScene();
    }

    public static void LoadGameScene()
    {
        SceneManager.LoadScene(gameScene);
        AudioTest3.AutoPlay = false;
    }

    public static void LoadMenuScene()
    {
        SceneManager.LoadScene(menuScene);
    }
}
