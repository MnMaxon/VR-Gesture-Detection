using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Rendering;
using System.IO;

public class Marker : MonoBehaviour {

	public static int inputSizeRoot = 28;
	public GameObject wandPrefab;
	Hand hand;
    List<GameObject> spheres = new List<GameObject>(), toolTips = new List<GameObject>();
    List<Vector3> locs = new List<Vector3>(), headForward = new List<Vector3>(), headPos = new List<Vector3>();
    bool triggerPressed = false;
    const int places = 1000;
	GameObject wand = null;
	List<GameObject> topObs = new List<GameObject>();
	bool lastZero = true;
	bool currentlyFlying = false;
	int rand;
	Vector3 lastFly;
	//public TextMesh pythonText = null;
	Selector sel;
	GameObject saveMenu = null;
	static bool pythonGuessing = false;
	bool instancePythonGuessing = false;

	// Use this for initialization
	void Start () {
		sel = new Selector(this, lookingForButton: true);
        //Controller = gameObject.GetComponent("SteamVR_TrackedObject") as SteamVR_TrackedObject;
        hand = gameObject.GetComponent("Hand") as Hand;
	}

	// Update is called once per frame
	void Update()
	{
		sel.select(hand.gameObject);
		if (sel.getHitObect() == null || sel.button == null) sel.drawLine(new Color(0, 0, 0, 0));
		else {
			sel.drawLine(Color.cyan);
			if (hand.grabPinchAction.GetStateDown(hand.handType)) sel.button.press();
		}

		GameObject handAttached = null;
		Tool tool = null;

		if (hand.AttachedObjects.Count != 0)
		{
			handAttached = hand.AttachedObjects[0].attachedObject;
			tool = handAttached.GetComponent<Tool>();
			foreach (GameObject obj in topObs) if (obj != handAttached) Destroy(obj);
			foreach (GameObject obj in toolTips) Destroy(obj);
		}
		else {
			string pythonGuess = GameInitializer.instance.recieved;
			if (pythonGuess != null && instancePythonGuessing)
			{
				pythonGuessing = false;
				instancePythonGuessing = false;
				GameInitializer.instance.recieved = null;
				Debug.Log("Python Guess: " + pythonGuess);
				string[] guess_ar = pythonGuess.Split('-');
				if (guess_ar.Length >= 2)
				{
					Symbol[] topSymbols = new Symbol[3];
					pythonGuess = guess_ar[0];
					string[] guesses = pythonGuess.Split(' ');
					pythonGuess = "";
					int index = 0;
					foreach (string num in guesses)
					{

						int x;
						if (Int32.TryParse(num, out x))
						{
							if (index != 0) pythonGuess += " | ";
							pythonGuess += SymbolHandler.fromId(x).getName();
							topSymbols[index] = SymbolHandler.fromId(x);
							index++;
						}
					}
					Debug.Log(pythonGuess);
					//pythonText.text = pythonGuess;


					float dis = .1F, hor = .3F;
					Vector3 right = GameInitializer.instance.transform.right;
					if (topSymbols[0].getHoverPrefab() != null)
						topObs.Add(Instantiate(topSymbols[0].getHoverPrefab(), transform.position + transform.up * dis, Quaternion.identity));
					if(topSymbols[1].getHoverPrefab()!=null)
						topObs.Add(Instantiate(topSymbols[1].getHoverPrefab(), transform.position + transform.up * dis - right * hor, Quaternion.identity));
					if (topSymbols[2].getHoverPrefab() != null)
						topObs.Add(Instantiate(topSymbols[2].getHoverPrefab(), transform.position + transform.up * dis + right * hor, Quaternion.identity));
					int i = 0;
					foreach (GameObject go in topObs)
					{
						while (topSymbols[i].getHoverPrefab() == null) i++;
						Rigidbody rb = go.GetComponent<Rigidbody>();
						if (rb != null) rb.useGravity = false;
						Tool goTool = go.GetComponent<Tool>();
						if (goTool != null)
						{
							GameObject toolTip = Instantiate(GameInitializer.instance.textTooltipPrefab);
							toolTips.Add(toolTip);
							toolTip.transform.parent = go.transform;
							toolTip.transform.localPosition = new Vector3(0, -.05F, 0);
							TextMesh text = toolTip.GetComponent<TextMesh>();
							text.text = goTool.getName(gameObject) + "\n" + topSymbols[i].getName();
						}
						i++;
					}
				}
			}
		}

		//foreach (GameObject sphere in spheres) sphere.GetComponent<MeshRenderer>().material.color = symbolTensorChecker.getColor();

		if (hand.startAction.GetStateDown(hand.handType) && tool == null)
			if (wand == null) toggleSaveMenu();
			else toggleSaveMenu();

		//if (hand.grabGripAction.GetStateDown(hand.handType)) curColor = (curColor + 1) % colors.Length;
		if (hand.grabGripAction.GetStateDown(hand.handType))
		{
			if (saveMenu == null) Destroy(saveMenu);
			if (wand == null) {
				wand = Instantiate(wandPrefab, new Vector3(0, 0, 0), Quaternion.identity);
				hand.AttachScripted(wand);
				while (topObs.Count > 0) {
					Destroy(topObs[0]);
					topObs.RemoveAt(0);
				}
			} else {
				GameObject.Destroy(wand);
				wand = null;
				//float[,] eval = symbolTensorChecker.debug();
				if (locs.Count > 0 && !pythonGuessing)
				{
					pythonGuessing = true;
					instancePythonGuessing = true;
					SymbolHandler.python_guess(locs, headPos, headForward);
				}
			}
			ClearSpheres();
		}
        if (hand.grabPinchAction.GetStateDown(hand.handType)) triggerPressed = true;
        if (hand.grabPinchAction.GetStateUp(hand.handType)) triggerPressed = false;
        if (triggerPressed)
        {
			if (wand != null) Spawn();
        }

		bool padActive = (hand.padTouch.GetAxis(hand.handType) * 50F).magnitude > 1;
		if (!padActive) lastZero = true;
		else if (lastZero) {
			padActive = false;
			lastZero = false;
		}

		if (tool != null) tool.handUpdate(gameObject, hand.grabPinchAction.GetStateDown(hand.handType), hand.startAction.GetStateDown(hand.handType),
			hand.padTouch.GetLastAxisDelta(hand.handType), padActive);

		if (hand.flying && hand.teleportAction.GetLastState(hand.handType))
		{
			Player p = getPlayerParent(gameObject);
			Vector3 curFly = p.transform.position - gameObject.transform.position;
			if (currentlyFlying)
			{
				p.gameObject.transform.position += curFly- lastFly;
			}
			else currentlyFlying = true;
			lastFly = curFly;
		}
		else currentlyFlying = false;

    }

	public Player getPlayerParent(GameObject go) {
		Player p = go.GetComponent<Player>();
		if (p != null) return p;
		Transform t = go.transform.parent;
		if (t == null) return null;
		return getPlayerParent(t.gameObject);
	}

    void Spawn() {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        spheres.Add(sphere);
        sphere.transform.position = transform.position + transform.forward *.3F;
        float size = .02F;
        sphere.transform.localScale = new Vector3(size,size,size);
        MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        locs.Add(sphere.transform.position - spheres[0].transform.position);
		GameObject vrcam = GameInitializer.instance.FollowHead;
		headForward.Add(vrcam.transform.forward);
		headPos.Add(vrcam.transform.position - spheres[0].transform.position);
	}

    void ClearSpheres()
    {
        foreach (GameObject go in spheres) Destroy(go);
        spheres.Clear();
        locs.Clear();
		headForward.Clear();
		headPos.Clear();
	}

    int round(float f) { return (int)(f * 1000F); }

	public float[,] getMatrix()
    {
		return SymbolHandler.getMatrix(locs, headPos, headForward);
    }

	public List<Vector3> getLocList() { return locs; }


	public void toggleSaveMenu()
	{
		if (saveMenu == null)
		{
			saveMenu = Instantiate(GameInitializer.instance.saveMenu, gameObject.transform);
			//saveMenu.transform.up = gameObject.transform.forward;
			//saveMenu.transform.right = gameObject.transform.right;
		}
		else GameObject.Destroy(saveMenu);
	}

}
