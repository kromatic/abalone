using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	private List<List<Space>> board;
	public Transform spacePrefab;
	public Transform blackPrefab;
	public Transform whitePrefab;
	public float paddingFactor = 1.1f;
	private static int height = 9;
	private static List<int> rowLengths = new List<int> {5, 6, 7, 8, 9, 8, 7, 6, 5};

	private static Dictionary<string, Vector> directions = new Dictionary<string, Vector>
	{
		{"NW", new Vector(-1, -1)},
		{"NE", new Vector(-1,  0)},
		{"E",  new Vector(0,   1)},
		{"SE", new Vector(1,   1)},
		{"SW", new Vector(1,   0)},
		{"W",  new Vector(0,  -1)}
	};

	void Awake()
	{
		// first create spaces
		board = new List<List<Space>>();
		var c = spacePrefab.localScale.x * transform.localScale.x * paddingFactor;
		var r = c / 2;
		for (int i = 0; i < height; i++)
		{
			var length = rowLengths[i];
			var row = new List<Space>();
			var x = (height - length) * r  - (height / 2) * c;
			var y = (height / 2 - i) * c;
			for (int j = 0; j < length; j++)
			{
				var position = new Vector3(x, y, 0);
				var space = Instantiate(spacePrefab, position, Quaternion.identity, this.transform).GetComponent<Space>();
				row.Add(space);
				x += c;
			}
			board.Add(row);
		}

		// then set up pieces in initial locations
		foreach (int i in new List<int> {0, 1, 2, 6, 7, 8})
		{
			var piecePrefab = (i < 3) ? whitePrefab : blackPrefab;
			for (int j = 0; j < board[i].Count; j++)
			{
				if ((i == 2 || i == 6) && (j < 2 || j > 4))
				{
					continue;
				}
				var piece = Instantiate(piecePrefab, board[i][j].transform).GetComponent<GamePiece>();
				piece.location = new Vector(i, j);
				board[i][j].piece = piece;
			}
		}
	}

	public void ResetPieces(List<Vector> locations)
	{
		foreach(var loc in locations)
		{
			GetSpace(loc).piece.Clear();
		}
	}

	public void Select(List<Vector> selection)
	{
		foreach(var loc in selection)
		{
			GetSpace(loc).piece.Select();
		}
	}

	public List<Vector> GetPotentialSelection(Vector anchorLocation)
	{
		var res = new List<Vector>();
		var anchor = GetSpace(anchorLocation).piece;
		anchor.MarkSelectable(0, null);
		res.Add(anchorLocation);
		foreach (var dir in directions.Keys)
		{
			var cur = anchorLocation;
			for(int c = 1; c < 3; c++)
			{
				// Debug.Log("trying to get neighbor");
				cur = GetNeighborLocation(cur, dir);
				if (!ValidLocation(cur)) break;
				var curSpace = GetSpace(cur);
				if (curSpace.Empty()) break;
				if (curSpace.piece.color != anchor.color) break;
				curSpace.piece.MarkSelectable(c, dir);
				res.Add(cur);
			}
		}
		return res;
	}

	public Space GetSpace(Vector loc)
	{
		return board[loc.x][loc.y];
	}

	public static List<Vector> GetLocations(Vector start, int distance, string dir)
	{
		var res = new List<Vector>();
		var cur = start; res.Add(cur);
		for (int c = 0; c < distance; c++)
		{
			cur = GetNeighborLocation(cur, dir);
			res.Add(cur);
		}
		return res;
	}

	private static Vector GetNeighborLocation(Vector location, string dir)
	{
		var delta = directions[dir];
		if (location.x == 4 && dir[0] == 'S')
		{
			delta.y--;
		}
		else if (location.x > 4 && dir.Length == 2)
		{
			delta.y += (dir[0] == 'N') ? 1 : -1;
		}
		return location + delta;
	}

	private static bool ValidLocation(Vector loc)
	{
		return 0 <= loc.x && loc.x < height && 0 <= loc.y && loc.y < rowLengths[loc.x]; 
	}

	public List<string> GetMoves(List<Vector> selection, string selectionDirection)
	{
		var res = new List<string>();
		foreach(var dir in directions.Keys)
		{
			if (CheckMove(selection, selectionDirection, dir))
			{
				res.Add(dir);
			}
		}
		return res;
	}

	private bool CheckMove(List<Vector> selection, string selectionDir, string dir)
	{
		var bla = selection[-1];
		foreach (var loc in selection)
		{
			continue;
		}
		return true;
	}

}

public struct Vector
{
	public int x, y;

	public Vector(int x, int y)
	{
		this.x = x; this.y = y;
	}

	public static Vector operator +(Vector v1, Vector v2)
	{
		return new Vector(v1.x + v2.x, v1.y + v2.y);
	}

	public static Vector operator -(Vector v1, Vector v2)
	{
		return new Vector(v1.x - v2.x, v1.y - v2.y);
	}

	public static Vector Delta(Vector end, Vector start)
	{
		return new Vector(Math.Sign(end.x - start.x), Math.Sign(end.y - start.y));
	}

	public static Vector Average(Vector v1, Vector v2)
	{
		return new Vector((v1.x + v2.x) / 2, (v1.y + v2.y) / 2);
	}

	public static bool operator ==(Vector v1, Vector v2)
	{
		return v1.x == v2.x && v1.y == v2.y;
	}

	public static bool operator !=(Vector v1, Vector v2)
	{
		return v1.x != v2.x || v1.y != v2.y;
	}

	public override bool Equals(object o)
	{
		var v = (Vector)o;
		return x == v.x && y == v.y;
	}

	public override int GetHashCode()
	{
		return x + y;
	}
}
