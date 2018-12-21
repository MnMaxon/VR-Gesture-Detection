using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class GameInitializer : MonoBehaviour {
	public static GameInitializer instance;

	public GameObject propMenu;
	public GameObject saveMenu;
	public GameObject textTooltipPrefab;
	//public TextAsset graphModel;
	public GameObject FollowHead;
	public GameObject squareHoverPrefab;
	public GameObject circleHoverPrefab;
	public GameObject starHoverPrefab;
	public GameObject diamondHoverPrefab;
	public GameObject squiggleHoverPrefab;
	public GameObject triangleHoverPrefab;
	public GameObject furniturePrefab;
	public GameObject wallBuilderPrefab;
	public GameObject lightPrefab;
	public GameObject chairPrefab;
	public GameObject sunPrefab;

	public GameObject[] props;

	public Material outlineMat, clearMat;

	private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	private byte[] _recieveBuffer = new byte[8142];

	Int32 port = 50003;
	public string recieved = null;

	internal Boolean socket_ready = false;
	internal String input_buffer = "";

	Process python;

	TcpClient tcp_socket;
	NetworkStream net_stream;

	StreamWriter socket_writer;
	StreamReader socket_reader;

	// Use this for initialization
	void Start () {
		instance = this;
		//SymbolHandler.graphModel = graphModel;

		Symbol square = SymbolHandler.fromName("Square");
		Symbol circle = SymbolHandler.fromName("Circle");
		Symbol star = SymbolHandler.fromName("Star");
		Symbol diamond = SymbolHandler.fromName("Diamond");
		Symbol squiggle = SymbolHandler.fromName("Squiggle");
		Symbol triangle = SymbolHandler.fromName("Triangle");
		SymbolHandler.fromName("Door").setHoverPrefab(furniturePrefab);
		SymbolHandler.fromName("Bed").setHoverPrefab(furniturePrefab);
		SymbolHandler.fromName("Dresser").setHoverPrefab(furniturePrefab);
		SymbolHandler.fromName("Sun").setHoverPrefab(sunPrefab);
		SymbolHandler.fromName("Floor Lamp").setHoverPrefab(lightPrefab);
		SymbolHandler.fromName("House").setHoverPrefab(wallBuilderPrefab);
		SymbolHandler.fromName("Chair").setHoverPrefab(chairPrefab);

		square.setHoverPrefab(squareHoverPrefab);
		circle.setHoverPrefab(circleHoverPrefab);
		star.setHoverPrefab(starHoverPrefab);
		diamond.setHoverPrefab(diamondHoverPrefab);
		squiggle.setHoverPrefab(squiggleHoverPrefab);
		triangle.setHoverPrefab(triangleHoverPrefab);

		foreach (GameObject go in props) PropHandler.register(go);

		startPython();
		StartCoroutine(SetupServer());

	}
	
	void Update()
	{
		//SendData("ping");
	}

	public void startPython(){
        ProcessStartInfo pythonInfo = new ProcessStartInfo();
        pythonInfo.FileName= @"C:\Users\Mason\AppData\Local\Programs\Python\Python36\python.exe";
        pythonInfo.Arguments= "\"D:/Unity Projects/ml-agents-master/QuickDraw-master/MyQD.py\"";
        pythonInfo.CreateNoWindow = false;
        pythonInfo.UseShellExecute = false;
		pythonInfo.RedirectStandardInput = false;
		pythonInfo.RedirectStandardOutput = false;
		python = Process.Start (pythonInfo);
        //python.WaitForExit ();
        //python.Close ();
    }

	void OnApplicationQuit()
	{
		SendData("Kill");
		_clientSocket.Close();
		python.Close();
	}

	IEnumerator SetupServer(int count = 0)
	{
		if (count < 30) yield return null;
		bool failed = false;
		try
		{
			_clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, port));
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log(ex.Message);
			failed = true;
		}
		if (failed)
		{
			yield return new WaitForSeconds(5);
			StartCoroutine(SetupServer(count + 1));
		}
		else
		{
			_clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
			yield return null;
		}

	}

	private void ReceiveCallback(IAsyncResult AR)
	{
		//Check how much bytes are recieved and call EndRecieve to finalize handshake
		int recieved = _clientSocket.EndReceive(AR);

		if (recieved <= 0)
			return;

		//Copy the recieved data into new buffer , to avoid null bytes
		byte[] recData = new byte[recieved];
		Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);

		//Process data here the way you want , all your bytes will be stored in recData
		this.recieved = System.Text.Encoding.Default.GetString(_recieveBuffer);
		UnityEngine.Debug.Log(this.recieved);
		//SendData("ping");
	}

	public void SendData(string data)
	{
		data += "--";
		UnityEngine.Debug.Log("PRINTING " + data);
		SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
		socketAsyncData.SetBuffer(System.Text.Encoding.Default.GetBytes(data), 0, data.Length);
		_clientSocket.SendAsync(socketAsyncData);
		_clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
	}

	public string waitForRecieve(bool consume = true) {
		while (recieved == null) ;
		string str = recieved;
		if (consume) recieved = null;
		return str;
	}
}
