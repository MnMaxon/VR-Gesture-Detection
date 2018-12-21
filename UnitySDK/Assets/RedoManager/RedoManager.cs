using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using System;

public class RedoManager {
	static Stack<RedoObject> undoStack = new Stack<RedoObject>(100);
	static Stack<RedoObject> redoStack = new Stack<RedoObject>(100);

	public static void addRedoObject(RedoObject ro) {
		undoStack.Push(ro);
	}

	public static void undo() {
		RedoObject ro;
		try { ro = undoStack.Pop(); }
		catch (InvalidOperationException ex) { return; }
		ro = handleRedoObject(ro);
		if (ro != null) redoStack.Push(ro);
	}

	public static void redo()
	{
		RedoObject ro;
		try { ro = redoStack.Pop(); }
		catch (InvalidOperationException ex) { return; }
		ro = handleRedoObject(ro);
		if(ro != null) undoStack.Push(ro);
	}

	public static RedoObject handleRedoObject(RedoObject ro) {
		UncreateObject uco = ro as UncreateObject;
		if (uco != null) {
			GameObject go = PropHandler.objectFromId(uco.id);
			Prop prop = go.GetComponent<Prop>();
			UnmoveObject unmoveObject = new UnmoveObject(uco.id, go.transform, prop.name, prop.paintHistory);
			PropHandler.untrack(go, true);
			return unmoveObject;
		}

		UnmoveObject umo = ro as UnmoveObject;
		if (umo != null)
		{
			GameObject go = PropHandler.objectFromId(umo.id);
			RedoObject redoOb;
			if (go == null)
			{
				go = GameObject.Instantiate(PropHandler.props[umo.propName]);
				Prop prop = go.GetComponent<Prop>();
				prop.paintHistory = umo.paintHistory;
				prop.propObjectId = umo.id;
				foreach (UnpaintObject unp in prop.paintHistory) Prop.paintFromPath(go, unp.getNewColor(), unp.getColorPath(), false, false);
				PropHandler.track(go);
				
				redoOb = new UncreateObject(umo.id);
			}
			else redoOb = new UnmoveObject(umo.id, go.transform.position, go.transform.rotation, go.transform.localScale, umo.propName);
			go.transform.position = umo.position;
			go.transform.rotation = umo.rotation;
			go.transform.localScale = umo.localScale;
			return redoOb;
		}

		UnpaintObject upo = ro as UnpaintObject;
		if (upo != null)
		{
			GameObject go = PropHandler.objectFromId(upo.id);
			return Prop.paintFromPath(go, upo.getNewColor(), upo.getColorPath(), true, false);
		}
		return null;
	}
}