using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
	public string direction;
	public float disabledAlpha;
	private bool on;
	private List<Vector> selection;
	private string selectionDirection;
	private List<Vector> enemyColumn;
	private Board board;
	private BoardDisplay boardDisplay;
	private Game game;

	void Awake()
	{
		on = false;
		boardDisplay = GameObject.Find("BoardDisplay").GetComponent<BoardDisplay>();
		game = GameObject.Find("Game").GetComponent<Game>();
	}

	void Start()
	{
		board = game.board;
		// Disable();
	}

	void OnMouseDown()
	{
		// Debug.Log("pressing");
		if (on)
		{
			// Debug.Log("im trying to move");
			// var game = GameObject.Find("Game").GetComponent<Game>();
			boardDisplay.ClearSelection();
			int scoreDelta = board.Move(selection, selectionDirection, enemyColumn, direction);
			boardDisplay.UpdateView();
			game.NextTurn(scoreDelta);
			foreach (var direction in Board.directions.Keys)
			{
				var moveButton = GameObject.Find("Move" + direction).GetComponent<MoveButton>();
				moveButton.Disable();
			}
		}
	}

	public void Enable(List<Vector> selection, string selectionDirection, List<Vector> enemyColumn)
	{
		on = true;
		this.selection = selection; this.selectionDirection = selectionDirection;
		this.enemyColumn = enemyColumn;
		// Debug.Log("making button clickable");
		ChangeAlpha(1);
	}

	public void Disable()
	{
		on = false;
		// selection = enemyColumn = null; selectionDirection = "";
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
