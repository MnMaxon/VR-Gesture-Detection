using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Tool : MonoBehaviour {
	public Symbol symbol = null;
	bool first = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (first) {
			Transform p1 = gameObject.transform.parent;
			if (p1 == null) return;
			first = false;
			Hand hand = p1.gameObject.GetComponent<Hand>();
			first = false;
			if (hand.handType != SteamVR_Input_Sources.LeftHand) return;
			gameObject.transform.up = hand.transform.up;
			gameObject.transform.right = hand.transform.right;
			gameObject.transform.forward = hand.transform.forward;
		}
	}

	public void Drop() {
		Destroy(gameObject.GetComponent<Equippable>());
		Destroy(gameObject.GetComponent<Throwable>());
	}

	public virtual string getName() { return "None"; }

	public virtual string getName(GameObject handOb) { return getName(); }

	public Symbol getSymbol() { return symbol; }

	public virtual void handUpdate() { }

	public virtual void handUpdate(GameObject handOb = null) { handUpdate(); }

	public virtual void handUpdate(GameObject handOb, bool pinch) { handUpdate(handOb); }

	public virtual void handUpdate(GameObject handOb, bool pinch, bool startButton) { handUpdate(handOb, pinch); }

	public virtual void handUpdate(GameObject handOb, bool pinch, bool startButton, Vector2 padTouch, bool touchedPad) { handUpdate(handOb, pinch, startButton); }
}
