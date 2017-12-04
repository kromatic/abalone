// This class contains the game state, such as the current player, scores, etc.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	// Players are represented by the chars 'B' and 'W'.
	// CurrentPlayer references player whose turn it is. Cannot be set publicly.
	public char CurrentPlayer { get; private set; }

	public bool GameOver { get; private set; }

	// Board references instance of logical representation of game board.
	// Cannot be set publicly. 
	public Board Board { get; private set; }
	
	// Common strings used to display information to user stored for convenience.
	public string blackTurnMessage;
	public string whiteTurnMessage;
	public string blackWinsMessage;
	public string whiteWinsMessage;
	public Button undoButton;
	public Button redoButton;

	// Reference to the displayed board.
	private BoardDisplay boardDisplay;

	// scores['B'] and scores['W'] store black and white scores, respectively.
	private Dictionary<char, int> scores;

	// displayedScores['B'/'W'] store references to text objects displaying black/white scores.
	private Dictionary<char, Text> displayedScores;

	// Reference to text object displaying current status of game (current turn / game over).
	private Text gameStatus;

	// Linked list containing history of made moves.
	// Used for undo / redo functionality.
	private LinkedList<Move> moveHistory;
	// Reference to the node in moveHistory that should be the next one to be undone.
	private LinkedListNode<Move> moveHistoryPointer;

	void Awake()
	{
		// Create a board and initialize scores and current player.
		Board = new Board();
		scores = new Dictionary<char, int> { {'B', 0}, {'W', 0} };
		CurrentPlayer = 'B';
		moveHistory = new LinkedList<Move>();
		moveHistoryPointer = moveHistory.Last;
		boardDisplay = GameObject.Find("BoardDisplay").GetComponent<BoardDisplay>();
		GameOver = false;

		// Find text objects displaying scores and game status.
		var blackScoreText = GameObject.Find("BlackScore").GetComponent<Text>();
		var whiteScoreText = GameObject.Find("WhiteScore").GetComponent<Text>();
		displayedScores = new Dictionary<char, Text> { {'B', blackScoreText}, {'W', whiteScoreText} };
		gameStatus = GameObject.Find("GameStatus").GetComponent<Text>();
	}

	public void MakeMove(Move move, bool newMove = true, bool undoMove = false)
	{
		// First make the move and update the state of the game.
		int scoreDelta = (!undoMove) ? Board.Move(move) : Board.UndoMove(move);
		NextTurn(scoreDelta, (newMove) ? move : null);
		// Then update the board display.
		boardDisplay.ClearSelected();
		boardDisplay.UpdateView();
		boardDisplay.DisableMoveButtons();
		// Flip the board if necessary.
		if (boardDisplay.FlipEveryTurn) boardDisplay.Flip();
	}

	// Update game state based on outcome of move.
	// scoreDelta stores number of pieces displaced by the move (always 0 or 1).
	private void NextTurn(int scoreDelta, Move lastMove)
	{
		// Update scores.
		scores[CurrentPlayer] += scoreDelta;
		displayedScores[CurrentPlayer].text = ScoreMessage(CurrentPlayer);
		// If lastMove is not null, we should add it to the history.
		// In this case a new move not (necessarily) in the history is being made.
		// When lastMove is null, that means that NextTurn is being called by Undo or Redo methods,
		// which handle the moveHistory themselves.
		if (lastMove != null)
		{
			// First remove any undone moves from the history, since a fresh move is being made. 
			while (moveHistoryPointer != moveHistory.Last)
			{
				moveHistory.RemoveLast();
			}
			// Then add our move and update the pointer.
			moveHistory.AddLast(lastMove);
			moveHistoryPointer = moveHistory.Last;
			undoButton.interactable = true;
			redoButton.interactable = false;
		}

		CurrentPlayer = (CurrentPlayer == 'B') ? 'W' : 'B';
		// End game or update game status display, based on score.
		if (scores[CurrentPlayer] == 6)
		{
			EndGame();
		}
		else
		{
			GameOver = false;
			gameStatus.text = (CurrentPlayer == 'B') ? blackTurnMessage : whiteTurnMessage;
		}
	}

	// Restart game.
	public void Restart()
	{
		// Create new board, reset turn to black, and zero scores.
		Board = new Board();
		CurrentPlayer = 'B';
		gameStatus.text = blackTurnMessage;
		foreach (var player in new List<char> {'B', 'W'})
		{
			// Debug.Log(player);
			scores[player] = 0;
			displayedScores[player].text = ScoreMessage(player);
		}
		// Reset move history.
		moveHistory = new LinkedList<Move>();
		moveHistoryPointer = moveHistory.Last;
		undoButton.interactable = redoButton.interactable = false;
	}

	// Undo a move.
	public void Undo()
	{
		MakeMove(moveHistoryPointer.Value, newMove: false, undoMove: true);
		moveHistoryPointer = moveHistoryPointer.Previous;
		if (moveHistoryPointer == null) undoButton.interactable = false;
		redoButton.interactable = true;
	}

	// Redo a move.
	public void Redo()
	{
		MakeMove(moveHistoryPointer.Next.Value, newMove: false);
		moveHistoryPointer = moveHistoryPointer.Next;
		if (moveHistoryPointer == moveHistory.Last) redoButton.interactable = false;
		undoButton.interactable = true;
	}

	// End game.
	private void EndGame()
	{
		GameOver = true;
		// Display victory message based on who won.
		gameStatus.text = (scores['B'] == 6) ? blackWinsMessage : whiteWinsMessage; // show a message
	}

	// Helper function for creating player score message string.
	private string ScoreMessage(char player)
	{
		string prefix = (player == 'B') ? "Black: " : "White: ";
		return prefix + scores[player].ToString();
	}
}
