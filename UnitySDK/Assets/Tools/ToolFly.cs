using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ToolFly : Tool
{

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public override void handUpdate(GameObject handOb)
	{
		Hand hand = handOb.GetComponent<Hand>();
		hand.flying = !hand.flying;
		Destroy(gameObject);
	}
}
