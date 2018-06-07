using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;


public abstract class DialogController : MonoBehaviour 
{
	#region MEMBER VARIABLES
	protected	bool				_IsEntered = false;				// Specifies if the Dialog has finished entering.
	protected 	bool				_IsExited = false;				// Specifies if the dialog hs finished exiting.
	protected 	bool 				_TerminateOnExit = false;		// Specifies if the dialog will be destroyed after exiting.
	protected 	List<DialogModule> 	_Modules = new List<DialogModule>();					// List of all Modules attached to this Dialog.

	public		int					SortingOrder;					// Determines what dialog is put on top. Larger numbers show on top.
	[HideInInspector]
	public 		bool 				ExitActionIsEnter = false;
	[HideInInspector]
	public 		string 				Name;							// The Unique Name id of the dialog.
	[HideInInspector] 
	public 		List<DialogTransitionDelegate> 		ExitCompleteActions = new List<DialogTransitionDelegate>();		// Action that will run when the window finishes entering.
	[HideInInspector] 
	public	 	List<DialogTransitionDelegate> 		EnterCompleteActions = new List<DialogTransitionDelegate>();		// Action that will run when the window finishes exiting.
	[HideInInspector] 
	public 		bool 				IsEntering, IsExiting;			// The dialog is currently Entering/Exiting.
	[HideInInspector]
	public 		DialogController 	ModuleParent = null;			// The parent object for any modules
	public 		List<GameObject>	ModulePrefabs;
	#endregion

	#region PROPERTIES
	protected Vector3 				Position{ get{return gameObject.GetComponent<RectTransform>().anchoredPosition;}}
	public bool 					IsEntered{get{return _IsEntered;}}
	public bool 					IsExited{get{return _IsExited;}}
	public bool						IsActive{ get{return _IsEntered || IsEntering;}}
	public string 					Canvas {get; set;}
	#endregion

	#region VIRTUAL FUNCTIONS
	public virtual bool Initialize(string name)
	{		
		// Initialize is called as soon as the dialog prefab is instantiated.
		// Initialization happens before Enter is called.

		Name = name;
		return OnInitialize();
	}

	protected abstract bool OnInitialize();

	public virtual void Terminate()
	{
		// Terminate is called after a dialog has completed it's exit function on a Remove.
		// DialogMnaager.Instance.Hide will not call Terminate on a dialog.
		Managers.DialogManager.RemoveFromActive(Name);
		OnTerminate();
		for(int i=0; i>_Modules.Count; i++)
		{
			_Modules[i].TerminateBase();
		}
		_Modules.Clear();
		GameObject.Destroy(gameObject);
	}

	protected abstract void OnTerminate();
	
	public void BaseEnter()
	{
		// Begins to display the dialog. Any Implementation of Enter 
		// needs to call Entered() when it has finished entering.

		gameObject.SetActive(true);
		IsEntering = true;
		_IsExited = false;
		OnEnter();
	}

	protected abstract void OnEnter();
	
	protected virtual void Entered()
	{
		// This should be called once the dialog has completed its enter animations.
		// Entered will run the DialogEnteredDelegate.

		IsEntering = false;
		_IsEntered = true;
		if(EnterCompleteActions!=null)
		{
			for(int i = 0; i < EnterCompleteActions.Count; i++)
			{
				if(EnterCompleteActions[i] != null)
				{
					EnterCompleteActions[i](true);
				}
			}
			EnterCompleteActions.Clear();
		}
	}
	
	public void BaseExit(bool terminate)
	{
		// Begins to remove the dialog. Any implementation of Exit
		// needs to call Exited() once the dialog has finished being removed.

		_IsEntered = false;
		IsExiting = true;
		_TerminateOnExit = terminate;
		OnExit();
	}

	protected abstract void OnExit();
	
	protected virtual void Exited()
	{
		// Exited completed the exit process. This calls the on exit functions.
		// If the dialog was removed, this will also call terminate on the dialog.

		IsExiting = false;
		_IsExited = true;

		gameObject.SetActive(false);
		//Game.Inst.DialogManager.RemoveFromActive(Name);
		
		if(_TerminateOnExit)
		{
			Terminate();
		}

		if(ExitCompleteActions!=null)
		{
			for(int i = 0; i < ExitCompleteActions.Count; i++)
			{
				if(ExitCompleteActions[i] != null)
				{
					ExitCompleteActions[i](true);
				}
			}
			ExitCompleteActions.Clear();
		}
	}
	#endregion

	#region PUBLIC CONTROLLER MODIFICATIONS

	public T AddModule<T>(string name, GameObject panel, Vector3 pos = default(Vector3)) where T : DialogModule
	{
		T module = null;
		for(int i=0;i<ModulePrefabs.Count; i++)
		{
			if(name == ModulePrefabs[i].name)
			{
				GameObject go = GameObject.Instantiate(ModulePrefabs[i], panel.transform) as GameObject;
				module = go.GetComponent<T>();
				if(module == null)
				{
					Debug.LogError("Dialog Module: " + name + " does not have a ModuleController.");
					GameObject.Destroy(go);
					return null;
				}
				module.InitializeBase(name, this, pos);
				module.Rect.anchoredPosition3D = pos;
				_Modules.Add(module);
				return module;
			}
		}
		Debug.LogWarning("Dialog Module: " + name + " was not found.");
		return null;
	}

    public IEnumerator AddModuleAsync<T>(string name, GameObject panel, Vector2 pos = default(Vector2), Action<T> _finished = null) where T : DialogModule
    {
        yield return StartCoroutine(Managers.DialogManager.CreateModuleAsync<T>(name, module => 
        {
            if (module == null)
            {
                _finished(null);
            }
            else
            {
                module.gameObject.transform.SetParent(panel.transform, false);
                module.InitializeBase(name, this, pos);
                _Modules.Add(module);

                _finished(module);
            }
        }));
    }

    public void SetPosition(Vector2 pos)
	{
		RectTransform rect = this.GetComponent<RectTransform>();
		if(rect != null)
		{
			rect.anchoredPosition = pos;
		}
	}

	public void DestroyModule(DialogModule module)
	{
		for(int i=0;i<_Modules.Count; i++)
		{
			if(module == _Modules[i])
			{
				GameObject.Destroy(_Modules[i].gameObject);
				_Modules.RemoveAt(i);
				return;
			}
		}
	}
	#endregion
	
	#region COMMON ANIMATION HELPER FUNCTIONS
	public void MoveBy(GameObject panel, Vector3 amount)
	{
		// This will move a UI panel instantly by the specified amount.
		RectTransform rt = panel.GetComponent<RectTransform>();
		rt.anchoredPosition3D = rt.anchoredPosition3D + amount;
	}
	
	public Red.RedTween.Set SlideUp(GameObject go, float amount, float time,  Action endAnimationAction = null, float delay=0.0f, Red.EaseType easeType = Red.EaseType.EaseOutQuart)
	{
		if(go==null)
		{
			if(endAnimationAction!=null){endAnimationAction();}
			return null;
		}

		return Red.RedTween.UIMoveBy(go.GetComponent<RectTransform>(), new Vector3(0,amount,0), time, easeType, endAnimationAction, delay);
	}
	
	public Red.RedTween.Set SlideDown(GameObject go, float amount, float time, Action endAnimationAction = null, float delay=0.0f, Red.EaseType easeType = Red.EaseType.EaseOutQuad)
	{
		if(go==null)
		{
			if(endAnimationAction!=null){endAnimationAction();}
			return null;
		}

		return Red.RedTween.UIMoveBy(go.GetComponent<RectTransform>(), new Vector3(0, -amount, 0), time, easeType, endAnimationAction, delay);
	}

	public Red.RedTween.Set SlideLeft(GameObject go, float amount, float time, Action endAnimationAction = null, float delay=0.0f, Red.EaseType easeType = Red.EaseType.EaseOutQuart)
	{
		if(go==null)
		{
			if(endAnimationAction!=null){endAnimationAction();}
			return null;
		}

		return Red.RedTween.UIMoveBy(go.GetComponent<RectTransform>(), new Vector3(-amount, 0, 0), time, easeType, endAnimationAction, delay);
	}
	
	public Red.RedTween.Set SlideRight(GameObject go, float amount, float time, Action endAnimationAction = null, float delay=0.0f, Red.EaseType easeType = Red.EaseType.EaseOutQuart)
	{
		if(go==null)
		{
			if(endAnimationAction!=null){endAnimationAction();}
			return null;
		}

		return Red.RedTween.UIMoveBy(go.GetComponent<RectTransform>(), new Vector3(amount, 0, 0), time, easeType, endAnimationAction, delay);
	}
	
	public Red.RedTween.Set Cover(GameObject go, float from, float to, float time, Action endAnimationAction = null, float delay = 0, Red.EaseType easeType = Red.EaseType.EaseOutQuad)
	{
		// This will fade both images and text 
		if(go==null)
		{
			if(endAnimationAction!=null){endAnimationAction();}
			return null;
		}

		return Red.RedTween.UIFadeTo(go.GetComponent<Graphic>(), to, time, easeType, endAnimationAction, delay);
	}

	public Red.RedTween.Set FadeContents(GameObject go, float from, float to, float time, Action endAnimationAction = null, float delay = 0, Red.EaseType easeType = Red.EaseType.EaseOutQuart)
	{
		// This will fade both images and text 
		if(go==null)
		{
			if(endAnimationAction!=null){endAnimationAction();}
			return null;
		}

		return Red.RedTween.UIFadeTo(go.GetComponentsInChildren<Graphic>(), to, time, easeType, endAnimationAction, delay);
	}

	public void AlphaContentsInstant(GameObject go, float alpha)
	{
		// This will fade both images and text 
		Image[] images = go.GetComponentsInChildren<Image>();
		Text[] texts = go.GetComponentsInChildren<Text>();
		for(int i=0; i<images.Length; i++)
		{
			Color c = images[i].color;
			images[i].color = new Color(c.r, c.g, c.b, alpha);
		}

		for(int i=0; i<texts.Length; i++)
		{
			Color c = texts[i].color;
			texts[i].color = new Color(c.r, c.g, c.b, alpha);
		}
	}
	#endregion
}

#region BASIC IMPLEMENTATION EXAMPLE
/// Dialog Controller Base Setup
/// Copy this when creating new Dialog Controllers
/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class <DIALOG NAME> : DialogController 
{
	public override void Enter ()
	{
		base.Enter ();
		
		Entered();
	}
	
	public override void Exit(bool terminate)
	{
		base.Exit(terminate);
		
		Exited();
	}
}
*/
#endregion
