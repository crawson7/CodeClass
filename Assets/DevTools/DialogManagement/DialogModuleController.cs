using UnityEngine;
using System.Collections;


public abstract class DialogModule : MonoBehaviour 
{
	protected DialogController Parent;
	public RectTransform Rect;
	protected string Name;

	public void InitializeBase(string name, DialogController parent, Vector2 pos)
	{
		Parent = parent;
		Name = name;
		gameObject.name = name;

		// Set the position of the module.
		Rect = this.GetComponent<RectTransform>();
		if(Rect != null)
		{
			Rect.anchoredPosition = pos;
		}

		Initialize();
	}

	public void TerminateBase()
	{
		Terminate();
		Object.Destroy(this.gameObject);
	}

	protected abstract void Initialize();
	public abstract void Terminate();
}

// Implementation skeleton
/*
public class <MODULE> : DialogModule
{
	public override void Initialize(string name, DialogController parent, Vector2 pos)
	{
		base.Initialize(name, parent, pos);
	}

	public override void Terminate()
	{}
} 
 * */
