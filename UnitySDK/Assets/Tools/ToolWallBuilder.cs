using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolWallBuilder : Tool {

	Color[] colors = new Color[]{ Color.blue, Color.red, Color.yellow, Color.green, Color.black };
	int currentColor = 0;
	Selector sel;

	// Use this for initialization
	void Start () {
		sel = new Selector(this);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public override void handUpdate(GameObject handOb, bool pinch, bool startButton)
	{
	}

	public override string getName()
	{
		return "Wall Builder";
	}
}
