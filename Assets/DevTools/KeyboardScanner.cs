using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputInterfaces;

public class KeyboardScanner : GameTools.ManagerBase, IKeyListener 
{
	private bool _active = false;
	private bool _scanComplete;
	private System.Text.StringBuilder _scan;
	private GameTools.KeystrokeManager _manager;

	#region implemented abstract members of ManagerBase
	public override IEnumerator RunInitialization ()
	{
		Debug.Log("Initializing Keyboard Scanner");
		InitState = GameTools.InitState.Initialized;
		yield return null;
	}
	public override IEnumerator RunSetup ()
	{
		Debug.Log("Initializing Keyboard Scanner");
		InitState = GameTools.InitState.Complete;
		yield return null;
	}

	public override void OnFullyInitialized ()
	{
		_manager = GameTools.ManagerDirector.Inst.GetManager<GameTools.KeystrokeManager>();
		GameTools.ManagerDirector.Inst.GetManager<GameTools.KeystrokeManager>().AddKeyListener(OnKeyDown);
		_scan = new System.Text.StringBuilder();
	}
	#endregion

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public string GetInput(System.Action<string> endAction)
	{
		_scan = new System.Text.StringBuilder();
		_active = true;
		_scanComplete = false;
		StartCoroutine(ScanKeys(endAction));

		return "";
	}

	public IEnumerator ScanKeys(System.Action<string> endAction)
	{
		yield return new WaitUntil(() => _scanComplete);
		if(endAction != null)
		{
			Debug.Log(_scan);
			endAction(_scan.ToString());
		}
	}

	public void OnKeyDown(Keystroke key)
	{
		if(!_active){return;}


		if(key == Keystroke.Return)
		{
			_scanComplete = true;
			_active = false;
			return;
		}

		string s = (_manager.Upper)? key.ToString() : key.ToString().ToLower();
		if(key == Keystroke.Space)
		{
			s = " ";
		}
		else if(key == Keystroke.BackSpace)
		{
			_scan.Remove(_scan.Length -1, 1);
			Console.UpdateInput(_scan.ToString());
			return;
		}
		_scan.Append(s);
		Console.UpdateInput(_scan.ToString());
	}
}
