using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour {

	public TextAsset graphModel;
	public GameObject squareHoverPrefab;
	public GameObject circleHoverPrefab;
	public GameObject starHoverPrefab;
	public GameObject brushHoverPrefab;
	public GameObject plusHoverPrefab;
	public GameObject triangleHoverPrefab;

	public GameObject[] props;

	// Use this for initialization
	void Start () {
		SymbolHandler.graphModel = graphModel;

		Symbol square = SymbolHandler.fromName("Square");
		Symbol circle = SymbolHandler.fromName("Circle");
		Symbol star = SymbolHandler.fromName("Star");
		Symbol brush = SymbolHandler.fromName("Brush");
		Symbol plus = SymbolHandler.fromName("Plus");
		Symbol triangle = SymbolHandler.fromName("Triangle");

		square.setHoverPrefab(squareHoverPrefab);
		circle.setHoverPrefab(circleHoverPrefab);
		star.setHoverPrefab(starHoverPrefab);
		brush.setHoverPrefab(brushHoverPrefab);
		plus.setHoverPrefab(plusHoverPrefab);
		triangle.setHoverPrefab(triangleHoverPrefab);

		foreach (GameObject go in props) PropHandler.register(go);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
