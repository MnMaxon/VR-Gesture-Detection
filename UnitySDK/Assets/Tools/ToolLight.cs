using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ToolLight : Tool
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
		foreach (ToolRemote toolRemote in Object.FindObjectsOfType<ToolRemote>()) {
			toolRemote.openPropMenu("Lights");
		}
		Destroy(gameObject);
	}

	public override string getName(GameObject handOb)
	{
		return "Lights";
	}
}
