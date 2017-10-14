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

		public void ResetPieces(List<Vector2> pieces)
		{
			foreach(var pos in pieces)
			{
				int i = (int)pos[0], j = (int)pos[1];
				var piece = board[i][j].GetComponent<GamePiece>();
				piece.completesSelection = false;
				piece.selected = false;
			}
		}

		public void Select(List<Vector2> selection)
		{
			foreach(var pos in selection)
			{
				int i = (int)pos[0], j = (int)pos[1];
				var sprite = board[i][j].GetComponent<SpriteRenderer>();
				var newColor = (sprite.color == Color.black) ? new Color(0, 0, 0, 0.7f): new Color(1, 1, 1, 0.7f);
				sprite.color = newColor;
				board[i][j].GetComponent<GamePiece>().selected = true; 
			}
		}

		public List<Vector2> GetPotentialSelection(Vector2 anchor)
		{
			var res = new List<Vector2>();
			int i = (int)anchor[0], j = (int)anchor[1];
			var color = board[i][j].GetComponent<SpriteRenderer>().color;
			for (int y = j; y < j + 3; y++)
			{
				if (board[i][y].childCount == 0) break;
				if (board[i][y].GetChild(0).GetComponent<SpriteRenderer>().color != color) break;
				res.Add(new Vector2(i, y));
			}
			for (int x = i; x < i + 3; x++)
			{
				if (board[x][j].childCount == 0) break;
				if (board[x][j].GetChild(0).GetComponent<SpriteRenderer>().color != color) break;
				res.Add(new Vector2(x, j));
			}
			int x = i, y = j;
			while (x - i < 3)
			{
				
			}
		}
	}
}
