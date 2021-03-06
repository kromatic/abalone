using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OldGame : MonoBehaviour
{
	public Dictionary<char, int> scores;
	public string currentPlayer;
	private Board board;

    private Vector anchorLocation;
	private List<Vector> selection = new List<Vector>();
	private string selectionDirection;
	private List<Vector> potentialSelection = new List<Vector>();
	private Dictionary<string, List<Vector>> potentialMovesSumito;

	void Awake()
	{
		board = new Board();
		scores = new Dictionary<char, int> { {'B', 0}, {'W', 0} };
	}

	public void CompleteSelection(int distance, string dir)
	{
		selection = UserBoard.GetColumn(anchorLocation, distance, dir);
		selectionDirection = dir;
		board.ResetPieces(potentialSelection);
		potentialSelection.Clear();
		board.Select(selection);
		potentialMovesSumito = board.GetMoves(selection, selectionDirection);
		// Debug.Log(potentialMoves.Count);
		ChangeButtonsStatus(true);
	}

	public void MakeMove(string dir)
	{
		board.ResetPieces(selection);
		int scoreDelta = board.Move(selection, potentialMovesSumito[dir], dir);
		scores[currentPlayer] += scoreDelta;
		Debug.Log(scores["black"]);
		Debug.Log(scores["white"]);
		selection.Clear();
		ChangeButtonsStatus(false);
		if (scores[currentPlayer] == 6)
		{
			currentPlayer = "none";
		}
		else
		{
			currentPlayer = (currentPlayer == "black") ? "white" : "black";
		}
	}

	private void ChangeButtonsStatus(bool activate)
	{
		var action = (activate) ? (Action<MoveButton>)ActivateButton : DeactivateButton;
		foreach (var direction in potentialMovesSumito.Keys)
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
			ChangeButtonsStatus(false);
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
