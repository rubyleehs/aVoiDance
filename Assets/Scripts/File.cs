using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class File : MonoBehaviour
{
    public UIManager uiManager;
    public string filePath;
    public string[] itemNames;
    public int selectedIndex;

    public Text[] fileUIList;
    public FileManager fileManager;

    private Vector3 scrollStartLot;


    public void FindItemsInFile()
    {
        //Object[] items = Resources.LoadAll(filePath);
        //Debug.Log(GlobalData.availableSongNames.Length);
        if (GlobalData.availableSongNames == null) return;
        itemNames = new string[GlobalData.availableSongNames.Length];
        for (int i = 0; i < itemNames.Length; i++)
        {
            //itemNames[i] = Path.GetFileNameWithoutExtension(filePath + "/" + items[i].name);
            itemNames[i] = GlobalData.availableSongNames[i];
        }
    }

    public void Select()
    {
        string _selectedFilePath = Application.streamingAssetsPath + "/" + itemNames[selectedIndex];
        AudioClip _selectedAudio = GlobalData.songsAvailable[selectedIndex];

        GlobalData.selectedSong = _selectedAudio;
        GlobalData.selectedSongIndex = selectedIndex;
        AudioTest3.songName = GlobalData.availableSongNames[selectedIndex];
        StartCoroutine(uiManager.LoadSceneWithAnim(1));

    }

    public void UpdateListUI(int _curIndex)
    {
        for (int i = -2; i < 3; i++)
        {
            if (_curIndex + i >= 0 && _curIndex + i < itemNames.Length)
            {
                fileUIList[i + 2].text = itemNames[_curIndex + i];
            }
            else fileUIList[i + 2].text = "";
        }
    }

    public void Scroll(int dir)
    {
        selectedIndex += -dir;
        selectedIndex = Mathf.Clamp(selectedIndex, 0, itemNames.Length -1);
        UpdateListUI(selectedIndex);
    }
}

