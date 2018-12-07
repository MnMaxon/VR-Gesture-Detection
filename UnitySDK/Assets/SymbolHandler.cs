using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Diagnostics;

public class SymbolHandler
{
	public static int inputSizeRoot = 28;
	const string basepath = "Assets/Resources/DrawingData/";

	//static string[] symbols = new string[] { "Square", "Circle", "Star", "Brush", "Plus", "Triangle" };
	static string[] symbols = new string[] { "Bed", "Chair", "Circle", "Diamond", "Door", "Dresser", "Floor Lamp", "House", "Square", "Squiggle", "Star", "Sun", "Triangle" };

	static Dictionary<string, Symbol> symbolMap = new Dictionary<string, Symbol>();
	public static bool updateAll = true;
	public static TextAsset graphModel;

	static SymbolHandler()
	{
		for (int i = 0; i < symbols.Length; i++) symbolMap[symbols[i].ToLower()] = new Symbol(symbols[i], i);
	}

	public static string python_guess(List<Vector3> locs, List<Vector3> headLocs, List<Vector3> headForwards)
	{
		string str = "";
		foreach(float f in getMatrix(locs, headLocs, headForwards))
			if (f < .5) str+=" ";
			else str+="*";
		GameInitializer.instance.SendData(str);
		return null;
	}

	public static float[,] getMatrix(List<Vector3> locs, List<Vector3> headLocs, List<Vector3> headForwards, string filepath = null)
	{
		if (filepath == null) return calcMatrix(locs, headLocs, headForwards);
		filepath = SymbolHandler.getMatrixPath(filepath);

		if (File.Exists(filepath))
		{
			System.IO.StreamReader file = new System.IO.StreamReader(filepath);
			float[,] mat = new float[inputSizeRoot, inputSizeRoot];
			string line;
			int i = 0;
			while ((line = file.ReadLine()) != null)
			{
				int j = 0;
				foreach (char c in line.ToCharArray()) {
					if (c == '*') mat[i, j] = 1;
					j++;
				}
				i++;
			}
			file.Close();
			return mat;
		}		else
		{

			float[,] mat = calcMatrix(locs, headLocs, headForwards);
			string text = "";
			for (int i = 0; i < mat.GetLength(0); i++) {
				for (int j = 0; j < mat.GetLength(1); j++) text += (mat[i, j] == 1) ? "*" : " ";
				text += "\n";
			}
			text = text.Substring(0, text.Length - 1);

			int last = filepath.LastIndexOf("/");
			System.IO.Directory.CreateDirectory(filepath.Substring(0, last));

			System.IO.File.WriteAllText(filepath, text);
			return mat;
		}
	}

	public static float[,,] get3DMatrix(List<Vector3> locs) {
		float[,,] arr = new float[inputSizeRoot, inputSizeRoot, inputSizeRoot];
		for (int i = 0; i < arr.GetLength(0); i++) for (int j = 0; j < arr.GetLength(1); j++)
				for (int k = 0; k < arr.GetLength(2); k++) arr[i, j, k] = 0;

		if (locs.Count == 0) return arr;

		Vector3 min = locs[0];
		Vector3 max = min;
		foreach (Vector3 loc in locs)
		{
			min = Vector3.Min(min, loc);
			max = Vector3.Max(max, loc);
		}

		float multVal = (float)(inputSizeRoot - 1) / (max - min).magnitude;
		foreach (Vector3 loc in locs)
		{
			Vector3 p = (loc - min) * multVal;
			arr[(int)p.x, (int)p.y, (int)p.z] = (float)1;
		}
		return arr;
	}

	public static string getMatrixPath(string filepath) {
		int last = filepath.LastIndexOf("/");
		if (last >= 0) filepath = filepath.Substring(0, last) + "/" + inputSizeRoot + "/" + filepath.Substring(last + 1);
		return filepath;
	}

	static float axisDist(Vector3 point1, Vector3 point2, Vector3 axis) {
		axis.Normalize();
		return Vector3.Dot(axis, point1 - point2);
	}

	static float[,] calcMatrix(List<Vector3> locs, List<Vector3> headLocs, List<Vector3> headForwards)
	{
		float[,] mat = new float[inputSizeRoot, inputSizeRoot];

		if (locs.Count == 0) { return mat; }
		Plane plane = averagePlane(locs, headLocs, headForwards);
		List<Vector3> tests = new List<Vector3>();
		Vector3 min = locs[0], max = min;
		tests.Add(min);
		for (int i = 1; i < locs.Count; i++)
		{
			Vector3 vec = plane.ClosestPointOnPlane(locs[i]);
			max = Vector3.Max(max, vec);
			min = Vector3.Min(min, vec);
			tests.Add(vec);
		}


		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = new Vector3(0f, 1f, 2.5f);
		cube.transform.forward = plane.normal;
		Vector3 right = cube.transform.right;
		Vector3 up = -cube.transform.up;

		if (true) {
			max = new Vector3(0, 0, 0);
			min = new Vector3(0, 0, 0);
			List<Vector3> newLocs = new List<Vector3>();

			foreach (Vector3 vec in tests)
			{
				Vector3 newVec = new Vector3(axisDist(vec, tests[0], right), axisDist(vec, tests[0], up), 0);
				max = Vector3.Max(max, newVec);
				min = Vector3.Min(min, newVec);
				newLocs.Add(newVec);
			}
			locs = newLocs;
		}
		GameObject.Destroy(cube);

		max = max - min;
		float maxVal = Mathf.Max(max.y, max.x);
		float multVal = 0;
		if (maxVal > 0) multVal = (float)(inputSizeRoot - 1) / maxVal;
		List<Vector3> velCheck = new List<Vector3>();
		foreach (Vector3 loc in locs)
		{
			Vector3 mod = (loc - min) * multVal;
			mod.z = 0;
			velCheck.Add(mod);
			mat[(int)mod.y, (int)mod.x] = 1;
			if (velCheck.Count < 2) continue;
			Vector3 dist = velCheck[velCheck.Count - 1] - velCheck[velCheck.Count - 2];
			if (dist.magnitude > 1)
			{
				mod += dist / 2;
				mod = Vector3.Max(mod, new Vector3(0, 0, 0));
				mod = Vector3.Min(mod, new Vector3(1, 1, 1) * (inputSizeRoot - 1));
				mat[(int)mod.y, (int)mod.x] = 1;
			}
		}
		return mat;
	}

	public static string display2D(float[,] mat, TextMesh textMesh = null)
	{
		string str = "";
		for (int i = 0; i < mat.GetLength(0); i++)
		{
			for (int j = 0; j < mat.GetLength(1); j++)
			{
				if (mat[i, j] == 0) str += "-";
				else str += "*";
			}
			str += "\n";
		}
		if (textMesh == null) UnityEngine.Debug.Log(str);
		else {
			textMesh.text = str.Replace('-', ' ');
		}
		return str;
	}

	public static Plane averagePlane(List<Vector3> locs, List<Vector3> headLocs, List<Vector3> headForwards) {
		Plane plane = new Plane();
		Vector3 avgNormal = new Vector3(0, 0, 0), avgVec = new Vector3(0, 0, 0);
		int amount = 0;
		int good = 0;
		for (int i = 0; i < locs.Count; i++) {
			if (headLocs != null) {
				float diff = Vector3.Dot(headForwards[i], headLocs[i] - locs[i]);
				//UnityEngine.Debug.Log(i+": " + diff);
				if (diff < -.2 || true) {
					good++;
					avgNormal += headForwards[i];
				}
			}
			avgVec += locs[i];
		}
		avgVec /= locs.Count;
		if (good > 5)
		{
			plane.SetNormalAndPosition(avgNormal, avgVec);
			return plane;
		}
		avgNormal = new Vector3(0, 0, 0);
		IEnumerable<Vector3> randLocs;
		randLocs = Shuffle(locs, new System.Random());
		for (int i = 0; i + 2 < locs.Count; i += 2)
		{
			amount++;
			Vector3 vec1 = randLocs.ElementAt(i+2), vec2 = randLocs.ElementAt(i), vec3 = randLocs.ElementAt(i + 1);
			plane.Set3Points(vec1, vec2, vec3);
			Vector3 normal = plane.normal * Vector3.Dot(vec1 - vec2, vec1 - vec3);
			if (Vector3.Dot(avgNormal, plane.normal) >= 0) avgNormal += normal;
			else avgNormal -= normal;
		}
		plane.SetNormalAndPosition(avgNormal, avgVec);
		return plane;
	}

	public static IEnumerable<T> Shuffle<T>(IEnumerable<T> source, System.Random rng)
	{
		T[] elements = source.ToArray();
		for (int i = elements.Length - 1; i >= 0; i--)
		{
			// Swap element "i" with a random earlier element it (or itself)
			// ... except we don't really need to swap it fully, as we can
			// return it immediately, and afterwards it's irrelevant.
			int swapIndex = rng.Next(i + 1);
			yield return elements[swapIndex];
			elements[swapIndex] = elements[i];
		}
	}
	public static string randomDirec()
	{
		string[] direcs = Directory.GetDirectories(basepath);
		return direcs[UnityEngine.Random.Range(0, direcs.Length)] + "/";
	}

	public static string randomFile(string direc)
	{
		DirectoryInfo direcInfo = new DirectoryInfo(direc);
		FileInfo[] Files = direcInfo.GetFiles("*.log");
		return direc + Files[UnityEngine.Random.Range(0, Files.Length)].Name;
	}

	public static string getFile(int id)
	{
		Symbol symbol = fromId(id);
		if (symbol != null) return randomFile(basepath + "/" + symbol.getName() + "/");
		return randomFile(randomDirec());
	}

	public static Symbol fromId(int id)
	{
		if (id < symbols.Length) return fromName(symbols[id]);
		else return null;
	}

	public static Symbol fromName(string name) {
		name = name.ToLower();
		if(symbolMap.ContainsKey(name)) return symbolMap[name];
		return null;
	}

	public static int getSymbolAmount() { return symbols.Length; }

	public static void updateAllFiles() {
		foreach (string direc in Directory.GetDirectories(basepath))
			foreach (FileInfo file in new DirectoryInfo(direc).GetFiles("*.log"))
				if (!File.Exists(direc + "/" + inputSizeRoot + "/" + file.Name) || !File.Exists(direc + "/goog/" + file.Name))
					getMatrix(getVectorsFromFile(direc + "/" + file.Name), null, null, direc + "/" + file.Name);
		updateAll = false;
	}

	public static List<Vector3> getVectorsFromFile(string filepath)
	{
		System.IO.StreamReader file = new System.IO.StreamReader(filepath);
		List<Vector3> list = new List<Vector3>();
		string line;
		while ((line = file.ReadLine()) != null)
		{
			string[] vecVals = line.Split(',');
			int x, y, z;
			if (!Int32.TryParse(vecVals[0], out x)) x = -1;
			if (!Int32.TryParse(vecVals[1], out y)) y = -1;
			if (!Int32.TryParse(vecVals[2], out z)) z = -1;
			list.Add(new Vector3(x, y, z));
		}
		file.Close();
		return list;
	}
}
