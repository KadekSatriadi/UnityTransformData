using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Crosstales.FB;

public class TransformDataFileLoader : MonoBehaviour
{


    public string jsonPath;
    public TransformDataManager transformSaveManager;
    public float playSpeed = 1;
    private bool isPlaying = false;

    public void Load()
    {
        string json = File.ReadAllText(jsonPath);
        transformSaveManager.LoadFile(json);
    }

    public void Play()
    {
        isPlaying = true;
        StartCoroutine(StartPlay());
    }

    public void LoadFile()
    {
        jsonPath = FileBrowser.OpenSingleFile();
        Debug.Log("Loading " + jsonPath);
        Load();
    }



    IEnumerator StartPlay()
    {
        while (isPlaying)
        {
            transformSaveManager.ApplyNext();
            if (!transformSaveManager.IsNext())
            {
                isPlaying = false;
                yield return null;
                Debug.Log("End of frame");
            }
            else
            {
                yield return new WaitForSeconds(1 / playSpeed);
            }
        }
    }
}
