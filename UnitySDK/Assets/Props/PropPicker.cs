using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropPicker : MonoBehaviour {
	static string[] catArray = {"All", "Furniture", "Chairs", "Lights", "Misc."};
	int page = 0;
	string cat = "All";
	public TextMesh catMesh;
	public TextMesh pageMesh;

	public ToolRemote toolRemote = null;

	int maxPages = 0;
	int perPage = 12;
	int propStartIndex = 2;
	List<GameObject> propsInWorld = new List<GameObject>();
	GameObject[] pointButtons = new GameObject[12 + 2 + 2];

	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			GameObject child = gameObject.transform.GetChild(i).gameObject;
			PointButton pb = child.GetComponent<PointButton>();
			if (pb != null) {
				pointButtons[pb.index] = child;
				pb.picker = this;
			}
		}
		setCategory("All", 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	List<GameObject> getProps() {
		if (!PropHandler.categories.ContainsKey(cat)) return null;
		return PropHandler.categories[cat];
	}

	GameObject propAtIndex(int index) {
		if (getProps() == null) return null;
		int mod = page * perPage + index - propStartIndex;
		if (mod >= getProps().Count) return null;
		return getProps()[mod];
	}

	public void setCategory(string cat, int page) {
		while (propsInWorld.Count > 0) {
			GameObject go = propsInWorld[propsInWorld.Count - 1];
			Destroy(go);
			propsInWorld.Remove(go);
		}

		this.cat = cat;
		this.page = page;
		catMesh.text = cat;
		if (getProps() == null) maxPages = 1;
		else maxPages = (getProps().Count / perPage) + 1;
		string zC = "", zT = "";
		if (page + 1 < 10) zC = "0";
		if (maxPages < 10) zT = "0";
		pageMesh.text = zC + (page + 1) + "/" + zT + maxPages;

		for (int i = propStartIndex; i < propStartIndex + perPage; i++) {
			GameObject prop = propAtIndex(i);
			if (prop == null || pointButtons[i] == null) continue;
			if (pointButtons[i] == null) Debug.Log("Prop at " + i + " IS NULL");
			GameObject menuProp = prop.GetComponent<Prop>().menuPrefab;
			if (menuProp == null) menuProp = prop;
			GameObject go = Instantiate(menuProp, pointButtons[i].transform);
			RemoveAllCollisionAndLight(go);
			Vector3 bounds = new Vector3(2,2,2);
			Renderer renderer = go.GetComponent<Renderer>();
			if (renderer != null) bounds = renderer.bounds.size;
			go.transform.localScale *= (1/Mathf.Max(bounds.x, bounds.y))/30;
			if (renderer != null) bounds = renderer.bounds.size;
			go.transform.localPosition -= new Vector3(0,(bounds.y / go.transform.parent.lossyScale.y) / 2F,0);
			go.transform.localScale *= prop.GetComponent<Prop>().menuScale;
			propsInWorld.Add(go);
		}
	}

	void RemoveAllCollisionAndLight(GameObject go) {
		if (go == null) return;
		for (int i = 0; i < go.transform.childCount; i++) RemoveAllCollisionAndLight(go.transform.GetChild(i).gameObject);
		Light light = go.GetComponent<Light>();
		Rigidbody rb = go.GetComponent<Rigidbody>();
		if (rb != null)	rb.detectCollisions = false;
		if (light != null) Destroy(light);
	}

	public void pressed(int index) {
		if (index < 2)
		{
			int catIndex = 0;
			foreach (string cat in catArray)
				if (cat.Equals(this.cat)) break;
				else catIndex++;
			if (index == 0) catIndex--;
			if (index == 1) catIndex++;
			if (catIndex >= catArray.Length) catIndex = 0;
			if (catIndex < 0) catIndex = catArray.Length - 1;
			setCategory(catArray[catIndex], 0);
		}
		else if (index < 2 + perPage) {
			toolRemote.setPropObject(propAtIndex(index));
		} else {
			int page = this.page;
			if (index == 2 + perPage) page--;
			else page++;
			if (page < 0) page = maxPages;
			if (page >= maxPages) page = 0;
			setCategory(cat, page);
		}
	}
}
