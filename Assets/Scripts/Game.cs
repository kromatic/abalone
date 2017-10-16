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
		var deltaX = Math.Sign(position[0] - selectionAnchor[1]);
		var deltaY = Math.Sign(position[1] - selectionAnchor[1]);
		int i = (int)selectionAnchor[0], j = (int)selectionAnchor[1];
		while ((i != position[0]) || (j != position[1]))
		{
			selection.Add(new Vector2(i, j));	
			i += deltaX; j += deltaY;
		}
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
		potentialSelection = board.GetPotentialSelection(position);
		if (potentialSelection.Count == 1)
		{
			CompleteSelection(position);
		}
	}
}
