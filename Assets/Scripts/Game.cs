// This class contains the game state, such as the current player, scores, etc.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	// Board references instance of logical representation of game board.
	// Cannot be set publicly. 
	public Board Board { get; private set; }

	// Reference to the displayed board.
	public BoardDisplay boardDisplay;

	// Players are represented by the chars 'B' and 'W'.
	// CurrentPlayer references player whose turn it is. Cannot be set publicly.
	public char CurrentPlayer { get; private set; }

	// Is the game over?
	public bool GameOver { get; private set; }
	
	// Common strings used to display information to user stored for convenience.
	public string blackTurnMessage;
	public string whiteTurnMessage;
	public string blackWinsMessage;
	public string whiteWinsMessage;

	// Undo and redo buttons.
	public Button undoButton;
	public Button redoButton;

	// Reference to text object displaying current status of game (current turn / game over).
	public Text gameStatus;

	// scores['B'] and scores['W'] store black and white scores, respectively.
	private Dictionary<char, int> scores;

	// displayedScores['B'/'W'] store references to text objects displaying black / white scores.
	private Dictionary<char, Text> displayedScores;

	// Linked list containing history of made moves.
	// Used for undo / redo functionality.
	private LinkedList<Move> moveHistory;
	// Reference to the node in moveHistory that should be the next one to be undone.
	private LinkedListNode<Move> moveHistoryPointer;

	void Awake()
	{
		// Create a board and initialize game state.
		Board = new Board();
		CurrentPlayer = 'B';
		GameOver = false;
		scores = new Dictionary<char, int> { {'B', 0}, {'W', 0} };
		moveHistory = new LinkedList<Move>();
		moveHistoryPointer = moveHistory.Last;
		// boardDisplay = GameObject.Find("BoardDisplay").GetComponent<BoardDisplay>();

		// Find text objects displaying scores and game status.
		var blackScoreText = GameObject.Find("BlackScore").GetComponent<Text>();
		var whiteScoreText = GameObject.Find("WhiteScore").GetComponent<Text>();
		displayedScores = new Dictionary<char, Text> { {'B', blackScoreText}, {'W', whiteScoreText} };
		// gameStatus = GameObject.Find("GameStatus").GetComponent<Text>();
	}

	public void ProcessMove(Move move, bool newMove = true, bool undoMove = false)
	{
		// First make the move and update the state of the game.
		int scoreDelta = (!undoMove) ? Board.Move(move) : Board.UndoMove(move);
		NextTurn(scoreDelta);
		// If this is a new move, i.e. we are not undoing or redoing a move, then we add it to the history.
		while (moveHistory.Last != moveHistoryPointer) moveHistory.RemoveLast();
		moveHistory.AddLast(move);
		undoButton.interactable = true; redoButton.interactable = false;
		// Then update the board display.
		boardDisplay.ClearSelected();
		boardDisplay.UpdateView();
		boardDisplay.DisableMoveButtons();
		// Flip the board if necessary.
		if (boardDisplay.FlipEveryTurn) boardDisplay.Flip();
	}

	// Update game state based on outcome of move.
	// scoreDelta stores number of pieces displaced by the move (always 0 or 1).
	private void NextTurn(int scoreDelta)
	{
		// Update scores.
		scores[CurrentPlayer] += scoreDelta;
		displayedScores[CurrentPlayer].text = ScoreMessage(CurrentPlayer);
		// End game if current player's score is now 6.
		if (scores[CurrentPlayer] == 6)
		{
			GameOver = true;
			// Display victory message based on who won.
			gameStatus.text = (CurrentPlayer == 'B') ? blackWinsMessage : whiteWinsMessage;
		}
		// Otherwise indicate the next turn.
		// If the game has ended, but NextTurn is called, that means that a move is being undone.
		// In that case game is no longer over.
		else
		{
			if (GameOver) GameOver = false;
			gameStatus.text = (CurrentPlayer == 'B') ? whiteTurnMessage : blackTurnMessage;
		}
		CurrentPlayer = (CurrentPlayer == 'B') ? 'W' : 'B';
	}

	// Restart game.
	public void Restart()
	{
		// Create new board, reset turn to black, and zero scores.
		Board = new Board();
		CurrentPlayer = 'B';
		gameStatus.text = blackTurnMessage;
		foreach (var player in "BW")
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
		ProcessMove(moveHistoryPointer.Value, newMove: false, undoMove: true);
		moveHistoryPointer = moveHistoryPointer.Previous;
		if (moveHistoryPointer == null) undoButton.interactable = false;
		redoButton.interactable = true;
	}

	// Redo a move.
	public void Redo()
	{
		ProcessMove(moveHistoryPointer.Next.Value, newMove: false);
		moveHistoryPointer = moveHistoryPointer.Next;
		if (moveHistoryPointer == moveHistory.Last) redoButton.interactable = false;
		undoButton.interactable = true;
	}

	// Helper function for creating player score message string.
	private string ScoreMessage(char player)
	{
		string prefix = (player == 'B') ? "Black: " : "White: ";
		return $"{prefix} {scores[player].ToString()}";
	}
}
