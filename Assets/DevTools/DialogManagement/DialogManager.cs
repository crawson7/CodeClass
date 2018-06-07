using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public delegate void DialogTransitionDelegate(bool success);

public class DialogManager : GameTools.ManagerBase
{
	#region MEMBER VARIABLES
	public const string kDialogFolder = "UI/Dialogs";		// The folder in Resources where all Dialog Controller prefabs are found.
	public const string kModulesFolder = "UI/Modules";		// The folder in Resources where all DialogModule prefabs are stored.
	public List<string> CanvasTags = new List<string>(); 	// An ordererd list of the UI canvases. Canvas order defines draw order.
    public DialogController LoadingScreen;

    [SerializeField] GameObject UISystemPrefab;             // GameObject to instantiate the UI System From

    private UISystem UI;
	private Dictionary<string, CanvasControl> _canvases {get{return UI.UICanvas;}}							// References to all of the assigned canvases by type.
	private Dictionary<string, GameObject> _loadedDialogs = new Dictionary<string, GameObject>();			// The List of the dialog prefabs that are loaded in memory.
	private Dictionary<string, GameObject> _loadedModules = new Dictionary<string, GameObject>();			// All of the module prefabs that are loaded in memory.
	private Dictionary<string, string> _dialogs = new Dictionary<string, string>();							// A pairing of Instantiated dialogs to thier canvases.
	#endregion

	#region INITIALIZATION & LOADING
	public override IEnumerator RunInitialization ()
	{
		Debug.Log("Initializing dialog manager");
		InitState = GameTools.InitState.Initialized;

        if(LoadingScreen != null)
            _loadedDialogs.Add(LoadingScreen.gameObject.name, LoadingScreen.gameObject);

        yield return null;
	}

	public override IEnumerator RunSetup ()
	{
		Debug.Log("Setting Up Dialog Manager");

        if(CreateUISystem())
            InitState = GameTools.InitState.Complete;
        else
            InitState = GameTools.InitState.Failed;

        yield break;
    }

    public override IEnumerator Reset(Action<string> _finished = null)
    {
        _loadedModules = new Dictionary<string, GameObject>();

        // Remove all dialogs that are not Loading
        List<string> dialogsToKill = new List<string>();
        foreach(string dialog in _dialogs.Keys)
        {
            if(dialog != LoadingScreen.name)
            {
                dialogsToKill.Add(dialog);
            }
        }

        foreach(string dialog in dialogsToKill)
        {
            Remove(dialog);
        }
     
        yield break;
    }

    private bool CreateUISystem()
    {
        ClearUISystem();
        GameObject uiObj = GameObject.Instantiate(UISystemPrefab, Vector3.zero, Quaternion.identity);
        uiObj.name = "UI";
        UI = uiObj.GetComponent<UISystem>();
        if (UI == null)
        {
            Debug.LogError("DialogManager Failed to load UI System Component");
            return false;
        }
        DontDestroyOnLoad(uiObj);

        UI.CreateCanvases();
        return true;
    }


	private void ClearUISystem()
	{
		UI = null;
		for(int s=0; s<UnityEngine.SceneManagement.SceneManager.sceneCount; s++)
		{
			List<GameObject> rootObjects = new List<GameObject>();
			UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(s);
			scene.GetRootGameObjects( rootObjects );

			// iterate root objects and do something
			for (int i = 0; i < rootObjects.Count; ++i)
			{
				UISystem uiSys = rootObjects[i].GetComponent<UISystem>();
				if(uiSys != null)
				{
					GameObject.Destroy(uiSys.gameObject);
				}
			}
		}
	}

	#endregion

	#region PRIVATE HELPERS
	private void SortCanvas(string canvas, DialogController dc)
	{
		// Sorts all of the UIs in this canvas.
		if(!_canvases[canvas].UseDialogSortOrder)
		{
			// Put this dialog controller at the top
			_canvases[canvas].Dialogs[dc.Name].gameObject.transform.SetAsLastSibling();
			return;
		}

		// Sort all dialogs accordint to thier sort order.
		List<DialogController> sorted = new List<DialogController>();
		foreach( DialogController dialog in _canvases[canvas].Dialogs.Values)
		{
			int insert = 0;
			for(int i=0; i<sorted.Count; i++)
			{
				if(dialog.SortingOrder >= sorted[i].SortingOrder){
					insert = i+1;
					continue;
				}
			}

			if(insert>=sorted.Count)
			{
				sorted.Add(dialog);
			}
			else
			{
				sorted.Insert(insert, dialog);
			}
		}

		for(int i=0; i<sorted.Count; i++)
		{
			sorted[i].gameObject.transform.SetAsLastSibling();
		}
	}

	private void ActivateDialog(string canvas, DialogController dialog)
	{
		dialog.gameObject.SetActive(true);
		_dialogs.Add(dialog.Name, canvas);
		_canvases[canvas].Dialogs[dialog.Name] = dialog;
		dialog.transform.SetParent(_canvases[canvas].MyCanvas.transform, false);
		dialog.gameObject.name = dialog.Name;
		dialog.Canvas = canvas;

		SortCanvas(canvas, dialog);
	}

	public void RemoveFromActive(string name)
	{
		if(_dialogs.ContainsKey(name))
		{
			if(_canvases[_dialogs[name]].Dialogs.ContainsKey(name))
			{
				_canvases[_dialogs[name]].Dialogs.Remove(name);
			}
			_dialogs.Remove(name);
		}
	}

	private T ShowInternal<T>(string name, string canvas, DialogTransitionDelegate onComplete, Action<T> setup=null) where T : DialogController
	{
		if(_dialogs.ContainsKey(name))
		{
			// This dialog is already active.
			DialogController dialog = _canvases[_dialogs[name]].Dialogs[name];
			if(dialog.IsExiting)
			{
				// The dialog is on it's way out. As soon as it has completed exiting, show the dialog.
				Debug.Log("Cannot show the active dialog " + name + " yet, Exited() has not been called!");
				dialog.ExitCompleteActions.Add( (bool s) => 
				{
					dialog.IsExiting = false; 
					ShowInternal<DialogController>(name, canvas, onComplete);
				});
				dialog.ExitActionIsEnter = true;
				return (T)dialog;
			}
			if(dialog.IsEntering)
			{
				Debug.Log("This dialog is already on it's way in. Do not need to call Enter again.");
				dialog.EnterCompleteActions.Add(onComplete);
				return (T)dialog;

			}
			if(dialog.IsEntered)
			{
				Debug.Log("This dialog has already entered. Invoking onComplete immediately.");
				if(onComplete != null)
				{
					onComplete(true);
				}
				return (T)dialog;
			}
			dialog.EnterCompleteActions.Add(onComplete);
			dialog.BaseEnter();
			return (T)_canvases[_dialogs[name]].Dialogs[name];
		}

		// Make sure the canvas target exists.
		if(!_canvases.ContainsKey(canvas))
		{
			Debug.LogError("There is no canvas: " + canvas + " for this Dialog: " + name);
			return null;
		}

		// Instantiate the dialog Prefab.
		GameObject go = GameObject.Instantiate(_loadedDialogs[name]) as GameObject;
		DialogController dc = go.GetComponent<DialogController>();
		if(dc==null)
		{
			Debug.LogWarning("The Dialog " + name + " does not have a controller."); 
			return null;
		}
		dc.Name = name;
		ActivateDialog(canvas, dc);

		// Do the dialog Setup before initialization.
		if(setup!=null)
		{
			setup((T)dc);
		}

		// Initialize the dialog and call its Enter Function.
		dc.Initialize(name);
		dc.EnterCompleteActions.Add(onComplete);
		dc.BaseEnter();
		return (T)dc;
	}

	private void HandleHideFail(string name, DialogTransitionDelegate onComplete)
	{
		Debug.LogWarning("The Dialog" + name + " does not exist.");
		RemoveFromActive(name);
		if(onComplete != null)
		{
			onComplete(false);
		}
	}

	private void HideInternal(string name, DialogTransitionDelegate onComplete, bool Remove)
	{
        //Debug.Log("==>  Hide" + name + "  " + Time.time);
        if (!_dialogs.ContainsKey(name) || !_canvases[_dialogs[name]].Dialogs.ContainsKey(name))
		{
			HandleHideFail(name, onComplete);
			return;
		}

		DialogController dc = null;
		_canvases[_dialogs[name]].Dialogs.TryGetValue(name, out dc);
		if(dc == null)
		{
			HandleHideFail(name, onComplete);
			return;
		}

		if(dc.IsEntering) // If in the process of entering, Hide when done.
		{
			Debug.LogWarning("Cannot hide " + name + " yet, Entered() has not been called!");
			dc.EnterCompleteActions.Add((bool s) => 
				{
					dc.IsEntering = false; 
					Hide(name, onComplete);
				});
			return;
		}
		if(dc.IsExiting)
		{
			Debug.LogWarning("Dialog " + name + " is already exiting");
			dc.ExitCompleteActions.Add(onComplete);
			return;
		}
		if(dc.IsExited)
		{
			Debug.Log("Dialog " + name + " is already inactive");
			if(Remove)
			{                
				dc.Terminate();                
			}
			if(onComplete != null)
			{
				onComplete(false);
			}
			return;
		}

        if (Remove)
        {
            //Managers.AssetBundleHelperManager.UnloadAssetFromMap<GameObject>(name);
            _loadedDialogs.Remove(name);
        }

        dc.ExitCompleteActions.Add(onComplete);
		dc.BaseExit(Remove);
	}
    #endregion
	 
	private IEnumerator LoadAsset(string name, System.Action<GameObject> endAction)
	{
		GameObject obj = Resources.Load<GameObject>("Dialogs/" + name) as GameObject;
		yield return null;
		endAction(obj);
	}

    #region PUBLIC SHOW / HIDE DIALOGS
    public IEnumerator ShowAsync<T>(string name, string canvas="", Action<T> setup=null, DialogTransitionDelegate onEntered = null, Action<T> onLoaded=null) where T : DialogController
    {
        // Shows the specified dialog. before the dialog runs its enter function, the Setup function will be called, allowing for unique situational setup.
        // if the specified dialog does not exist, this will return null.

        //float startTime = Time.time;
        //Debug.Log("==> " + name + " START - <" + startTime + " , " + Time.realtimeSinceStartup + ">");

        // Check to see if this dialog needs to be loaded from Resources.
        T dialog = null;
        if (!_loadedDialogs.ContainsKey(name))
        {
            bool wasLoaded = false;
            //yield return StartCoroutine(Managers.AssetBundleHelperManager.LoadAssetAsyncFromMap<GameObject>(name, false, result =>
			yield return StartCoroutine(LoadAsset(name, result =>
            {
                if (result != null)
                {
                    wasLoaded = true;
                    _loadedDialogs.Add(result.name, result);
                }
            }));

            // We need to load dialog first
            if (!wasLoaded)
            {
                Debug.LogError("Could not Load Dialog: " + name);
				if(onEntered != null)
				{
                	onEntered(false);
				}
                yield break;
            }
        }
        dialog = _loadedDialogs[name].GetComponent<T>();
        if (dialog == null)
        {
            Debug.LogError("The Dialog " + name + " does not have a DialogController Component. Type" + _loadedDialogs[name].GetType().ToString());
            onEntered(false);
            yield break;
        }

        // Show the specified dialog by name. This returns the specified dialog controller type.
        if (string.IsNullOrEmpty(canvas))
        {
            // Assign to the default canvas unless this dialog already exists.
            canvas = CanvasTags[0];
            if (_canvases.ContainsKey(name))
            {
                canvas = _dialogs[name];
            }
        }
        
        // Run the special initialization on the dialog. Then Initialize.
        T  t = ShowInternal<T>(name, canvas, onEntered, setup);
        
        //Debug.Log("==> " + name + " END - <" + Time.time + " , " + (Time.time - startTime) + " , " + Time.realtimeSinceStartup + ">");

        if (onLoaded != null)
            onLoaded(t);
    }

    public void Hide(string name, DialogTransitionDelegate onComplete = null)
	{
		if(!_dialogs.ContainsKey(name))
		{
			Debug.LogWarning("The Dialog " + name + " is not currently active.");
			if(onComplete != null)
			{
				onComplete(true);
			}
			return;
		}

		HideInternal(name, onComplete, false);
	}
		
	public void Remove(string name, DialogTransitionDelegate onComplete = null)
	{
		if(!_loadedDialogs.ContainsKey(name))
		{
			Debug.LogWarning("The Dialog " + name + " has yet to be loaded."); 
			if(onComplete != null)
			{
				onComplete(true);
			}
			return;
		}

		HideInternal(name, onComplete, true);
	}
	#endregion

	public void ClearCacheOfType(string type)
	{
		// TODO: Impelement
	}

	public void ClearAllCache()
	{
		// TODO: Impelement
	}

	#region MODULES

    public IEnumerator CreateModuleAsync<T>(string _name, Action<T> _finished=null) where T : DialogModule
    {
        // Do we need to load the dialog
        if (!_loadedModules.ContainsKey(_name))
        {
            //yield return StartCoroutine(Managers.AssetBundleHelperManager.LoadAssetAsyncFromMap<GameObject>(_name, false, result =>
			yield return StartCoroutine(LoadAsset(name, result =>
            {
                if (result != null)
                {
                    _loadedModules.Add(result.name, result);
                }
                else
                {
                    Debug.LogError("DialogManager::CreateModuleAsync - ERROR: Could not load " + _name + " is it in a correct bundle?");                    
                }
            }));
        }

        DialogModule moduleTemplate = _loadedModules[_name].GetComponent<DialogModule>();
        if (moduleTemplate == null)
        {
            Debug.LogWarning("The Module " + _name + " does not have a DialogModule Component.");
            _finished(null);
            yield break;
        }

        GameObject go = GameObject.Instantiate(_loadedModules[_name]) as GameObject;
        if (go == null)
        {
            Debug.LogWarning("Warning: cound not instantiate Module: " + _name);
            _finished(null);
            yield break;
        }

        go.name = name;
        T module = go.GetComponent<T>();
        if (module == null)
        {
            Debug.LogWarning("The Dialog " + _name + " does not have a controller.");
            _finished(null);
            yield break;
        }
        _finished(go.GetComponent<T>());
    }
	#endregion

	#region PUBLIC GETTERS
	public Canvas GetCanvasType(string type)
	{
		if(_canvases.ContainsKey(type))
		{
			return _canvases[type].MyCanvas;
		}
		return null;
	}

	public CanvasControl GetCanvasControl(string type)
	{
		if(_canvases.ContainsKey(type))
		{
			return _canvases[type];
		}
		return null;
	}
	
	public bool IsActiveUI(string name)
	{
		if(!_dialogs.ContainsKey(name))
		{
			return false;
		}

		DialogController dc = null;
		_canvases[_dialogs[name]].Dialogs.TryGetValue(name, out dc);
		if(dc==null || !dc.IsActive)
		{
			return false;
		}
			
		return true;
	}
	
	public DialogController GetDialog(string name)
	{
		if(!_dialogs.ContainsKey(name))
		{
			return null;
		}

		DialogController dc = null;
		_canvases[_dialogs[name]].Dialogs.TryGetValue(name, out dc);
		return dc;
	}
	
	public T GetDialog<T>(string name) where T : DialogController
	{
		if(!_dialogs.ContainsKey(name))
		{
			Debug.LogWarning("Cound not get the Dialog: " + name + ". It may not be active.");
			return null;
		}

		if(!_canvases[_dialogs[name]].Dialogs.ContainsKey(name))
		{
			return null;
		}

		return (T)_canvases[_dialogs[name]].Dialogs[name];
	}

	public float GetCanvasScale(string type)
	{
		if(!_canvases.ContainsKey(type) || _canvases[type].Scalar == null)
		{
			return 1f;
		}

		float height = Screen.height;
		float scalarHeight = _canvases[type].Scalar.referenceResolution.y;
		float scale = scalarHeight / height;
		return scale;
	}

	public Vector2 GetCanvasSizeDelta(string type)
	{
		if(!_canvases.ContainsKey(type))
		{
			return Vector2.one;
		}
		return _canvases[type].MyCanvas.GetComponent<RectTransform>().sizeDelta;
	}
	#endregion

	#region PUBLIC DIALOG MANIPULATIONS
	public void MoveToFront(string name)
	{
		if(_dialogs.ContainsKey(name) && _canvases[_dialogs[name]].Dialogs.ContainsKey(name))
		{
			_canvases[_dialogs[name]].Dialogs[name].gameObject.transform.SetAsLastSibling();
		}
		else
		{
			Debug.LogWarning("Dialog " + name + " not found.");
		}
	}
	
	public void MoveToBack(string name)
	{
		if(_dialogs.ContainsKey(name) && _canvases[_dialogs[name]].Dialogs.ContainsKey(name))
		{
			_canvases[_dialogs[name]].Dialogs[name].gameObject.transform.SetAsFirstSibling();
		}
		else
		{
			Debug.LogWarning("Dialog " + name + " not found.");
		}
	}

	public void DisableCanvas(string type, float fade)
	{
		if(_canvases.ContainsKey(type))
		{
			CanvasGroup group = _canvases[type].MyCanvas.GetComponent<CanvasGroup>();
			if(group==null){return;}

			group.interactable = false;
			if(!Mathf.Approximately(fade, 1.0f))
			{
				// Set the alpha also
				group.alpha = fade;
			}
		}
	}

	public void EnableCanvas(string type)
	{
		if(_canvases.ContainsKey(type))
		{
			CanvasGroup group = _canvases[type].MyCanvas.GetComponent<CanvasGroup>();
			if(group==null){return;}

			group.interactable = false;
			group.alpha = 1.0f;

		}
	}
	#endregion

	#region DEBUG
	public void PrintDialogs()
	{
		string s = " --------------- Loaded Dialogs ------------------- \n";
		foreach(GameObject go in _loadedDialogs.Values)
		{
			s += "     "  + go.name + "\n";
		}

		Debug.Log(s);
	}
	#endregion
}
