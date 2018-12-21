using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ToolChair : Tool
{
	// Use this for initialization
	void Start()
	{
	}

	public override void handUpdate(GameObject handOb)
	{
		foreach (ToolRemote toolRemote in Object.FindObjectsOfType<ToolRemote>()) {
			toolRemote.openPropMenu("Chairs");
		}
		Destroy(gameObject);
	}

	public override string getName(GameObject handOb)
	{
		return "Chair";
	}
}
