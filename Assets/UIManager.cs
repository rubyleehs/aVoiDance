using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text comboText;
    public float comboTextFadeDuration;
    public float comboTextEndRatioSize;

    private Color comboTextRevealColor;
    private Color comboTextEndColor;
    private float comboTextOriSize;

    private int score = 0;

    public Text scoreText;
    public float scoreTextExpandRatio;
    public float scoreTextShrinkDuration;
    private float scoreTextOriSize;

    public Text songNameText;

    private void Awake()
    {
        comboTextRevealColor = comboText.color;
        comboTextEndColor = comboText.color;
        comboTextEndColor.a = 0;
        comboTextOriSize = comboText.fontSize;
        comboText.enabled = false;
        scoreTextOriSize = scoreText.fontSize;
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
        score += (10 * AudioTest3.combo);
        scoreText.text = "" + score;
        scoreText.fontSize = (int)(scoreTextOriSize * scoreTextExpandRatio);
        while (_progress < 1)
        {
            scoreText.fontSize = (int)Mathf.Lerp(scoreTextOriSize, scoreTextOriSize * scoreTextExpandRatio, _progress);
            _progress = (Time.time - _startTime) / scoreTextShrinkDuration;
            yield return new WaitForEndOfFrame();
        }
    }
}
