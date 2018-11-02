using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SymbolHandler
{
	public static int inputSizeRoot = 28;
	const string basepath = "Assets/Resources/DrawingData/";
	static string[] symbols = new string[] { "Square", "Circle", "Cube", "Cone" };

	public static float[][] getMatrix(List<Vector3> locs, string filepath = null)
	{
		if (filepath == null) return calcMatrix(locs);
		filepath = SymbolHandler.getMatrixPath(filepath);
		if (File.Exists(filepath))
		{
			System.IO.StreamReader file = new System.IO.StreamReader(filepath);
			float[][] mat = initializeMatrix();
			string line;
			int i = 0;
			while ((line = file.ReadLine()) != null)
			{
				int j = 0;
				foreach (char c in line.ToCharArray()) {
					if (c == '*') mat[i][j] = 1;
					j++;
				}
				i++;
			}
			file.Close();
			return mat;
		}
		else
		{

			float[][] mat = calcMatrix(locs);
			string text = "";
			foreach (float[] arr in mat) {
				foreach (float f in arr) text += (f == 1) ? "*" :" ";
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

	static float[][] initializeMatrix() {
		float[][] mat = new float[inputSizeRoot][];
		for (int i = 0; i < mat.Length; i++)
		{
			mat[i] = new float[mat.Length];
			for (int j = 0; j < mat[i].Length; j++) mat[i][j] = 0;
		}
		return mat;
	}

	static float[][] calcMatrix(List<Vector3> locs)
	{
		float[][] mat = initializeMatrix();

		if (locs.Count == 0) { return mat; }
		Plane plane = averagePlane(locs);
		Vector3 min = new Vector3(0, 0, 0), max = new Vector3(0, 0, 0);
		List<Vector3> tests = new List<Vector3>();
		bool first = true;
		foreach (Vector3 vec in locs)
		{
			Vector3 pVec = plane.ClosestPointOnPlane(vec);
			Vector3 test = pVec - Vector3.Project(pVec, Vector3.forward);
			if (first)
			{
				min = test;
				max = test;
				first = false;
			}
			else
			{
				max = Vector3.Max(max, test);
				min = Vector3.Min(min, test);
			}
			tests.Add(test);
		}
		max = max - min;
		float maxVal;
		if (max.x > max.y) maxVal = max.x;
		else maxVal = max.y;
		float multVal = 0;
		if (maxVal > 0) multVal = (float)(inputSizeRoot - 1) / maxVal;
		foreach (Vector3 vec in tests)
		{
			Vector3 mod = vec - min;
			mod = mod * multVal;
			mat[(int)mod.x][(int)mod.y] = 1;
		}
		return mat;
	}

	public static void display2D(float[][] mat)
	{
		string str = "";
		for (int i = 0; i < mat.Length; i++)
		{
			for (int j = 0; j < mat[i].Length; j++)
			{
				if (mat[i][j] == 0) str += "-";
				else str += "*";
			}
			str += "\n";
		}
		Debug.Log(str);
	}

	public static Plane averagePlane(List<Vector3> locs)
	{
		Plane plane = new Plane();
		Vector3 avgNormal = new Vector3(0, 0, 0);
		int amount = 0;
		for (int i = 1; i + 1 < locs.Count; i += 2)
		{
			amount++;
			plane.Set3Points(locs[0], locs[i], locs[i + 1]);
			if (Vector3.Dot(avgNormal, plane.normal) >= 0) avgNormal += plane.normal;
			else avgNormal -= plane.normal;
		}
		plane.SetNormalAndPosition(avgNormal, locs[0]);
		return plane;
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
		string symbol = symbolFromId(id);
		if (symbol != null) return randomFile(basepath + "/" + symbol + "/");
		return randomFile(randomDirec());
	}

	public static string symbolFromId(int id)
	{
		if (id < symbols.Length) return symbols[id];
		else return null;
	}

	public static int getSymbolAmount() { return symbols.Length; }
}
