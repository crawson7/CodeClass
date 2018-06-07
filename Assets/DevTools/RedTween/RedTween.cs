using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Red
{
	public delegate void TweenFunction(float value);

	public class RedTween : MonoBehaviour 
	{
		private static List<Set> _sets;

		public static Set CreateSet(string tag="", System.Action end=null)
		{
			// Creates and returns a new RedTweenSet. Set is not added active _sets list until it is played.
			Set comp = new Set(tag, end);
			return comp;
		}

		public static void KillTweens()
		{
			// Kills all running tweens instantly without running any onCompletes
			for(int i=0; i<_sets.Count; i++)
			{
				_sets[i].Kill();
			}
		}

		public static void EndTweens()
		{
			// Moves all tweens to thier final target condition instantly 
			// and calls all of the OnComplete actions in the same order they would have been called if things had ended naturally.
			for(int i=0; i<_sets.Count; i++)
			{
				_sets[i].ForceEnd();
			}
		}

		public static void KillTweensWithTag(string tag)
		{
			for(int i=0; i<_sets.Count; i++)
			{
				if(_sets[i].Tag == tag)
				{
					_sets[i].Kill();
				}
			}
		}

		#region PRIVATE
		private void Awake()
		{
			DontDestroyOnLoad(this.gameObject);
			_sets = new List<Set>();
		}

		private void Start () 
		{
		}


		private void Update () 
		{
			for(int i=0; i<_sets.Count; i++)
			{
				_sets[i].Update();
			}
		}

		protected static bool PlaySet(Set set)
		{
			// Add the comp to RedTweens comps.
			// Make sure that RedTween does not already contain this comp.
	  		for(int i=0; i<_sets.Count; i++)
			{
				if(_sets[i] == set)
				{
					return false;
				}
			}

			_sets.Add(set);
			return true;
		}

		protected static void EndSet(Set set)
		{
			if(!_sets.Remove(set))
			{
				Debug.LogWarning("The Set could not be removed");
				return;
			}

			//Debug.Log("The Tween Set: " + set.Tag + " has completed - " + Time.time);
		}
		#endregion

		#region INSTANT PLAY
		public static Set StepFunction(TweenFunction function, float time, EaseType ease, System.Action endAction=null, float delay=0f)
		{
			Set set = new Set("Instant Step Clip", endAction);
			set.Track(1).AddStepFunction(function, time, ease, null);
			set.Play();
			return set;
		}

		public static Set Wait(float time, System.Action endAction, string tag="")
		{
			string setTag = (string.IsNullOrEmpty(tag))? "Instant Wait Clip" : tag;
			Set set = new Set(setTag, endAction);
			set.Track(1).AddWait(time);
			set.Play();
			return set;
		}

		public static Set MoveTo(GameObject obj, Vector3 target, float time, EaseType ease, System.Action end=null, float delay=0f, string tag="")
		{
			string setTag = (string.IsNullOrEmpty(tag))? "Instant MoveTo Clip" : tag;
			Set set = new Set(setTag, end);
			set.Track(1).AddMoveTo(obj, target, time, ease);
			set.SetDelay(delay);
			set.Play();
			return set;
		}

		public static Set MoveBy(GameObject obj, Vector3 amount, float time, EaseType ease, System.Action end=null, float delay=0f, string tag="")
		{
			string setTag = (string.IsNullOrEmpty(tag))? "Instant MoveBy Clip" : tag;
			Set set = new Set(setTag, end);
			set.Track(1).AddMoveBy(obj, amount, time, ease);
			set.SetDelay(delay);
			set.Play();
			return set;
		}

		public static Set LocalMoveTo(GameObject obj, Vector3 target, float time, EaseType ease, System.Action end=null, float delay=0f, string tag="")
		{
			string setTag = (string.IsNullOrEmpty(tag))? "Instant LocalMoveBy Clip" : tag;
			Set set = new Set(setTag, end);
			set.Track(1).AddLocalMoveTo(obj, target, time, ease);
			set.SetDelay(delay);
			set.Play();
			return set;
		}

		public static Set LocalMoveBy(GameObject obj, Vector3 amount, float time, EaseType ease, System.Action end=null, float delay=0f, string tag="")
		{
			string setTag = (string.IsNullOrEmpty(tag))? "Instant LocalMoveBy Clip" : tag;
			Set set = new Set(setTag, end);
			set.Track(1).AddLocalMoveBy(obj, amount, time, ease);
			set.SetDelay(delay);
			set.Play();
			return set;
		}

		public static Set UIMoveBy(RectTransform rect, Vector3 amount, float time, EaseType ease, System.Action end=null, float delay=0f, string tag="")
		{
			string setTag = (string.IsNullOrEmpty(tag))? "Instant UI Move By Clip" : tag;
			Set set = new Set(setTag, end);
			set.Track(1).AddUIMoveBy(rect, amount, time, ease);
			set.SetDelay(delay);
			set.Play();
			return set;
		}

		public static Set UIFadeTo(UnityEngine.UI.Graphic graphic, float target, float time, EaseType ease, System.Action end=null, float delay=0, string tag="")
		{
			string setTag = (string.IsNullOrEmpty(tag))? "Instant UI Fade To Clip" : tag;
			Set set = new Set(setTag, end);
			set.Track(1).AddUIFadeTo(graphic, target, time, ease);
			set.SetDelay(delay);
			set.Play();
			return set;
		}

		public static Set UIFadeTo(List<UnityEngine.UI.Graphic> graphics, float target, float time, EaseType ease, System.Action end=null, float delay=0, string tag="")
		{
			string setTag = (string.IsNullOrEmpty(tag))? "Instant MoveTo Clip" : tag;
			Set set = new Set(setTag, end);
			set.Track(1).AddUIFadeTo(graphics, target, time, ease);
			set.SetDelay(delay);
			set.Play();
			return set;
		}

		public static Set UIFadeTo(UnityEngine.UI.Graphic[] graphics, float target, float time, EaseType ease, System.Action end=null, float delay=0, string tag="")
		{
			string setTag = (string.IsNullOrEmpty(tag))? "Instant UI Fade To Clip" : tag;
			Set set = new Set(setTag, end);
			List<UnityEngine.UI.Graphic> list = new List<UnityEngine.UI.Graphic>();
			list.AddRange(graphics);
			set.Track(1).AddUIFadeTo(list, target, time, ease);
			set.SetDelay(delay);
			set.Play();
			return set;
		}

		public static Set ScaleTo(GameObject obj, Vector3 target, float time, EaseType ease, System.Action end=null, float delay=0f, string tag="")
		{
			string setTag = (string.IsNullOrEmpty(tag))? "Instant Scale To Clip" : tag;
			Set set = new Set(setTag, end);
			set.Track(1).AddScaleTo(obj, target, time, ease, null);
			set.SetDelay(delay);
			set.Play();
			return set;
		}

		public static Set ScaleTo(GameObject obj, float target, float time, EaseType ease, System.Action end=null, float delay=0f, string tag="")
		{
			Vector3 vScale = Vector3.one * target;
			return ScaleTo(obj, vScale, time, ease, end, delay, tag);
		}

		public static Set RotateTo(GameObject obj, Vector3 target, float time, EaseType ease, System.Action end=null, float delay=0f, string tag="")
		{
			string setTag = (string.IsNullOrEmpty(tag))? "Instant Rotate To Clip" : tag;
			Set set = new Set(setTag, end);
			set.Track(1).AddLocalRotateTo(obj, target, time, ease, null);
			set.SetDelay(delay);
			set.Play();
			return set;
		}
		#endregion

		#region DEBUG
		public static void PrintSets()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("Red Tween Comps:\n");
			if(_sets.Count==0)
			{
				sb.AppendLine("EMPTY");
				Debug.Log(sb.ToString());
				return;
			}
			for(int i=0; i<_sets.Count; i++)
			{
				sb.AppendLine("Set: " + _sets[i].Tag + (_sets[i].Active? " - Active" : " - Inactive"));
			}
			Debug.Log(sb.ToString());
		}
		#endregion

		#region HELPER CLASSES
		public class Set
		{
			#region VARIABLES AND PROPERTIES
			private List<System.Action> mEndActions = new List<System.Action>();
			private List<System.Action> mStartActions = new List<System.Action>();
			private Dictionary<int, Track> mTracks = new Dictionary<int, Track>();
			private bool mActive = false;
			private string _tag;
			private float mTimeScale = 1.0f;
			private bool mDelaySet = false;
			private float mDelay = 0.0f;
			private float mDelayTicker = 0.0f;
			private RTSetViz mVisualizer;

			public string Tag{get{return _tag;}}
			public bool Active{get{return mActive;}}
			public int TrackCount{get{return mTracks.Count;}}
			public Dictionary<int, Track> Tracks {get{return mTracks;}}
			public float Length
			{
				get
				{
					float length = 0;
					foreach(RedTween.Track track in mTracks.Values)
					{
						if(track.Length > length)
						{
							length = track.Length;
						}
					}
					return length;
				}
			}
			#endregion

			public Set(string tag, System.Action end)
			{
				_tag = tag;
				if(end!=null)
				{
					mEndActions.Add(end);
				}
			}

			public void Update()
			{
				if(!mActive){return;}

				float delta = Time.deltaTime * mTimeScale;
				// Wait for Delay if applicable.
				if(mDelaySet)
				{
					mDelayTicker += delta;
					if(mDelayTicker>=mDelay)
					{
						mDelaySet = false;
					}
					else
					{
						return;
					}
				}

				// Update all Tracks in parallel.
				foreach(Track track in mTracks.Values)
				{
					if(track.Update(delta))
					{
						CheckCompletion();
					}
				}

				if(mVisualizer!=null)
				{
					mVisualizer.Update();
				}
			}

			public void Visualize(float height)
			{
				mVisualizer = RedTweenVisualizer.Display(this, height);
			}

			#region PRIVATE HELPERS
			private void CheckCompletion()
			{
				foreach(Track track in mTracks.Values)
				{
					if(!track.Complete){return;}
				}

				// All Tracks are complete. The Set is now complete.
				End();
			}

			private void End()
			{
				mActive = false;
				RedTween.EndSet(this);

				for(int i=0; i<mEndActions.Count; i++)
				{
					if(mEndActions[i] !=null)
					{
						mEndActions[i]();
					}
				}
				mEndActions.Clear();
				RedTweenVisualizer.RemoveVizualization(mVisualizer);
			}

			public void ForceEnd()
			{
				mActive = false;

				foreach(RedTween.Track track in mTracks.Values)
				{
					track.Kill();
				}

				RedTween.EndSet(this);
				for(int i=0; i<mEndActions.Count; i++)
				{
					if(mEndActions[i] !=null)
					{
						mEndActions[i]();
					}
				}
				mEndActions.Clear();
				RedTweenVisualizer.RemoveVizualization(mVisualizer);
			}
			#endregion

			#region SET MANIPULATIONS
			public void Play()
			{
				//Debug.Log("Starting Tween Set " + Tag + " - " + Time.time);
				if(mActive)
				{
					// Comp is already Active.
					Debug.LogWarning("This Tween Comp is already Active and cannot be played again.");
					return;
				}

				if(mTracks.Count==0)
				{
					// You cannot play a comp that has no sequences
					Debug.LogWarning("You cannot play this Comp because it does not contain any tracks.");
					return;
				}

				// Run any start actions that have been assigned.
				for(int i=0; i<mStartActions.Count; i++)
				{
					if(mStartActions[i] != null)
					{
						mStartActions[i]();
					}
				}
				mStartActions.Clear();

				// Start the Set.
				RedTween.PlaySet(this);
				mActive = true;

				// Start Playing All Tracks.
				foreach(Track track in mTracks.Values)
				{
					track.StartInternal();
				}
			}

			public void Play(float timeScale)
			{
				mTimeScale = timeScale;
				Play();
			}

			public void Pause()
			{
				mActive = false;
			}

			public void SetTimeScale(float scale)
			{
				mTimeScale = scale;
			}

			public void SetEndAction(System.Action end)
			{
				if(end==null){return;}
				mEndActions.Add(end);
			}

			public void SetStartAction(System.Action start)
			{
				if(start == null){return;}
				mStartActions.Add(start);
			}

			public void Kill()
			{
				// Kills the Set and all of its tweens witout running any of the end actions.
				// It also does not attemtp to put all tweens to thier final targets.
				mEndActions.Clear();
				mActive = false;

				foreach(RedTween.Track track in mTracks.Values)
				{
					track.Kill();
				}
				RedTween.EndSet(this);
				RedTweenVisualizer.RemoveVizualization(mVisualizer);
			}

			public void SetDelay(float delay)
			{
				if(Mathf.Approximately(delay, 0.0f))
				{
					mDelaySet = false;
					return;
				}

				if(mActive)
				{
					Debug.LogWarning("You canot set a delay once the TweenSet has started playing");
					mDelaySet = false;
					return;
				}

				mDelay = delay;
				mDelayTicker = 0;
				mDelaySet = true;
			}
			#endregion

			public Track Track(int index, bool createIfNotFound=true)
			{
				// This will return the track at the specified index.
				// If the index is not found and create is true, a new track will be created and returned.
				// If create is false this function can be used to check if a track exists.
				if(mTracks.ContainsKey(index))
				{
					return mTracks[index];
				}

				if(createIfNotFound)
				{
					RedTween.Track t = new RedTween.Track(this, index);
					mTracks.Add(index, t);
					return t;
				}

				return null;
			}

			public Track AddTrack(int index)
			{
				return Track(index, true);
			}
		}

		public class Track
		{
			// If you play a track that has no clips it will end imediately.
			#region VARIABLES & PROPERTIES
			List<RedTweenClip> mClips = new List<RedTweenClip>();
			int mActiveIndex;
			private bool mComplete;
			private Set mSet;
			private int mTrackIndex;
			private System.Action mEndAction;
			private int mLoops = 0;		// The number of times this track should play before it is ended.
			private int mPlayCount = 0; // The number of times this track has played, counting the first play.
			private float mRunTime = 0; // The total length of this track in seconds.
			private float mCurrentTime = 0;

			public bool Complete{get{return mComplete;}}
			public List<RedTweenClip> Clips {get{return mClips;}}
			public float Length {get{return mRunTime;}} 
			public float CurrentTime {get{return mCurrentTime;}}
			public int TrackId {get{return mTrackIndex;}}
			#endregion

			#region PUBLIC ACCESS
			public Track(RedTween.Set set, int setIndex)
			{
				mSet = set;
				mTrackIndex = setIndex;
			}

			public void Play(float timeScale=1.0f)
			{
				mSet.Play(timeScale);
				mPlayCount = 1;
			}

			public void StartInternal()
			{
				//Debug.Log("Starting Track " + mTrackIndex + " of Set " + mSet.Tag + " - " + Time.time);
				if(mClips.Count==0)
				{
					EndTrack();
					return;
				}

				mComplete = false;
				mActiveIndex = 0;
				mClips[mActiveIndex].Start(Time.deltaTime);
			}

			public bool Update(float delta)
			{
				// Advances the current clip by the delta time.
				// delta is the amount of time (adjusted) that has passed since the last animation step.
				// Returns true if the entire track has completed.
				if(mComplete){return false;}

				float remainder = 0;
				mCurrentTime += delta;
				if(mClips[mActiveIndex].Update(delta, out remainder))
				{
					// The active clip finished.
					return AdvanceClips(remainder);
				}
				return false;
			}

			public void Kill()
			{
				mEndAction = null;
				mComplete = true;
			}
			#endregion

			#region PRIVATE HELPERS
			private bool AdvanceClips(float startTime)
			{
				// Returns true if this is the end of the track.
				// Advances sequence to the next clip and updates it to the specified start time.
				mActiveIndex ++;
				if(mActiveIndex >= mClips.Count)
				{
					// There are no more clips. The sequence is complete.
					if(EndTrack())
					{return true;}
				}

				mClips[mActiveIndex].Start(startTime);
				return Update(startTime);
			}

			private bool EndTrack()
			{
				//Debug.Log("Track " + mTrackIndex + " of Set " + mSet.Tag + " has Ended - " + Time.time);
				if(mLoops==0 || mPlayCount==mLoops)
				{
					mComplete = true;
					if(mEndAction!=null)
					{
						mEndAction();
					}
					return true;
				}
				else
				{
					mPlayCount ++;
					mActiveIndex = 0;
					mCurrentTime = 0;
					return false;
				}
			}
			#endregion

			#region TRACK MANIPULATIONS
			public Track SetEndAction(System.Action endAction)
			{
				// Specifies an action that should run once the track is complete.
				mEndAction = endAction;
				return this;
			}

			public Track Loop(int count=-1)
			{
				// This will cause a track to loop.
				// if Count Less than 0 or no count is sent, the track will loop indefinitly.
				// The end action on the track will be called after the track finishes its last loop.

				mLoops = count;
				return this;
			}

			public Track PingPong(int count=-1)
			{
				// TODO: Implement
				return this;
			}
			#endregion

			#region CLIP ADDING
			public FunctionClip AddStepFunction(TweenFunction func, float time, EaseType ease, System.Action endAction=null)
			{
				FunctionClip functionClip = new FunctionClip(this, func, time, ease, endAction);
				mClips.Add(functionClip);
				Register(functionClip);
				return functionClip;
			}

			public WaitTweenClip AddWait(float time, System.Action endAction=null)
			{
				WaitTweenClip clip = new WaitTweenClip(this, time, endAction);
				mClips.Add(clip);
				Register(clip);
				return clip;
			}

			public MoveToTweenClip AddMoveTo(GameObject obj, Vector3 target, float time, EaseType ease, System.Action end=null)
			{
				MoveToTweenClip move = new MoveToTweenClip(this, obj, target, time, ease, end, false);
				mClips.Add(move);
				Register(move);
				return move;
			}

			public MoveByTweenClip AddMoveBy(GameObject obj, Vector3 amount, float time, EaseType ease, System.Action end=null)
			{
				// Check to see if there is a previous MoveBy Clip on this track
				MoveByTweenClip previous = null;
				if(mClips.Count>0 && mClips[mClips.Count-1].TweenType == RedTweenType.MoveBy)
				{
					previous = (MoveByTweenClip)mClips[mClips.Count-1];
				}
				MoveByTweenClip move = new MoveByTweenClip(this, obj, amount, time, ease, end, previous, false, false);
				mClips.Add(move);
				Register(move);
				return move;
			}

			public MoveByTweenClip AddMoveToModifier(GameObject obj, Vector3 amount, float time, EaseType ease, System.Action end=null)
			{
				// Requires that this be adjusting the position of a move to track that runs on a higher priority.
				MoveByTweenClip previous = null;

				// Check to see if there is a previous MoveBy Clip on this track
				if(mClips.Count>0 && (mClips[mClips.Count-1].TweenType == RedTweenType.MoveBy || mClips[mClips.Count-1].TweenType == RedTweenType.MoveToModifier))
				{
					previous = (MoveByTweenClip)mClips[mClips.Count-1];
				}
				MoveByTweenClip move = new MoveByTweenClip(this, obj, amount, time, ease, end, previous, true, false);
				mClips.Add(move);
				Register(move);
				return move;
			}

			public MoveToTweenClip AddLocalMoveTo(GameObject obj, Vector3 target, float time, EaseType ease, System.Action end=null)
			{
				MoveToTweenClip move = new MoveToTweenClip(this, obj, target, time, ease, end, true);
				mClips.Add(move);
				Register(move);
				return move;
			}

			public MoveByTweenClip AddLocalMoveBy(GameObject obj, Vector3 amount, float time, EaseType ease, System.Action end=null)
			{
				// Check to see if there is a previous MoveBy Clip on this track
				MoveByTweenClip previous = null;
				if(mClips.Count>0 && (mClips[mClips.Count-1].TweenType == RedTweenType.LocalMoveBy || mClips[mClips.Count-1].TweenType == RedTweenType.MoveToModifier))
				{
					previous = (MoveByTweenClip)mClips[mClips.Count-1];
				}
				MoveByTweenClip move = new MoveByTweenClip(this, obj, amount, time, ease, end, previous, false, true);
				mClips.Add(move);
				Register(move);
				return move;
			}

			public UIMoveByTweenClip AddUIMoveBy(RectTransform obj, Vector3 amount, float time, EaseType ease, System.Action end=null)
			{

				UIMoveByTweenClip uiMove = new UIMoveByTweenClip(this, obj, amount, time, ease, end);
				mClips.Add(uiMove);
				Register(uiMove);
				return uiMove;
			}

			public UIFadeToTweenClip AddUIFadeTo(List<UnityEngine.UI.Graphic> graphics, float target, float time, EaseType ease, System.Action end=null)
			{
				UIFadeToTweenClip uiFade = new UIFadeToTweenClip(graphics, target, time, ease, end);
				mClips.Add(uiFade);
				Register(uiFade);
				return uiFade;
			}

			public UIFadeToTweenClip AddUIFadeTo(UnityEngine.UI.Graphic graphics, float target, float time, EaseType ease, System.Action end=null)
			{
				List<UnityEngine.UI.Graphic> list = new List<UnityEngine.UI.Graphic>();
				list.Add(graphics);
				UIFadeToTweenClip uiFade = new UIFadeToTweenClip(list, target, time, ease, end);
				mClips.Add(uiFade);
				Register(uiFade);
				return uiFade;
			}

			public ScaleTweenClip AddScaleTo(GameObject obj, Vector3 scale, float time, EaseType ease, System.Action end=null)
			{
				ScaleTweenClip scaleClip = new ScaleTweenClip(this, obj, scale, time, ease, end);
				mClips.Add(scaleClip);
				Register(scaleClip);
				return scaleClip;
			}

			public ScaleTweenClip AddScaleTo(GameObject obj, float scale, float time, EaseType ease, System.Action end=null)
			{
				Vector3 vScale = Vector3.one * scale;
				ScaleTweenClip scaleClip = new ScaleTweenClip(this, obj, vScale, time, ease, end);
				mClips.Add(scaleClip);
				Register(scaleClip);
				return scaleClip;
			}

			public LocalRotateToTweenClip AddLocalRotateTo(GameObject obj, Vector3 angle, float time, EaseType ease, System.Action end=null)
			{
				LocalRotateToTweenClip rotateClip = new LocalRotateToTweenClip(this, obj, angle, time, ease, end);
				mClips.Add(rotateClip);
				Register(rotateClip);
				return rotateClip;
			}

			public RotateToTweenClip AddRotateTo(GameObject obj, Vector3 angle, float time, EaseType ease, System.Action end=null)
			{
				RotateToTweenClip rotateClip = new RotateToTweenClip(this, obj, angle, time, ease, end);
				mClips.Add(rotateClip);
				Register(rotateClip);
				return rotateClip;
			}

			private void Register(RedTweenClip clip)
			{
				mRunTime += clip.Length;

				#if DEV_MODE || TEST_MODE
				string stack = System.Environment.StackTrace;
				clip.SetSource(stack);
				#endif
			}
			#endregion
		}
		#endregion
	}
}
	