using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileManager : MonoBehaviour {

    public Text enterText;
    public File currentFile;
    public GameObject fileGO;//5 textboxes but show 3 only
    public static bool IsSelecting;
    public Text modeText;
    public SpriteRenderer upArrow;
    public SpriteRenderer downArrow;

    public Color defaultColor;
    public Color pressedColor;
    public float clickChangeDuration;
    private float lastClickTime;

    // Use this for initialization
    void Start () {
        IsSelecting = false;//
        currentFile.FindItemsInFile();
        currentFile.UpdateListUI(0);
        modeText.text = "      " + GlobalData.modeText[GlobalData.mode];
        modeText.text += "  ▶";
        lastClickTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
        float progress = (Time.time - lastClickTime)/clickChangeDuration;
        upArrow.color = Color.Lerp(upArrow.color, defaultColor, progress);
        downArrow.color = Color.Lerp(downArrow.color, defaultColor, progress);

        if (Input.anyKeyDown)
        {
            if (IsSelecting && (Input.GetAxis("Horizontal") < 0 || Input.GetKey(KeyCode.Escape)))
            {
                IsSelecting = false;
                currentFile.gameObject.SetActive(false);
                enterText.gameObject.SetActive(true);
                return;
            }

            if ((Input.GetAxis("Horizontal") > 0 || Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return)))
            {
                if (!IsSelecting)
                {
                    IsSelecting = true;
                    currentFile.gameObject.SetActive(true);
                    enterText.gameObject.SetActive(false);
                    return;
                }
                currentFile.Select();
                return;
            }


            if (Input.GetAxis("Vertical") != 0)
            {
                if (IsSelecting)
                {
                    if (Input.GetAxis("Vertical") > 0) currentFile.Scroll(1);
                    else currentFile.Scroll(-1);
                }
                else
                {
                    lastClickTime = Time.time;
                    if (Input.GetAxis("Vertical") > 0)
                    {
                        upArrow.color = pressedColor;
                        GlobalData.mode--;
                    }
                    else
                    {
                        downArrow.color = pressedColor;
                        GlobalData.mode++;
                    }
                    GlobalData.mode = Mathf.Clamp(GlobalData.mode, 0, 2 * GlobalData.modeText.Length - 1);
                    modeText.text = "      ";
                    modeText.text += GlobalData.modeText[GlobalData.mode % GlobalData.modeText.Length];

                    if (GlobalData.mode == 0) upArrow.gameObject.SetActive(false);
                    else upArrow.gameObject.SetActive(true);

                    if (GlobalData.mode == 2 * GlobalData.modeText.Length - 1) downArrow.gameObject.SetActive(false);
                    else downArrow.gameObject.SetActive(true);

                    if (GlobalData.mode >= GlobalData.modeText.Length)
                    {
                        modeText.text += " - No Assist";
                    }
                    modeText.text += "  ▶";
                }
            }
        }
    }
}
