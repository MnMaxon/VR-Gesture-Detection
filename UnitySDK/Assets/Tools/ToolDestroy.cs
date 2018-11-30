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


	// Update is called once per frame
	void Update () {
		
	}


	public override void handUpdate(GameObject handOb, bool pinch)
	{
		sel.select(handOb);
		Color color = Color.gray;
		if (sel.getSelected() != null) {
			color = Color.red;
			if (pinch) Destroy(sel.getSelected().gameObject);
		}
		sel.drawLine(color);
	}
}
