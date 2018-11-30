using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector {

	Prop selected = null;
	float distance;
	Vector3 dir, handObLoc;
	LineRenderer lr = null;
	bool hitOb = false;
	GameObject hitGameObject = null;

	public Selector(MonoBehaviour parent, float lineWidth = .06F)
	{
		if (parent == null) return;
		lr = parent.gameObject.AddComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Additive"));
		lr.startWidth = lineWidth;
		lr.endWidth = lr.startWidth;
	}

	public Prop getSelected() { return selected; }

	public float getDistance() { return distance; }

	public Vector3 getRayStart() { return dir * .1F + handObLoc; }

	public Vector3 getEnd() { return getRayStart() + dir * getDistance(); }

	public void select(GameObject handOb, float maxDist = 100F)
	{
		handObLoc = handOb.transform.position;
		dir = handOb.transform.forward;
		RaycastHit hit;
		hitOb = Physics.Raycast(getRayStart(), dir, out hit, maxDist);
		if (hitOb)
		{
			distance = hit.distance;
			selected = PropHandler.getOldestParent(hit.collider.gameObject).GetComponent<Prop>();
			hitGameObject = hit.collider.gameObject;
			//Debug.Log(PathString(hitGameObject));
		}
	}

	static string PathString(GameObject go) {
		string str = go + " -> ";
		if (go.transform.parent != null) str = PathString(go.transform.parent.gameObject) + str;
		return str;
	}

	public void drawLine(Color color) { drawLine(color, color); }

	public void drawLine(Color c1, Color c2) {
		if (lr == null) return;
		lr.SetPosition(0, handObLoc);
		lr.SetPosition(1, getEnd());

		lr.startColor = c1;
		lr.endColor = c2;
	}

	public bool hitObject() { return hitOb; }

	public GameObject getHitObect() { return hitGameObject; }
}
