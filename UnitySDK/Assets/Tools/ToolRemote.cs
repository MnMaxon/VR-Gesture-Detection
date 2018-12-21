using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolRemote : Tool {
	GameObject propObject = null;
	int currentProp = 0;
	Selector sel;
	Quaternion presnapped;
	bool usepresnapped = false;
	int snapCool = 0;
	GameObject propMenu = null;

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

	public void openPropMenu(string cat = "All", int page = 0)
	{
		if (propMenu == null) {
			propMenu = Instantiate(GameInitializer.instance.propMenu, gameObject.transform);
			propMenu.GetComponent<PropPicker>().toolRemote = this;
		}
		propMenu.GetComponent<PropPicker>().setCategory(cat, page);
	}

	public void setPropObject(GameObject prefab) {
		if (propObject != null) Destroy(propObject);
		propObject = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
		SetAllCollision(propObject, false);
		usepresnapped = false;
	}

	public override void handUpdate(GameObject handOb, bool pinch, bool startButton, Vector2 delta, bool touchedPad)  {
		if (startButton)
			if (propMenu != null) GameObject.Destroy(propMenu);
			else openPropMenu();
		if (propObject == null) setPropObject(PropHandler.props[PropHandler.nameList[currentProp]]);

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
			if (snapCool == 0)
			{
				propObject.transform.position = sel.getEnd();
				if(PropHandler.snap(propObject)) snapCool = 4;
			}
			else snapCool--;
			if (pinch) {
				SetAllCollision(propObject, true);
				PropHandler.track(propObject);
				Prop prop = propObject.GetComponent<Prop>();
				RedoManager.addRedoObject(new UncreateObject(prop.propObjectId));
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

	public override string getName()
	{
		return "Spawn Remote";
	}
}
