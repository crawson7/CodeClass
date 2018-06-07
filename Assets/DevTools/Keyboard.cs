using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Keyboard 
{

	public static void GetInput(System.Action<string> endAction)
	{
		GameTools.ManagerDirector.Inst.GetManager<KeyboardScanner>().GetInput(endAction);
		//Debug.Log("");
	}
}
