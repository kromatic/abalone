using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBoard
{
	public class Board
	{
		private List<List<Transform>> board;

		private Dictionary<string, Vector2> directions = new Dictionary<string, Vector2>
		{
			{"NW", new Vector2(-1, -1)},
			{"NE", new Vector2(-1,  0)},
			{"E",  new Vector2(0,   1)},
			{"SE", new Vector2(1,   1)},
			{"SW", new Vector2(1,   0)},
			{"W",  new Vector2(0,  -1)}
		};

		public Board(Transform boardPrefab, Transform spacePrefab, Transform piecePrefab)
		{
			// first create board
			var boardSurface = GameObject.Instantiate(boardPrefab, GameObject.Find("Game").transform);
			// then create spaces
			board = new List<List<Transform>>();
			var rowLengths = new List<int> {5, 6, 7, 8, 9, 8, 7, 6, 5};
			var c = spacePrefab.localScale.x * boardPrefab.localScale.x * 1.1f;
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

		public void ResetPieces(List<GamePiece> pieces)
		{
			foreach(var piece in pieces)
			{
				piece.Clear();
			}
			pieces.Clear();
		}

		public void Select(List<GamePiece> selection)
		{
			foreach(var piece in selection)
			{
				piece.Select();
			}
		}

		public void GetPotentialSelection(GamePiece anchor, List<GamePiece> potentialSelection)
		{
			anchor.MarkAnchor();
			potentialSelection.Add(anchor);
			foreach (var dir in new List<string> {"NW", "NE", "E", "SE", "SW", "W"})
			{
				var cur = anchor;
				for(int c = 1; c < 3; c++)
				{
					Debug.Log("trying to get neighbor");
					cur = GetNeighbor(cur, dir);
					if (cur == null) break;
					if (cur.color != anchor.color) break;
					cur.MarkSelectable();
					potentialSelection.Add(cur);
				}
			}
		}

		public GamePiece GetPiece(Vector2 pos)
		{
			int i = (int)pos[0], j = (int)pos[1];
			return board[i][j].GetChild(0).GetComponent<GamePiece>();
		}

		private GamePiece GetNeighbor(GamePiece piece, string dir)
		{
			var vector = directions[dir];
			int deltaX = (int)vector.x, deltaY = (int)vector.y;
			int i = (int)piece.position.x, j = (int)piece.position.y;
			if (i == 4 && dir[0] == 'S')
			{
				deltaY--;
			}
			else if (i > 4 && dir.Length == 2)
			{
				deltaY += (dir[0] == 'N') ? 1 : -1;
			}
			int x = i + deltaX, y = j + deltaY;
			if (x < 0 || x == board.Count || y < 0 || y == board[x].Count)
			{
				return null;
			}
			if (board[x][y].childCount == 0)
			{
				return null;
			}
			return GetPiece(new Vector2(x, y));
		}


	}
}
