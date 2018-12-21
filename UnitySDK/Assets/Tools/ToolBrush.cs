using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBrush : Tool {
	
	Color[] colors = new Color[]{ Color.blue, Color.red, Color.yellow, Color.green, Color.black };
	int currentColor = 0;
	Selector sel;

	// Use this for initialization
	void Start () {
		sel = new Selector(this);
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

		if (sel.getSelected() != null && sel.getHitObect().GetComponent<Renderer>() != null) {
			if(colors[currentColor] != Color.black) color = colors[currentColor];
			if (pinch) sel.getSelected().paintObject(sel.getHitObect(), colors[currentColor], true, true);
		}
		sel.drawLine(color);
	}

	public override string getName()
	{
		return "Brush";
	}
}
