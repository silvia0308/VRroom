using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class DataCollector : MonoBehaviour
{
    public bool conductColletion;
    public string folderPath;
    public string motion;
    public string user;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (conductColletion)
        {
            DataInfo dataInfo = new DataInfo();
            dataInfo.time = Time.time;
            Device headset = new Device();
            headset.position = new Position(transform.GetChild(0).position);
            headset.rotation = new Rotation(transform.GetChild(0).rotation);
            dataInfo.headset = headset;
            Device leftController = new Device();
            leftController.position = new Position(transform.GetChild(1).position);
            leftController.rotation = new Rotation(transform.GetChild(1).rotation);
            dataInfo.leftController = leftController;
            Device rightController = new Device();
            rightController.position = new Position(transform.GetChild(2).position);
            rightController.rotation = new Rotation(transform.GetChild(2).rotation);
            dataInfo.rightController = rightController;
            string path = folderPath + "/" + motion + "_" + user + ".json";
            if (!File.Exists(path)) File.AppendAllText(path, "[");
            else File.AppendAllText(path, ",");
            File.AppendAllText(path, JsonConvert.SerializeObject(dataInfo));
        }
    }
    private void OnApplicationQuit()
    {
        string path = folderPath + "/" + motion + "_" + user + ".json";
        File.AppendAllText(path, "]");
        string jsonString = File.ReadAllText(path);
        List<DataInfo> results = JsonConvert.DeserializeObject<List<DataInfo>>(jsonString);
    }
}

[System.Serializable]
public class DataInfo
{
    public float time { get; set; }
    public Device headset { get; set; }
    public Device leftController { get; set; }
    public Device rightController { get; set; }
}

[System.Serializable]
public class Device
{
    public Position position { get; set; }
    public Rotation rotation { get; set; }
}

[System.Serializable]
public class Position
{
    public Position(Vector3 position)
    {
        x=position.x; 
        y=position.y; 
        z=position.z;
    }
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
}

[System.Serializable]
public class Rotation
{
    public Rotation(Quaternion quaternion)
    {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public float w { get; set; }
}