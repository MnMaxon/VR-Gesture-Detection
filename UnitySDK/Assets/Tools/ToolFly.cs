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

	public override void handUpdate(GameObject handOb)
	{
		Hand hand = handOb.GetComponent<Hand>();
		hand.flying = !hand.flying;
		Destroy(gameObject);
	}

	public override string getName(GameObject handOb)
	{
		Hand hand = handOb.GetComponent<Hand>();
		string enable = "Enable";
		if(hand.flying) enable = "Disable";
		return enable + " Flying";
	}
}
