using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol {
	string name;
	int id;
	GameObject hoverPrefab = null;

	public Symbol(string name, int id) {
		this.name = name;
		this.id = id;
	}

	public void setHoverPrefab(GameObject hoverPrefab) { this.hoverPrefab = hoverPrefab; }

	public GameObject getHoverPrefab() { return hoverPrefab; }

	public string getName() { return name; }

	public int getId() { return id; }
}
