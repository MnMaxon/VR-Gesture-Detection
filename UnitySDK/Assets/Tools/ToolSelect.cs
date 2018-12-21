using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSelect : Tool {
	GameObject propObject = null;
	Selector sel;
	int curFrame;
	static ToolSelect tool1 = null, tool2 = null;
	int snapCool = 0;
	Vector3 lastScale = new Vector3(0, 0, 0);
	Vector3 initPosition;
	Quaternion initRotation;
	Vector3 initLocalScale;

	// Use this for initialization
	void Start()
	{
		sel = new Selector(this);
	}

	public override void handUpdate(GameObject handOb, bool pinch, bool startButton)
	{
		if (tool2 != null && tool1 != null) tool2 = tool1;
		if (tool1 == null) tool1 = this;
		else if (tool1 != this) tool2 = this;
		sel.select(handOb, ignored: propObject);
		Color color = Color.red;
		if (propObject == null) {
			if (sel.getSelected() != null) {
				color = Color.green;
				if (pinch)
				{
					propObject = sel.getSelected().gameObject;
					initPosition = propObject.transform.position;
					initRotation = propObject.transform.rotation;
					initLocalScale = propObject.transform.localScale;
				}
			}
		} else if (propObject != null)
		{
			color = Color.green;
			if (tool1 != null && tool2 != null && tool1.propObject == tool2.propObject) {
				Vector3 newScale = tool1.transform.position - tool2.transform.position;
				tool1.propObject.transform.localScale += newScale - lastScale;
				lastScale = newScale;
			} else {
				lastScale = new Vector3(0,0,0);
				if (snapCool != 0) snapCool--;
				else if (sel.hitObject())
				{
					propObject.transform.position = sel.getEnd();
					if (PropHandler.snap(propObject)) snapCool = 2;
				}
			}
			if (pinch)
			{
				ToolRemote.SetAllCollision(propObject, true);
				//PropHandler.track(propObject);
				addUndo();
				propObject = null;
			}
		}
		sel.drawLine(color);
	}

	public void OnDestroy()
	{
		if (propObject != null)
		{
			ToolRemote.SetAllCollision(propObject, true);
			addUndo();
		}
	}

	public void addUndo() {
		Prop prop = propObject.GetComponent<Prop>();
		RedoManager.addRedoObject(new UnmoveObject(prop.propObjectId, initPosition, initRotation, initLocalScale, prop.name));
	}

	public override string getName()
	{
		return "Mover";
	}
}
