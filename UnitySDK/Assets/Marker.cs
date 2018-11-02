using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Rendering;
using System.IO;

public class Marker : MonoBehaviour {

    int frames = 0;
    Hand hand;
    List<GameObject> spheres = new List<GameObject>();
    List<Vector3> locs = new List<Vector3>();
    bool triggerPressed = false;
    string curtype = "Cone";
    const int places = 1000;
    SymbolTensorChecker symbolTensorChecker = null;
	bool spawnObs = true;
	public static int inputSizeRoot = 28;
	bool draw = true;
	public GameObject cubePrefab;
	public GameObject bulletPrefab;



	// Use this for initialization
	void Start () {
        //Controller = gameObject.GetComponent("SteamVR_TrackedObject") as SteamVR_TrackedObject;
        hand = gameObject.GetComponent("Hand") as Hand;
		symbolTensorChecker = gameObject.GetComponent("SymbolTensorChecker") as SymbolTensorChecker;
    }
	
	// Update is called once per frame
	void Update ()
    {
		foreach (GameObject sphere in spheres) sphere.GetComponent<MeshRenderer>().material.color = symbolTensorChecker.getColor();

		//if (hand.grabGripAction.GetStateDown(hand.handType)) curColor = (curColor + 1) % colors.Length;
		if (hand.grabGripAction.GetStateDown(hand.handType))
		{
			if (spawnObs)
			{
				symbolTensorChecker.debug();
				if (spheres.Count > 0)// && symbolTensorChecker.getProb() > .3)
				{
					Vector3 min = spheres[0].transform.position;
					Vector3 max = spheres[0].transform.position;
					foreach (GameObject sphere in spheres)
					{
						Vector3 loc = sphere.transform.position;
						min = Vector3.Min(min, loc);
						max = Vector3.Max(max, loc);
					}
					float dist = Vector3.Distance(min, max);
					int guess = symbolTensorChecker.getGuess();

					if (guess == 1 || guess == 0)
					{
						Vector3 scale = new Vector3(dist, dist, dist);


						scale = scale;


						PrimitiveType pt;
						if (guess == 0) pt = PrimitiveType.Cube;
						else pt = PrimitiveType.Sphere;

						GameObject go = GameObject.CreatePrimitive(pt);
						go.transform.position = min;
						go.transform.localScale = scale;
						MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
						Rigidbody gameObjectsRigidBody = go.AddComponent<Rigidbody>();
						if (guess == 1)
						{
							gameObjectsRigidBody.velocity = hand.GetTrackedObjectVelocity() * 5;
							go.transform.position = gameObject.transform.position;
						}
						else
						{
							go.transform.position = min;
						}
					}
					else if (guess == 2)
					{
						List<Vector3> vecMod = new List<Vector3> {
							new Vector3(-1, -1, -1), new Vector3(-1, -1, 0) , new Vector3(-1, -1, 1),
							new Vector3(-1, 0, -1), new Vector3(-1, 0, 0) , new Vector3(-1, 0, 1),
							new Vector3(-1, 1, -1), new Vector3(-1, 1, 0) , new Vector3(-1, 1, 1),
							new Vector3(0, -1, -1), new Vector3(0, -1, 0) , new Vector3(-1, -1, 1),
							new Vector3(0, 0, -1), new Vector3(0, 0, 0) , new Vector3(0, 0, 1),
							new Vector3(0, 1, -1), new Vector3(0, 1, 0) , new Vector3(0, 1, 1),
							new Vector3(1, -1, -1), new Vector3(1, -1, 0) , new Vector3(1, -1, 1),
							new Vector3(1, 0, -1), new Vector3(1, 0, 0) , new Vector3(1, 0, 1),
							new Vector3(1, 1, -1), new Vector3(1, 1, 0) , new Vector3(1, 1, 1)
						};
						foreach (Vector3 mod in vecMod)
						{
							Vector3 spawnVec = (min + max) / 2;
							for (int i = 0; i < 3; i++) spawnVec[i] += mod[i] * .7f;
							Instantiate(cubePrefab, spawnVec, Quaternion.identity);
						}
					}
					else if (guess == 3)
					{
						GameObject bullet = Instantiate(bulletPrefab, gameObject.transform.position, gameObject.transform.rotation);
						bullet.GetComponent<Rigidbody>().velocity = gameObject.transform.forward*10f;
					}
				}
			} else WriteSpheres(curtype);
		}
        if (hand.grabGripAction.GetStateDown(hand.handType)) ClearSpheres();
        if (hand.grabPinchAction.GetStateDown(hand.handType)) triggerPressed = true;
        if (hand.grabPinchAction.GetStateUp(hand.handType)) triggerPressed = false;
        if (triggerPressed && draw && hand.AttachedObjects.Count == 0)
        {
            Spawn();
        }
    }

    void Spawn() {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        spheres.Add(sphere);
        sphere.transform.position = transform.position;
        float size = .02F;
        sphere.transform.localScale = new Vector3(size,size,size);
        MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        meshRenderer.material.color = symbolTensorChecker.getColor();
        locs.Add(transform.position - spheres[0].transform.position);
        //StartCoroutine(Erase(sphere, 8));
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

    void ClearSpheres()
    {
        foreach (GameObject go in spheres) Destroy(go);
        spheres.Clear();
        locs.Clear();
    }

    int round(float f) { return (int)(f * 1000F); }

    //[MenuItem("Tools/Write file")]
    void WriteSpheres(string type)
    {
        string dir = "Assets/Resources/DrawingData/" + type + "/";
        string path = dir + type.ToLower() +DateTime.Now.ToString(" MM-dd-yyyy HH-mm-ss.fff") +".log";
        System.IO.Directory.CreateDirectory(dir);
		//System.IO.File.Create(path);

		if (spheres.Count != 0)
		{
			string str = "";
			float[,,] arr = getMatrix();
;

			for (int i = 0; i < arr.GetLength(0); i++) for (int j = 0; j < arr.GetLength(1); j++)
					for (int k = 0; k < arr.GetLength(2); k++)
					{
						if (j == 0 && k == 0 && i != 0) str += "\n";
						str += (arr[i,j,k] == 0F) ? ' ' : '*';
					}

			using (StreamWriter writer = new StreamWriter(path))
			{
				writer.Write(str);
				writer.Close();
			}
		}

		/* OLD 2D Symbols
        Vector3 origin;
        if (spheres.Count != 0)
        {
            string str = "";
            origin = spheres[0].transform.position;
            foreach (GameObject sphere in spheres)
            {
                Vector3 p = origin - sphere.transform.position;
                str+=round(p.x) + "," + round(p.y) + "," + round(p.z)+"\n";
            }


            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(str);
                writer.Close();
            }
        }
		*/
	}

    public float[,,] getMatrix()
    {
		return SymbolHandler.get3DMatrix(locs);
    }

}
