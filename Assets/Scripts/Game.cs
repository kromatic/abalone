using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Game : MonoBehaviour
{
	public char currentPlayer;
	public Board board;
	public string blackTurnMessage;
	public string whiteTurnMessage;
	public string blackWinsMessage;
	public string whiteWinsMessage;
	private Dictionary<char, int> scores;
	private Dictionary<char, Text> displayedScores;
	private Text gameStatus;

	void Awake()
	{
		board = new Board();
		scores = new Dictionary<char, int> { {'B', 0}, {'W', 0} };
		currentPlayer = 'B';
		var blackScoreText = GameObject.Find("BlackScore").GetComponent<Text>();
		var whiteScoreText = GameObject.Find("WhiteScore").GetComponent<Text>();
		displayedScores = new Dictionary<char, Text> { {'B', blackScoreText}, {'W', whiteScoreText} };
		gameStatus = GameObject.Find("GameStatus").GetComponent<Text>();
	}

	public void NextTurn(int scoreDelta)
	{
		scores[currentPlayer] += scoreDelta;
		displayedScores[currentPlayer].text = ScoreMessage(currentPlayer);
		if (scores[currentPlayer] == 6)
		{
			EndGame();
		}
		else
		{
			currentPlayer = (currentPlayer == 'B') ? 'W' : 'B';
			gameStatus.text = (currentPlayer == 'B') ? blackTurnMessage : whiteTurnMessage;
		}
	}

	private string ScoreMessage(char player)
	{
		string prefix = (player == 'B') ? "Black: " : "White: ";
		return prefix + scores[player].ToString();
	}

	private void EndGame()
	{
		currentPlayer = 'N'; // no current player; disables board pieces
		gameStatus.text = (scores['B'] == 6) ? blackWinsMessage : whiteWinsMessage; // show a message
	}

	public void Restart()
	{
		board = new Board();
		foreach (var player in new List<char> {'B', 'W'})
		{
			Debug.Log(player);
			scores[player] = 0;
			displayedScores[player].text = ScoreMessage(player);
		}
		gameStatus.text = blackTurnMessage;
	}
	
}
