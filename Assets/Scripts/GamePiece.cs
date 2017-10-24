using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
	public Vector location;
	public string color = "black";
	public bool anchor = false;
	private bool selectable = false;
	private int selectableDistance = -1;
	private string selectableDirection = null;
	private bool selected = false;
	public Color normalColor;
	public Color selectedColor;
	public Color selectableColor;
	public Color anchorColor;

	void OnMouseDown()
	{
		var game = GameObject.Find("Game").GetComponent<Game>();
		if (selectable)
		{
			game.CompleteSelection(selectableDistance, selectableDirection);
		}
		else
		{
			game.Anchor(location);
		}
	}

	public void Select()
	{
		selected = true;
		GetComponent<SpriteRenderer>().color = selectedColor;
	}

	public void MarkSelectable(int distance, string dir)
	{
		selectable = true;
		selectableDistance = distance;
		selectableDirection = dir;
		GetComponent<SpriteRenderer>().color = (anchor == true) ? anchorColor : selectableColor;
	}

	public void Clear()
	{
		selected = false; selectable = false; anchor = false;
		selectableDistance = -1; selectableDirection = null;
		// Debug.Log("here");
		GetComponent<SpriteRenderer>().color = normalColor;
	}
}
