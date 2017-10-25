using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{
	public int player1Score = 0;
	public int player2Score = 0;
	private Board board;
	private Vector anchorLocation;
	private List<Vector> selection = new List<Vector>();
	private string selectionDirection = null;
	private List<Vector> potentialSelection = new List<Vector>();
	private Dictionary<string, List<Vector>> potentialMoves;

	void Awake()
	{
		board = GameObject.Find("Board").GetComponent<Board>();
	}

	public void CompleteSelection(int distance, string dir)
	{
		selection = Board.GetColumn(anchorLocation, distance, dir);
		board.ResetPieces(potentialSelection);
		board.Select(selection);
		potentialMoves = board.GetMoves(selection, selectionDirection);
	}


	public void Anchor(Vector anchorLocation)
	{
		this.anchorLocation = anchorLocation;
		if (selection.Count > 0)
		{
			board.ResetPieces(selection);
			selection.Clear();

		}
		else if (potentialSelection.Count > 0)
		{
			board.ResetPieces(potentialSelection);
			potentialSelection.Clear();
		}
		board.GetSpace(anchorLocation).piece.anchor = true;
		potentialSelection = board.GetPotentialSelection(anchorLocation);
		if (potentialSelection.Count == 1)
		{
			CompleteSelection(0, null);
		}
	}
}
