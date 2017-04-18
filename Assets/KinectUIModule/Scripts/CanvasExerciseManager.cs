using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class CanvasExerciseManager : MonoBehaviour
{
    public Text TextConfidence;
    public Text TextGesture;
    public Text TextTime;
    public Text TextTimeWait;

    public Toggle[] ToggleArray;
    private Toggle _toggleCurrent;
    private Toggle _toggleNext;
    private Toggle _toggleLast;

    public Image ImageFillBar;

    public GameObject GestureManager;

    private Stopwatch _timer;

    private float _timeWait;

    private string _ready;

    public GameObject CanvasManager;
    private CanvasManager _canvasManager;

	// Use this for initialization
	void Start ()
	{
	    // First toggle in array
	    _toggleCurrent = ToggleArray[0];
        // Next toggle in array
	    _toggleNext = ToggleArray[System.Array.IndexOf(ToggleArray, _toggleCurrent) + 1];
	    _toggleLast = ToggleArray[ToggleArray.Length - 1];
	    _ready = "true";

        _timer = new Stopwatch();

	    _timeWait = 2.0f;

	    _canvasManager = CanvasManager.GetComponent<CanvasManager>();
	    Debug.Log("canvasexercise");
	}

    void Update()
    {
        if (_ready == "false" && !_toggleLast.isOn)
        {
            WaitForNextAttempt();
        }
    }

    public string GetReadyState()
    {
        return _ready;
    }

    public void StartTime()
    {
        if (_timer.IsRunning)
        {

        }
        else
        {
            _timer.Start();
        }
        TextTime.text = _timer.ElapsedMilliseconds.ToString();


        // Exercise duration completed
        if (_timer.ElapsedMilliseconds >= 5000f)
        {

            StopTime();
            // Reset fillbar
            //ResetFilling();

            // Checkmark
            GoalSuccess();

            //GestureManager.SetActive(false);

        }
        else
        {
            StartFilling(_timer.ElapsedMilliseconds * 0.001f);
        }

    }

    public void StopTime()
    {
        _timer.Reset();
        TextTime.text = "0";
        ResetFilling();
        //GestureManager.SetActive(false);
    }

    private void StartFilling(float time)
    {
        ImageFillBar.fillAmount = time / 5.0f;
    }

    private void ResetFilling()
    {
        ImageFillBar.fillAmount = 0.0f;
        //WaitForNextAttempt();
    }

    private void GoalSuccess()
    {
        _toggleCurrent.isOn = true;

        if (_toggleCurrent == ToggleArray[ToggleArray.Length-1])
        {
            _ready = "false";
            GestureManager _gm;
            // TODO: eventuell eventhandler von tracking id löschen damit das script nicht mehr ausgeführt wird
            // disbale gesture manager
            if (GestureManager.activeSelf)
            {
                _gm = GestureManager.GetComponent<GestureManager>();
                GestureManager.SetActive(false);
                _gm.enabled = false;
            }

            StartCoroutine(WaitBeforeNextCanvas(2.0f));
            // load next canvas

            // Finished
            // Display finish text
            // Click on next button to go to next exercise
            // Play next scene
        }
        else
        {   _toggleCurrent = ToggleArray[System.Array.IndexOf(ToggleArray, _toggleCurrent) + 1];
            //_toggleNext = ToggleArray[System.Array.IndexOf(ToggleArray, _toggleCurrent) + 1];
            _ready = "false";
            // nexttoggle is currenttoggle
        }
    }

    public void WaitForNextAttempt()
    {
        _timeWait -= Time.deltaTime;
        TextTimeWait.text = Mathf.Round(_timeWait).ToString();
        if (_timeWait <= 0.0f)
        {
            Debug.Log("WHATDAFACK");
            TextTimeWait.text = "";
            _ready = "true";
            _timeWait = 2.0f;
        }
    }

    IEnumerator WaitBeforeNextCanvas(float time)
    {
        Debug.Log("before");
        yield return new WaitForSeconds(time);
        Debug.Log("after");
        _canvasManager.NextCanvas();
    }
}
