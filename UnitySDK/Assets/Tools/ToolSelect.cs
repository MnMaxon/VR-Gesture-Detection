using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSelect : Tool {
	GameObject propObject = null;
	Selector sel;

	// Use this for initialization
	void Start()
	{
		sel = new Selector(this);
	}

	public override void handUpdate(GameObject handOb, bool pinch, bool startButton)
	{
		sel.select(handOb);
		Color color = Color.red;
		if (propObject == null) {
			if (sel.getSelected() != null) {
				color = Color.green;
				if (pinch)
				{
					propObject = sel.getSelected().gameObject;
					ToolRemote.SetAllCollision(propObject, false);
				}
			}
		}else if (sel.hitObject() && propObject != null)
		{
			color = Color.green;
			propObject.transform.position = sel.getEnd();
			PropHandler.snap(propObject);
			if (pinch)
			{
				ToolRemote.SetAllCollision(propObject, true);
				PropHandler.track(propObject);
				propObject = null;
			}
		}
		sel.drawLine(color);
	}

	public void OnDestroy()
	{
		if(propObject != null) ToolRemote.SetAllCollision(propObject, true);
	}
}
