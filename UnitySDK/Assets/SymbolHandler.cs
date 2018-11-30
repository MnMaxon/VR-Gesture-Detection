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
	static string[] symbols = new string[] { "Brush", "Circle", "Plus", "Square", "Star", "Triangle" };

	static Dictionary<string, Symbol> symbolMap = new Dictionary<string, Symbol>();
	public static bool updateAll = true;
	public static TextAsset graphModel;

	static SymbolHandler()
	{
		for (int i = 0; i < symbols.Length; i++) symbolMap[symbols[i]] = new Symbol(symbols[i], i);
	}

	static string vecPathString(List<Vector3> locs) {
		if (locs.Count == 0) return "";
		List<Vector2> vec2s = calcVec2(locs);
		List<Vector2> simplified = new List<Vector2>();
		LineUtility.Simplify(vec2s, 2f, simplified);
		vec2s = simplified;
		string xstr = "", ystr = "";
		for (int i = 0; i < vec2s.Count; i++)
		{
			if (i != 0)
			{
				xstr += ",";
				ystr += ",";
			}
			xstr += vec2s[i].x;
			ystr += vec2s[i].y;
		}
		return "[[" + xstr + "],[" + ystr + "]]";
	}

	public static string python_guess(List<Vector3> locs, List<int> seperators=null)
	{
		if (seperators == null) seperators = new List<int>();
		int sepIndex = 0;
		string points = "";
		List<Vector3> smallVecs = new List<Vector3>();
		for (int i = 0; i < locs.Count; i++) {
			if (false&&sepIndex < seperators.Count && i == seperators[sepIndex]) {
				sepIndex++;
				if (sepIndex != 1) points += ",";
				points += vecPathString(smallVecs);
				smallVecs.Clear();
			}
			smallVecs.Add(locs[i]);
		}
		points = "[" + points + vecPathString(smallVecs) + "]";

		string path = "D:/Games/Coding/GoogleDraw - Copy/";
		string filepath = "\"" + path + "classify.py\"";
		string args = "";
		args += " --classes_file=\"" + path + "rnn_tutorial_data/training.tfrecord.classes\"";
		args += " --model_dir=\"" + path + "test\"";
		args += " --predict_for_data=\"" + points + "\"";
		ProcessStartInfo start = new ProcessStartInfo();
		start.FileName = "C:/Users/Mason/AppData/Local/Programs/Python/Python36/python.exe"; //TODO Fix

		start.Arguments = string.Format("{0} {1}", filepath, args);
		start.UseShellExecute = false;
		start.RedirectStandardOutput = true;
		using (Process process = Process.Start(start))
		{
			using (StreamReader reader = process.StandardOutput)
			{
				string result = reader.ReadToEnd();
				UnityEngine.Debug.Log("PYTHONN: '" + result + "'");
				string[] split = result.Split('\n');
				return split[split.Length-2].Split('~')[1];
			}
		}
	}

	public static float[,] getMatrix(List<Vector3> locs, string filepath = null, bool old = false)
	{
		if (filepath == null) return calcMatrix(locs, old);
		filepath = SymbolHandler.getMatrixPath(filepath);

		string googfp = filepath.Replace("/" + inputSizeRoot + "/", "/goog/");
		if (!File.Exists(googfp)) {
			List<Vector2> vec2s = calcVec2(locs, old);
			List<Vector2> simplified = new List<Vector2>();
			LineUtility.Simplify(vec2s, 2f, simplified);
			vec2s = simplified;
			string xstr = "", ystr = "";
			for (int i = 0; i < vec2s.Count; i++) {
				if (i != 0) {
					xstr += ",";
					ystr += ",";
				}
				xstr += vec2s[i].x;
				ystr += vec2s[i].y;
			}
			int last = googfp.LastIndexOf("/");
			System.IO.Directory.CreateDirectory(googfp.Substring(0, last));
			System.IO.File.WriteAllText(googfp, "[[["+xstr+"],["+ystr+"]]]");
		}

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

			float[,] mat = calcMatrix(locs, old);
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

	static float[,] calcMatrix(List<Vector3> locs, bool old = false)
	{
		float[,] mat = new float[inputSizeRoot, inputSizeRoot];

		if (locs.Count == 0) { return mat; }
		Plane plane = averagePlane(locs, old);
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
		cube.transform.right = plane.normal;

		max = max - min;
		float maxVal = Mathf.Max(max.y, max.x);
		float multVal = 0;
		if (maxVal > 0) multVal = (float)(inputSizeRoot - 1) / maxVal;
		float size = .02F;
		cube.transform.localScale = new Vector3(size, size, size);
		List<GameObject> newspheres = new List<GameObject>();
		foreach (Vector3 vec in tests)
		{
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			float s2 = .02F;
			sphere.transform.localScale = new Vector3(s2, s2, s2);

			sphere.transform.parent = cube.transform;
			sphere.transform.position = vec - min + cube.transform.position;
			newspheres.Add(sphere);
		}
		cube.transform.right = new Vector3(0, 0, 1);

		locs.Clear();
		while (newspheres.Count > 0)
		{
			GameObject sphere = newspheres[0];
			Vector3 mod = sphere.transform.position - cube.transform.position;
			if (locs.Count == 0)
			{
				min = mod;
				max = mod;
			}
			else
			{
				min = Vector3.Min(min, mod);
				max = Vector3.Max(max, mod);
			}
			locs.Add(mod);
			newspheres.RemoveAt(0);
			GameObject.Destroy(sphere);
		}
		GameObject.Destroy(cube);

		max = max - min;
		maxVal = Mathf.Max(max.y, max.x);
		if (maxVal > 0) multVal = (float)(inputSizeRoot - 1) / maxVal;
		List<Vector3> velCheck = new List<Vector3>();
		foreach (Vector3 loc in locs)
		{
			Vector3 mod = (loc - min) * multVal;
			mod.z = 0;
			velCheck.Add(mod);
			mat[(int)mod.x, (int)mod.y] = 1;
			if (old || velCheck.Count < 2) continue;
			Vector3 dist = velCheck[velCheck.Count - 1] - velCheck[velCheck.Count - 2];
			if (dist.magnitude > 1)
			{
				mod += dist / 2;
				mod = Vector3.Max(mod, new Vector3(0, 0, 0));
				mod = Vector3.Min(mod, new Vector3(1, 1, 1) * (inputSizeRoot - 1));
				mat[(int)mod.x, (int)mod.y] = 1;
			}
		}

		return mat;
	}

	static List<Vector2> calcVec2(List<Vector3> locs, bool old = false)
	{
		List<Vector2> vec2s = new List<Vector2>();

		if (locs.Count == 0) { return vec2s; }
		Plane plane = averagePlane(locs, old);
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
		cube.transform.right = plane.normal;

		max = max - min;
		float maxVal = Mathf.Max(max.y, max.x);
		float multVal = 0;
		if (maxVal > 0) multVal = (float)(255 - 1) / maxVal;
		float size = .02F;
		cube.transform.localScale = new Vector3(size, size, size);
		List<GameObject> newspheres = new List<GameObject>();
		foreach (Vector3 vec in tests)
		{
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			float s2 = .02F;
			sphere.transform.localScale = new Vector3(s2, s2, s2);

			sphere.transform.parent = cube.transform;
			sphere.transform.position = vec - min + cube.transform.position;
			newspheres.Add(sphere);
		}
		cube.transform.right = new Vector3(0, 0, 1);

		locs.Clear();
		while (newspheres.Count > 0)
		{
			GameObject sphere = newspheres[0];
			Vector3 mod = sphere.transform.position - cube.transform.position;
			if (locs.Count == 0)
			{
				min = mod;
				max = mod;
			}
			else
			{
				min = Vector3.Min(min, mod);
				max = Vector3.Max(max, mod);
			}
			locs.Add(mod);
			newspheres.RemoveAt(0);
			GameObject.Destroy(sphere);
		}
		GameObject.Destroy(cube);

		max = max - min;
		maxVal = Mathf.Max(max.y, max.x);
		if (maxVal > 0) multVal = (float)(255 - 1) / maxVal;
		List<Vector3> velCheck = new List<Vector3>();
		foreach (Vector3 loc in locs)
		{
			Vector3 mod = (loc - min) * multVal;
			mod.z = 0;
			velCheck.Add(mod);
			vec2s.Add(new Vector2((int)mod.x, (int)mod.y));
			
			// Guesses missed points
			//if (old || velCheck.Count < 2) continue;
			//Vector3 dist = velCheck[velCheck.Count - 1] - velCheck[velCheck.Count - 2];
			//if (dist.magnitude > 1)
			//{
			//	mod += dist / 2;
			//	mod = Vector3.Max(mod, new Vector3(0, 0, 0));
			//	mod = Vector3.Min(mod, new Vector3(1, 1, 1) * (inputSizeRoot - 1));
			//	mat[(int)mod.x, (int)mod.y] = 1;
			//}
		}

		return vec2s;
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

	public static Plane averagePlane(List<Vector3> locs, bool old = false) {
		Plane plane = new Plane();
		Vector3 avgNormal = new Vector3(0, 0, 0), avgVec = new Vector3(0, 0, 0);
		int amount = 0;
		for (int i = 0; i< locs.Count; i++) avgVec += locs[i];
		avgVec /= locs.Count;
		IEnumerable<Vector3> randLocs;
		if (old) randLocs = locs;
		else randLocs = Shuffle(locs, new System.Random());
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
		if(symbolMap.ContainsKey(name)) return symbolMap[name];
		return null;
	}

	public static int getSymbolAmount() { return symbols.Length; }

	public static void updateAllFiles() {
		foreach (string direc in Directory.GetDirectories(basepath))
			foreach (FileInfo file in new DirectoryInfo(direc).GetFiles("*.log"))
				if (!File.Exists(direc + "/" + inputSizeRoot + "/" + file.Name) || !File.Exists(direc + "/goog/" + file.Name))
					getMatrix(getVectorsFromFile(direc + "/" + file.Name), direc + "/" + file.Name);
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
