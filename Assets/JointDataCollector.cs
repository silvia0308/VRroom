using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using Newtonsoft.Json;
using System.IO;

public class JointDataCollector : MonoBehaviour
{
    private XRHandSubsystem m_Subsystem;
    private int[] jointIndexes = new int[] { 0, 2, 3, 4, 7, 8, 9, 12, 13, 14, 17, 18, 19, 22, 23, 24 };

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
            Hand left = new Hand();
            Hand right = new Hand();
            jointData.leftHand = left;
            jointData.rightHand = right;
            left.positions = new List<float>();
            left.rotations = new List<float>();
            right.positions = new List<float>();
            right.rotations = new List<float>();
            XRHand leftHand = m_Subsystem.leftHand;
            XRHand rightHand = m_Subsystem.rightHand;
            for(int i = 0; i < jointIndexes.Length; i++)
            {
                var trackingData = leftHand.GetJoint(XRHandJointIDUtility.FromIndex(jointIndexes[i]));
                if (trackingData.TryGetPose(out Pose pose))
                {
                    left.positions.Add(pose.position.x);
                    left.positions.Add(pose.position.y);
                    left.positions.Add(pose.position.z);
                    left.rotations.Add(pose.rotation.x);
                    left.rotations.Add(pose.rotation.y);
                    left.rotations.Add(pose.rotation.z);
                    left.rotations.Add(pose.rotation.w);
                }
                trackingData = rightHand.GetJoint(XRHandJointIDUtility.FromIndex(jointIndexes[i]));
                if (trackingData.TryGetPose(out Pose pose2))
                {
                    right.positions.Add(pose2.position.x);
                    right.positions.Add(pose2.position.y);
                    right.positions.Add(pose2.position.z);
                    right.rotations.Add(pose2.rotation.x);
                    right.rotations.Add(pose2.rotation.y);
                    right.rotations.Add(pose2.rotation.z);
                    right.rotations.Add(pose2.rotation.w);
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
                // Update game logic that uses hand data
                break;
            case XRHandSubsystem.UpdateType.BeforeRender:
                // Update visual objects that use hand data
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
    public Hand leftHand { get; set; }
    public Hand rightHand { get; set; }
}
public class Hand
{
    public List<float> positions { get; set; }
    public List<float> rotations { get; set; }
}
