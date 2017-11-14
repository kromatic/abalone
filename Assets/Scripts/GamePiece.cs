using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
	public Vector location;
	public char color;
	public Color normalColor;
	public Color selectedColor;
	public Color selectableColor;
	public Color anchorColor;
	private bool anchor;
	private bool selectable;
	private string selectableDirection;
	private BoardDisplay boardDisplay;
	private Game game;
	// private bool selected = false;

	void Awake()
	{
		boardDisplay = GameObject.Find("BoardDisplay").GetComponent<BoardDisplay>();
		game = GameObject.Find("Game").GetComponent<Game>();
	}

	void OnMouseDown()
	{
		if (color != game.currentPlayer) return;
		if (selectable)
		{
			boardDisplay.CompleteSelection(location, selectableDirection);
		}
		else
		{
			boardDisplay.Anchor(location);
		}
	}

	public void Select()
	{
		// selected = true;
		GetComponent<SpriteRenderer>().color = selectedColor;
	}

	public void MarkSelectable(string direction)
	{
		selectable = true;
		selectableDirection = direction;
		GetComponent<SpriteRenderer>().color = (anchor == true) ? selectedColor : selectableColor;
	}

	public void Clear()
	{
		selectable = false; anchor = false; // selected = false;
		// selectableDirection = "";
		GetComponent<SpriteRenderer>().color = normalColor;
	}
}
