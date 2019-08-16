using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformDataConnector
{
    public Transform t;
    protected TransformData transformData;

    public string GetJSON()
    {
        UpdateData();
        return JsonUtility.ToJson(transformData);
    }

    private void UpdateData()
    {
        if (t == null) return;
        transformData = new TransformData();
        transformData.name = t.name;
        transformData.position = new float[] { t.position.x, t.position.y, t.position.z };
        transformData.rotation = new float[] { t.rotation.eulerAngles.x, t.rotation.eulerAngles.y, t.rotation.eulerAngles.z }; 
        transformData.scale = new float[] { t.lossyScale.x, t.lossyScale.y, t.lossyScale.z }; 
    }

    public TransformData GetTransformData()
    {
        UpdateData();
        return transformData;
    }

    public void AddByName(TransformData d)
    {
        GameObject g = GameObject.Find(d.name);
        if(g != null)
        {
            t = g.transform;
            transformData = d;
        }
    }

    public void ApplyData()
    {
        if(t == null)
        {
            Debug.LogWarning("Transform is null");
            return;
        }
        t.position = new Vector3(transformData.position[0], transformData.position[1], transformData.position[2]);
        Vector3 euler = new Vector3(transformData.rotation[0], transformData.rotation[1], transformData.rotation[2]);
        t.rotation = Quaternion.Euler(euler);
        t.localScale = new Vector3(transformData.scale[0], transformData.scale[1], transformData.scale[2]);
    }




}
