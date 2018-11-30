using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolRemote : Tool {
	GameObject propObject = null;
	int currentProp = 0;
	Selector sel;
	Quaternion presnapped;
	bool usepresnapped = false;

	// Use this for initialization
	void Start () {
		sel = new Selector(this);
	}

	void modCurrentProp(int propMod)
	{
		int totalProps = PropHandler.nameList.Count;
		if (totalProps == 0) return;
		if (propMod != 0)
		{
			Destroy(propObject);
			propObject = null;
			currentProp += propMod;
			currentProp %= totalProps;
		}
	}

	public override void handUpdate(GameObject handOb, bool pinch, bool startButton, Vector2 delta, bool touchedPad)  {
		if (startButton) modCurrentProp(1);
		if (propObject == null)
		{
			propObject = Instantiate(PropHandler.props[PropHandler.nameList[currentProp]], new Vector3(0, 0, 0), Quaternion.identity);
			SetAllCollision(propObject, false);
			usepresnapped = false;
		}

		sel.select(handOb);
		Color color = Color.red;
		if (touchedPad)
		{
			if (usepresnapped) propObject.transform.rotation = presnapped;
			rotate(propObject.transform, delta);
			presnapped = propObject.transform.rotation;
			usepresnapped = true;
		}

		if (sel.hitObject())
		{
			color = Color.green;
			propObject.transform.position = sel.getEnd();
			PropHandler.snap(propObject);
			if (pinch) {
				SetAllCollision(propObject, true);
				PropHandler.track(propObject);
				propObject = null;
			}
		}
		sel.drawLine(color);
	}

	public static void rotate(Transform t, Vector2 delta) {
		if (t == null) return;
		delta *= 50F;
		float min = 1F, max = 20F;
		if (Mathf.Abs(delta.x) > min && Mathf.Abs(delta.x) < max) t.Rotate(t.transform.up, delta.x);
	}

	public static void SetAllCollision(GameObject go, bool enable)
	{
		Collider coll = go.GetComponent<Collider>();
		if (coll != null) coll.enabled = enable;
		foreach (Transform child in go.transform) SetAllCollision(child.gameObject, enable);
	}

	public void OnDestroy()
	{
		Destroy(propObject);
	}
}
