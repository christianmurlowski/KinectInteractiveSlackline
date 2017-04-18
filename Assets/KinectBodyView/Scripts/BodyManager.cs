using Windows.Kinect;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BodyManager : MonoBehaviour
{
    public static BodyManager BM;
    // Sensor data from Kinect
    private KinectSensor _Sensor;

    // Frame reader for body frame source
    private BodyFrameReader _Reader;

    // Body array for detected bodies
    private Body[] _bodies = null;

    private bool _newBodyData;

    // get # of bodies (default 6)
    public Body[] GetBodies()
    {
        return _bodies;
    }

    public BodyFrameReader GetBodyFrameReader()
    {
        return _Reader;
    }

    public KinectSensor GetSensor()
    {
        return _Sensor;
    }

    public bool NewBodyData()
    {
        return _newBodyData;
    }

    private void Awake()
    {
        if (BM == null)
        {
            DontDestroyOnLoad(gameObject);
            BM = this;
        }
        else
        {
            if (BM != null)
            {
                Destroy(gameObject);
            }
        }
    }


    // Establish connection for Kinect
    void Start ()
    {


        _Sensor = KinectSensor.GetDefault ();

        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader ();

            // check if body data is available in frame
            if (_bodies == null)
            {
                Debug.Log("_bodies: " + GetBodies());
                _bodies = new Body[_Sensor.BodyFrameSource.BodyCount];
                // initialize bodies array with # of bodies (default 6)
                Debug.Log("_bodies new: " + GetBodies());
            }

            if (!_Sensor.IsOpen)
            {
                // open KinectSensor for usage
                _Sensor.Open ();
            }
        }
    }

    // Store data in Body array once per frame
    void Update ()
    {
        _newBodyData = false;

        // check if reader has data
        if (_Reader != null)
        {
            // most recent body frame
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {

                // get list of refreshed body data
                frame.GetAndRefreshBodyData(_bodies);
                _newBodyData = true;

                // clean up running stream
                frame.Dispose ();
                frame = null;
            }
        }
    }

    // Disposes Reader, close sensor stream
    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose ();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close ();
            }
            _Sensor = null;
        }
    }
}
