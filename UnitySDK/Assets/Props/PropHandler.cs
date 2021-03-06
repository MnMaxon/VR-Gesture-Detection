﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PropHandler {
	public static Dictionary<string, GameObject> props = new Dictionary<string, GameObject>();
	public static Dictionary<string, List<GameObject>> categories = new Dictionary<string, List<GameObject>>();
	public static List<string> nameList = new List<string>(), categoryList = new List<string>();
	public static List<GameObject> spheres = new List<GameObject>();
	//public static List<GameObject> propObjects = new List<GameObject>();
	static Dictionary<int, GameObject> propDict = new Dictionary<int, GameObject>();
	public static bool usingSnap = false;

	public static void save(string path = "save", int count = -1) {
		if (count >= 0) path = path + count;
		string str = "";
		foreach (int key in propDict.Keys) {
			GameObject go = propDict[key];
			Prop prop = go.GetComponent<Prop>();
			str += prop.name + "|||" + go.transform.position.x + "," + go.transform.position.y + "," + go.transform.position.z + "|||"
				+ go.transform.localScale.x + "," + go.transform.localScale.y + "," + go.transform.localScale.z + "|||"
				+ go.transform.localRotation.x + "," + go.transform.localRotation.y + "," + go.transform.localRotation.z+"," + go.transform.localRotation.w
				+ "\n";
		}
		Debug.Log("We SAVED: " + str);
		string dir = "Assets/saves/";
		path = dir + path + ".txt";
		System.IO.Directory.CreateDirectory(dir);

		using (StreamWriter writer = new StreamWriter(path))
		{
			writer.Write(str);
			writer.Close();
		}
	}

	public static void load(string path = "save", int count = -1)
	{
		if (count >= 0) path = path + count;
		untrackAll(true);

		System.IO.StreamReader file = new System.IO.StreamReader("Assets/saves/"+path+".txt");
		string line;
		while ((line = file.ReadLine()) != null) {
			string[] info = line.Split(new string[] { "|||" }, System.StringSplitOptions.None);
			string[] posInfo = info[1].Split(',');
			string[] scaleInfo = info[2].Split(',');
			string[] quatInfo = info[3].Split(',');
			GameObject propObj = GameObject.Instantiate(props[info[0]], new Vector3(float.Parse(posInfo[0]), float.Parse(posInfo[1]), float.Parse(posInfo[2])),
				new Quaternion(float.Parse(quatInfo[0]), float.Parse(quatInfo[1]), float.Parse(quatInfo[2]), float.Parse(quatInfo[3])));
			propObj.transform.localScale = new Vector3(float.Parse(scaleInfo[0]), float.Parse(scaleInfo[1]), float.Parse(scaleInfo[2]));
			track(propObj);
			Debug.Log("ADDED:" + propObj);
		}
		file.Close();
	}

	public static void register(GameObject go) {
		Prop prop = go.GetComponent<Prop>();
		if (prop != null){
			if (nameList.Contains(prop.name)) {
				string orig = prop.name;
				int i = 1;
				while (nameList.Contains(orig + " " + i)) i++;
			}
			props[prop.name] = go;
			nameList.Add(prop.name);
			foreach (string cat in prop.categories) registerCategory(go, cat);
			registerCategory(go, "All");
		}
	}

	public static void registerCategory(GameObject go, string cat)
	{
		List<GameObject> prefabs;
		if (!categories.ContainsKey(cat))
		{
			prefabs = new List<GameObject>();
			categories.Add(cat, prefabs);
			categoryList.Add(cat);
		}
		else prefabs = categories[cat];
		prefabs.Add(go);
		categories[cat] = prefabs;

	}

	public static void track(GameObject go)
	{
		Prop p = go.GetComponent<Prop>();
		if (p.propObjectId == -1) p.propObjectId = Random.Range(0, 10000000);
		int id = p.propObjectId;
		if (propDict.ContainsKey(id))
		{
			if(propDict[id]!=null)GameObject.Destroy(propDict[id]);
			propDict[id] = go;
		}
		else propDict.Add(id, go);
	}

	public static void untrack(GameObject go, bool destroy)
	{
		Prop p = go.GetComponent<Prop>();
		propDict.Remove(p.propObjectId);
		if (destroy) GameObject.Destroy(go);
	}

	public static void untrackAll(bool destroyAll) {
		if (destroyAll) foreach (int key in propDict.Keys) {
			 GameObject.Destroy(propDict[key]);
		}
		propDict.Clear();
	}

	public static bool snap(GameObject go) {
		if (!usingSnap) return false;
		
		Vector3[] snapTo = new Vector3[] { Vector3.left, Vector3.right, Vector3.forward, Vector3.back};
		string str = go.transform.forward + ": ";
		foreach (Vector3 vec in snapTo) {
			str += vec + ", ";
			float dot = Vector3.Dot(vec, go.transform.forward);
			if (dot > .96) { go.transform.LookAt(go.transform.position + vec); break; }
		}
		Debug.Log(str);

		//BoxCollider goBox = getBoxCollider(go);
		//if (goBox == null) return;
		Vector3[] goVerts = getVerts(go);
		//Plane[] goPlanes = getPlanes(goVerts, goBox);
		Collider[] hit = Physics.OverlapSphere(go.transform.position, 8);

		float minMag = Mathf.Infinity;
		Vector3 minDis = new Vector3(0,0,0);

		while (spheres.Count > 0) {
			GameObject sphere = spheres[0];
			spheres.RemoveAt(0);
			GameObject.Destroy(sphere);
		}

		if(true)
			foreach (Vector3 vec in goVerts)
			{
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.localScale /= 10f;
				sphere.transform.position = vec;
				spheres.Add(sphere);
				sphere.transform.parent = go.transform;
			}

		foreach (Collider coll in hit) {
			if (!propDict.ContainsValue(getOldestParent(coll.gameObject))) continue;

			Vector3[] hitVerts = getVerts(coll.gameObject);

			foreach (Vector3 goVert in goVerts)
				foreach (Vector3 hitVert in hitVerts) {
					Vector3 dis1 = hitVert - goVert;
					float d1Mag = dis1.magnitude;
					if (d1Mag < minMag)
					{
						minMag = d1Mag;
						minDis = dis1;
					}
				}
		}

		if (minMag < 1)
		{
			go.transform.position += minDis;
			return true;
		}
		return false;
	}

	static Vector3[] getVertecies(GameObject go)
	{
		MeshFilter mf = go.GetComponent<MeshFilter>();
		if (mf != null) {
			Vector3[] verts = mf.mesh.vertices;
			for (int i = 0; i < verts.Length; i++)
			{
				verts[i] += go.gameObject.transform.position;
			}
			return mf.mesh.vertices;
		}
		for (int i = 0; i < go.transform.childCount; i++)
		{
			Vector3[] verts = getVertecies(go.transform.GetChild(i).gameObject);
			if (verts != null) return verts;
		}
		return null;
	}

	public static GameObject getOldestParent(GameObject go) {
		Transform parent = go.transform.parent;
		if (parent == null) return go;
		return getOldestParent(parent.gameObject);
	}

	static Plane[] getPlanes(Vector3[] verts, BoxCollider bc) {
		Plane[] planes = new Plane[6];

		int i = 0;
		foreach (Vector3 vec in new Vector3[] { bc.transform.up, bc.transform.right, bc.transform.forward })
		{
			planes[i] = new Plane(vec, bc.ClosestPointOnBounds(bc.center + vec * 5));
			GameObject sphere;
			if(false){
				sphere= GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.localScale /= 10f;
				sphere.transform.position = bc.ClosestPointOnBounds(bc.center + vec * 5);
				spheres.Add(sphere);
				sphere.transform.parent = bc.transform;
		}

			planes[i + 1] = new Plane(-vec, bc.ClosestPointOnBounds(bc.center - vec * 5));
			i += 2;
		}

		return planes;
	}

	static BoxCollider getBoxCollider(GameObject go) {
		BoxCollider coll = go.GetComponent<BoxCollider>();
		if (coll != null) return coll;
		foreach (Transform child in go.transform) {
			coll = getBoxCollider(child.gameObject);
			if (coll != null) return coll;
		}
		return null;
	}

	static Renderer GetRenderer(GameObject go) {
		Renderer rend = go.GetComponent<Renderer>();
		if (rend != null) return rend;
		foreach (Transform child in go.transform)
		{
			rend = GetRenderer(child.gameObject);
			if (rend != null) return rend;
		}
		return null;
	}

	static Vector3[] getVerts(GameObject go) {
		//points = getVertecies(bo.gameObject);
		//if (points != null && points.Length != 0) { Debug.Log("This much: " + points.Length); return points; }
		Renderer rend = GetRenderer(go.gameObject);
		Vector3 pos = go.transform.position;
		Vector3 f = go.transform.forward;
		Vector3 r = go.transform.right;
		Vector3 u = go.transform.up;
		if (rend != null) {
			return getVerts(new Vector3(0,0,0), Vector3.forward, Vector3.right, Vector3.up, rend.bounds.min, rend.bounds.max);
		}
		return null;
		//Vector3 min = bo.transform.TransformPoint(bo.center - bo.size * 0.5f) - pos;
		//Vector3 max = bo.transform.TransformPoint(bo.center + bo.size * 0.5f) - pos;
		//return getVerts(pos, f, r, u, min, max);
	}

	static Vector3[] getVerts(Vector3 pos, Vector3 f, Vector3 r, Vector3 u, Vector3 min, Vector3 max)
	{
		Vector3[] points = new Vector3[8];
		points[0] = pos + r * min.x + u * min.y + f * min.z;
		points[1] = pos + r * min.x + u * min.y + f * max.z;
		points[2] = pos + r * min.x + u * max.y + f * min.z;
		points[3] = pos + r * min.x + u * max.y + f * max.z;
		points[4] = pos + r * max.x + u * min.y + f * min.z;
		points[5] = pos + r * max.x + u * min.y + f * max.z;
		points[6] = pos + r * max.x + u * max.y + f * min.z;
		points[7] = pos + r * max.x + u * max.y + f * max.z;
		return points;
	}

	public static GameObject objectFromId(int id)
	{
		if (!propDict.ContainsKey(id)) return null;
		return propDict[id];
	}

	public static Prop propFromId(int id)
	{
		GameObject go = objectFromId(id);
		if (go == null) return null;
		return go.GetComponent<Prop>();
	}
}
