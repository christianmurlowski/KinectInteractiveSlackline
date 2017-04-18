using Kinect = Windows.Kinect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyView : MonoBehaviour
{

    public Material BoneMaterial;						// material to draw the bone
    public GameObject BodyManager;						// BodyManager object

    public Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();			// store tracked bodies
    private BodyManager _BodyManager;					// private BodyManager object

    public int SkeletonScale = 10;				// scale for skeleton

    // Map of bones
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType> ()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft }, //Need this for hand sates
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight }, //Need this for hand state
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //DontDestroyOnLoad();
    }

    // check if BodyManager has data
    void Update ()
    {
        int state = 0;

        if (BodyManager == null)
        {
            return;
        }

        _BodyManager = BodyManager.GetComponent<BodyManager> ();	// Store gameobject for usage
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetBodies ();	// Get bodies of bodymanager
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();		// List to store tracked body ids
        foreach (var body in data) // Store body from data
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);	// add body with its id into the list
            }
        }

        List<ulong> knownIds = new List<ulong> (_Bodies.Keys); // List with current bodies
        foreach(ulong trackingId in knownIds)
        {
            // If curent tracked body list contains not any tracking id --> Delete untracked body
            if (!trackedIds.Contains (trackingId))
            {
                Destroy (_Bodies [trackingId]);
                _Bodies.Remove (trackingId);
            }
        }

        foreach (var body in data) // Store body from data
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId)) // if list of bodies contains not current tracking id
                {
                    _Bodies [body.TrackingId] = CreateBodyObject (body.TrackingId); // Create body object with id and connected joints
                    _Bodies[body.TrackingId].gameObject.tag = "Player";
                }
                RefreshBodyObject (body, _Bodies [body.TrackingId]); // Refresh current body and its GameObject
            }

            // Tracking for UI Component
            if(body.IsTracked)
                KinectInputModule.instance.TrackBody(body);
        }
    } // Update

    // creates the object in the view regarding the body id
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject ("Body:" + id); // Crate a new body GameObject with id name

        // loop over all joints to draw body (ThumbRight is the highest value)
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive (PrimitiveType.Cube); // Create cube for every joint

            LineRenderer lr = jointObj.AddComponent<LineRenderer> (); // linerender to connect joints
            lr.SetVertexCount(2);			// Segments of line
            lr.material = BoneMaterial;		// Line material
            lr.SetWidth (0.05f, 0.05f);		// Line width

            jointObj.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);	// Joint cube scale
            jointObj.name = jt.ToString();									// Joint cube name
            jointObj.transform.parent = body.transform;						// Parent ob the Cube is the Body + id GameObject
        }
        return body;
    }

    // Refresh the body game object
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyobject)
    {

        //loop over each joint type
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints [jt];	// Current joint in loop is source joint
            Kinect.Joint? targetJoint = null;				// Target joint

            // if bonemap contains joint --> it is the targetjoint
            if (_BoneMap.ContainsKey (jt))
            {
                targetJoint = body.Joints [_BoneMap [jt]];	// Target joint is next joint
            }

            Transform jointObj = bodyobject.transform.FindChild (jt.ToString ());	// Transform of child joint of bodyobject through string
            jointObj.localPosition = GetVector3FromJoint (sourceJoint);				// Position of transform joint is position of sourceJoint

            LineRenderer lr = jointObj.GetComponent<LineRenderer> ();
            // Draw line from source to target if targetJoint is present
            if (targetJoint.HasValue)
            {
                lr.SetPosition (0, jointObj.localPosition);					    // current join position
                lr.SetPosition (1, GetVector3FromJoint (targetJoint.Value));    // position of target join
                lr.SetColors (GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState)); // color line regarding state
            }
            else
            {
                lr.enabled = false;
            }
        }
    }
    // return current position of given joint
    private Vector3 GetVector3FromJoint(Kinect.Joint joint) {
        // return position with scale to enlarge skeleton
        return new Vector3 (joint.Position.X * SkeletonScale, joint.Position.Y * SkeletonScale, joint.Position.Z * SkeletonScale);
    }

    // return the color for curent state
    private static Color GetColorForState(Kinect.TrackingState state) {
        switch (state) {
            case Kinect.TrackingState.Tracked:
                return Color.green;
            case Kinect.TrackingState.Inferred:
                return Color.red;
            default:
                return Color.black;
        }
    }
}
