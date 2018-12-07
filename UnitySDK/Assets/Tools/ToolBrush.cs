using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBrush : Tool {
	
	Color[] colors = new Color[]{ Color.blue, Color.red, Color.yellow, Color.green, Color.black };
	int currentColor = 0;
	Selector sel;

	// Use this for initialization
	void Start () {
		name = "Brush";
		sel = new Selector(this);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public override void handUpdate(GameObject handOb, bool pinch, bool startButton)
	{
		int colorMod = 0;
		if (startButton) colorMod = 1;
		if (colorMod != 0)
		{
			int totalColors = colors.Length;
			currentColor += colorMod;
			while (currentColor >= totalColors) currentColor -= totalColors;
			while (currentColor < 0) currentColor += totalColors;

			Renderer rend = GetComponent<Renderer>();
			if (rend != null)
			{
				rend.material.color = colors[currentColor];
			}
		}

		sel.select(handOb);
		
		Color color = Color.gray;
		//color = colors[currentColor];

		if (sel.getSelected() != null) {
			Renderer rend = sel.getHitObect().GetComponent<Renderer>();
			if (rend != null)
			{
				if(pinch) rend.material.color = colors[currentColor];
				color = colors[currentColor];
			}
		}
		sel.drawLine(color);
	}
}
