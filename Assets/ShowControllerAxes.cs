using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ShowControllerAxes : MonoBehaviour
{
    public float axisLength = 0.2f; // Length of each axis line

    private ControllerVisualizer leftControllerVisualizer;
    private ControllerVisualizer rightControllerVisualizer;

    void Start()
    {
        // Create visualizers for the left and right controllers
        leftControllerVisualizer = new ControllerVisualizer(XRNode.LeftHand, axisLength);
        rightControllerVisualizer = new ControllerVisualizer(XRNode.RightHand, axisLength);
    }

    void Update()
    {
        // Update axes for both controllers
        leftControllerVisualizer.UpdateAxes();
        rightControllerVisualizer.UpdateAxes();
    }

    // Helper class to handle visualization for one controller
    private class ControllerVisualizer
    {
        public XRNode controllerNode;
        public float axisLength;

        private GameObject xAxisLine;
        private GameObject yAxisLine;
        private GameObject zAxisLine;

        public ControllerVisualizer(XRNode node, float length)
        {
            controllerNode = node;
            axisLength = length;

            // Create line renderers for the axes
            xAxisLine = CreateLineRenderer(Color.red, $"{node}_X_Axis");
            yAxisLine = CreateLineRenderer(Color.green, $"{node}_Y_Axis");
            zAxisLine = CreateLineRenderer(Color.blue, $"{node}_Z_Axis");
        }

        public void UpdateAxes()
        {
            // Get the controller position and rotation
            if (InputDevices.GetDeviceAtXRNode(controllerNode).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position) &&
                InputDevices.GetDeviceAtXRNode(controllerNode).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
            {
                // Update the axes positions
                UpdateLineRenderer(xAxisLine, position, rotation * Vector3.right);
                UpdateLineRenderer(yAxisLine, position, rotation * Vector3.up);
                UpdateLineRenderer(zAxisLine, position, rotation * Vector3.forward);
            }
        }

        private GameObject CreateLineRenderer(Color color, string name)
        {
            GameObject lineObj = new GameObject(name);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = color;
            lr.endColor = color;
            lr.positionCount = 2;
            return lineObj;
        }

        private void UpdateLineRenderer(GameObject lineObj, Vector3 start, Vector3 direction)
        {
            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
            lr.SetPosition(0, start);
            lr.SetPosition(1, start + direction * axisLength);
        }
    }
}
