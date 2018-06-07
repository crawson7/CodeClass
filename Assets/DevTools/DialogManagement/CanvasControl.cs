using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasControl : MonoBehaviour 
{
	// Defines How UI elements in this canvas will behave
	public Canvas MyCanvas;
	public bool UseDialogSortOrder;

	public Dictionary<string, DialogController> Dialogs = new Dictionary<string, DialogController>();

	public CanvasScaler Scalar
	{
		get
		{
			CanvasScaler s = gameObject.GetComponentInChildren<CanvasScaler>();
			return s;
		}
	}
}
