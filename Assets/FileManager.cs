using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileManager : MonoBehaviour {

    public File currentFile;
    public GameObject fileGO;//5 textboxes but show 3 only

    // Use this for initialization
    void Start () {
        currentFile.FindItemsInFile();
        currentFile.UpdateListUI(0);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown)
        {
            if ((Input.GetAxis("Horizontal") > 0))
            {
                currentFile.Select();
            }


            if (Input.GetAxis("Vertical") != 0)
            {
                if (Input.GetAxis("Vertical") > 0) currentFile.Scroll(1);
                else currentFile.Scroll(-1);
            }
        }
    }
}
