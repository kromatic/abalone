﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBoard;

public class Game : MonoBehaviour
{
	public Transform boardPrefab;
	public Transform spacePrefab;

	public int player1Score = 0;
	public int player2Score = 0;
	private Board board;
	// Use this for initialization
	void Awake()
	{
		board = new Board(boardPrefab, spacePrefab);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}