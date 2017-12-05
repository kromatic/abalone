// MoveButton is a MonoBehaviour class representing the buttons used by the human player(s) to make moves.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
	//public string direction;
	// The alpha used to draw the button when it is off.
	public float disabledAlpha;

	// The move to be made when the button is clicked. If null the button is off.
	private Move move;

	// References to the game and displayed board.
	private Game game;	
	private BoardDisplay boardDisplay;

	void Awake()
	{
		game = GameObject.Find("Game").GetComponent<Game>();
		boardDisplay = GameObject.Find("BoardDisplay").GetComponent<BoardDisplay>();
	}

	// What happens when the button is pressed is handled here.
	void OnMouseDown()
	{
		// If the button is on, then we make the move.
		if (move != null)
		{
			game.ProcessMove(move);
		}
	}

	// Enable the button to be pressed.
	public void Enable(Move move)
	{
		this.move = move;
		ChangeAlpha(1);
	}

	// Disable the button.
	public void Disable()
	{
		move = null;
		ChangeAlpha(disabledAlpha);
	}

	// Helper method for changing the alpha of the button.
	private void ChangeAlpha(float alpha)
	{
		var sprite = GetComponent<SpriteRenderer>();
		var color = sprite.color;
		color.a = alpha;
		sprite.color = color;
	}
}
