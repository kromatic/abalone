using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	private List<List<Transform>> board;
	public Transform spacePrefab;
	public Transform blackPrefab;
	public Transform whitePrefab;

	private Dictionary<string, Location> directions = new Dictionary<string, Location>
	{
		{"NW", new Location(-1, -1)},
		{"NE", new Location(-1,  0)},
		{"E",  new Location(0,   1)},
		{"SE", new Location(1,   1)},
		{"SW", new Location(1,   0)},
		{"W",  new Location(0,  -1)}
	};

	void Awake()
	{
		// first create spaces
		board = new List<List<Transform>>();
		var rowLengths = new List<int> {5, 6, 7, 8, 9, 8, 7, 6, 5};
		var c = spacePrefab.localScale.x * transform.localScale.x * 1.1f;
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
				var cell = Instantiate(spacePrefab, position, Quaternion.identity, this.transform);
				row.Add(cell);
				x += c;
			}
			board.Add(row);
		}

		// then set up pieces in initial Locations
		foreach (int i in new List<int> {0, 1, 2, 6, 7, 8})
		{
			var piecePrefab = (i < 3) ? whitePrefab : blackPrefab;
			for (int j = 0; j < board[i].Count; j++)
			{
				if ((i == 2 || i == 6) && (j < 2 || j > 4))
				{
					continue;
				}
				var piece = Instantiate(piecePrefab, board[i][j]).GetComponent<GamePiece>();
				piece.location = new Location(i, j);
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
		anchor.MarkSelectable();
		potentialSelection.Add(anchor);
		foreach (var dir in directions.Keys)
		{
			var cur = anchor;
			for(int c = 1; c < 3; c++)
			{
				// Debug.Log("trying to get neighbor");
				cur = GetNeighbor(cur, dir);
				if (cur == null) break;
				if (cur.color != anchor.color) break;
				cur.MarkSelectable();
				potentialSelection.Add(cur);
			}
		}
	}

	public GamePiece GetPiece(Location loc)
	{
		return board[loc.x][loc.y].GetChild(0).GetComponent<GamePiece>();
	}

	private GamePiece GetNeighbor(GamePiece piece, string dir)
	{
		var vector = directions[dir];
		int deltaX = (int)vector.x, deltaY = (int)vector.y;
		int i = piece.location.x, j = piece.location.y;
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
		return GetPiece(new Location(x, y));
	}

	public List<string> GetMoves(List<GamePiece> selection)
	{
		var res = new List<string>();
		foreach(var dir in directions.Keys)
		{
			if (CheckMove(selection, dir))
			{
				res.Add(dir);
			}
		}

		return res;
	}

	private bool CheckMove(List<GamePiece> selection, string dir)
	{
		foreach (var piece in selection)
		{
			continue;
		}
		return true;
	}

}

public struct Location
{
	public int x, y;

	public Location(int x, int y)
	{
		this.x = x; this.y = y;
	}
}
