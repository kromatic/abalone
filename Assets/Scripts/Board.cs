// The Board class is the logical representation of the Abalone game board.
// This is NOT a MonoBehaviour
// Internally, the board is just a 2D List of chars.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class Board
{
	// height is the number of rows in the board.
	public static int height;

	// rowLengths[i] is the number of spaces in the ith row.
	public static List<int> rowLengths;

	// Directions on the board are represented by the strings "NW", "NE", "E", "SE", "SW", "W".
	public static IList<string> Directions { get; private set; }

	// directionVectors is a mapping from these strings to corresponding vectors for the upper half of the board.
	private static Dictionary<string, Vector> directionVectors;

	// 2D List representing spaces and pieces on the board. 'O' is an empty, and 'B' / 'W' represent player pieces.
	private List<List<char>> board;

	static Board()
	{
		height = 9;
		rowLengths = new List<int> {5, 6, 7, 8, 9, 8, 7, 6, 5};
		Directions = new List<string> {"NW", "NE", "E", "SE", "SW", "W"}.AsReadOnly();

		// Again, this mapping is for the upper half of the board.
		directionVectors = new Dictionary<string, Vector>
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
		// First create a board with all spaces empty.
		board = new List<List<char>>();
		for (int i = 0; i < height; i++)
		{
			var row = Enumerable.Repeat('O', rowLengths[i]).ToList();
			board.Add(row);
		}

		// Then set up pieces in initial locations.
		foreach (int i in new List<int> {0, 1, 2, 6, 7, 8})
		{
			// White pieces start on the upper half.
			var piece = (i < 3) ? 'W' : 'B';
			for (int j = 0; j < board[i].Count; j++)
			{
				if ((i == 2 || i == 6) && (j < 2 || j > 4))
				{
					// These spaces remain empty. Only 3 pieces start in each of rows 2 and 6.
					continue;
				}
				board[i][j] = piece;
			}
		}
	}


	// This helper function returns an iterator over all spaces in the board.
	// It is used by BoardDisplay to update the board shown to the user.
	public IEnumerable<IEnumerable<char>> View()
	{
		return
			from row in board select
			from space in row select space;
	}

	// Returns iterator of (location, direction) pairs indicating which pieces can be
	// used to complete a selection given the location of the anchoring piece.
	// That is, walking from the anchor in the direction until the location gives a valid selection.
	public IEnumerable<Tuple<Vector, string>> GetSelectables(Vector anchorLocation)
	{
		// First get color of anchoring piece.
		var anchor = GetSpace(anchorLocation);

		// The anchor itself completes a singleton selection.
		yield return Tuple.Create(anchorLocation, "");

		// Iterate over possible directions.
		// Walk in each direction until a maximum possible selection (starting with the anchor) is reached,
		// yielding the location of each piece along the way together with the direction.
		// These pieces correspond to all possible selections.

		// A selection can contain at most 3 consecutive pieces of the same color.
		// So to reach a maximum possible selection we walk as long as
		// 1) we only encounter pieces of the same color as the anchor,
		// 2) we do not hit the edge of the board, and
		// 3) our distance from the anchor is less than 3.
		foreach (var direction in Directions)
		{
			var current = anchorLocation;

			// Ensure our selection is at most 3 pieces.
			for (var distance = 1; distance < 3; distance++)
			{
				// Get the location of the next space in this direction.
				current = GetNeighborLocation(current, direction);
				// If location is off the board we are done.
				if (!ValidLocation(current)) break;
				// If an enemy piece is at the location we are also done.
				if (GetSpace(current) != anchor) break;
				// Otherwise the location holds a piece that can complete a valid selection starting at the anchor.
				yield return Tuple.Create(current, direction);
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
		var firstLocation = column.First();
		var targetLocation = GetNeighborLocation(firstLocation, direction);
		if (!ValidLocation(targetLocation))
		{
			score = 1;
		}
		else
		{
			// Debug.Log(targetLocation.x); Debug.Log(targetLocation.y);
			SetSpace(targetLocation, GetSpace(firstLocation));
			// Debug.Log("moved first piece");
		}
		targetLocation = firstLocation;
		// int j = 2;
		foreach (var location in column.Skip(1))
		{
			// Debug.Log("moved piece"); Debug.Log(j); j++;
			// Debug.Log(targetLocation.x); Debug.Log(targetLocation.y);
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
