using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpaintObject : RedoObject {
	public int id;
	Color oldColor, newColor;
	List<int> colorPath = null;

	public UnpaintObject(int id, Color oldColor, Color newColor, List<int> colorPath) {
		this.id = id;
		this.oldColor = oldColor;
		this.newColor = newColor;
		this.colorPath = colorPath;
	}

	public Color getOldColor() { return oldColor; }

	public Color getNewColor() { return newColor; }

	public List<int> getColorPath() { return colorPath; }
}
