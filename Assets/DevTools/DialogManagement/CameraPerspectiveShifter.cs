using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPerspectiveShifter : MonoBehaviour 
{
	public Camera Cam;
	public float Aspect3x4;
	public float Aspect16x9;
	public float MoreThan16x9;

	// Use this for initialization
	void Start () 
	{
		float aspect = 1f;
		if(Screen.width > Screen.height)
		{
			aspect = (float)Screen.width/(float)Screen.height;
		}
		else
		{
			aspect = (float)Screen.height/(float)Screen.width;
		}

		if(aspect > 1.8f)		// Wider than 16:9
		{
			Cam.fieldOfView = MoreThan16x9;
		}
		else if(aspect > 1.35) 	// Wider than 3:4 up to 16:9 
		{
			Cam.fieldOfView = Aspect16x9; 
		}
		else 					// 3:4 and below
		{
			Cam.fieldOfView = Aspect3x4;
		}
	}
}
