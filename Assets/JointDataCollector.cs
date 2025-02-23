using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using Newtonsoft.Json;
using System.IO;

public class JointDataCollector : MonoBehaviour
{
    private XRHandSubsystem m_Subsystem;
    private int[] jointIndexes = new int[] {0, 7, 8, 9, 12, 13, 14, 22, 23, 24, 17, 18, 19, 2, 3, 4};
    public string folderPath;
    public string motion;
    public string user;

    // Start is called before the first frame update
    void Start()
    {
        m_Subsystem =
           XRGeneralSettings.Instance?
               .Manager?
               .activeLoader?
               .GetLoadedSubsystem<XRHandSubsystem>();
            if (m_Subsystem != null)
                m_Subsystem.updatedHands += OnHandUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Subsystem != null)
        {
            JointData jointData = new JointData();
            jointData.time = Time.time;
            jointData.leftHand = new List<float>();
            jointData.rightHand = new List<float>();
            XRHand leftHand = m_Subsystem.leftHand;
            XRHand rightHand = m_Subsystem.rightHand;
            for(int i = 0; i < jointIndexes.Length; i++)
            {
                var trackingData = leftHand.GetJoint(XRHandJointIDUtility.FromIndex(jointIndexes[i]));
                if (trackingData.TryGetPose(out Pose pose))
                {
                    jointData.leftHand.Add(pose.position.x);
                    jointData.leftHand.Add(pose.position.y);
                    jointData.leftHand.Add(pose.position.z);
                }
                trackingData = rightHand.GetJoint(XRHandJointIDUtility.FromIndex(jointIndexes[i]));
                if (trackingData.TryGetPose(out Pose pose2))
                {
                    jointData.rightHand.Add(pose2.position.x);
                    jointData.rightHand.Add(pose2.position.y);
                    jointData.rightHand.Add(pose2.position.z);
                }
            }
            string path = folderPath + "/" + motion + "_" + user + ".json";
            if (!File.Exists(path)) File.AppendAllText(path, "[");
            else File.AppendAllText(path, ",");
            File.AppendAllText(path, JsonConvert.SerializeObject(jointData));
        }
    }

    void OnHandUpdate(XRHandSubsystem subsystem,XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,XRHandSubsystem.UpdateType updateType)
    {
        switch (updateType)
        {
            case XRHandSubsystem.UpdateType.Dynamic:
                break;
            case XRHandSubsystem.UpdateType.BeforeRender:
                break;
        }
    }
    private void OnApplicationQuit()
    {
        string path = folderPath + "/" + motion + "_" + user + ".json";
        File.AppendAllText(path, "]");
        string jsonString = File.ReadAllText(path);
        List<JointData> results = JsonConvert.DeserializeObject<List<JointData>>(jsonString);
    }
}

[System.Serializable]
public class JointData
{
    public float time { get; set; }
    public List<float> leftHand { get; set; }
    public List<float> rightHand { get; set; }
}
