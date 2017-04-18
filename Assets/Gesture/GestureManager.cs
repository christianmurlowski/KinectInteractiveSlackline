using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Kinect;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class GestureManager : MonoBehaviour
{
    // GameObject of BodyManager
    private GameObject _gameObjectBM;

    // private variable for usage of BodyManager
    private BodyManager _bodyManager;

    // Kinect sensor for usage
    private KinectSensor _kinectSensor;

    public GameObject  CanvasExerciseManager;
    private CanvasExerciseManager _canvasExerciseManager;

    // Text GameObjects
    public Text ConfidenceTextGameObject;
    public Text GestureTextGameObject;
    public Text TimeTextGameObject;


    /////     STOPWATCH     /////
//    public Text TimeTextGameObjectSW;
//    public Text TimeTextGameObjectSW2;

//    public Stopwatch timer;

    public Image TimeImage;

    private byte[] _colorData;
    private Texture2D _colorTexture;

    private BodyFrameReader _bodyFrameReader;
    private Body[] _bodies = null;


    private string _leanRightGestureName = "WaveTest_Right";
    private string _leanLeftGestureName = "WaveTest_Left";

    private UnityEngine.Color[] _bodyColors;

    private List<GestureDetector> _gestureDetectorList = null;


    // Name of Gesture
    public GestureName gestureName = GestureName.GroundLeg_Right;

    public enum GestureName
    {
        GroundLeg_Right,GroundLeg_Left,
        OneLegArmsOut_Right, OneLegArmsOut_Left,
        OneLegSquats_Right, OneLegSquats_Left,
        SlacklineStartNearFeet_Right, SlacklineStartNearFeet_Left,
        SlacklineStartOtherFeet_Right, SlacklineStartOtherFeet_Left,
        SlacklineStartNearFeetThenBoth_Right, SlacklineStartNearFeetThenBoth_Left,
        SlacklineStartOtherFeetThenBoth_Right, SlacklineStartOtherFeetThenBoth_Left,
        SlacklineStayBothFeet,
        SlacklineStayFeet_Right, SlacklineStayFeet_Left,
        SlacklineGoUpWalk2Steps,
        WaveTest_Right, WaveTest_Left,
        WaveTestUp_Right, WaveTestUp_Left
    }



    // Use this for initialization
    void Start()
    {
//        timer = new Stopwatch();
/*
        _gameObjectBM = BodyManager.BM;
        if (BodyManager == null)
        {
            return;
        }
*/
       // _bodyManager = BodyManager.GetComponent<BodyManager>();
        _bodyManager = BodyManager.BM;
        if (_bodyManager == null)
        {
            return;
        }
        _canvasExerciseManager = CanvasExerciseManager.GetComponent<CanvasExerciseManager>();

        _kinectSensor = _bodyManager.GetSensor();

        // initialize Body array with # of maximum bodies
        _bodies = _bodyManager.GetBodies();
        Debug.Log(_bodyManager + " | " + _bodies);
        //_bodyFrameReader = _kinectSensor.BodyFrameSource.OpenReader();

        // Initialize new GestureDetector list
        _gestureDetectorList = new List<GestureDetector>();

        // For every body add a new GestureDetector instance to the list
        for (int bodyIndex = 0; bodyIndex < _bodies.Length; bodyIndex++)
        {
            GestureTextGameObject.text = "none";
            _gestureDetectorList.Add(new GestureDetector(_kinectSensor));
        }
    }

    // Update is called once per frame
    void Update()
    {
    /*
        bool newBodyData = false;
        using (BodyFrame bodyFrame = _bodyFrameReader.AcquireLatestFrame())
        {
            if (bodyFrame != null)
            {
                bodyFrame.GetAndRefreshBodyData(_bodies);
                newBodyData = true;
            }
        }
    */

        //if (newBodyData)
        if (_bodyManager.NewBodyData())
        {
            for (int bodyIndex = 0; bodyIndex < _bodies.Length; bodyIndex++)
            {
                var body = _bodies[bodyIndex];
                if (body != null)
                {
                    var trackingId = body.TrackingId;

                    if (trackingId != _gestureDetectorList[bodyIndex].TrackingId)
                    {
                        GestureTextGameObject.text = "none";

                        _gestureDetectorList[bodyIndex].TrackingId = trackingId;

                        _gestureDetectorList[bodyIndex].IsPaused = (trackingId == 0);
                        _gestureDetectorList[bodyIndex].OnGestureDetected += CreateOnGestureHandler(bodyIndex);
                    }
                }
            }
        }
    }// Update

    private EventHandler<GestureEventArgs> CreateOnGestureHandler(int bodyIndex)
    {
        return (object sender, GestureEventArgs e) => OnGestureDetected(sender, e, bodyIndex);
    }

    private void OnGestureDetected(object sender, GestureEventArgs e, int bodyIndex)
    {
        var isDetected = e.IsBodyTrackingIdValid && e.IsGestureDetected;

       // Debug.Log(_canvasExerciseManager.IsReady());
       // Debug.Log("GESTUREID: " + e.GestureID);

       // if ((e.GestureID == _leanRightGestureName.ToString()) && _canvasExerciseManager.GetReadyState() == "true" )
        if ((e.GestureID == gestureName.ToString() ) && _canvasExerciseManager.GetReadyState() == "true" )
        {

            GestureTextGameObject.text = "if Gesture: " + isDetected;
            ConfidenceTextGameObject.text = "if Confidence: " + e.DetectionConfidence;

            if (e.DetectionConfidence > 0.3f)
            {
               /* if (timer.IsRunning)
                {
                    if (timer.ElapsedMilliseconds >= 5000f)
                    {
                        Debug.Log("5sec");
                    }
                }
                else
                {
                    timer.Start();
                }
                var testtimer = timer.ElapsedMilliseconds * 0.001f;
                TimeTextGameObjectSW.text = timer.ElapsedMilliseconds.ToString();
                TimeTextGameObjectSW2.text = testtimer.ToString();*/

                // Fill the bar for duration
                _canvasExerciseManager.StartTime();
            }
            else
            {

                GestureTextGameObject.text = "Gesture: " + isDetected;

             /*   timer.Reset();

                var testtimer = timer.ElapsedMilliseconds * 0.001f;
                TimeTextGameObjectSW.text = timer.ElapsedMilliseconds.ToString();
                TimeTextGameObjectSW2.text = testtimer.ToString();
*/
                // Stop the timer
                _canvasExerciseManager.StopTime();

            }
        }

    }// OnGestureDetected
}// Class