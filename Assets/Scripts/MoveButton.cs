using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
	public string direction;
	public float unavailableAlpha;
	private bool available = false;

	void OnMouseDown()
	{
		Debug.Log("pressing");
		if (available)
		{
			Debug.Log("im trying to move");
			var game = GameObject.Find("Game").GetComponent<Game>();
			game.MakeMove(direction);
		}
	}

	public void Activate()
	{
		available = true;
		Debug.Log("making button clickable");
		ChangeAlpha(1);
	}

	public void Deactivate()
	{
		available = false;
		ChangeAlpha(unavailableAlpha);
	}

	private void ChangeAlpha(float alpha)
	{
		var sprite = GetComponent<SpriteRenderer>();
		var color = sprite.color;
		color.a = alpha;
		sprite.color = color;
	}
}
