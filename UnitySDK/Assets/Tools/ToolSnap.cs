using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSnap : Tool {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void handUpdate() {
		PropHandler.usingSnap = !PropHandler.usingSnap;
		Destroy(gameObject);
	}
}
