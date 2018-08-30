using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PPListAnim : MonoBehaviour {

    public Text[] textArray;
    public float statsStartScaleRatio;
    public float staggerDuration;
    public Vector3 itemToMoveDir;
    public float itemAnimDuration;
    private Color listEndColor;

    public IEnumerator StartListEntranceAnim()
    {
        listEndColor = textArray[0].color;
        for (int i = 0; i < textArray.Length; i++)
        {
            StartCoroutine(StartItemEntranceAnim(textArray[i]));

            yield return new WaitForSeconds(staggerDuration);
        }
    }

    public IEnumerator StartItemEntranceAnim(Text _text)
    {
        Color _startColor = listEndColor;
        _startColor.a = 0;
        _text.color = _startColor;
        Vector3 _endPos = _text.rectTransform.localPosition;
        _text.rectTransform.localScale = Vector3.one * statsStartScaleRatio;
        _text.rectTransform.localPosition = _endPos - itemToMoveDir;
        _text.gameObject.SetActive(true);

        float _progress = 0;
        float _smoothProgress = 0;
        float _startTime = Time.time;
        while (_progress < 1)
        {
            _progress = (Time.time - _startTime) / itemAnimDuration;
            _smoothProgress = Mathf.SmoothStep(0, 1, _progress);
            _text.color = Color.Lerp(_startColor, listEndColor, Mathf.SmoothStep(0, 1, _smoothProgress));
            _text.rectTransform.localPosition = Vector3.Lerp(_endPos - itemToMoveDir, _endPos, _smoothProgress);
            _text.rectTransform.localScale = Vector3.one * Mathf.SmoothStep(statsStartScaleRatio, 1, _smoothProgress);

            yield return new WaitForEndOfFrame();
        }
    }

    public void UpdateListText(string[] text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            textArray[i].text = text[i].ToString();
        }
    }
}
