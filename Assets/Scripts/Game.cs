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
	private Vector2 selectionAnchor;
	private List<Vector2> selection = new List<Vector2>();
	private List<Vector2> potentialSelection = new List<Vector2>();

	void Awake()
	{
		board = new Board(boardPrefab, spacePrefab, piecePrefab);
	}

	public void CompleteSelection(Vector2 position)
	{
		selection.Add(selectionAnchor);
		int x = (int)selectionAnchor[0], y = (int)selectionAnchor[1];
		int endX = (int)position[0], endY = (int)position[1];
		var deltaX = Math.Sign(endX - x);
		var deltaY = Math.Sign(endY - y);
		x += deltaX; y += deltaY;
		if (x != endX || y != endY) selection.Add(new Vector2(x, y));
		selection.Add(position);
		board.ResetPieces(potentialSelection);
		board.Select(selection);
	}

	public void Anchor(Vector2 position)
	{
		selectionAnchor = position;
		if (selection.Count > 0)
		{
			board.ResetPieces(selection);
		}
		else if (potentialSelection.Count > 0)
		{
			board.ResetPieces(potentialSelection);
		}
		board.GetPotentialSelection(position, potentialSelection);
		if (potentialSelection.Count == 1)
		{
			CompleteSelection(position);
		}
	}
}
