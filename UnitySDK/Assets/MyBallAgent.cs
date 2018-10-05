using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class MyBallAgent : Agent
{
    public Drawer drawer;


    // Use this for initialization
    void Start()
    {
        drawer = drawObject.GetComponent(typeof(Drawer)) as Drawer;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override List<float> CollectState()
    {
        return drawer.locs;
    }

    public override void AgentReset()
    {
        drawObject.ClearSpheres();
    }

    public override void AgentStep(float[] act)
    {
        int max = 0;
        drawObject.ClearSpheres();
        for (int i = 0; i < act.Length; i++)
        {
            drawer.guess[i] += act[i];
            if (drawer.guess[i] > drawer.guess[max]) max = i;
        }
        if (max == drawer.attempt) reward = .1f;
    }
}
