using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
	public string direction;
	public float disabledAlpha;
	private Board board;
	private bool enabled;
	private List<Vector> enemyColumn;

	void Awake()
	{
		enabled = false;
		boardDisplay = GameObject.Find("BoardDisplay").GetComponent<BoardDisplay>();
	}

	void OnMouseDown()
	{
		// Debug.Log("pressing");
		if (enabled)
		{
			// Debug.Log("im trying to move");
			// var game = GameObject.Find("Game").GetComponent<Game>();
			boardDisplay.MakeMove(direction);
		}
	}

	public void Enable(List<Vector> enemyColumn)
	{
		enabled = true;
		enemyColumn = 
		// Debug.Log("making button clickable");
		ChangeAlpha(1);
	}

	public void Disable()
	{
		enabled = false;
		enemyColumn = null;
		ChangeAlpha(disabledAlpha);
	}

	private void ChangeAlpha(float alpha)
	{
		var sprite = GetComponent<SpriteRenderer>();
		var color = sprite.color;
		color.a = alpha;
		sprite.color = color;
	}
}
