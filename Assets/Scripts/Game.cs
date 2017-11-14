using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{
	public Dictionary<char, int> scores;
	public char currentPlayer;
	public Board board;

	void Awake()
	{
		board = new Board();
		scores = new Dictionary<char, int> { {'B', 0}, {'W', 0} };
		currentPlayer = 'B';
	}

	public void NextTurn(int scoreDelta)
	{
		scores[currentPlayer] += scoreDelta;
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
