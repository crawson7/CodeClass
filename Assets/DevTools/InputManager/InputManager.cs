using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using InputInterfaces;
using InputClasses;

/// <summary>
/// Input manager. Controls touching the screen and binding functions to gesture actions
//---- Delegates for listeners
//----------------------------
public delegate void InputPosition(Vector3 position);
public delegate void InputPositionDelta(Vector3 posiiton, Vector3 delta);
public delegate void InputDoubleTouch(Vector3 firstPosition, Vector3 secondPosition);
public delegate void InputSwipe(Vector3 start,Vector3 end,float time);

/// </summary>
public class InputManager : GameTools.ManagerBase, IDownListener, IUpdateListener, IUpListener, ITapListener, IDragListener, ISwipeListener, IPinchListener, IRotateListener
{

    //---- Flags
    //----------
    public enum TouchTypes : int
    {
        NONE =      0x0000,
        DOWN =      0x0001,
        TAP =       0x0002,
        DRAG =      0x0004,
        PINCH =     0x0008,
        ROTATE =    0x0010,
        SWIPE =     0x0020,
        UP =        0x0040,
        UPDATE =    0x0080,
        DOUBLETAP = 0x0100,
        ALL =       DOWN | TAP | DRAG | PINCH | ROTATE | SWIPE | UP | UPDATE | DOUBLETAP
    }

    //---- Variables
    //--------------
    [Header("Constraints")]
    public float TapTimeDelta;
    public float TapDistanceTolerance;
    public float DoubleTapTimeDelta;
    public float DragDeltaThreshold;
    public float SwipeTimeDelta;
    public float SwipeDistDelta;
    public float PinchDeltaThreshold; // this should be a percent to be even on all devices
    public float RotationDeltaThreshold;
    public bool Active;

    private InputPosition _downListeners;
    private InputPosition _tapListeners;
    private InputPosition _doubleTapListeners;
    private InputPosition _upListeners;
    private InputPosition _updateListeners;

    private InputPosition _dragStartListeners;
    private InputPositionDelta _dragUpdateListeners;
    private InputPosition _dragEndListeners;

    private InputSwipe _swipeListeners;

    private InputDoubleTouch _pinchStartListeners;
    private InputDoubleTouch _pinchUpdateListeners;
    private InputDoubleTouch _pinchEndListeners;

    private InputDoubleTouch _rotateStartListeners;
    private InputDoubleTouch _rotateUpdateListeners;
    private InputDoubleTouch _rotateEndListeners;

    private Pointer _firstPointer;
    private Pointer _secondPointer;
    private int _activeTouchTypes;
    private bool _touchStart;

	private bool _multiTouchActive = true;

	#region implemented abstract members of ManagerBase

	public override IEnumerator RunInitialization ()
	{
		// Run Initialization functions.
		Debug.Log("Initializing Input manager");
		SetActiveTouchTypes((int)TouchTypes.ALL);
		InitState = GameTools.InitState.Initialized;
		yield return null;
	}

	public override IEnumerator RunSetup ()
	{
		// Run Initialization functions.
		Debug.Log("Setting Up Input manager");
		InitState = GameTools.InitState.Complete;
		yield return null;
	}
	#endregion

    //---- Unity
    //----------
    void Awake()
    {
        _firstPointer = new Pointer();
        _secondPointer = new Pointer();
    }

    void Update()
    {
        // Android back press
        if(Input.GetKeyDown(KeyCode.Escape))
        {
			// TODO: Implement this...
            //MessageSignal.Instance.Send("BackPressed");
        }

		if (!Active || InitState != GameTools.InitState.Initialized)
		{
            return;
		}

        // Input types
        #if UNITY_EDITOR
        MouseInput();
        #else
        TouchInput();
        #endif
    }

    public void SetActiveTouchTypes(int types)
    {
        _activeTouchTypes = types;
    }

	public void SetMultiTouchActive(bool active)
	{
		// If set false, all multi-touch input will be treated as single touches (from the first finger)
		_multiTouchActive = active;

		// Do something about in progress multi-touches.
		if(_firstPointer.Down && _secondPointer.Down)
		{
			HandleMultInputEnd(_firstPointer.Position, _secondPointer.Position);
		}
	}

    //---- Interfaces
    //---------------
    #region Down
    public void AddDownListener(InputPosition toAdd)
    {
        _downListeners += toAdd;
    }

    public void RemoveDownListener(InputPosition toRemove)
    {
        if (_downListeners == null)
		{
            return;
		}
        _downListeners -= toRemove;
    }

    public void OnDown(Vector3 position)
    {
        if (_downListeners == null || ((_activeTouchTypes & (int)TouchTypes.DOWN) != (int)TouchTypes.DOWN))
		{
            return;
		}
        _downListeners(position);
    }
    #endregion

    #region Update
    public void AddUpdateListener(InputPosition toAdd)
    {
        _updateListeners += toAdd;
    }

    public void RemoveUpdateListener(InputPosition toRemove)
    {
        if (_updateListeners == null)
            return;
        _updateListeners -= toRemove;
    }

    public void OnUpdate(Vector3 position)
    {
        if (_updateListeners == null || ((_activeTouchTypes & (int)TouchTypes.UPDATE) != (int)TouchTypes.UPDATE))
		{
            return;
		}
        _updateListeners(position);
    }
    #endregion

    #region Up
    public void AddUpListener(InputPosition toAdd)
    {
        _upListeners += toAdd;
    }

    public void RemoveUpListener(InputPosition toRemove)
    {
        if (_upListeners == null)
		{
            return;
		}
        _upListeners -= toRemove;
    }

    public void OnUp(Vector3 position)
    {
        if (_upListeners == null || ((_activeTouchTypes & (int)TouchTypes.UP) != (int)TouchTypes.UP))
		{
            return;
		}
        _upListeners(position);
    }
    #endregion

    #region Tap
    public void AddTapListener(InputPosition toAdd)
    {
        _tapListeners += toAdd;
    }

    public void RemoveTapListener(InputPosition toRemove)
    {
        if (_tapListeners == null)
		{
            return;
		}
        _tapListeners -= toRemove;
    }

    public void OnTap(Vector3 position)
    {
        if (_tapListeners == null || ((_activeTouchTypes & (int)TouchTypes.TAP) != (int)TouchTypes.TAP))
		{
            return;
		}
        _tapListeners(position);
    }
    #endregion

    #region Double Tap
    public void AddDoubeTapListener(InputPosition toAdd)
    {
        _doubleTapListeners += toAdd;
    }

    public void RemoveDoubleTapListener(InputPosition toRemove)
    {
        if (_doubleTapListeners == null)
		{
            return;
		}
        _doubleTapListeners -= toRemove;
    }

    public void OnDoubleTap(Vector3 position)
    {
        if (_doubleTapListeners == null || ((_activeTouchTypes & (int)TouchTypes.DOUBLETAP) != (int)TouchTypes.DOUBLETAP))
		{
            return;
		}
        _doubleTapListeners(position);
    }
    #endregion

    #region Drag
    public void AddDragListener(IDragListener listener)
    {
        _dragStartListeners += listener.OnDragStart;
        _dragUpdateListeners += listener.OnDragUpdate;
        _dragEndListeners += listener.OnDragEnd;
    }

    public void RemoveDragListener(IDragListener listener)
    {
        if(_dragStartListeners != null)
            _dragStartListeners -= listener.OnDragStart;
        if(_dragUpdateListeners != null)
            _dragUpdateListeners -= listener.OnDragUpdate;
        if (_dragEndListeners != null)
            _dragEndListeners -= listener.OnDragEnd;
    }

    public void OnDragStart(Vector3 position)
    {
        if (_dragStartListeners == null || ((_activeTouchTypes & (int)TouchTypes.DRAG) != (int)TouchTypes.DRAG))
		{
            return;
		}
        _dragStartListeners(position);
    }

    public void OnDragUpdate(Vector3 position, Vector3 delta)
    {
        if (_dragUpdateListeners == null || ((_activeTouchTypes & (int)TouchTypes.DRAG) != (int)TouchTypes.DRAG))
		{
        	return;
		}
        _dragUpdateListeners(position, delta);
    }

    public void OnDragEnd(Vector3 position)
    {
        if (_dragEndListeners == null || ((_activeTouchTypes & (int)TouchTypes.DRAG) != (int)TouchTypes.DRAG))
		{
            return;
		}
        _dragEndListeners(position);
    }
    #endregion

    #region Swipe
    public void AddSwipeListener(ISwipeListener listener)
    {
        _swipeListeners += listener.OnSwipe;
    }

    public void RemoveSwipeListener(ISwipeListener listener)
    {
        if (_swipeListeners != null)
            _swipeListeners -= listener.OnSwipe;
    }

    public void OnSwipe(Vector3 start, Vector3 end, float time)
    {
        if (_swipeListeners == null || ((_activeTouchTypes & (int)TouchTypes.SWIPE) != (int)TouchTypes.SWIPE))
		{
            return;
		}
        _swipeListeners(start, end, time);
    }
    #endregion

    #region Pinch
    public void AddPinchListener(IPinchListener listener)
    {
        _pinchStartListeners += listener.OnPinchStart;
        _pinchUpdateListeners += listener.OnPinchUpdate;
        _pinchEndListeners += listener.OnPinchEnd;
    }

    public void RemovePinchListener(IPinchListener listener)
    {
        if (_pinchStartListeners != null)
            _pinchStartListeners -= listener.OnPinchStart;
        if (_pinchUpdateListeners != null)
            _pinchUpdateListeners -= listener.OnPinchUpdate;
        if (_pinchEndListeners != null)
            _pinchEndListeners -= listener.OnPinchEnd;
    }

    public void OnPinchStart(Vector3 firstPosition, Vector3 secondPosition)
    {
        if (_pinchStartListeners == null || ((_activeTouchTypes & (int)TouchTypes.PINCH) != (int)TouchTypes.PINCH))
		{
            return;
		}
        _pinchStartListeners(firstPosition, secondPosition);
    }

    public void OnPinchUpdate(Vector3 firstPosition, Vector3 secondPosition)
    {
        if (_pinchUpdateListeners == null || ((_activeTouchTypes & (int)TouchTypes.PINCH) != (int)TouchTypes.PINCH))
		{
            return;
		}
        _pinchUpdateListeners(firstPosition, secondPosition);
    }

    public void OnPinchEnd(Vector3 firstPosition, Vector3 secondPosition)
    {
        if (_pinchEndListeners == null || ((_activeTouchTypes & (int)TouchTypes.PINCH) != (int)TouchTypes.PINCH))
		{
            return;
		}
        _pinchEndListeners(firstPosition, secondPosition);
    }
    #endregion

    #region Rotate
    public void AddRotationListener(IRotateListener listener)
    {
        _rotateStartListeners += listener.OnRotateStart;
        _rotateUpdateListeners += listener.OnRotateUpdate;
        _rotateEndListeners += listener.OnRotateEnd;
    }

    public void RemoveRotationListener(IRotateListener listener)
    {
        if (_rotateStartListeners != null)
            _rotateStartListeners -= listener.OnRotateStart;
        if (_rotateUpdateListeners != null)
            _rotateUpdateListeners -= listener.OnRotateUpdate;
        if (_rotateEndListeners != null)
            _rotateEndListeners -= listener.OnRotateEnd;
    }

    public void OnRotateStart(Vector3 firstPosition, Vector3 secondPosition)
    {
        if (_rotateStartListeners == null || ((_activeTouchTypes & (int)TouchTypes.ROTATE) != (int)TouchTypes.ROTATE))
		{
            return;
		}
        _rotateStartListeners(firstPosition, secondPosition);
    }

    public void OnRotateUpdate(Vector3 firstPosition, Vector3 secondPosition)
    {
        if (_rotateUpdateListeners == null || ((_activeTouchTypes & (int)TouchTypes.ROTATE) != (int)TouchTypes.ROTATE))
		{
            return;
		}
        _rotateUpdateListeners(firstPosition, secondPosition);
    }

    public void OnRotateEnd(Vector3 firstPosition, Vector3 secondPosition)
    {
        if (_rotateEndListeners == null || ((_activeTouchTypes & (int)TouchTypes.ROTATE) != (int)TouchTypes.ROTATE))
		{
            return;
		}
        _rotateEndListeners(firstPosition, secondPosition);
    }
    #endregion

    //---- Private
    //------------
    private void MouseInput()
    {   
        // There are 4 options with mouse input
        // no mouse down, no pointer down
        // no mouse down, pointer down
        // mouse down, no pointer down
        // mouse down, pointer down

        bool mouseOne = Input.GetMouseButton(0);
        bool mouseTwo = Input.GetMouseButton(1);

        // Input
		if(_multiTouchActive && mouseOne && mouseTwo)
        {
            HandleMultMouse(mouseOne, mouseTwo);
        }
        else if (mouseOne)
        {
            HandleSingleMouse(mouseOne);
        }

        // Check release
		if(_multiTouchActive && _secondPointer.Down && (!mouseTwo || !mouseOne))
        {
            // Mult release    
            HandleMultInputEnd(_firstPointer.StartPosition, Input.mousePosition);
        }
        else if (!mouseOne)
        {
            // Single release
            HandleSingleInputEnd(Input.mousePosition);
        }
    }

    private void TouchInput()
    {
        if(Input.touchCount == 0)
            return;
		if(_multiTouchActive && Input.touchCount > 1)
		{
			HandleMultTouch();
		}
        else if(Input.touchCount >= 1)
        {
            HandleSingleTouch();
        }
    }

    #region Touch functions
    private void HandleSingleTouch()
    {
        Touch touch = Input.GetTouch(0);
        if(touch.phase == TouchPhase.Began)
        {            
            HandleSingleInputDown(touch.position, touch.fingerId);
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            HandleSingleInputUpdate(touch.position);
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            HandleSingleInputEnd(touch.position);
        }
    }

    private void HandleMultTouch() // will only ever get 2 touches
    {
        Touch t1 = Input.GetTouch(0);
        Touch t2 = Input.GetTouch(1);

        if(t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began)
        {
            HandleMultInputStart(t1.position, t2.position);
        }
        else if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
        {
            HandleMultInputUpdate(t1.position, t2.position);
        }
        else if (t1.phase == TouchPhase.Ended || t2.phase == TouchPhase.Ended)
        {
            HandleMultInputEnd(t1.position, t2.position);
        }
    }
    #endregion

    #region Mouse functions
    private void HandleSingleMouse(bool mouse)
    {        
        if(!_touchStart && !_firstPointer.Down)
        {            
            HandleSingleInputDown(Input.mousePosition);
        }
        else if (_touchStart && _firstPointer.Down)
        {
            HandleSingleInputUpdate(Input.mousePosition);
        }
    }

    private void HandleMultMouse(bool mouseOne, bool mouseTwo)
    {
        // Different cases
        // - Start double
        // - Update double
        // - End double

        if(!_firstPointer.Down || !_secondPointer.Down) // Start
        {            
            HandleMultInputStart(new Vector3(Screen.width * .5f, Screen.height * .5f), Input.mousePosition);
        }
        else if (_firstPointer.Down && _secondPointer.Down)
        {
            HandleMultInputUpdate(_firstPointer.StartPosition, Input.mousePosition);
        }
    }
    #endregion

    #region Single Input
    private void HandleSingleInputDown(Vector3 pos, int pointerID = -1)
    {   
        if(EventSystem.current == null) { return; }
        
        _touchStart = true; 
        // if over UI bail
        // pointerID should be -1 for mouse presses, and some finger id for touches.
        bool pointerOverUI = EventSystem.current.IsPointerOverGameObject(pointerID);
        if (pointerOverUI)
        {
            return;
        }
        _firstPointer.Down = true;
        _firstPointer.Drag = false;
        _firstPointer.StartPosition = pos;
        _firstPointer.Time = Time.time;
        _firstPointer.Update(pos);

        // Send down
        OnDown(_firstPointer.Position);
    }

    private void HandleSingleInputUpdate(Vector3 pos)
    {
        _firstPointer.Update(pos);

        // Send Drag update
        if (!_firstPointer.Drag && Mathf.Abs(Vector3.Distance(_firstPointer.StartPosition, pos)) > DragDeltaThreshold)
        {
            _firstPointer.Drag = true;
            OnDragStart(_firstPointer.Position);
        }
        else if (_firstPointer.Drag)
        {            
            OnDragUpdate(_firstPointer.Position, _firstPointer.Delta);
        }

        // Send update
        OnUpdate(pos);
    }

    private void HandleSingleInputEnd(Vector3 pos)
    {
        _touchStart = false;
        if (!_firstPointer.Down)
            return;

        _firstPointer.Update(pos);

        // Is this considered a tap
        if ((Time.time - _firstPointer.Time) < TapTimeDelta && (pos - _firstPointer.StartPosition).magnitude < TapDistanceTolerance)
        {
            // Send tap
            OnTap(_firstPointer.Position);
        }

        // Is this a swipe?
        if((Time.time - _firstPointer.Time) < SwipeTimeDelta && Vector3.Distance(pos, _firstPointer.StartPosition) > SwipeDistDelta)
        {
            OnSwipe(_firstPointer.StartPosition, pos, Time.time - _firstPointer.Time);
        }

        // End drag?
        if (_firstPointer.Drag)
        {
            OnDragEnd(_firstPointer.Position);
        }

        OnUp(pos);

        _firstPointer.Down = false;
        _firstPointer.Drag = false;
    }        
    #endregion

    #region Mult Input
    private void HandleMultInputStart(Vector3 pos1, Vector3 pos2)
    {
        _firstPointer.Down = true;
        _firstPointer.StartPosition = pos1;
        _firstPointer.Time = Time.time;
        _firstPointer.Update(pos1);

        _secondPointer.Down = true;
        _secondPointer.StartPosition = pos2;
        _secondPointer.Time = Time.time;
        _secondPointer.Update(pos2);

        // Send events
        OnPinchStart(pos1, pos2);
        OnRotateStart(pos1, pos2);
    }

    private void HandleMultInputUpdate(Vector3 pos1, Vector3 pos2)
    {
        _firstPointer.Update(pos1);
        _secondPointer.Update(pos2);

        // Send events
        float lastAngle = Vector3.Angle((_secondPointer.LastPosition - _firstPointer.LastPosition).normalized, Vector3.right);
        float currentAngle = Vector3.Angle((_secondPointer.Position - _firstPointer.Position).normalized, Vector3.right);
        if (Mathf.Abs(currentAngle - lastAngle) > RotationDeltaThreshold)
        {
            OnRotateUpdate(pos1, pos2);
        }

        float lastDist = (_secondPointer.LastPosition - _firstPointer.LastPosition).magnitude;
        float currentDist = (_secondPointer.Position - _firstPointer.Position).magnitude;
        if (Mathf.Abs(currentDist - lastDist) > PinchDeltaThreshold)
        {
            OnPinchUpdate(pos1, pos2);
        }
    }

    private void HandleMultInputEnd(Vector3 pos1, Vector3 pos2)
    {
        _firstPointer.Update(pos1);
        _firstPointer.Down = false;

        _secondPointer.Update(pos2);
        _secondPointer.Down = false;

        // Send events
        OnPinchEnd(pos1, pos2);
        OnRotateEnd(pos1, pos2);
    }
    #endregion
}
