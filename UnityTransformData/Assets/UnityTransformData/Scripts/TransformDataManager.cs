using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;


public class TransformDataManager : MonoBehaviour
{
    public int speed; 
    public string filePath;
    public List<Transform> transforms = new List<Transform>();
    private List<TransformDataConnector> transformDataConnectors = new List<TransformDataConnector>();
    private bool isRecording = false;
    private bool isPlaying = false;

    Dictionary<int, List<TransformData>> transformDataConnectorsTimeDictionary = new Dictionary<int, List<TransformData>>();

    private void Start()
    {
        CreateDictionary();
    }

    private void Update()
    {
        //probably not the best idea to record every update! 
        if (isRecording)
        {
            Record();
        }
        if (isPlaying)
        {
            if (!transformDataConnectorsTimeDictionary.ContainsKey(idx)) return;

            List<TransformData> data = transformDataConnectorsTimeDictionary[idx];
            foreach (TransformData td in data)
            {
                TransformDataConnector connector = new TransformDataConnector();
                connector.AddByName(td);
                connector.ApplyData();
            }

            if (idx == maxIdx)
            {
                isPlaying = false;
            }
            else
            {
                idx++;
            }
        }
    }

    private void Record()
    {
        List<TransformData> transformDataConnectorsTime = new List<TransformData>();
        foreach (TransformDataConnector t in transformDataConnectors)
        {
            transformDataConnectorsTime.Add(t.GetTransformData());
        }

        transformDataConnectorsTimeDictionary.Add(Time.frameCount, transformDataConnectorsTime);   
    }

  

    public void CreateDictionary()
    {
        foreach(Transform t in transforms)
        {
            if(t != null)
            {
                TransformDataConnector td = new TransformDataConnector();
                td.t = t;
                transformDataConnectors.Add(td);
            }
        }
    }

    int nextId = 0;
    public bool IsNext()
    {
        List<int> keys = transformDataConnectorsTimeDictionary.Keys.ToList<int>();
        return (nextId <= keys.Count - 1);
    }

    protected void Apply(List<TransformData> data)
    {
        foreach (TransformData t in data)
        {
            if (t != null)
            {
                TransformDataConnector connector = new TransformDataConnector();
                connector.AddByName(t);
                connector.ApplyData();
            }
        }
    }

    public void ApplyNext()
    {
        List<int> keys = transformDataConnectorsTimeDictionary.Keys.ToList<int>();
        nextId = (nextId > keys.Count - 1) ? 0 : nextId;
        int currentKey = keys[nextId];
        Apply(transformDataConnectorsTimeDictionary[currentKey]);
        nextId++;
    }

    public void StartRecording()
    {
        isRecording = true;
        Debug.Log("Start Recording Transform Data");
    }

    public void StopRecording()
    {
        Debug.Log("End Recording Transform Data");
        isRecording = false;
        WriteFile();

    }

    public void WriteFile()
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(transformDataConnectorsTimeDictionary);
        File.WriteAllText(@filePath, json);
        Debug.Log("Transform file written");
    }

    public void LoadFile(string jsonPath)
    {
        string json = File.ReadAllText(jsonPath);
        transformDataConnectorsTimeDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject <Dictionary<int, List<TransformData>>>(json);
        Debug.Log("Transform data loaded");
    }

    private int idx = 0;
    private int maxIdx = 0;
    public void Play()
    {
        isPlaying = true;
        idx = transformDataConnectorsTimeDictionary.Keys.Min();
        maxIdx = transformDataConnectorsTimeDictionary.Keys.Max();
        StartCoroutine(PlayScenario());
    }


    private IEnumerator PlayScenario()
    {
        while (idx <= maxIdx)
        {
            if (transformDataConnectorsTimeDictionary.ContainsKey(idx))
            {
                List<TransformData> data = transformDataConnectorsTimeDictionary[idx];
                foreach (TransformData td in data)
                {
                    TransformDataConnector connector = new TransformDataConnector();
                    connector.AddByName(td);
                    connector.ApplyData();
                }
            }
            idx++;
            yield return new WaitForSeconds(1f/speed);
        }
        isPlaying = false;
    }

}
