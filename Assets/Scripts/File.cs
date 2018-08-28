using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class File : MonoBehaviour
{
    public string filePath;
    public string[] itemNames;
    public int selectedIndex;

    public Text[] fileUIList;
    public FileManager fileManager;

    private Vector3 scrollStartLot;


    public void FindItemsInFile()
    {
        Object[] items = Resources.LoadAll(filePath);
        itemNames = new string[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            itemNames[i] = Path.GetFileNameWithoutExtension(filePath + "/" + items[i].name);
        }
    }

    public void Select()
    {
        string _selectedFilePath = filePath + "/" + itemNames[selectedIndex];
        Object[] _selectedObjs = Resources.LoadAll(_selectedFilePath);

        if(_selectedObjs.Length == 1 && _selectedObjs[0].GetType() == typeof(AudioClip))
        {
            GlobalData.selectedSong = (AudioClip)_selectedObjs[0];
            GlobalData.LoadGameScene();
        }
        else
        {
            File _selectedFile = Instantiate(fileManager.fileGO, Vector3.right, Quaternion.identity, this.transform).GetComponent<File>();
            _selectedFile.filePath = _selectedFilePath;
            _selectedFile.FindItemsInFile();
        }
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

