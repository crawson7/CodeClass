using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputInterfaces;

public class Game : GameTools.ManagerBase, IKeyListener 
{
	public StartScript StartCode;

	#region implemented abstract members of ManagerBase
	public override IEnumerator RunInitialization ()
	{
		Debug.Log("Initializing Game");
		InitState = GameTools.InitState.Initialized;
		yield return null;
	}
	public override IEnumerator RunSetup ()
	{
		InitState = GameTools.InitState.Complete;
		yield return null;
	}

	public override void OnFullyInitialized ()
	{
		GameTools.ManagerDirector.Inst.GetManager<GameTools.KeystrokeManager>().AddKeyListener(OnKeyDown);
		StartGame();
		Debug.Log("Game Fully Initialized");
	}
	#endregion

	private void StartGame()
	{
		Debug.Log("Starting");
		StartCoroutine(GameTools.ManagerDirector.Inst.GetManager<DialogManager>().ShowAsync<Console_Ctrl>("Console", "Standard", null, null, (ctrl)=>{
			Console.Initialize(ctrl);
			Red.RedTween.Wait(1.0f,()=>{
			if(StartCode != null)
			{
				StartCode.Run();
				}});
		}));
	}

	public void OnKeyDown(Keystroke key)
	{
	}

	public void UseInput(string s)
	{
		Debug.Log(s);
	}
}
