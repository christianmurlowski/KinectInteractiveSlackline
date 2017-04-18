using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{

    private Button _button;

    public GameObject CanvasManager;
    private CanvasManager _canvasManager;

	// Use this for initialization
	void Start ()
	{
        Debug.Log("btn start");
	    _button = GetComponent<Button>();

	    _canvasManager = CanvasManager.GetComponent<CanvasManager>();

	    _button.onClick.AddListener(() =>
	    {
        Debug.Log("btn click");
	        _canvasManager.NextCanvas();
	    });
	}


}
