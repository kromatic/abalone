using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBoard;
using System;

public class Game : MonoBehaviour
{
	public Transform boardPrefab;
	public Transform spacePrefab;
	public Transform piecePrefab;
	public int player1Score = 0;
	public int player2Score = 0;
	private Board board;
	private GamePiece anchor;
	private List<GamePiece> selection = new List<GamePiece>();
	private List<GamePiece> potentialSelection = new List<GamePiece>();

	void Awake()
	{
		board = new Board(boardPrefab, spacePrefab, piecePrefab);
	}

	public void CompleteSelection(GamePiece piece)
	{
		selection.Add(anchor);
		int x = (int)anchor.position.x, y = (int)anchor.position.y;
		int endX = (int)piece.position.x, endY = (int)piece.position.y;
		var deltaX = Math.Sign(endX - x);
		var deltaY = Math.Sign(endY - y);
		x += deltaX; y += deltaY;
		if (x != endX || y != endY)
		{
			selection.Add(board.GetPiece(new Vector2(x, y)));
		}
		selection.Add(piece);
		board.ResetPieces(potentialSelection);
		board.Select(selection);
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
		board.GetPotentialSelection(anchor, potentialSelection);
		if (potentialSelection.Count == 1)
		{
			CompleteSelection(anchor);
		}
	}
}
