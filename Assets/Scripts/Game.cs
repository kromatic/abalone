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
	private List<Vector> selection;
	private string selectionDirection;
	private List<Vector> potentialSelection;
	private Dictionary<string, List<Vector>> potentialMoves;

	void Awake()
	{
		board = GameObject.Find("Board").GetComponent<Board>();
	}

	public void CompleteSelection(int distance, string dir)
	{
		selection = Board.GetColumn(anchorLocation, distance, dir);
		board.ResetPieces(potentialSelection);
		potentialSelection.Clear();
		board.Select(selection);
		potentialMoves = board.GetMoves(selection, selectionDirection);
		// Debug.Log(potentialMoves.Count);
		ChangeButtonsStatus(true);
	}

	public void MakeMove(string dir)
	{
		board.ResetPieces(selection);
		board.Move(selection, dir);
		board.Move(potentialMoves[dir], dir);
		selection.Clear();
		ChangeButtonsStatus(false);
	}

	private void ChangeButtonsStatus(bool activate)
	{
		var action = (activate) ? (Action<MoveButton>)DeactivateButton : ActivateButton;
		foreach (var direction in potentialMoves.Keys)
		{	
			var button = GameObject.Find("Move" + direction).GetComponent<MoveButton>();
			action(button);
		}
	}

	private static void DeactivateButton(MoveButton button)
	{
		button.Deactivate();
	}

	private static void ActivateButton(MoveButton button)
	{
		button.Activate();
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
			CompleteSelection(0, "");
		}
	}
}
