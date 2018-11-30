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
    List<GameObject> spheres = new List<GameObject>();
    List<Vector3> locs = new List<Vector3>(), oldLocs = new List<Vector3>();
    List<int> seperators = new List<int>();
    bool triggerPressed = false;
    string curtype = "Triangle";
    const int places = 1000;
    SymbolTensorChecker symbolTensorChecker = null;
	bool spawnObs = true; // True to have it actually do something, false to have it draw new things
	GameObject wand = null;
	List<GameObject> topObs = new List<GameObject>();
	bool passiveLearn = true;
	bool lastZero = true;
	bool currentlyFlying = false;
	int rand;
	Vector3 lastFly;
	public TextMesh pythonText = null;

	// Use this for initialization
	void Start () {
        //Controller = gameObject.GetComponent("SteamVR_TrackedObject") as SteamVR_TrackedObject;
        hand = gameObject.GetComponent("Hand") as Hand;
		symbolTensorChecker = gameObject.GetComponent("SymbolTensorChecker") as SymbolTensorChecker;
	}

	// Update is called once per frame
	void Update()
	{
		GameObject handAttached = null;
		Tool tool = null;
		
		if (hand.AttachedObjects.Count != 0) {
			handAttached = hand.AttachedObjects[0].attachedObject;
			tool = handAttached.GetComponent<Tool>();
			foreach (GameObject obj in topObs) if(obj!=handAttached) Destroy(obj);
		}

		foreach (GameObject sphere in spheres) sphere.GetComponent<MeshRenderer>().material.color = symbolTensorChecker.getColor();

		if (hand.startAction.GetStateDown(hand.handType) && tool == null)
			if (wand == null) PropHandler.save();
			else PropHandler.load();

		//if (hand.grabGripAction.GetStateDown(hand.handType)) curColor = (curColor + 1) % colors.Length;
		if (hand.grabGripAction.GetStateDown(hand.handType))
		{
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
				float[,] eval = symbolTensorChecker.debug();

				string pythonGuess = SymbolHandler.python_guess(locs, seperators);
				Debug.Log(pythonGuess);
				if (pythonText != null)
				{
					pythonText.text = pythonGuess;
				}

				if (spawnObs)
				{
					List<KeyValuePair<int, float>> top = SymbolTensorChecker.topThree(eval);
					Symbol[] topSymbols = new Symbol[3];
					for(int i = 0; i < topSymbols.Length; i++) topSymbols[i] = SymbolHandler.fromId(top[i].Key);
					float dis = .1F, hor = .3F;
					topObs.Add(Instantiate(topSymbols[0].getHoverPrefab(), transform.position + transform.up * dis, Quaternion.identity));
					topObs.Add(Instantiate(topSymbols[1].getHoverPrefab(), transform.position + transform.up * dis - transform.right * hor, Quaternion.identity));
					topObs.Add(Instantiate(topSymbols[2].getHoverPrefab(), transform.position + transform.up * dis + transform.right * hor, Quaternion.identity));
					foreach (GameObject go in topObs) {
						Rigidbody rb = go.GetComponent<Rigidbody>();
						if (rb != null) rb.useGravity = false;
					}
				} else WriteSpheres(curtype);
			}
			ClearSpheres();
		}
        if (hand.grabPinchAction.GetStateDown(hand.handType)) triggerPressed = true;
        if (hand.grabPinchAction.GetStateUp(hand.handType)) triggerPressed = false;
        if (triggerPressed)
        {
			if (wand != null) {
				if (hand.grabPinchAction.GetStateDown(hand.handType)) seperators.Add(locs.Count);
				Spawn();
			}
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
        meshRenderer.material.color = symbolTensorChecker.getColor();
        locs.Add(sphere.transform.position - spheres[0].transform.position);
    }

    void ClearSpheres()
    {
        foreach (GameObject go in spheres) Destroy(go);
        spheres.Clear();
        locs.Clear();
		seperators.Clear();
	}

    int round(float f) { return (int)(f * 1000F); }

    //[MenuItem("Tools/Write file")]
    void WriteSpheres(string type)
    {
        string dir = "Assets/Resources/DrawingData/" + type + "/";
        string path = dir + type.ToLower() +DateTime.Now.ToString(" MM-dd-yyyy HH-mm-ss.fff") +".log";
        System.IO.Directory.CreateDirectory(dir);
		
        Vector3 origin;
        if (spheres.Count != 0)
        {
            string str = "";
            origin = spheres[0].transform.position;
			List<Vector3> vecs = new List<Vector3>();
            foreach (GameObject sphere in spheres)
            {
                Vector3 p = origin - sphere.transform.position;
                str+=round(p.x) + "," + round(p.y) + "," + round(p.z)+"\n";
				vecs.Add(p);
            }


            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(str);
                writer.Close();
            }

			//Saves the Matrix
			SymbolHandler.getMatrix(vecs, path);
        }
	}

	public float[,] getMatrix()
    {
		return SymbolHandler.getMatrix(locs);
    }

	public List<Vector3> getLocList() { return locs; }

}
