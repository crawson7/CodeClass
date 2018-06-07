using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Managers
{
	public static DialogManager DialogManager
	{get {return GameTools.ManagerDirector.Inst.GetManager<DialogManager>();}}

	public static InputManager InputManager
	{get {return GameTools.ManagerDirector.Inst.GetManager<InputManager>();}}
}
