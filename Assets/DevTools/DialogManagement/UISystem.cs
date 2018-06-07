using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour 
{
	//public Dictionary
	public Camera DefaultCamera;
	public Dictionary<string, CanvasControl> UICanvas = new Dictionary<string, CanvasControl>();
	public bool Editor;

    public List<GameObject> Canvases; // An ordererd GameObjects containing UI canvases. Canvas order defines draw order.

    // Use this for initialization
    void Awake () 
	{
		if(Editor)
		{
			GameObject.Destroy(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateCanvases()
    {
        for(int i=0; i<Canvases.Count; i++)
        {
            GameObject prefab = Canvases[i];

            GameObject go = GameObject.Instantiate(prefab, this.transform, true);
            go.name = prefab.name;
            CanvasControl can = go.GetComponent<CanvasControl>();
            if (can == null)
            {
                Debug.LogError("Unable to Load Canvas Component: " + prefab.name);
                Destroy(go);
            }
            can.MyCanvas.sortingOrder = (i + 1) * 10;
            if (can.MyCanvas.worldCamera == null)
            {
                can.MyCanvas.worldCamera = DefaultCamera;
            }
            if (UICanvas.ContainsKey(prefab.name))
            {
                UICanvas[prefab.name] = can;
            }
            else
            {
                UICanvas.Add(prefab.name, can);
            }
        }
    }
}
