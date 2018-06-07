using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console_Ctrl : DialogController
{
	public Transform LogContainer;
	private List<LogModule_Ctrl> _logMods;
	public Text Input;

	#region implemented abstract members of DialogController
	protected override bool OnInitialize ()
	{
		_logMods = new List<LogModule_Ctrl>();
		for(int i=0;i<22;i++)
		{
			LogModule_Ctrl mod = AddModule<LogModule_Ctrl>("LogModule", LogContainer.gameObject, new Vector3(0,-60 * i, 0) );
			mod.Log.text = "";
			_logMods.Add(mod);
		}
		return true;
	}
	protected override void OnTerminate ()
	{
		
	}
	protected override void OnEnter ()
	{
		Entered();
	}
	protected override void OnExit ()
	{
		Exited();
	}
	#endregion

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateView()
	{
		//Test.text = Console.Logs.Last().Message;
		int last = Console.Logs.Size;
		if(last>_logMods.Count)
		{
			last = _logMods.Count;
		}

		for(int i=0;i<_logMods.Count; i++)
		{
			if(last<i)
			{
				_logMods[i].Log.text = "";
				continue;
			}
			else
			{
				ConsoleLog log = Console.Logs.ValueAt((last-1) - i);
				if(log != null)
				{
					_logMods[i].Log.text = log.Message;
				}
				else
				{
					_logMods[i].Log.text = "";
				}
			}
		}
		Input.text = "";
	}

	public void UpdateInput(string s)
	{
		Input.text = s;
	}
}
