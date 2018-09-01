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

    public List<SpriteRenderer> keySprites;
    public int keyFlashNum;
    public float keyFadeScale;
    public float keyFadeDuration;
    public float keyFlashInterval;
    public float keyFlashDuration;
    public float keyFlashAlphaRatio;
    public float keyFlashScale;

    public Transform pausedScreen;
    public Text timer;
    private bool unpausing = false;

    private void Awake()
    {
        Time.timeScale = 1;
        if (pausedScreen != null) pausedScreen.gameObject.SetActive(false);
        if (timer != null) timer.gameObject.SetActive(false);
        score = 0;
        comboTextRevealColor = comboText.color;
        comboTextEndColor = comboText.color;
        comboTextEndColor.a = 0;
        comboTextOriSize = comboText.fontSize;
        comboText.enabled = false;
        scoreTextOriSize = scoreText.fontSize;
        float _highScore = GlobalData.LoadSongHighScore(AudioTest3.songName);
        scoreText.text = "Highscore: " + _highScore;
        
        statTextText = new string[6] { "Score", "Highscore", "Max Combo", "Beat Captured", "Beat Missed", "Hits Taken" };
        StartCoroutine(OpenSceneTransition());
    }
    void Start () {
        songNameText.text = AudioTest3.songName;
	}
	
	// Update is called once per frame
	void Update () {

        if (unpausing) return;

        if (Input.anyKeyDown && AudioTest3.SongEnded)
        {
            StartCoroutine(LoadSceneWithAnim(0));
        }
        else if (!AudioTest3.AutoPlay && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)))
        {
            if (!GlobalData.Paused)
            {
                Time.timeScale = 0;
                GlobalData.Paused = true;
                pausedScreen.gameObject.SetActive(true);
                keySprites[0].gameObject.SetActive(true);
                timer.gameObject.SetActive(true);
                timer.text = "Paused";
                audioManager.Pause();
            }
            else
            {
                StartCoroutine(Unpause());
            }
        }
        if (!AudioTest3.AutoPlay && GlobalData.Paused && Input.GetKeyDown(KeyCode.Q))
        {
            GlobalData.Paused = false;
            StartCoroutine(LoadSceneWithAnim(0));
        }
	}

    public IEnumerator Unpause()
    {
        unpausing = true;
        float unpausedTime = Time.time;
        keySprites[0].gameObject.SetActive(false);
        pausedScreen.gameObject.SetActive(false);
        //timer.fontSize = (int)timer.fontSize/2;
        float _durationLeft = 3.0000001f;
        string _string = "";
        while(_durationLeft > 0.1f)
        {
            _durationLeft -= Time.unscaledDeltaTime;
            _string = _durationLeft.ToString();
            timer.text = _string.Substring(0, 4);
            yield return new WaitForEndOfFrame();//
        }
        timer.gameObject.SetActive(false);
        GlobalData.Paused = false;
        audioManager.Unpause();
        Time.timeScale = 1;
        unpausing = false;
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
        screenTransition.localScale = Vector3.one * 0.01f;
        screenTransition.localScale = Vector3.one * 0.01f;
        screenTransition.gameObject.SetActive(true);
        float _progress = 0;
        float _timeTransitionStart = Time.realtimeSinceStartup;
        while(_progress < 1)
        {
            _progress = (Time.realtimeSinceStartup - _timeTransitionStart) / transitionDuration;
            screenTransition.localScale = Vector3.one * Mathf.SmoothStep(0.1f, screenTransitionToSize,_progress * _progress *_progress);

            audioManager.SetAudioVolume(1 - _progress);
            yield return new WaitForEndOfFrame();
        }
        GlobalData.DoOpenSceneTransition = true;
        if (sceneID == 1) GlobalData.LoadGameScene();
        else if (sceneID == 2) { }//load end scene
        else if (sceneID == 0) GlobalData.LoadMenuScene();
    }

    public IEnumerator OpenSceneTransition()
    {
        if (!GlobalData.DoOpenSceneTransition) yield break;

        GlobalData.DoOpenSceneTransition = false;
        if (!AudioTest3.AutoPlay && GlobalData.LoadSongHighScore(AudioTest3.songName) == 0) StartCoroutine(ShowKeys(keyFlashNum));
        screenTransition.gameObject.SetActive(true);
        screenTransition.transform.localScale = Vector3.one * screenTransitionToSize;
        float _progress = 0;
        float _timeTransitionStart = Time.realtimeSinceStartup;
        while (_progress < 1)
        {
            _progress = (Time.realtimeSinceStartup - _timeTransitionStart) / transitionDuration;
            screenTransition.localScale = Vector3.one * Mathf.SmoothStep(screenTransitionToSize, 1, Mathf.SmoothStep(0,1,_progress));

            yield return new WaitForEndOfFrame();
        }
        screenTransition.gameObject.SetActive(false);
    }

    public IEnumerator EndSongStuff()
    {
        //Debug.Log("Song End Stuff");
        UpdateStats();
        StartCoroutine(songStatsName.StartListEntranceAnim());
        StartCoroutine(songStatsNum.StartListEntranceAnim());
        AudioTest3.SongEnded = true;
        yield break;
    }

    public void UpdateStats()
    {
        string[] _stats = new string[6] { score.ToString(), GlobalData.LoadSongHighScore(AudioTest3.songName).ToString(), maxCombo.ToString(), beatCaptured.ToString(), beatMissed.ToString(), hitsTaken.ToString()};
        songStatsNum.UpdateListText(_stats);
    }

    public IEnumerator ShowKeys(int _flashCount)
    {
        Color _keyOriColor = keySprites[0].color;
        Color _keyFlashColor = keySprites[0].color;
        Vector3 _oriScale = keySprites[0].transform.localScale;
        _keyFlashColor.a = _keyFlashColor.a * keyFlashAlphaRatio;
        yield return new WaitForSeconds(transitionDuration);
        StartCoroutine(ScaleChanger(keySprites[0].transform, keyFadeDuration, _oriScale * keyFadeScale, keyFlashScale * _oriScale));
        for (int i = 0; i < keySprites.Count; i++)
        {
            keySprites[i].color = Color.clear;
            keySprites[i].gameObject.SetActive(true);
            StartCoroutine(SpriteAnim(keySprites[i], keyFadeDuration, Color.clear, _keyOriColor));
        }
        yield return new WaitForSeconds(keyFadeDuration);
        for (int c = 0; c < _flashCount; c++)
        {
            StartCoroutine(ScaleChanger(keySprites[0].transform, keyFlashDuration, keyFlashScale * _oriScale, _oriScale));
            for (int i = 0; i < keySprites.Count; i++)
            {
                StartCoroutine(SpriteAnim(keySprites[i], keyFlashDuration, _keyFlashColor, _keyOriColor));
            }
            yield return new WaitForSeconds(keyFlashDuration);
            StartCoroutine(ScaleChanger(keySprites[0].transform, keyFlashDuration, _oriScale, keyFlashScale * _oriScale));
            for (int i = 0; i < keySprites.Count; i++)
            {
                StartCoroutine(SpriteAnim(keySprites[i], keyFlashDuration, _keyOriColor, _keyFlashColor));
            }
            yield return new WaitForSeconds(keyFlashDuration);      
        }
        StartCoroutine(ScaleChanger(keySprites[0].transform, keyFadeDuration, keyFlashScale * _oriScale, keyFadeScale * _oriScale));
        for (int i = 0; i < keySprites.Count; i++)
        {
            StartCoroutine(SpriteAnim(keySprites[i], keyFlashDuration, _keyOriColor, Color.clear));
        }
    }

    public IEnumerator SpriteAnim(SpriteRenderer _spriteRenderer, float _duration, Color _startColor, Color _endColor)
    {
        float _startTime = Time.time;
        float _progress = 0;
        float _smoothProgress = 0;

        while(_progress < 1)
        {
            _progress = (Time.time - _startTime) / _duration;
            _smoothProgress = Mathf.SmoothStep(0, 1, _progress);

            //_spriteRenderer.transform.localScale = Vector3.Lerp(_startScale, _endScale, _smoothProgress);
            _spriteRenderer.color = Color.Lerp(_startColor, _endColor, _smoothProgress);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator ScaleChanger(Transform _transform, float _duration, Vector3 _startScale, Vector3 _endScale)
    {
        float _startTime = Time.time;
        float _progress = 0;
        float _smoothProgress = 0;

        while (_progress < 1)
        {
            _progress = (Time.time - _startTime) / _duration;
            _smoothProgress = Mathf.SmoothStep(0, 1, _progress);

            _transform.localScale = Vector3.Lerp(_startScale, _endScale, _smoothProgress);
            yield return new WaitForEndOfFrame();
        }
    }

}
