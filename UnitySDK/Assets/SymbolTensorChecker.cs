using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TensorFlow;
using MLAgents;

public class SymbolTensorChecker : MonoBehaviour
{
    public GameObject parentObject = null;
    public TextMesh textMesh = null;
	public TextMesh graphTextMesh = null;
	public TextMesh graphTextMeshOld = null;
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
		if (SymbolHandler.updateAll) SymbolHandler.updateAllFiles();
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
		float[,] matrix = (parentObject.GetComponent(typeof(Marker)) as Marker).getMatrix();
		SymbolHandler.display2D(matrix, graphTextMesh);
		//SymbolHandler.display2D(SymbolHandler.getMatrix((parentObject.GetComponent(typeof(Marker)) as Marker).get.getLocList(), old: true), graphTextMeshOld);

		int count = 0;
		float[] flat = new float[matrix.GetLength(0) * matrix.GetLength(1)];
		for (int i = 0; i < matrix.GetLength(0); i++)
			for (int j = 0; j < matrix.GetLength(1); j++) { 
				flat[count] = matrix[i,j];
				count++;
			}

		float[,] recurrentTensor;
		using (TFGraph graph = new TFGraph())
		{
			graph.Import(SymbolHandler.graphModel.bytes);
			var session = new TFSession(graph);

			var runner = session.GetRunner();

			// implicitally convert a C# array to a tensor
			TFTensor input = flat;

			// set up input tensor and input
			// KEY: I am telling my session to go find the placeholder named "input_placeholder_x" and use my input TENSOR instead

			//runner.AddInput(graph["inp"][0], new float[][] { flat });
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

	public float[,] debug() {
		float[,] eval = evaluate();
		max = 0;
		for (int i = 1; i < eval.GetLength(1); i++) if (eval[0, i] > eval[0, max]) max = i;
		prob = eval[0, max];
		if (textMesh != null)
		{
			textMesh.text = SymbolHandler.fromId(max).getName();
			color = new Color(1f - prob, prob, 0f);
			textMesh.color = color;
		}
		return eval;
	}

	public static List<KeyValuePair<int, float>> topThree(float[,] eval) {
		List<KeyValuePair<int, float>> myList = new List<KeyValuePair<int, float>>();
		float min = 10000;
		for (int i = 0; i < eval.GetLength(1); i++) {
			bool add = false;
			if (i < 3 || eval[0, i] > min) {
				if (i >= 3) for (int j = 0; j < myList.Count; j++) if (min == myList[j].Value) {
							myList.RemoveAt(j);
							min = 10000;
							break;
						}
				myList.Add(new KeyValuePair<int, float>(i, eval[0, i]));
				for (int j = 0; j < myList.Count; j++) min = Mathf.Min(min, myList[j].Value);
			}
		}
		List<KeyValuePair<int, float>> sortedList = new List<KeyValuePair<int, float>>();
		while (myList.Count > 0) { 
			for (int j = 0; j < myList.Count; j++) min = Mathf.Min(min, myList[j].Value);
			for (int j = 0; j < myList.Count; j++) if (min == myList[j].Value)
				{
					sortedList.Insert(0,myList[j]);
					myList.RemoveAt(j);
					min = 10000;
					break;
				}
		}
		return sortedList;
	}
}
