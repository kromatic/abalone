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

	// Board references instance of logical representation of game board.
	// Cannot be set publicly. 
	public Board Board { get; private set; }
	
	// Common strings used to display information to user stored for convenience.
	public string blackTurnMessage;
	public string whiteTurnMessage;
	public string blackWinsMessage;
	public string whiteWinsMessage;

	// scores['B'] and scores['W'] store black and white scores, respectively.
	private Dictionary<char, int> scores;

	// displayedScores['B'/'W'] store references to text objects displaying black/white scores.
	private Dictionary<char, Text> displayedScores;

	// Reference to text object displaying current status of game (current turn / game over).
	private Text gameStatus;

	void Awake()
	{
		// Create a board and initialize scores and current player.
		Board = new Board();
		scores = new Dictionary<char, int> { {'B', 0}, {'W', 0} };
		CurrentPlayer = 'B';

		// Find text objects displaying scores and game status.
		var blackScoreText = GameObject.Find("BlackScore").GetComponent<Text>();
		var whiteScoreText = GameObject.Find("WhiteScore").GetComponent<Text>();
		displayedScores = new Dictionary<char, Text> { {'B', blackScoreText}, {'W', whiteScoreText} };
		gameStatus = GameObject.Find("GameStatus").GetComponent<Text>();
	}

	// Update game state based on outcome of move.
	// scoreDelta stores number of pieces displaced by the move (always 0 or 1).
	public void NextTurn(int scoreDelta)
	{
		// Update scores.
		scores[CurrentPlayer] += scoreDelta;
		displayedScores[CurrentPlayer].text = ScoreMessage(CurrentPlayer);

		// End game or switch turns and update game status display, based on score.
		if (scores[CurrentPlayer] == 6)
		{
			EndGame();
		}
		else
		{
			CurrentPlayer = (CurrentPlayer == 'B') ? 'W' : 'B';
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
	}

	// End game.
	private void EndGame()
	{
		// Disable board pieces by setting current player ('N' stands for "None").
		CurrentPlayer = 'N';
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
