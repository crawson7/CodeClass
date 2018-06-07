using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Red
{
	public enum RedTweenType
	{
		Wait,
		MoveTo,
		LocalMoveTo,
		MoveBy,
		LocalMoveBy,
		MoveToModifier,
		ScaleTo,
		ScaleFrom,
		RotateTo,
		RotateBy,
		Function,
		UIMoveBy,
		UIMoveTo,
		UIFadeTo
	}
		
	public abstract class RedTweenClip
	{
		// This is the interface class for all RedTweenClips.
		// Tween clips define a signle animation applied to one parameter of an object over a set period of time.
		// Tween clips can be run in sequence on the same track or can be run in parralel on different tracks in a set. 

		protected System.Action mEndAction;			// The action that will run as soon as the clip is finished running
		protected System.Action mStartAction;		// The action that will run right before the clip starts running.
		protected float mTime;						// The durration of this clip. This may not be the true running durration if the TimeScale of the set is not 1.0f.
		protected float mTicker = 0;				// The amount of time that has already passed in this clip.
		protected RedTweenType mType;				// The type of clip that this is.
		protected bool mInitialized = false; 		// The clip has been set up and is ready to start. If start is called and the clip is not initialized, it will end imediately.
		protected bool mActive = false; 			// The state of the clip. If the clip is inactive when Update is called, the clip will automatically complete.
		protected Easing.Function mFunction;		// The Easing delegate function to be used for the animation.
		protected RedTween.Track mTrack;			// The track that controls this Clip. 
		protected string mSource;

		public RedTweenType TweenType {get{return mType;}}
		public float Length {get{return mTime;}}

		public virtual bool Update(float delta, out float remainder)
		{
			remainder = 0;
			if(!mInitialized || !mActive)
			{
				EndClip(0, delta);
				return true;
			}

			// Returns true if the clip has ended.
			mTicker += delta;
			if(mTicker>=mTime)
			{
				// Sequence has ended.
				remainder = mTicker-mTime;
				EndClip(remainder, delta);
				return true;
			}
			return false;
		}

		public virtual void EndClip(float remainder, float delta)
		{
			//Debug.Log(mType.ToString() + " Clip Ended. - " + (Time.time-remainder) + " remainder: " + remainder);
			if(mEndAction!=null)
			{
				mEndAction();
			}
		}

		public virtual void Start(float startOffset)
		{
			// Run start action and set starting settings.
			if(mStartAction!=null)
			{
				mStartAction();
			}

			if(mInitialized)
			{
				mActive = true;
			}

			mTicker = 0;
			//Debug.Log("Starting " + mType.ToString() + " Clip - " + (Time.time-startOffset) + " offset: " + startOffset);
		}

		public void SetEndAction(System.Action end)
		{
			mEndAction = end;
		}

		public void SetStarAction(System.Action start)
		{
			mStartAction = start;
		}

		public void SetSource(string source)
		{
			mSource = source;
		}

		public void PrintWarning(string message)
		{
			Debug.LogWarning(message + "\n-------------------------\nCALLER STACK;\n" + mSource + "\n------------------------\n\n");
		}
	}

	#region SPECICAL TWEENS
	public class WaitTweenClip : RedTweenClip
	{
		// Wait Tween Clip will not do anything for a given period of time. 
		// It can be used as a spacer in a Track. Or just to wait some amount of time before running an action.

		public WaitTweenClip(RedTween.Track track, float time, System.Action end)
		{
			mTrack = track;
			mType = RedTweenType.Wait;
			mEndAction = end;
			mTime = time;
			mInitialized = true;
		}
	}
	#endregion

	#region MOVE TWEENS
	public class MoveToTweenClip : RedTweenClip 
	{
		// Move To Tween Clip will animate a game objects world position from whatever it's current position is to the target.
		// Move To position changes are absolute so this cannot be used to modify another tween move. For that use MoveBy.

		private Transform mTrans;
		private Vector3 mTarget;
		private Vector3 mOrigin;
		private bool mLocal;

		public MoveToTweenClip(RedTween.Track track, GameObject obj, Vector3 target, float time, EaseType ease, System.Action end, bool local)
		{
			if(obj==null)
			{
				// Object is null there is nothing to move.
				return;
			}

			mTrack = track;
			mType = (local)? RedTweenType.LocalMoveTo : RedTweenType.MoveTo;
			mTrans = obj.transform;
			mTarget = target;
			mTime = time;
			mEndAction = end;
			mFunction = Easing.EaseFunction(ease);
			mLocal = local;
			if(mFunction!=null)
			{
				mInitialized = true;
			}
		}

		public override bool Update(float f, out float remainder)
		{
			if(base.Update(f, out remainder))
			{
				return true;
			}

			if(mTrans==null)
			{
				PrintWarning(" Warning!: Transform has been Destroyed."); 
				return true;
			}

			float percent = mTicker / mTime;
			Vector3 value = Easing.EaseVector(mFunction, mOrigin, mTarget, percent);
			if(mLocal)
			{
				mTrans.localPosition = value;
			}
			else
			{
				mTrans.position = value;
			}
			return false;
		}

		public override void EndClip(float remainder, float delta)
		{
			// Put the object in its final position.
			if(mLocal)
			{
				mTrans.localPosition = mTarget;
			}
			else
			{
				mTrans.position = mTarget;
			}

			base.EndClip(remainder, delta);
		}

		public override void Start(float startOffset)
		{
			if(mTrans == null)
			{
				EndClip(0f, 0f);
				return;
			}

			if(mLocal)
			{
				mOrigin = mTrans.localPosition;
			}
			else
			{
				mOrigin = mTrans.position;
			}
			base.Start(startOffset);
		}
	}

	public class MoveByTweenClip : RedTweenClip
	{
		private Transform mTrans;
		private Vector3 mAmount;

		private Vector3 mLastTotalMove;
		private Vector3 mLastStart;
		private Vector3 mLastEnd;
		private MoveByTweenClip mPreviousClip;
		private bool mMoveToModefier;
		private bool mLocal;

		public MoveByTweenClip(RedTween.Track track, GameObject obj, Vector3 amount, float time, EaseType ease, System.Action end, MoveByTweenClip previousClip, bool moveToMod, bool local)
		{
			if(obj==null)
			{
				// Object is null there is nothing to move.
				return;
			}

			mType = (local)? RedTweenType.LocalMoveBy : ((previousClip==null)? RedTweenType.MoveBy : RedTweenType.MoveToModifier);
			mLocal = local;
			mTrans = obj.transform;
			mAmount = amount;
			mTime = time;
			mEndAction = end;
			mFunction = Easing.EaseFunction(ease);
			mPreviousClip = previousClip;
			mMoveToModefier = moveToMod;
			mTrack = track;
			if(mFunction!=null)
			{
				mInitialized = true;
			}
		}

		public override bool Update(float delta, out float remainder)
		{
			if(base.Update(delta, out remainder))
			{
				return true;
			}

			if(mTrans==null)
			{
				PrintWarning(" Warning!: Transform has been Destroyed."); 
				return true;
			}
			// Determine how much more movement should be applied this frame. (thisMove)
			float percent = mTicker / mTime; // percent of total time that has passed since last frame.
			Vector3 totalMove = Easing.EaseVector(mFunction, Vector3.zero, mAmount, percent);
			Vector3 thisMove = totalMove - mLastTotalMove;
			mLastTotalMove = totalMove;

			Vector3 pos = (mLocal)? mTrans.localPosition : mTrans.position;
			if(mMoveToModefier && pos != mLastEnd)
			{
				// Make Adjustments for other MoveTo tweens that may have moved the object back during this frame.
				Vector3 moveToDiff = pos - mLastStart;  	// The amount of movement that was caused by previous priority move tweens.
				mLastStart = pos;
				Vector3 newStart = mLastEnd + moveToDiff; // The adjusted reference starting position for this move by. Factoring in the move to.

				mLastEnd = newStart + thisMove;
			}
			else
			{
				mLastEnd = pos + thisMove;
			}

			if(mLocal)
			{
				mTrans.localPosition = mLastEnd;
			}
			else
			{
				mTrans.position = mLastEnd;
			}
			return false;
		}

		public override void EndClip(float remainder, float delta)
		{
			base.EndClip(remainder, delta);
		}

		public override void Start(float startOffset)
		{
			base.Start(startOffset);
			mLastTotalMove = Vector3.zero;

			if(mPreviousClip==null){return;}

			mLastStart = mPreviousClip.mLastStart;
			mLastEnd = mPreviousClip.mLastEnd;
		}


	}
	#endregion

	#region SCALE TWEENS
	public class ScaleTweenClip : RedTweenClip
	{
		// Adjusts the local scale of an object over time.

		private Transform mTrans;
		private Vector3 mTargetScale;
		private Vector3 mOriginScale;

		public ScaleTweenClip(RedTween.Track track, GameObject obj, Vector3 scale, float time, EaseType ease, System.Action end)
		{
			if(obj==null)
			{return;}
			mTrans = obj.transform;
			mTrack = track;
			mTargetScale = scale;
			mTime = time;
			mType = RedTweenType.ScaleTo;
			mEndAction = end;
			mFunction = Easing.EaseFunction(ease);
			if(mFunction!=null)
			{
				mInitialized = true;
			}
		}

		public override bool Update (float delta, out float remainder)
		{
			if(base.Update(delta, out remainder))
			{
				return true;
			}

			if(mTrans==null)
			{
				PrintWarning("Transform was destroyed while animating scale");
				return true;
			}

			float percent = mTicker / mTime;
			Vector3 value = Easing.EaseVector(mFunction, mOriginScale, mTargetScale, percent);
			mTrans.localScale = value;
			return false;
		}

		public override void EndClip (float remainder, float delta)
		{
			if(mTrans==null)
			{
				PrintWarning("Transform was destroyed while animating scale");
			}
			else
			{
				mTrans.localScale = mTargetScale;
			}

			base.EndClip (remainder, delta);
		}

		public override void Start (float startOffset)
		{
			base.Start (startOffset);
			if(mTrans == null)
			{
				EndClip(0f, 0f);
				return;
			}
			mOriginScale = mTrans.localScale;
		}
	}

	public class ScaleFromTweenClip : RedTweenClip
	{
		// Adjusts the local scale of an object while at the same time adjusting 
		// the position of the object in order to give the appearance that it is scaling from a specified point.
		// Keep in mind this could affect the scale and position of child objects.

		// TODO: Implement
	}
	#endregion

	#region ROTATION TWEENS
	public class RotateByTweenClip : RedTweenClip
	{
		// TODO: Implement
	}

	public class LocalRotateToTweenClip : RedTweenClip
	{
		// Adjusts the local scale of an object over time.

		private Transform mTrans;
		private Vector3 mTargetAngle;
		private Vector3 mOriginAngle;

		public LocalRotateToTweenClip(RedTween.Track track, GameObject obj, Vector3 angle, float time, EaseType ease, System.Action end)
		{
			if(obj==null)
			{return;}
			mTrans = obj.transform;
			mTrack = track;
			mTargetAngle = angle;
			mTime = time;
			mType = RedTweenType.RotateTo;
			mFunction = Easing.EaseFunction(ease);
			if(mFunction!=null)
			{
				mInitialized = true;
			}
		}

		public override bool Update (float delta, out float remainder)
		{
			if(base.Update(delta, out remainder))
			{
				return true;
			}

			if(mTrans==null)
			{
				PrintWarning("Transform was destroyed while animating scale");
				return true;
			}

			float percent = mTicker / mTime;
			Vector3 value = Easing.EaseVector(mFunction, mOriginAngle, mTargetAngle, percent);
			mTrans.localEulerAngles = value;
			return false;
		}

		public override void EndClip (float remainder, float delta)
		{
			if(mTrans==null)
			{
				PrintWarning("Transform was destroyed while animating scale");
			}
			else
			{
				mTrans.localEulerAngles = mTargetAngle;
			}

			base.EndClip (remainder, delta);
		}

		public override void Start (float startOffset)
		{
			base.Start (startOffset);
			if(mTrans == null)
			{
				EndClip(0f, 0f);
				return;
			}
			mOriginAngle = mTrans.localEulerAngles;
		}
	}

	public class RotateToTweenClip : RedTweenClip
	{
		// Adjusts the local scale of an object over time.

		private Transform mTrans;
		private Vector3 mTargetAngle;
		private Vector3 mOriginAngle;

		public RotateToTweenClip(RedTween.Track track, GameObject obj, Vector3 angle, float time, EaseType ease, System.Action end)
		{
			if(obj==null)
			{return;}
			mTrans = obj.transform;
			mTrack = track;
			mTargetAngle = angle;
			mTime = time;
			mType = RedTweenType.RotateTo;
			mFunction = Easing.EaseFunction(ease);
			if(mFunction!=null)
			{
				mInitialized = true;
			}
		}

		public override bool Update (float delta, out float remainder)
		{
			if(base.Update(delta, out remainder))
			{
				return true;
			}

			if(mTrans==null)
			{
				PrintWarning("Transform was destroyed while animating scale");
				return true;
			}

			float percent = mTicker / mTime;
			Vector3 value = Easing.EaseVector(mFunction, mOriginAngle, mTargetAngle, percent);
			mTrans.eulerAngles = value;
			return false;
		}

		public override void EndClip (float remainder, float delta)
		{
			if(mTrans==null)
			{
				PrintWarning("Transform was destroyed while animating scale");
			}
			else
			{
				mTrans.eulerAngles = mTargetAngle;
			}

			base.EndClip (remainder, delta);
		}

		public override void Start (float startOffset)
		{
			base.Start (startOffset);
			if(mTrans == null)
			{
				EndClip(0f, 0f);
				return;
			}
			mOriginAngle = mTrans.eulerAngles;
		}
	}
	#endregion

	#region FUNCTION TWEENS
	public class FunctionClip : RedTweenClip
	{
		// Adjusts the local scale of an object over time.

		private TweenFunction mFunctionTween;

		public FunctionClip(RedTween.Track track, TweenFunction function, float time, EaseType ease, System.Action end)
		{
			if(function==null)
			{return;}
			mTrack = track;
			mTime = time;
			mFunctionTween = function;
			mEndAction = end;
			mType = RedTweenType.Function;
			mFunction = Easing.EaseFunction(ease);
			if(mFunction!=null)
			{
				mInitialized = true;
			}
		}

		public override bool Update (float delta, out float remainder)
		{
			if(base.Update(delta, out remainder))
			{
				return true;
			}

			if(mFunctionTween==null){return true;}

			float percent = mTicker / mTime;
			float value = mFunction(0f, 1.0f, percent);
			mFunctionTween(value);
			return false;
		}

		public override void EndClip (float remainder, float delta)
		{
			if(mFunctionTween!=null)
			{mFunctionTween(1.0f);}
			base.EndClip (remainder, delta);
		}

		public override void Start (float startOffset)
		{
			base.Start (startOffset);
			mFunctionTween(0.0f);
		}
	}
	#endregion

	#region UI MOVES & FADES
	public class UIMoveByTweenClip : RedTweenClip
	{
		private RectTransform mRect;
		private Vector3 mAmount;
		private float mLastPercent;
		private Vector3 mTotalMoved;

		public UIMoveByTweenClip(RedTween.Track track, RectTransform rect, Vector3 amount, float time, EaseType ease, System.Action end)
		{
			mType = RedTweenType.MoveBy;
			mRect = rect;
			mAmount = amount;
			mTime = time;
			mEndAction = end;
			mFunction = Easing.EaseFunction(ease);
			mTrack = track;
			if(mFunction!=null){mInitialized = true;}
		}

		public override bool Update(float delta, out float remainder)
		{
			if(base.Update(delta, out remainder))
			{
				return true;
			}

			if(mRect==null)
			{
				PrintWarning("Warning!: Rect Transform has been deleted during update, and con no longer be moved."); 
				return true;
			}

			float percent = mTicker / mTime;
			float thisPercent = mFunction(0, 1, percent);
			float percentDelta = thisPercent - mLastPercent;
			Vector3 thisMove = mAmount * percentDelta;
			mRect.anchoredPosition += (Vector2)thisMove;
			mTotalMoved += thisMove;
			mLastPercent = thisPercent;
			return false;
		}

		public override void EndClip (float remainder, float delta)
		{
			if(mRect==null)
			{
				PrintWarning("Warning!: Rect Transform has been deleted before EndClip was called, and con no longer be moved.");
			}
			else
			{
				mRect.anchoredPosition += (Vector2)(mAmount - mTotalMoved);
			}
			base.EndClip(remainder, delta);
		}

		public override void Start(float startOffset)
		{
			mLastPercent = 0;
			mTotalMoved = Vector3.zero;
			base.Start(startOffset);
		}
	}

	public class UIMoveToTweenClip : RedTweenClip
	{
		private RectTransform mRect;
		private Vector3 mTarget;
		private Vector3 mOrigin;

		public UIMoveToTweenClip(RedTween.Track track, RectTransform rect, Vector3 target, float time, EaseType ease, System.Action end)
		{
			mType = RedTweenType.MoveBy;
			mRect = rect;
			mTarget = target;
			mTime = time;
			mEndAction = end;
			mFunction = Easing.EaseFunction(ease);
			mTrack = track;
			if(mFunction!=null){mInitialized = true;}
		}

		public override bool Update(float delta, out float remainder)
		{
			if(base.Update(delta, out remainder))
			{
				return true;
			}

			if(mRect==null)
			{
				PrintWarning("Warning!: Rect Transform has been deleted durring update and con no longer be moved."); 
				return true;
			}

			float percent = mTicker / mTime;
			Vector3 value = Easing.EaseVector(mFunction, mOrigin, mTarget, percent);
			mRect.anchoredPosition = value;
			return false;
		}

		public override void Start(float startOffset)
		{
			mOrigin = mRect.anchoredPosition;
			base.Start(startOffset);
		}

		public override void EndClip (float remainder, float delta)
		{
			if(mRect==null)
			{
				PrintWarning("Warning!: Rect Transform has been deleted before EndClip was called, and con no longer be moved.");
			}
			else
			{
				mRect.anchoredPosition = mTarget;
			}
			base.EndClip (remainder, delta);
		}
	}

	public class UIFadeToTweenClip : RedTweenClip
	{
		private List<UnityEngine.UI.Graphic> mGraphics = new List<UnityEngine.UI.Graphic>();
		private float mTargetOpacity;
		private List<Color> mOriginColors = new List<Color>();

		public UIFadeToTweenClip(UnityEngine.UI.Graphic graphic, float target, float time, EaseType ease, System.Action end)
		{
			mType = RedTweenType.UIFadeTo;
			mFunction = Easing.EaseFunction(ease);
			mGraphics.Add(graphic);
			mTargetOpacity = target;
			mTime = time;
			mTicker = 0;
			mEndAction = end;
			if(mFunction!=null && graphic!=null){mInitialized = true;}
		}

		public UIFadeToTweenClip(List<UnityEngine.UI.Graphic> graphics, float target, float time, EaseType ease, System.Action end)
		{
			mType = RedTweenType.UIFadeTo;
			mFunction = Easing.EaseFunction(ease);
			mGraphics.AddRange(graphics);
			mTargetOpacity = target;
			mTime = time;
			mEndAction = end;
			mTicker = 0;
			if(mFunction!=null && mGraphics.Count!=0){mInitialized = true;}
		}

		public override bool Update (float delta, out float remainder)
		{
			if(base.Update(delta, out remainder))
			{
				return true;
			}

			float percent = mTicker / mTime;
			for(int i=0; i<mGraphics.Count; i++)
			{
				if(i >= mOriginColors.Count)
				{
					Debug.LogWarning("RedTween Fade Warning: Origin Colors did not load correctly.");
					return false;
				}
				float thisOpacity = mFunction(mOriginColors[i].a, mTargetOpacity, percent);
				if(mGraphics[i] != null)
				{
					mGraphics[i].color = new Color(mOriginColors[i].r, mOriginColors[i].g, mOriginColors[i].b, thisOpacity);
				}
			}
			return false;
		}

		public override void EndClip (float remainder, float delta)
		{
			for(int i=0; i<mGraphics.Count; i++)
			{
				if(mGraphics[i] == null){continue;}
				mGraphics[i].color = new Color(mOriginColors[i].r, mOriginColors[i].g, mOriginColors[i].b, mTargetOpacity);
			}

			base.EndClip (remainder, delta);
		}

		public override void Start (float startOffset)
		{
			base.Start (startOffset);

			// Set all of the origin colors.
			mOriginColors.Clear();
			for(int i=0; i<mGraphics.Count; i++)
			{
				mOriginColors.Add(mGraphics[i].color);
			}
		}
	}
	#endregion
}