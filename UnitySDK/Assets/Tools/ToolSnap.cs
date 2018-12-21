using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ToolSnap : Tool {

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	public override void handUpdate(GameObject handOb)
	{
		PropHandler.usingSnap = !PropHandler.usingSnap;
		Destroy(gameObject);
	}

	public override string getName() {
		string enable = "Enable";
		if (PropHandler.usingSnap) enable = "Disable";
		return enable + " Snap";
	}
}
