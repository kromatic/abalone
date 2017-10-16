using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBoard;

public class GamePiece : MonoBehaviour
{
	public Vector2 position;
	// public bool anchor = false;
	public bool completesSelection = false;
	public bool selected = false;

	void OnMouseDown()
	{
		var game = GameObject.Find("Game").GetComponent<Game>();
		if (completesSelection)
		{
			Debug.Log("completing selection");
			game.CompleteSelection(position);
		}
		else
		{
			Debug.Log("anchoring");
			game.Anchor(position);
		}
	}
}
