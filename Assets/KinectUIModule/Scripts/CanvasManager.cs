using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{

    public CanvasGroup[] CanvasGroupArray;

    private CanvasGroup _canvasGroupCurrent;
    private CanvasGroup _canvasGroupNext;
    private Canvas _canvasCurrent;

    public GameObject GestureManager;

    // Use this for initialization
	void Start ()
	{
	    _canvasGroupCurrent = CanvasGroupArray[0];
	    _canvasGroupNext = CanvasGroupArray[System.Array.IndexOf(CanvasGroupArray, _canvasGroupCurrent) + 1];
	}
	
    public void NextCanvas()
    {
        _canvasGroupCurrent.gameObject.SetActive(false);
        _canvasGroupCurrent.alpha = 0.0f;
        _canvasGroupCurrent = CanvasGroupArray[System.Array.IndexOf(CanvasGroupArray, _canvasGroupCurrent) + 1];
        _canvasGroupCurrent.gameObject.SetActive(true);
        _canvasGroupCurrent.alpha = 1.0f;

        // Debug.Log("Canvasname: " + _canvasGroupCurrent.name);
        // If current canvas is Canvas Exercise --> activate GestureManager, else nextchild is next canvas in array
        if (_canvasGroupCurrent.name == "Canvas Exercise")
        {    Debug.Log("If condition");
            // activate GestureManager
            if (!GestureManager.activeSelf)
            {
                GestureManager.SetActive(true);
            }
        }
        if (_canvasGroupCurrent.name == "Canvas Finished")
        {
            DontDestroyOnLoad(GameObject.FindGameObjectWithTag("Player"));
            StartCoroutine(WaitForUserRead(3.0f));
        }
        else
        {
//            nextChild = canvasChilds[System.Array.IndexOf(canvasChilds, child)+1];
        }
  //      Debug.Log(currentChild + " " + nextChild);
    }

    IEnumerator WaitForUserRead(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("TestScene2", LoadSceneMode.Single);
    }
}
