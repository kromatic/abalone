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
			// first create board
			var boardSurface = GameObject.Instantiate(boardPrefab, GameObject.Find("Game").transform);
			// then create spaces
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
					var piece = GameObject.Instantiate(piecePrefab, board[i][j]).GetComponent<GamePiece>();
					piece.position = new Vector2(i, j);
					if (i < 3)
					{
						var sprite = piece.GetComponent<SpriteRenderer>();
						sprite.color = Color.white;
						piece.color = "white";
					}
				}
			}
		}

		public void ResetPieces(List<Vector2> positions)
		{
			foreach(var pos in positions)
			{
				GetPiece(pos).Clear();
			}
			positions.Clear();
		}

		public void Select(List<Vector2> selection)
		{
			foreach(var pos in selection)
			{
				GetPiece(pos).Select();
			}
		}

		public void GetPotentialSelection(Vector2 anchorPosition, List<Vector2> potentialSelection)
		{
			var anchor = GetPiece(anchorPosition);
			anchor.MarkSelectable();
			potentialSelection.Add(anchorPosition);
			int i = (int)anchorPosition[0], j = (int)anchorPosition[1];
			for (int deltaX = -1; deltaX < 2; deltaX++)
			{
				for (int deltaY = -1)
				int deltaX = (int)delta[0], deltaY = (int)delta[1];
				int x = i + deltaX, y = j + deltaY;
				while (x < board.Count && 0 <= y && y < board[x].Count)
				{
					if (board[x][y].childCount == 0) break;
					var pos = new Vector2(x, y); var piece = GetPiece(pos);
					if (piece.color != anchor.color) break;
					Console.Write(x); Console.WriteLine(y);
					piece.MarkSelectable();
					potentialSelection.Add(pos);
					x += deltaX; y += deltaY;
				}
			}
		}

		GamePiece GetPiece(Vector2 pos)
		{
			int i = (int)pos[0], j = (int)pos[1];
			return board[i][j].GetChild(0).GetComponent<GamePiece>();
		}
	}
}
