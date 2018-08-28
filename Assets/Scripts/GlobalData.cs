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


    public static int mode = 3;
    public static float scoreMultiplier = 1;
    public static string[] modeText;
    public static float[] modeStarSpawn;
    public static float[] modeScoreMultiplier;
    public static bool Assist = true;
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
        modeText = new string[5] { "Zen", "Easy", "Normal", "Hard", "Lunatic" };
        modeStarSpawn = new float[5] { -1, 21, 27, 32, 38};
        modeScoreMultiplier = new float[10] { 0.5f, 0.8f, 1f, 1.2f, 1.6f, 0.9f, 1.2f, 1.5f,1.7f, 1.9f};
        //LoadGameScene();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) LoadGameScene();
    }

    public static void LoadGameScene()
    {
        if (mode > modeText.Length - 1) Assist = false; 
        else Assist = true;

        scoreMultiplier = modeScoreMultiplier[mode];
        SceneManager.LoadScene(gameScene);
        AudioTest3.AutoPlay = false;
    }

    public static void LoadMenuScene()
    {
        SceneManager.LoadScene(menuScene);
    }

    public static int LoadSongHighScore(string songName)
    {
        if (PlayerPrefs.HasKey(songName))
        {
            return PlayerPrefs.GetInt(songName);
        }
        else return 0;
    }

    public static void SaveSongHighScore(string songName, int highScore)
    {
        int _highScore = Mathf.Max(highScore, LoadSongHighScore(songName));
        PlayerPrefs.SetInt(songName, _highScore);

    }
}
