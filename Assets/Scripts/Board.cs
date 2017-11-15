using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
	private List<List<char>> board;
	public static int height;
	public static List<int> rowLengths;
	public static Dictionary<string, Vector> directions;

	static Board()
	{
		height = 9;
		rowLengths = new List<int> {5, 6, 7, 8, 9, 8, 7, 6, 5};
		directions = new Dictionary<string, Vector>
		{
			{"NW", new Vector(-1, -1)},
			{"NE", new Vector(-1,  0)},
			{"E",  new Vector(0,   1)},
			{"SE", new Vector(1,   1)},
			{"SW", new Vector(1,   0)},
			{"W",  new Vector(0,  -1)}
		};
	}

	public Board()
	{
		// first create board with all spaces empty
		board = new List<List<char>>();
		for (int i = 0; i < height; i++)
		{
			var row = Enumerable.Repeat('O', rowLengths[i]).ToList();
			board.Add(row);
		}

		// then set up pieces in initial locations
		foreach (int i in new List<int> {0, 1, 2, 6, 7, 8})
		{
			var piece = (i < 3) ? 'W' : 'B';
			for (int j = 0; j < board[i].Count; j++)
			{
				if ((i == 2 || i == 6) && (j < 2 || j > 4))
				{
					continue;
				}
				board[i][j] = piece;
			}
		}
	}

	public IEnumerable<IList<char>> View()
	{
		foreach (var row in board)
		{
			yield return row.AsReadOnly();
		}
	}

	public IEnumerable<KeyValuePair<Vector, string>> GetSelectables(Vector anchorLocation)
	{
		var anchor = GetSpace(anchorLocation);
		yield return new KeyValuePair<Vector, string>(anchorLocation, "");
		foreach (var direction in directions.Keys)
		{
			var current = anchorLocation;
			var distance = 1;
			while (distance < 3)
			{
				current = GetNeighborLocation(current, direction);
				if (!ValidLocation(current)) break;
				if (GetSpace(current) != anchor) break;
				yield return new KeyValuePair<Vector, string>(current, direction);
				distance++;
			}
		}
	}

	public char GetSpace(Vector loc)
	{
		return board[loc.x][loc.y];
	}


	public static List<Vector> GetColumn(Vector start, Vector end, string direction)
	{
		var column = new List<Vector>();
		var current = start;
		while (current != end)
		{
			column.Add(current);
			current = GetNeighborLocation(current, direction);
		}
		column.Add(current);
		return column;
	}

	private static Vector GetNeighborLocation(Vector location, string direction)
	{
		if (direction == "") return new Vector(-1, -1); // throwaway value when direction unnecessary
		var delta = directions[direction];
		if (location.x == 4 && direction[0] == 'S')
		{
			delta.y--;
		}
		else if (location.x > 4 && direction.Length == 2)
		{
			delta.y += (direction[0] == 'N') ? 1 : -1;
		}
		return location + delta;
	}

	private static bool ValidLocation(Vector loc)
	{
		return 0 <= loc.x && loc.x < height && 0 <= loc.y && loc.y < rowLengths[loc.x];
	}

	public IEnumerable<KeyValuePair<string, List<Vector>>> GetMoves(List<Vector> selection, string selectionDirection)
	{
		// Debug.Log("selectionDirection: " + selectionDirection);
		foreach(var direction in directions.Keys)
		{
			var enemyColumn = new List<Vector>();
			if (CheckMove(selection, selectionDirection, direction, enemyColumn))
			{
				yield return new KeyValuePair<string, List<Vector>>(direction, enemyColumn);
			}
		}
	}

	private bool CheckMove(List<Vector> selection, string selectionDirection, string direction, List<Vector> enemyColumn)
	{
		if (SameAxis(selectionDirection, direction))
		{
			// Debug.Log("doing sumito check");
			var enemyColor = (GetSpace(selection[0]) == 'B') ? 'W' : 'B';
			var selectionEdge = (selectionDirection == direction) ? selection[selection.Count - 1] : selection[0];
			var enemyColumnStart = GetNeighborLocation(selectionEdge, direction);
			return Sumito(enemyColumnStart, direction, enemyColor, selection.Count - 1, enemyColumn);
		}

		// if a side step move, just make sure target spaces are empty
		// Debug.Log("doing regular check");
		foreach (var location in selection)
		{
			var neighborLocation = GetNeighborLocation(location, direction);
			if (!ValidLocation(neighborLocation) || GetSpace(neighborLocation) != 'O') return false;
		}
		// Debug.Log("move checks out");
		return true;
	}

	private bool Sumito(Vector start, string direction, char color, int bound, List<Vector> column)
	{
		var current = start;
		while (column.Count < bound)
		{
			if (!ValidLocation(current)) return column.Count > 0; // edge of board: ok as long as column nonempty
			var space = GetSpace(current);
			if (space == 'O') return true; // empty space - ok
			if (space != color) return false; // opposite color piece in sequence - not ok
			// remaining case: piece of this color
			column.Add(current);
			current = GetNeighborLocation(current, direction);
		}
		// if we did not return yet, then we have an opposing column of maximum possible size
		// current space has to be empty or edge of board
		return !ValidLocation(current) || GetSpace(current) == 'O';
	}

	private static bool SameAxis(string direction1, string direction2)
	{
		if (direction1 == direction2) return true;
		// sort strings if necessary
		// Debug.Log(dir1);
		// Debug.Log(dir2);
		if (string.Compare(direction1, direction2) > 0)
		{
			var temp = direction1; direction1 = direction2; direction2 = temp;
		}
		if (direction1 == "E" && direction2 == "W") return true;
		if (direction1 == "NW" && direction2 == "SE") return true;
		if (direction1 == "NE" && direction2 == "SW") return true;
		return false;
	}

	public int Move(List<Vector> selection, string selectionDirection, List<Vector> enemyColumn, string direction)
	{
		var orderedSelection = (selectionDirection == direction) ? Enumerable.Reverse(selection) : selection;
		var entireColumn = Enumerable.Concat(Enumerable.Reverse(enemyColumn), orderedSelection);
		if (SameAxis(selectionDirection, direction))
		{
			return MoveColumnInLine(entireColumn, direction);
		}
		else
		{
			MoveColumnSideStep(entireColumn, direction);
			return 0;
		}
	}

	private void MoveColumnSideStep(IEnumerable<Vector> column, string direction)
	{
		foreach(var location in column)
		{
			SetSpace(GetNeighborLocation(location, direction), GetSpace(location));
			SetSpace(location, 'O');
			Debug.Log("moved piece and should have cleared space");
		}
	}

	private int MoveColumnInLine(IEnumerable<Vector> column, string direction)
	{
		int score = 0;
		var targetLocation = GetNeighborLocation(column.First(), direction);
		if (!ValidLocation(targetLocation))
		{
			score = 1;
		}
		else
		{
			var location = column.First();
			Debug.Log(targetLocation.x); Debug.Log(targetLocation.y);
			SetSpace(targetLocation, GetSpace(location));
			targetLocation = location;
			// Debug.Log("moved first piece");
		}
		// int j = 2;
		foreach (var location in column.Skip(1))
		{
			// Debug.Log("moved piece"); Debug.Log(j); j++;
			Debug.Log(targetLocation.x); Debug.Log(targetLocation.y);
			SetSpace(targetLocation, GetSpace(location));
			targetLocation = location;
		}
		SetSpace(targetLocation, 'O');
		Debug.Log("should have cleared a space");
		return score;
	}

	private void SetSpace(Vector location, char space)
	{
		board[location.x][location.y] = space;
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

	//public static Vector operator -(Vector v1, Vector v2)
	//{
	//	return new Vector(v1.x - v2.x, v1.y - v2.y);
	//}

	//public static Vector Delta(Vector end, Vector start)
	//{
	//	return new Vector(Math.Sign(end.x - start.x), Math.Sign(end.y - start.y));
	//}

	//public static Vector Average(Vector v1, Vector v2)
	//{
	//	return new Vector((v1.x + v2.x) / 2, (v1.y + v2.y) / 2);
	//}

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
