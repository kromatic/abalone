using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{
	public int player1Score = 0;
	public int player2Score = 0;
	private Board board;
	private GamePiece anchor;
	private List<GamePiece> selection = new List<GamePiece>();
	private List<GamePiece> potentialSelection = new List<GamePiece>();
	private List<string> moves = new List<string>();

	void Awake()
	{
		board = GameObject.Find("Board").GetComponent<Board>();
	}

	public void CompleteSelection(GamePiece piece)
	{
		selection.Add(anchor);
		int x = anchor.location.x, y = anchor.location.y;
		int endX = piece.location.x, endY = piece.location.y;
		var deltaX = Math.Sign(endX - x);
		var deltaY = Math.Sign(endY - y);
		x += deltaX; y += deltaY;
		if (x != endX || y != endY)
		{
			selection.Add(board.GetPiece(new Location(x, y)));
		}
		selection.Add(piece);
		board.ResetPieces(potentialSelection);
		board.Select(selection);
		moves = board.GetMoves(selection);
	}

	public void Anchor(GamePiece anchor)
	{
		this.anchor = anchor;
		if (selection.Count > 0)
		{
			board.ResetPieces(selection);
		}
		else if (potentialSelection.Count > 0)
		{
			board.ResetPieces(potentialSelection);
		}
		if (anchor == null)
		{
			Debug.Log("asdlfk");
		}

		board.GetPotentialSelection(anchor, potentialSelection);
		if (potentialSelection.Count == 1)
		{
			CompleteSelection(anchor);
		}
	}
}
