using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{
	public Dictionary<char, int> scores;
	public char currentPlayer;
	private Board board;

	void Awake()
	{
		board = new Board();
		scores = new Dictionary<char, int> { {'B', 0}, {'W', 0} };
		currentPlayer = 'B';
	}
	
}
