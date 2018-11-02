using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class MyBallAgent : Agent
{
    Drawer drawer;
    public GameObject drawObject = null;
    public MyDrawAcademy ac;


    // Use this for initialization
    void Start()
    {
        if (drawObject == null) drawObject = gameObject;
        drawer = drawObject.GetComponent(typeof(Drawer)) as Drawer;

    }

    // Update is called once per frame
    void Update()
    {

    }
    float currentCheck = 0f;
	public override void CollectObservations()
	{
		drawer.ClearSpheres();

		float[][] matrix = drawer.getMatrix();
		AddVectorObs((float)currentCheck);
		for (int i = 0; i < matrix.Length; i++) {
			for (int j = 0; j < matrix.Length; j++)
			{
				AddVectorObs(matrix[i][j]);
			}
		}
	}

    public override void AgentReset()
    {

    }

    int yes = 0;
    int no = 0;
    public override void AgentAction(float[] act, string textAction)
    {
		currentCheck = (int)Mathf.Clamp(4F*act[0], 0, 2);
         
        if (!drawer.drawn) return;
		if (currentCheck == drawer.attempt) {
            SetReward(1f);
            yes++;
        }
        else
        {
            no++;
            SetReward(-1f);
        }
		if (UnityEngine.Random.Range(0, 10000) == 1)
        Debug.Log(((int)currentCheck == drawer.attempt) + ": " + (((float)yes) / (float)(no + yes)));
		if (drawer.textMesh == null) return;
        drawer.textMesh.text = ""+(int)currentCheck;
        if (currentCheck != drawer.attempt) drawer.textMesh.color = Color.red;
        else drawer.textMesh.color = Color.green;
        //Done();
        // ac.Done();
    }
}
