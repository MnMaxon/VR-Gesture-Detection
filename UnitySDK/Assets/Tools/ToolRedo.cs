using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolRedo : Tool {

	public override void handUpdate(GameObject handOb, bool pinch, bool startButton)
	{
		if (startButton) RedoManager.redo();
		if (pinch) RedoManager.undo();
	}

	public override string getName()
	{
		return "Brush";
	}
}
