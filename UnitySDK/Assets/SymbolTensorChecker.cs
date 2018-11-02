using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TensorFlow;
using MLAgents;

public class SymbolTensorChecker : MonoBehaviour
{
    public GameObject parentObject = null;
    public TextMesh textMesh = null;
	public TextAsset graphModel;
	Color color;
	float prob;
	int max;
	int frame = 0;


	public Color getColor()
	{
		return color;
	}

	public float getProb()
	{
		return prob;
	}

	public int getGuess()
	{
		return max;
	}


	// Use this for initialization
	void Start()
    {
        if (parentObject == null) parentObject = gameObject;
	}

    // Update is called once per frame
    void Update()
    {
		if (frame == -1)
		{
			debug();
			frame = 0;
		}
		else frame++;

	}

	public float[,] evaluate() {
		float[,,] matrix = (parentObject.GetComponent(typeof(Marker)) as Marker).getMatrix();

		int count = 0;
		float[] flat = new float[matrix.GetLength(0) * matrix.GetLength(1)*matrix.GetLength(2)];
		for (int i = 0; i < matrix.GetLength(0); i++)
			for (int j = 0; j < matrix.GetLength(1); j++)
				for (int k = 0; k < matrix.GetLength(2); k++)
				{
					flat[count] = matrix[i,j,k];
					count++;
				}

		float[,] recurrentTensor;
		using (var graph = new TFGraph())
		{
			graph.Import(graphModel.bytes);
			print(graph);
			var session = new TFSession(graph);

			var runner = session.GetRunner();

			// implicitally convert a C# array to a tensor
			TFTensor input = flat;

			// set up input tensor and input
			// KEY: I am telling my session to go find the placeholder named "input_placeholder_x" and use my input TENSOR instead
			runner.AddInput(graph["inp"][0], new float[][] { flat });

			// set up output tensor
			runner.Fetch(graph["Wx_b/output_node"][0]);

			// run model - recurrentTensor now holds the probabilities for each outcome
			recurrentTensor = runner.Run()[0].GetValue() as float[,];

			// frees up resources - very important if you are running graph > 400 or so times
			session.Dispose();
			graph.Dispose();
			return recurrentTensor;
		}
	}

	public void debug() {
		float[,] eval = evaluate();
		max = 0;
		for (int i = 1; i < eval.GetLength(1); i++) if (eval[0, i] > eval[0, max]) max = i;
		prob = eval[0, max];
		if (textMesh != null)
		{
			textMesh.text = SymbolHandler.symbolFromId(max);
			color = new Color(1f - prob, prob, 0f);
			textMesh.color = color;
		}
	}
}
