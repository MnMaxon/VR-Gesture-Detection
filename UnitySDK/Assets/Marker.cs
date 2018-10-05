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
    Color[] colors = { Color.red, Color.blue, Color.green, Color.black };
    List<GameObject> spheres = new List<GameObject>();
    int curColor = 0;
    bool triggerPressed = false;
    string curtype = "Square";
    const int places = 1000;


    // Use this for initialization
    void Start () {
		//Controller = gameObject.GetComponent("SteamVR_TrackedObject") as SteamVR_TrackedObject;
		hand = gameObject.GetComponent("Hand") as Hand;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //if (hand.grabGripAction.GetStateDown(hand.handType)) curColor = (curColor + 1) % colors.Length;
        if (hand.grabGripAction.GetStateDown(hand.handType)) ClearSpheres();
        if (hand.startAction.GetStateDown(hand.handType)) {
            WriteSpheres(curtype);
            ClearSpheres();
        }
        if (hand.grabPinchAction.GetStateDown(hand.handType)) triggerPressed = true;
        if (hand.grabPinchAction.GetStateUp(hand.handType)) triggerPressed = false;
        if (triggerPressed)
        {
            Spawn();
        }
    }

    void Spawn() {
        if (frames == 0)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spheres.Add(sphere);
            sphere.transform.position = transform.position;
            float size = .02F;
            sphere.transform.localScale = new Vector3(size,size,size);
            MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.material.color =colors[curColor];

            //StartCoroutine(Erase(sphere, 8));
        }
        frames = (frames + 1) % 1;
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
    }

    int round(float f) { return (int)(f * 1000F); }

    //[MenuItem("Tools/Write file")]
    void WriteSpheres(string type)
    {
        string dir = "Assets/Resources/DrawingData/" + type + "/";
        string path = dir+ DateTime.Now.ToString(type.ToLower() + " MM-dd-yyyy HH-mm-ss")+".log";
        System.IO.Directory.CreateDirectory(dir);
        //System.IO.File.Create(path);
        

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
    }

}
