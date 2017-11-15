using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Game : MonoBehaviour
{
	private Dictionary<char, int> scores;
	private Dictionary<char, Text> displayedScores;
	public char currentPlayer;
	public Board board;

	void Awake()
	{
		board = new Board();
		scores = new Dictionary<char, int> { {'B', 0}, {'W', 0} };
		currentPlayer = 'B';
		var blackScoreText = GameObject.Find("BlackScore").GetComponent<Text>();
		var whiteScoreText = GameObject.Find("WhiteScore").GetComponent<Text>();
		displayedScores = new Dictionary<char, Text> { {'B', blackScoreText}, {'W', whiteScoreText} };
	}

	public void NextTurn(int scoreDelta)
	{
		scores[currentPlayer] += scoreDelta;
		displayedScores[currentPlayer].text = scores[currentPlayer].ToString(); 
		if (scores[currentPlayer] == 6)
		{
			EndGame();
		}
		else
		{
			currentPlayer = (currentPlayer == 'B') ? 'W' : 'B';
		}
	}

	private void EndGame()
	{
		currentPlayer = 'N'; // no current player; disables board pieces
		// show a message
	}
	
}
