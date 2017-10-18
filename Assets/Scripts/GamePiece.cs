using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBoard;

public class GamePiece : MonoBehaviour
{
	public Vector2 position;
	public string color = "black";
	public bool selectable = false;
	public bool selected = false;
	public Color selectedColorWhite;
	public Color selectedColorBlack;
	public Color selectableColorWhite;
	public Color selectableColorBlack;

	void OnMouseDown()
	{
		var game = GameObject.Find("Game").GetComponent<Game>();
		if (selectable)
		{
			Debug.Log("completing selection");
			game.CompleteSelection(this);
		}
		else
		{
			Debug.Log("anchoring");
			game.Anchor(this);
		}
	}

	public void Select()
	{
		selected = true;
		GetComponent<SpriteRenderer>().color = (color == "black") ? selectedColorBlack : selectedColorWhite;
	}

	public void MarkSelectable()
	{
		selectable = true;
		GetComponent<SpriteRenderer>().color = (color == "black") ? selectableColorBlack : selectableColorWhite;
	}

	public void Clear()
	{
		selected = false; selectable = false;
		GetComponent<SpriteRenderer>().color = (color == "black") ? Color.black : Color.white;
	}
}
