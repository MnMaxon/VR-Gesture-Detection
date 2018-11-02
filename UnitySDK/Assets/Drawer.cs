using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Rendering;
using System.IO;

public class Drawer : MonoBehaviour {
    
    Hand hand;
    List<GameObject> spheres = new List<GameObject>();
    public List<Vector3> locs = new List<Vector3>();
    bool triggerPressed = false;
    const int places = 1000;
    System.IO.StreamReader file = null;
    const string basepath = "Assets/Resources/DrawingData/";
    string filepath = null;
    public float[] guess = new float[SymbolHandler.getSymbolAmount()];
    public int attempt = 0;
    public bool simulated = false;
    public bool drawOnFrame = true;
    public bool drawn = false;
    bool randomType = false;
    public int maxGuess = 0;
    public GameObject textObject = null;
    public bool runDebugText = true;
    public TextMesh textMesh = null;
    int clears = 0;
    int[] wins = new int[SymbolHandler.getSymbolAmount()];
    int[] losses = new int[SymbolHandler.getSymbolAmount()];
    int[] thousandWins = new int[SymbolHandler.getSymbolAmount()];
    int[] thousandLosses = new int[SymbolHandler.getSymbolAmount()];
    int[] thousandWinsOld = new int[SymbolHandler.getSymbolAmount()];
    int[] thousandLossesOld = new int[SymbolHandler.getSymbolAmount()];
	public bool showAvgPlane = false;



    // Use this for initialization
    void Start ()
    {
        for (int i = 0; i < wins.Length; i++) wins[i] = 0;
        for (int i = 0; i < losses.Length; i++) losses[i] = 0;
        if(textObject!=null) textMesh = textObject.GetComponent(typeof(TextMesh)) as TextMesh;
        ClearSpheres();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (simulated) return;
        if (drawOnFrame) {
            if (showAvgPlane && locs.Count != 0) {
                Plane plane = SymbolHandler.averagePlane(locs);
                Debug.DrawLine(locs[0] + plane.normal * 10000, locs[0] + plane.normal * -10000, Color.green);
            }
            if (!SpawnNext()) ClearSpheres();
        } else if (!drawn)
        {
			/*
            if (savedShapes.ContainsKey(filepath)) drawn = true;
            else spawnAll();
			*/
		}
	}

	public void spawnAll() {
		while (SpawnNext()) ;
		drawn = true;
	}

    bool SpawnNext() {
        string line;
        if ((line = file.ReadLine()) != null)
        {
            string[] vecVals = line.Split(',');
            int x,y,z;
            if (!Int32.TryParse(vecVals[0], out x)) x = -1;
            if (!Int32.TryParse(vecVals[1], out y)) y = -1;
            if (!Int32.TryParse(vecVals[2], out z)) z = -1;
            Vector3 vec = new Vector3(x,y,z);
            if (!simulated)
                DrawSphere(vec/places + gameObject.transform.position, Color.red);
            vec = new Vector3(x, y, z);
            locs.Add(vec);
        }
        else return false;
        return true;
    }

    void DrawSphere(Vector3 vec, Color color)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        spheres.Add(sphere);
        sphere.transform.position = vec;
        float size = .02F;
        sphere.transform.localScale = new Vector3(size, size, size);
        MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        meshRenderer.material.color = color;
    }

    IEnumerator Erase(GameObject go, int time)
    {
        yield return new WaitForSeconds(time);
        DeleteSphere(go);
    }

    void DeleteSphere(GameObject go)
    {
        if (spheres.Contains(go))
        {
            Destroy(go);
            spheres.Remove(go);
        }
    }

    public void ClearSpheres()
    {
        for (int i = 0; i < guess.Length; i++) guess[i] = 0;
        foreach (GameObject go in spheres) Destroy(go);
        spheres.Clear();
        locs.Clear();
        attempt = UnityEngine.Random.Range(0, guess.Length);
        if (file != null)
        {
            file.Close();
            file = null;
        }
        filepath = SymbolHandler.getFile(attempt);
		if (!File.Exists(SymbolHandler.getMatrixPath(filepath)))
		{
			file = new System.IO.StreamReader(filepath);
			if (simulated) spawnAll();
			else drawn = false;
			Debug.Log(drawn);
		}
		else drawn = true;
        clears++;
    }

    int round(float f) { return (int)(f * 1000F); }

    public void addGuess(float[] addVals) {
        maxGuess = 0;
        for (int i = 0; i < addVals.Length; i++)
        {
            float add = Mathf.Clamp(addVals[i], -1, 1);
            guess[i] += add;
            if (guess[i] > guess[maxGuess]) maxGuess = i;
        }
        if(runDebugText)debugText();
    }

    public void debugText() {
        if (textObject == null) return;
        textMesh.text = maxGuess + " / " + attempt;
        if (clears % 1000 == 0) {
            for (int i = 0; i < thousandWins.Length;i++) {
                thousandWinsOld[i] = thousandWins[i];
                thousandLossesOld[i] = thousandLosses[i];
                thousandWins[i] = 0;
                thousandLosses[i] = 0;
            }
        }
        if (maxGuess == attempt)
        {
            wins[attempt]++;
            thousandWins[attempt]++;
            textMesh.color = Color.green;
        }
        else
        {
            losses[attempt]++;
            thousandLosses[attempt]++;
            textMesh.color = Color.red;
        }
        if (clears % 10 == 0)
        {
            Debug.Log("--- Results "+clears+" ---");
            int totalWins = 0;
            int totalLosses = 0;
            int totalThousandWins = 0;
            int totalThousandLosses = 0;
            for (int i = 0; i < wins.Length; i++) {
				String type = SymbolHandler.symbolFromId(i);
				totalLosses += losses[i];
                totalWins += wins[i];
                totalThousandLosses += thousandLossesOld[i];
                totalThousandWins += thousandWinsOld[i];
                DebugLine(type, wins[i], losses[i]);
                DebugLine("Thousand " + type, thousandWinsOld[i], thousandLossesOld[i]);
            }
            DebugLine("Total", totalWins, totalLosses);
            DebugLine("Thousand Total", totalThousandWins, totalThousandLosses);
            Debug.Log("---------------");
        }
    }

	public float[][] getMatrix() { return SymbolHandler.getMatrix(locs, filepath);  }

    void DebugLine(String str, int wins, int losses) {
        Debug.Log(str + ": " + (((float)wins) / (float)(losses+wins))+" - "+ (wins+losses));
    }
}
