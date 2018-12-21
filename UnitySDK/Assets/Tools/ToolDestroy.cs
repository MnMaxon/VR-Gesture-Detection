using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolDestroy : Tool {

	Selector sel;

	// Use this for initialization
	void Start()
	{
		sel = new Selector(this);
	}


	public override void handUpdate(GameObject handOb, bool pinch)
	{
		sel.select(handOb);
		Color color = Color.gray;
		if (sel.getSelected() != null) {
			color = Color.red;
			if (pinch) {
				UnmoveObject unmoveObject = new UnmoveObject(sel.getSelected().propObjectId, sel.getSelected().gameObject.transform, sel.getSelected().name, sel.getSelected().paintHistory);
				RedoManager.addRedoObject(unmoveObject);
				PropHandler.untrack(sel.getSelected().gameObject, true);
			}
		}
		sel.drawLine(color);
	}


	public override string getName()
	{
		return "Destroy";
	}
}
