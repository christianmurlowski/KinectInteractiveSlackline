using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using UnityEngine.UI;

public class InitialTextScript : MonoBehaviour
{

    public Text SensorText;

    public GameObject BodyManager;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{

      if (KinectSensor.GetDefault().IsOpen)
	    {
	        SensorText.text = "Kinect Sensor is online";
	    }
	}
}
