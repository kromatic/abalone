using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
	public Location location;
	public string color = "black";
	public bool anchor = false;
	public bool selectable = false;
	public bool selected = false;
	public Color normalColor;
	public Color selectedColor;
	public Color selectableColor;
	public Color anchorColor;

	void OnMouseDown()
	{
		var game = GameObject.Find("Game").GetComponent<Game>();
		if (selectable)
		{
			game.CompleteSelection(this);
		}
		else
		{
			anchor = true;
			game.Anchor(this);
		}
	}

	public void Select()
	{
		selected = true;
		GetComponent<SpriteRenderer>().color = selectedColor;
	}

	public void MarkSelectable()
	{
		selectable = true;
		GetComponent<SpriteRenderer>().color = (anchor == true) ? anchorColor : selectableColor;

	}

	public void Clear()
	{
		selected = false; selectable = false; anchor = false;
		GetComponent<SpriteRenderer>().color = normalColor;
	}
}
