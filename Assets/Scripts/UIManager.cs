using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public AudioTest3 audioManager;

    public Text comboText;
    public float comboTextFadeDuration;
    public float comboTextEndRatioSize;

    private Color comboTextRevealColor;
    private Color comboTextEndColor;
    private float comboTextOriSize;

    public static int score = 0;
    public static int maxCombo = 0;
    public static int beatCaptured = 0;
    public static int beatMissed = 0;
    public static int hitsTaken = 0;

    public Text scoreText;
    public float scoreTextExpandRatio;
    public float scoreTextShrinkDuration;
    private float scoreTextOriSize;

    public Text songNameText;

    public Transform screenTransition;
    public float transitionDuration;
    public float screenTransitionToSize = 50;

    public PPListAnim songStatsName;
    public PPListAnim songStatsNum;
    private string[] statTextText;

    private void Awake()
    {
        score = 0;
        comboTextRevealColor = comboText.color;
        comboTextEndColor = comboText.color;
        comboTextEndColor.a = 0;
        comboTextOriSize = comboText.fontSize;
        comboText.enabled = false;
        scoreTextOriSize = scoreText.fontSize;
        scoreText.text = "Highscore: " + GlobalData.LoadSongHighScore(AudioTest3.songName);
        statTextText = new string[6] { "Score", "Highscore", "Max Combo", "Beat Captured", "Beat Missed", "Hits Taken" };
        StartCoroutine(OpenSceneTransition());
    }
    void Start () {
        songNameText.text = AudioTest3.songName;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator ShowCombo()
    {
        float _startTime = Time.time;
        float _progress = 0;
        comboText.text = "" + AudioTest3.combo + "";
        comboText.color = comboTextRevealColor;
        comboText.enabled = true;
        while(_progress < 1)
        {
            comboText.color = Color.Lerp(comboTextRevealColor, comboTextEndColor, _progress);
            comboText.fontSize = (int)Mathf.Lerp(comboTextOriSize, comboTextOriSize * comboTextEndRatioSize, _progress);
            _progress = (Time.time - _startTime) / comboTextFadeDuration;
            yield return new WaitForEndOfFrame();
        }
        comboText.enabled = false;
    }

    public void CheckCombo()
    {
        StartCoroutine(AddScore());
        if (AudioTest3.combo %50 ==0 || AudioTest3.combo == 10)//
        {
            StartCoroutine(ShowCombo());
        }
    }

    public IEnumerator AddScore()
    {
        float _startTime = Time.time;
        float _progress = 0;
        score += (int)(5 *GlobalData.scoreMultiplier * AudioTest3.combo);
        scoreText.text = "" + score;
        scoreText.fontSize = (int)(scoreTextOriSize * scoreTextExpandRatio);
        while (_progress < 1)
        {
            scoreText.fontSize = (int)Mathf.SmoothStep(scoreTextOriSize * scoreTextExpandRatio, scoreTextOriSize, _progress);
            _progress = (Time.time - _startTime) / scoreTextShrinkDuration;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator LoadSceneWithAnim(int sceneID)
    {
        screenTransition.localScale = Vector3.one * 0.1f;
        screenTransition.gameObject.SetActive(true);
        float _progress = 0;
        float _timeTransitionStart = Time.time;
        while(_progress < 1)
        {
            _progress = (Time.time - _timeTransitionStart) / transitionDuration;
            screenTransition.localScale = Vector3.one * Mathf.SmoothStep(0.1f, screenTransitionToSize,_progress * _progress *_progress);

            audioManager.SetAudioVolume(1 - _progress);
            yield return new WaitForEndOfFrame();
        }
        GlobalData.DoOpenSceneTransition = true;
        if (sceneID == 1) GlobalData.LoadGameScene();
        else if (sceneID == 2) { }//load end scene
        else if (sceneID == 3) GlobalData.LoadMenuScene();
    }

    public IEnumerator OpenSceneTransition()
    {
        if (!GlobalData.DoOpenSceneTransition) yield break;

        GlobalData.DoOpenSceneTransition = false;
        screenTransition.gameObject.SetActive(true);
        screenTransition.transform.localScale = Vector3.one * screenTransitionToSize;
        float _progress = 0;
        float _timeTransitionStart = Time.time;
        while (_progress < 1)
        {
            _progress = (Time.time - _timeTransitionStart) / transitionDuration;
            screenTransition.localScale = Vector3.one * Mathf.SmoothStep(screenTransitionToSize, 1, Mathf.SmoothStep(0,1,_progress));

            yield return new WaitForEndOfFrame();
        }
    }

    public void EndSongStuff()
    {
        Debug.Log("Song End Stuff");
        UpdateStats();
        StartCoroutine(songStatsName.StartListEntranceAnim());
        StartCoroutine(songStatsNum.StartListEntranceAnim());
    }

    public void UpdateStats()
    {
        string[] _stats = new string[6] { score.ToString(), GlobalData.LoadSongHighScore(AudioTest3.songName).ToString(), maxCombo.ToString(), beatCaptured.ToString(), beatMissed.ToString(), hitsTaken.ToString()};
        songStatsNum.UpdateListText(_stats);
    }
}
