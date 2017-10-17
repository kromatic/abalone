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
	public float selectableAlpha = 0.7f;
	public float selectedAlpha = 0.5f;

	void OnMouseDown()
	{
		var game = GameObject.Find("Game").GetComponent<Game>();
		if (selectable)
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

	public void Select()
	{
		selected = true;
		var sprite = GetComponent<SpriteRenderer>();
		var color = sprite.color; color.a = selectedAlpha;
		sprite.color = color;
	}

	public void MarkSelectable()
	{
		selectable = true;
		var sprite = GetComponent<SpriteRenderer>();
		var color = sprite.color; color.a = selectableAlpha;
		sprite.color = color;
	}

	public void Clear()
	{
		selected = false; selectable = false;
		var sprite = GetComponent<SpriteRenderer>();
		var color = sprite.color; color.a = 1;
		sprite.color = color;
	}
}
