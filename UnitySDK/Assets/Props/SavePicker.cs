using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavePicker : MonoBehaviour {
	GameObject[] pointButtons = new GameObject[6 * 2];

	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			GameObject child = gameObject.transform.GetChild(i).gameObject;
			PointButton pb = child.GetComponent<PointButton>();
			if (pb != null) {
				pointButtons[pb.index] = child;
				pb.savePicker = this;
			}
		}
		updateText();
	}

	void updateText() {
		foreach (GameObject go in pointButtons) {
			PointButton pb = go.GetComponent<PointButton>();
			if (pb != null) for (int j = 0; j < pb.transform.childCount; j++)
				{
					GameObject childChild = pb.transform.GetChild(j).gameObject;
					TextMesh tm = childChild.GetComponent<TextMesh>();
					if (tm != null)
						if (!isSave(pb.index) && !saveExists(pb.index)) tm.text = "";
						else tm.text = "" + (getSlot(pb.index) + 1);
				}
		}
	}

	public bool saveExists(int index) {
		return File.Exists(string.Format("Assets/saves/save" + (getSlot(index) + 1) + ".txt"));
	}

	int getSlot(int index) {
		return (index / 4) * 2 + (index % 2);
	}

	public bool isSave(int index) {
		return index % 4 < 2;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void pressed(int index) {
		int slot = getSlot(index);
		if (isSave(index)) PropHandler.save(count: slot + 1);
		else PropHandler.load(count: slot + 1);
		updateText();
	}
}
