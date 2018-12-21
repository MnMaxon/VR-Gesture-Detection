using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointButton : MonoBehaviour {
	public int index = 0;
	public PropPicker picker = null;
	public SavePicker savePicker = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool canSelect() {
		if (savePicker != null && !savePicker.isSave(index) && !savePicker.saveExists(index)) return false;
		return true;
	}

	public void press() {
		if(picker != null) picker.pressed(index);
		if(savePicker != null) savePicker.pressed(index);
	}
}
