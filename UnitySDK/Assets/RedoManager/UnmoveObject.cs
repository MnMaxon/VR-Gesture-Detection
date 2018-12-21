using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnmoveObject : RedoObject {
	public int id;
	public string propName;
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 localScale;
	public List<UnpaintObject> paintHistory = null;

	public UnmoveObject(int id, Vector3 position, Quaternion rotation, Vector3 localScale, string propName) {
		this.id = id;
		this.position = position;
		this.rotation = rotation;
		this.localScale = localScale;
		this.propName = propName;
	}

	public UnmoveObject(int id, Transform transform, string propName, List<UnpaintObject> paintHistory)
	{
		this.id = id;
		this.position = transform.position;
		this.rotation = transform.rotation;
		this.localScale = transform.localScale;
		this.propName = propName;
		this.paintHistory = paintHistory;
	}
}
