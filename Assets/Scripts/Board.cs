using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBoard
{
	public class Board
	{
		private List<List<Transform>> board;

		public Board(Transform boardPrefab, Transform spacePrefab, Transform piecePrefab)
		{
			// first create board and spaces
			var boardSurface = GameObject.Instantiate(boardPrefab);
			board = new List<List<Transform>>();
			var rowLengths = new List<int> {5, 6, 7, 8, 9, 8, 7, 6, 5};
			var c = spacePrefab.localScale[0] * boardPrefab.localScale[0] * 1.1f;
			var r = c / 2;
			for (int i = 0; i < 9; i++)
			{
				var len = rowLengths[i];
				var row = new List<Transform>();
				var x = (9 - len) * r  - 4 * c;
				var y = (4 - i) * c;
				for (int j = 0; j < len; j++)
				{
					var position = new Vector3(x, y, 0);
					var cell = GameObject.Instantiate(spacePrefab, position, Quaternion.identity, boardSurface);
					row.Add(cell);
					x += c;
				}
				board.Add(row);
			}

			// then set up pieces in initial positions

			foreach (int i in new List<int> {0, 1, 2, 6, 7, 8})
			{
				for (int j = 0; j < board[i].Count; j++)
				{
					if ((i == 2 || i == 6) && (j < 2 || j > 4))
					{
						continue;
					}
					var piece = GameObject.Instantiate(piecePrefab, board[i][j]);
					piece.GetComponent<GamePiece>().position = new Vector2(i, j);
					if (i < 3)
					{
						var sprite = piece.GetComponent<SpriteRenderer>();
						sprite.color = Color.white;
					}
				}
			}
		}
	}
}
