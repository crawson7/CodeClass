using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Red
{
	public static class RedTweenVisualizer
	{
		private static List<RTSetViz> mSets = new List<RTSetViz>();
		private static GameObject mParent;
		private static Canvas mCanvas;
		private static bool mInitialized = false;

		public static RTSetViz Display(RedTween.Set rt, float height)
		{
			if(!mInitialized){Initialize();}

			RTSetViz setViz = new RTSetViz(rt, mParent, height);
			mSets.Add(setViz);
			return setViz;
		}

		public static void RemoveVizualization(RTSetViz viz)
		{
			if(viz==null){return;}
			viz.Destroy();
			mSets.Remove(viz);
		}

		private static void Initialize()
		{
			mParent = new GameObject("RedTweenVisualizer");
			mCanvas = mParent.AddComponent<Canvas>();
			mCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			mCanvas.sortingOrder = 100;
			CanvasScaler cs = mParent.AddComponent<CanvasScaler>();
			cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			cs.referenceResolution = new Vector2(600, 800);
			cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
			cs.referencePixelsPerUnit = 100.0f;
			mInitialized = true;
		}

		public static void Terminate()
		{
			GameObject.Destroy(mParent);
		}

		public static RectTransform SetRectFull(GameObject panel, GameObject parent, float posX, float posY, float w, float h, float ancMinX, float ancMinY, float ancMaxX, float ancMaxY, float pivX, float pivY, Vector3 scale)
		{
			RectTransform rect = panel.GetComponent<RectTransform>();
			if(parent!=null)
			{
				rect.transform.SetParent(parent.transform, false);
			}
			rect.anchoredPosition = new Vector2(posX, posY);
			rect.sizeDelta = new Vector2(w, h);
			rect.anchorMax = new Vector2(ancMaxX, ancMaxY);
			rect.anchorMin = new Vector2(ancMinX, ancMinY);
			rect.pivot = new Vector2(pivX, pivY);
			rect.localScale = scale;
			return rect;
		}

		public static RectTransform SetRect(GameObject panel, GameObject parent, float posX, float posY, float w, float h, float ancX, float ancY, float pivX, float pivY)
		{
			RectTransform rect = panel.GetComponent<RectTransform>();
			if(parent!=null)
			{
				rect.transform.SetParent(parent.transform, false);
			}
			rect.anchoredPosition = new Vector2(posX, posY);
			rect.sizeDelta = new Vector2(w, h);
			rect.anchorMax = new Vector2(ancX, ancY);
			rect.anchorMin = new Vector2(ancX, ancY);
			rect.pivot = new Vector2(pivX, pivY);
			return rect;
		}

		public static Image SetImage(GameObject go, Color c, float alpha)
		{
			Image img = go.AddComponent<Image>();
			img.color = new Color(c.r, c.g, c.b, alpha);
			return img;
		}

		public static Text SetText(GameObject go, string s, int size, Color c, TextAnchor anchor=TextAnchor.MiddleLeft)
		{
			Text txt = go.AddComponent<Text>();
			txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			txt.color = new Color(c.r, c.g, c.b);
			txt.text = s;
			txt.fontSize = size;
			txt.alignment = anchor;
			return txt;
		}
	}

	public class RTSetViz
	{
		public RedTween.Set Set;
		public GameObject Panel;
		public RectTransform Rect;
		public List<RTTrackViz> Tracks = new List<RTTrackViz>();

		public RTSetViz(RedTween.Set set, GameObject parent, float height)
		{
			Set = set;
			Panel = new GameObject("Set " + Set.Tag, typeof(RectTransform));
			Rect = RedTweenVisualizer.SetRect(Panel, parent, 0, height, 600, Set.TrackCount*12+ 12, 0,0,0,0);
			RedTweenVisualizer.SetImage(Panel, Color.black, 0.95f);
			int i=0; 
			foreach(RedTween.Track track in set.Tracks.Values)
			{
				Tracks.Add(new RTTrackViz(track, Panel, i, Set.Length));
				i++;
			}

			AddText();
		}

		private void AddText()
		{
			GameObject name = new GameObject("Name" , typeof(RectTransform));
			Rect = RedTweenVisualizer.SetRect(name, Panel, 10, -1, 200, 12, 0,1,0,1);
			RedTweenVisualizer.SetText(name, Set.Tag + " - Time: " + Set.Length.ToString() + " sec.", 9, Color.grey);
		}

		public void Update()
		{
			for (int i = 0; i < Tracks.Count; i++) 
			{
				Tracks[i].Update();	
			}
		}

		public void Destroy()
		{
			GameObject.Destroy(Panel);
		}
	}

	public class RTTrackViz
	{
		public RedTween.Track Track;
		public GameObject Panel;
		public RectTransform Marker;
		public List<RTClipViz> Clips = new List<RTClipViz>();
		private float mMaxTrackSize = 598.0f;
		private float mTrackSize;

		public RTTrackViz(RedTween.Track track, GameObject parent, int count, float setLength)
		{
			Track = track;
			Panel = new GameObject("Track", typeof(RectTransform));
			mTrackSize = (Track.Length/setLength) * mMaxTrackSize;
			RedTweenVisualizer.SetRect(Panel, parent, 2, -12*count - 12, mTrackSize, 10, 0,1,0,1);
			RedTweenVisualizer.SetImage(Panel, Color.white, 0.25f);

			float lastPercent = 0;
			for(int i=0; i<Track.Clips.Count; i++)
			{
				RTClipViz clip = new RTClipViz(Track.Clips[i], Panel, Track.Length, lastPercent);
				Clips.Add(clip);
				lastPercent = clip.EndPercent;
			}

			GameObject markerObj = new GameObject("Track", typeof(RectTransform));
			RedTweenVisualizer.SetRect(markerObj, Panel, 0, 0, 1, 12, 0, 0.5f, 0, 0.5f);
			RedTweenVisualizer.SetImage(markerObj, Color.white, 1f);
			Marker = markerObj.GetComponent<RectTransform>();

			AddText();
		}

		private void AddText()
		{
			
		}

		public void Update()
		{
			float currentTime = Track.CurrentTime;
			float totalTime = Track.Length;
			float percent = currentTime/totalTime;
			float pos = percent*mTrackSize;
			Marker.anchoredPosition = new Vector2(pos, 0);
		}
	}

	public class RTClipViz
	{
		public RedTweenClip Clip;
		public GameObject Panel;
		public float ThisPercent;
		public float StartPercent; 
		public float EndPercent;

		public RTClipViz(RedTweenClip clip, GameObject parent, float trackLength, float lastPercent)
		{
			Clip = clip;
			Panel = new GameObject("Clip", typeof(RectTransform));
			ThisPercent = (Clip.Length/trackLength);
			StartPercent = lastPercent;
			EndPercent = StartPercent + ThisPercent;
			RedTweenVisualizer.SetRectFull(Panel, parent, 0, 0, 0, 8, StartPercent, 0.5f, EndPercent, 0.5f, 0, 0.5f, Vector3.one);

			RectTransform rt = Panel.GetComponent<RectTransform>();
			rt.offsetMin = new Vector2(1, rt.offsetMin.y); 
			rt.offsetMax = new Vector2(-1, rt.offsetMax.y); 

			Color c = GetClipColor(Clip.TweenType);
			RedTweenVisualizer.SetImage(Panel, c, 0.5f);
		}

		private Color GetClipColor(RedTweenType type)
		{
			switch(type)
			{
			case RedTweenType.Wait:
				return Color.black;
			case RedTweenType.MoveBy:
				return PSDColor(5, 93, 225);
			case RedTweenType.MoveTo:
				return PSDColor(145, 30, 249);
			case RedTweenType.LocalMoveBy:
				return PSDColor(129, 179, 254);
			case RedTweenType.LocalMoveTo:
				return PSDColor(170, 130, 252);
			case RedTweenType.MoveToModifier:
				return PSDColor(3, 206, 219);
			case RedTweenType.ScaleTo:
				return PSDColor(0, 142, 71);
			case RedTweenType.ScaleFrom:
				return PSDColor(61, 197, 122);
			case RedTweenType.RotateTo:
				return PSDColor(197, 69, 61);
			case RedTweenType.RotateBy:
				return PSDColor(223, 152, 148);
			case RedTweenType.Function:
				return PSDColor(203, 23, 188);

			}
			return Color.black;
		}

		public static Color PSDColor(int r, int g, int b, int a=255)
		{
			float red = (float)r/255.0f;
			float green = (float)g/255.0f;
			float blue = (float)b/255.0f;
			float alpha = (float)a/255.0f;
			return new Color(red, green, blue, alpha);
		}
	}
}