using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour {
	public string name;
	public string[] categories = {"Misc."};
	public int propObjectId = -1;
	public List<UnpaintObject> paintHistory = new List<UnpaintObject>();
	public GameObject menuPrefab = null;
	public float menuScale = 1F;

	// Use this for initialization
	void Start () {
		if (propObjectId == -1) propObjectId = Random.Range(0, 10000000);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public UnpaintObject paintObject(GameObject objectToPaint, Color color, bool addToHistory, bool addToUndo)
	{
		Renderer rend = objectToPaint.GetComponent<Renderer>();
		if (rend == null) return null;
		List<int> path = getPathTo(gameObject, objectToPaint);
		UnpaintObject upo = new UnpaintObject(propObjectId, rend.material.color, color, path);
		if (addToHistory) paintHistory.Add(upo);
		if (addToUndo) RedoManager.addRedoObject(upo);
		rend.material.color = color;
		return upo;
	}


	public static List<int> getPathTo(GameObject top, GameObject bottom)
	{
		GameObject cur = bottom;
		List<int> list = new List<int>();
		string p = "Path Start";
		while (cur != top)
		{
			p += " -> ";
			Transform parent = cur.transform.parent;
			for (int i = 0; i < parent.childCount; i++)
				if (cur == parent.GetChild(i).gameObject)
				{
					list.Add(i);
					p += i;
					break;
				}
			cur = parent.gameObject;
		}
		Debug.Log(p);
		list.Reverse();
		return list;
	}

	public static GameObject followPath(GameObject go, List<int> path)
	{
		string p = "Following Path:";
		foreach (int i in path)
		{ go = go.transform.GetChild(i).gameObject; p += " " + i; }
		Debug.Log(p);
		return go;
	}

	public static UnpaintObject paintFromPath(GameObject go, Color color, List<int> path, bool addToHistory, bool addToUndo)
	{
		return go.GetComponent<Prop>().paintObject(followPath(go, path), color, addToHistory, addToUndo);
	}

}
