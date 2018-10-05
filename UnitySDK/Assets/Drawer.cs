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
    Color[] colors = { Color.red, Color.blue, Color.green, Color.black };
    List<GameObject> spheres = new List<GameObject>();
    public List<float> locs = new List<float>();
    int curColor = 0;
    bool triggerPressed = false;
    const int places = 1000;
    System.IO.StreamReader file = null;
    const string basepath = "Assets/Resources/DrawingData/";
    string filepath = null;
    bool fake = false;
    float[] guess = new float[2];
    int attempt = 0;

    string randomDirec()
    {
        string[] direcs = Directory.GetDirectories(basepath);
        return direcs[UnityEngine.Random.Range(0, direcs.Length)] + "/";
    }

    string randomFile(string direc)
    {
        DirectoryInfo direcInfo = new DirectoryInfo(direc);
        FileInfo[] Files = direcInfo.GetFiles("*.log");
        return direc + Files[UnityEngine.Random.Range(0, Files.Length)].Name;
    }

    string getFile() {
        string type;
        if (attempt == 0) return randomFile(basepath + "/Circle/");
        if (attempt == 1) return randomFile(basepath + "/Square/");
        return randomFile(randomDirec());
    }

    // Use this for initialization
    void Start () {
        for (int i; i < guess.Length; i++) guess[i] = 0;
        if (filepath == null) filepath = getFile();
        file = new System.IO.StreamReader(filepath);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!SpawnNext()) {
            ClearSpheres();
            file.Close();
            filepath = getFile();
            file = new System.IO.StreamReader(filepath);
        }
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
            Debug.Log("x: " +x +", " + vecVals[0], gameObject);
            Vector3 vec = new Vector3((float) x / (float) places, (float)y / (float)places, (float)z / (float)places)
                + gameObject.transform.position;
            if (!fake)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                spheres.Add(sphere);
                sphere.transform.position = vec;
                float size = .02F;
                sphere.transform.localScale = new Vector3(size, size, size);
                MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
                meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                meshRenderer.material.color = colors[curColor];
            }
            locs.Add(vec.x);
            locs.Add(vec.y);
            locs.Add(vec.z);
        }
        else return false;
        return true;
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
        for (int i; i < guess.Length; i++) guess[i] = 0;
        attempt = UnityEngine.Random.Range(0, 1);
    }

    int round(float f) { return (int)(f * 1000F); }

    static void ReadString()
    {
        string path = "Assets/Resources/test.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
}
