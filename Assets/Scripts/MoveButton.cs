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
		if (available)
		{
			var game = GameObject.Find("Game").GetComponent<Game>();
			game.MakeMove(direction);
		}
	}

	public void MakeAvailable()
	{
		available = true;
		ChangeAlpha(255);
	}

	public void MakeUnavailable()
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
