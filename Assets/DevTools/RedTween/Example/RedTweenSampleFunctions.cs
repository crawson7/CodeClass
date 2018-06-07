using UnityEngine;
using System.Collections;

public class RedTweenSampleFunctions : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	#region WAIT TESTS
	public void PlayComps()
	{
		Red.RedTween.Set set3 = Red.RedTween.CreateSet("test2", TestOnComplete);
		set3.Track(1).AddWait(2);
		set3.Track(1).AddWait(4);
		set3.Track(1).AddWait(6);
		set3.Play();
	}

	public void InstantPlay()
	{
		Red.RedTween.Wait(2, TestOnComplete);
	}

	public void ClipEndActionTest()
	{
		Red.RedTween.Set set3 = Red.RedTween.CreateSet("EndActionTest", TestOnComplete);
		set3.Track(1).AddWait(2, OnCompleteOne);
		set3.Track(1).AddWait(2, OnCompleteTwo);
		Red.RedTweenClip clip = set3.Track(1).AddWait(2);
		clip.SetStarAction(OnStartOne);
		clip.SetEndAction(OnCompleteThree);
		set3.Play();
	}

	public void ManyClipsTest()
	{
		Red.RedTween.Set set = Red.RedTween.CreateSet("MultipleTimeTest", TestOnComplete);
		for(int i=0; i<20; i++)
		{
			set.Track(1).AddWait(0.1f);
		}
		set.Play();
	}

	public void ClipRace()
	{
		Red.RedTween.Set set = Red.RedTween.CreateSet("MultipleTimeTest", OnCompleteOne);
		for(int i=0; i<100; i++)
		{
			set.Track(1).AddWait(0.05f);
		}
		set.Play();

		Red.RedTween.Wait(5, OnCompleteTwo);
	}
	#endregion

	#region MOVE TESTS
	public void InstantMoveToTest()
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.position = new Vector3(-2.5f, 0.0f, 0f);
		Red.RedTween.MoveTo(go, new Vector3(2.5f, 0.0f, 0f), 2.0f, Red.EaseType.EaseInOutQuad, ()=>{GameObject.Destroy(go);}); 
	}

	public void InstantMoveByTest()
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.position = new Vector3(-2.5f, 0.0f, 0f);
		Red.RedTween.MoveBy(go, new Vector3(5f, 0.0f, 0f), 2.0f, Red.EaseType.EaseInOutQuad, ()=>{GameObject.Destroy(go);}); 
	}

	public void MoveToTest()
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.position = new Vector3(-2.5f, -1.0f, 0f);
		
		Red.RedTween.Set moveSet = Red.RedTween.CreateSet("Move To Test", ()=>{GameObject.Destroy(go);});
		moveSet.Track(1).AddWait(2, OnCompleteOne);
		moveSet.Track(1).AddMoveTo(go, new Vector3(2.5f,1f,0), 2, Red.EaseType.EaseInOutBack, OnCompleteTwo);
		moveSet.Visualize(380);
		moveSet.Play();
	}

	public void MoveByTest()
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.position = new Vector3(-2.5f, 1.0f, 0f);

		Red.RedTween.Set moveSet = Red.RedTween.CreateSet("Move By Test", ()=>{GameObject.Destroy(go);});
		moveSet.Track(1).AddWait(2, OnCompleteOne);
		moveSet.Track(1).AddMoveBy(go, new Vector3(5f,-2f,0), 2, Red.EaseType.EaseInOutBack, OnCompleteTwo);
		moveSet.Visualize(380);
		moveSet.Play();
	}

	public void MoveToWithModifierTest()
	{
		// This tests some move to clips running in parallel with MoveToModefiers.
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.position = new Vector3(-2.5f, 0f, 0f);
		go.transform.localScale = Vector3.one * 0.25f;

		Red.RedTween.Set moveSet = Red.RedTween.CreateSet("test2", ()=>{GameObject.Destroy(go);});
		moveSet.Track(1).AddMoveTo(go, new Vector3(2.5f,0,0), 2, Red.EaseType.EaseInOutSine, OnCompleteOne);
		moveSet.Track(1).AddMoveTo(go, new Vector3(-2.5f,0,0), 2, Red.EaseType.EaseInOutSine, OnCompleteOne);
		moveSet.Track(2).AddMoveToModifier(go, new Vector3(0,1.5f,0), 1, Red.EaseType.EaseOutSine);
		moveSet.Track(2).AddMoveToModifier(go, new Vector3(0,-3f,0), 2, Red.EaseType.EaseInOutSine);
		moveSet.Track(2).AddMoveToModifier(go, new Vector3(0,1.5f,0), 1, Red.EaseType.EaseInSine);
		moveSet.Visualize(380);
		moveSet.Play();
	}

	public void LocalMoveToTest()
	{
		// This tests a basic local move by.
		GameObject go = CreateBox(new Vector3(0f, -1.5f, 0f), 0.25f);

		Red.RedTween.Set moveSet = Red.RedTween.CreateSet("LocalMoveByTest", ()=>{GameObject.Destroy(go);});
		moveSet.Track(1).AddLocalMoveTo(go, new Vector3(0, 1.5f, 0f), 1, Red.EaseType.EaseOutQuad);
		moveSet.Track(1).AddLocalMoveTo(go, new Vector3(0, -1.5f, 0f), 1, Red.EaseType.EaseInQuad);
		moveSet.Visualize(380);
		moveSet.Play();
	}

	public void LocalMoveByTest()
	{
		// This tests a basic local move by.
		GameObject go = CreateBox(new Vector3(0f, -1.5f, 0f), 0.25f);

		Red.RedTween.Set moveSet = Red.RedTween.CreateSet("LocalMoveByTest", ()=>{GameObject.Destroy(go);});
		moveSet.Track(1).AddLocalMoveBy(go, new Vector3(0, 3, 0f), 1, Red.EaseType.EaseOutQuad);
		moveSet.Track(1).AddLocalMoveBy(go, new Vector3(0, -3, 0f), 1, Red.EaseType.EaseInQuad);
		moveSet.Visualize(380);
		moveSet.Play();
	}

	public void LocalMoveByNestedTest()
	{
		GameObject go = CreateBox(new Vector3(-2.5f, 0.0f, 0f), 0.25f);
		GameObject go2 = CreateBall(new Vector3(-2.5f, 0.0f, 0f), 0.25f, go.transform);

		Red.RedTween.Set moveSet = Red.RedTween.CreateSet("LocalMoveByTest1", ()=>{GameObject.Destroy(go); GameObject.Destroy(go2);});
		moveSet.Track(1).AddMoveBy(go, new Vector3(5,0,0), 2, Red.EaseType.Linear);
		moveSet.Track(2).AddLocalMoveBy(go2, new Vector3(0, 1.5f, 0f), .5f, Red.EaseType.EaseOutQuad);
		moveSet.Track(2).AddLocalMoveBy(go2, new Vector3(0, -3f, 0f), 1f, Red.EaseType.EaseInOutQuad);
		moveSet.Track(2).AddLocalMoveBy(go2, new Vector3(0, 1.5f, 0f), .5f, Red.EaseType.EaseInQuad);
		moveSet.Visualize(380);
		moveSet.Play();
	}
	#endregion

	#region SCALE TESTS
	public void InstantScaleTest()
	{
		// Simple Scale
		GameObject go = CreateBox(new Vector3(0f, 0.0f, 0f), 0.25f);
		Red.RedTween.ScaleTo(go, Vector3.one*2, 1.5f, Red.EaseType.EaseInOutQuad, ()=>{GameObject.Destroy(go);});
	}

	public void SimpleScaleToTest()
	{
		// Simple Scale
		GameObject go = CreateBox(new Vector3(0f, 0.0f, 0f), 0.25f);

		Red.RedTween.Set scaleSet = Red.RedTween.CreateSet("Move By Test", ()=>{GameObject.Destroy(go);});
		scaleSet.Track(1).AddScaleTo(go, Vector3.one*2, 1f, Red.EaseType.EaseOutQuad);
		scaleSet.Track(1).AddScaleTo(go, Vector3.zero, 0.75f, Red.EaseType.EaseInQuad);
		scaleSet.Visualize(380);
		scaleSet.Play();
	}

	public void SimpleScaleFromTest()
	{
		// Implement
	}

	public void LoopingScaleTest()
	{
		// Scale in combination with a move tween
		GameObject go = CreateBox(new Vector3(-2.5f, 0.0f, 0f), 0.25f);
		Red.RedTween.Set set = Red.RedTween.CreateSet("ScaleTest 2", ()=>{GameObject.Destroy(go);});
		set.Track(1).AddScaleTo(go, 1, 0.75f, Red.EaseType.EaseOutSine);
		set.Track(1).AddScaleTo(go, 0.05f, 1.5f, Red.EaseType.EaseInOutSine);
		set.Track(1).AddScaleTo(go, 0.25f, 0.75f, Red.EaseType.EaseInSine);
		set.Track(2).AddMoveBy(go, new Vector3(5,0,0), 1.5f, Red.EaseType.EaseInOutQuad);
		set.Track(2).AddMoveBy(go, new Vector3(-5,0,0), 1.5f, Red.EaseType.EaseInOutQuad);
		set.Track(1).Loop(2);
		set.Track(2).Loop(2);
		set.Visualize(380);
		set.Play();
	}
	#endregion

	#region FUNCTION TESTS
	private GameObject mObject;
	public void InstanctFunctionTest()
	{
		mObject = CreateBox(new Vector3(-2.5f, 0.0f, 0f), 0.25f);
		Red.RedTween.StepFunction(FunctionTweenOne, 2.0f, Red.EaseType.EaseOutBounce, ()=>{GameObject.Destroy(mObject);});
	}

	public void NormalFunctionTest()
	{
		mObject = CreateBox(new Vector3(-2.5f, 0.0f, 0f), 0.25f);

		Red.RedTween.Set scaleSet = Red.RedTween.CreateSet("Step Function Test", ()=>{GameObject.Destroy(mObject);});
		scaleSet.Track(1).AddStepFunction(FunctionTweenOne, 2.0f, Red.EaseType.EaseInOutQuad);
		scaleSet.Visualize(380);
		scaleSet.Play();
	}

	public void FunctionTweenOne(float val)
	{
		float newX = Mathf.Lerp(-2.5f, 2.5f, val);
		mObject.transform.position = new Vector3(newX, 0, 0);
	}
	#endregion

	#region OBJECT CREATION HELPERS
	private GameObject CreateBox(Vector3 pos, float scale, Transform parent=null)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.position = pos;
		go.transform.localScale = Vector3.one * scale;
		if(parent!=null)
		{
			go.transform.SetParent(parent, true);
		}
		return go;
	}

	private GameObject CreateBall(Vector3 pos, float scale, Transform parent)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		go.transform.position = pos;
		go.transform.localScale = Vector3.one * scale;
		if(parent!=null)
		{
			go.transform.SetParent(parent, true);
		}
		return go;
	}
	#endregion

	#region ON COMPLETE TEST FUNCTIONS
	private void TestOnComplete()
	{
		Debug.Log("COMPLETE!!! - " + Time.time);
	}

	private void OnCompleteOne()
	{
		Debug.Log("COMPLETE ACTION 1 - " + Time.time);
	}

	private void OnCompleteTwo()
	{
		Debug.Log("COMPLETE ACTION 2 - " + Time.time);
	}

	private void OnCompleteThree()
	{
		Debug.Log("COMPLETE ACTION 3 - " + Time.time);
	}

	private void OnStartOne()
	{
		Debug.Log("START ACTION 1 - " + Time.time);
	}
	#endregion

	public void ViewComps()
	{
		Red.RedTween.PrintSets();
	}
}
