using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Tool : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Drop() {
		Destroy(gameObject.GetComponent<Equippable>());
		Destroy(gameObject.GetComponent<Throwable>());
	}

	public virtual void handUpdate() { }

	public virtual void handUpdate(GameObject handOb = null) { handUpdate(); }

	public virtual void handUpdate(GameObject handOb, bool pinch) { handUpdate(handOb); }

	public virtual void handUpdate(GameObject handOb, bool pinch, bool startButton) { handUpdate(handOb, pinch); }

	public virtual void handUpdate(GameObject handOb, bool pinch, bool startButton, Vector2 padTouch, bool touchedPad) { handUpdate(handOb, pinch, startButton); }
}
