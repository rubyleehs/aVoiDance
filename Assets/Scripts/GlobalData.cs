using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;

public class GlobalData : MonoBehaviour
{
    public Text I_debugText;
    public static bool DoOpenSceneTransition;
    public string ImenuScene;
    public string IgameScene;
    public AudioClip testAudio;

    public static string menuScene;
    public static string gameScene;

    public static GlobalData instance;
    public static AudioClip selectedSong;
    public static int selectedSongIndex;

    public static int mode = 3;
    public static float scoreMultiplier = 1;
    public static string[] modeText;
    public static float[] modeStarSpawn;
    public static float[] modeScoreMultiplier;
    public static bool Assist = true;
    public static bool Paused = false;

    public static string[] availableSongNames;
    public static List<AudioClip> songsAvailable;
    public static Text debugText;

    void Awake()
    {
        debugText = I_debugText;
        DoDebugText("Setting up the Big Bang...");//
        if (instance == null)
        {           
            DontDestroyOnLoad(gameObject);
            instance = this;
            DoDebugText("Creating String Theory...");
            menuScene = ImenuScene;
            gameScene = IgameScene;
            selectedSong = testAudio;
            modeText = new string[5] { "Zen", "Easy", "Normal", "Hard", "Lunatic" };
            modeStarSpawn = new float[5] { -1, 21, 27, 32, 38 };
            modeScoreMultiplier = new float[10] { 0.5f, 0.8f, 1f, 1.2f, 1.6f, 0.9f, 1.2f, 1.5f, 1.7f, 1.9f };
            StartCoroutine(UpdateSongList());
            //if (songsAvailable == null || songsAvailable.Count == 0) StartCoroutine(UpdateSongList());
            //LoadGameScene();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        //if (songsAvailable == null || songsAvailable.Count == 0) StartCoroutine(UpdateSongList());
        //LoadGameScene();
    }

    public static void DoDebugText(string _text)
    {
        if (debugText == null) return;
        //Debug.Log(_text);
        debugText.text = _text;
    }

    public static IEnumerator UpdateSongList()
    {        
        if(SceneManager.GetActiveScene().name != "SampleScene") SceneManager.LoadScene("SampleScene");
        DoDebugText("Updating Entrophy Mod...");
        songsAvailable = new List<AudioClip>();
        //Debug.Log(Application.streamingAssetsPath);
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);

        FileInfo[] _oggFiles = directoryInfo.GetFiles("*.ogg");
        FileInfo[] _wavFiles = directoryInfo.GetFiles("*.wav");

        FileInfo[] _files = new FileInfo[_oggFiles.Length + _wavFiles.Length];
        _oggFiles.CopyTo(_files,0);
        _wavFiles.CopyTo(_files, _oggFiles.Length);
        DoDebugText("Inventing photons");
        for (int i = 0; i < _files.Length; i++)
        {
            WWW _audioFileUrl = new WWW("");
            DoDebugText("Remeasuring Planck length..." );
            /*
            //FileInfo _fileInfo;           
            try
            {
                //var uri = new Uri("file://");
                //uri.LocalPath = _files[i];
                //_audioFileUrl = new WWW(uri.AbsoluteUri);
                //_fileInfo = _files[i].Replace("" + Path.DirectorySeparatorChar, "/");
                //_audioFileUrl = new WWW(string.Format("file://{0}", _fileInfo));
                //_audioFileUrl = new WWW(new Uri(_files[i].FullName).AbsoluteUri);
                //_audioFileUrl = new WWW(string.Format("file://{0}", _files[i])); // GETS STUCK HERE IN BUILD BUT NOT EDITOR
                //DoDebugText(_audioFileUrl.url);

            }
            catch(Exception e)
            {
                DoDebugText(_audioFileUrl.url + " || " + e.Message);
                Debug.LogException(e);
            }
            */
            _audioFileUrl = new WWW(new Uri(_files[i].FullName).AbsoluteUri);
            //yield return new WaitForSecondsRealtime(5);
            yield return _audioFileUrl;
            DoDebugText("Coding multiverse #" + i + " into existance...");
            songsAvailable.Add(_audioFileUrl.GetAudioClip());
        }
        DoDebugText("Creating first elementary particles...");
        if (songsAvailable.Count == 0)
        {
            AudioClip[] _defaultSongs = Resources.LoadAll<AudioClip>("Audio");

            for (int i = 0; i < _defaultSongs.Length; i++)
            {
                songsAvailable.Add(_defaultSongs[i]);
            }
        }
        availableSongNames = new string[songsAvailable.Count];
        for (int i = 0; i < songsAvailable.Count; i++)
        {
            
            availableSongNames[i] = Path.GetFileNameWithoutExtension(_files[i].Name);
        }
        //yield return new WaitForSeconds(2);
        DoDebugText("Polishing Physics...");
        LoadMenuScene();//
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) LoadGameScene();
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
