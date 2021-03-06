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

	// Get a list of consecutive locations (a column) starting at start and going in direction until end.
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

	// GetSelectables returns iterator over (location, direction) pairs indicating which pieces can be
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

	// GetMoves returns an iterator over possible moves given a selection. The iterator yields pairs representing valid moves.
	// Each pair consists of a direction in which the selection can be moved and the associated column of enemy pieces
	// (which may be empty) that would be pushed by moving the column in that direction.
	public IEnumerable<Move> GetMoves(Selection selection)
	{
		// Debug.Log("selectionDirection: " + selectionDirection);
		foreach (var direction in Directions)
		{
			var move = CheckMove(selection, direction);
			yield return move;
		}
	}

	// Move performs the specified move on the board and returns the number of enemy pieces displaced.
	public int Move(Move move)
	{
		// If enemyColumn is null that means that the move is a sidestep.
		if (move.EnemyColumn == null)
		{
			return MoveSideStep(move);
		}
		// Otherwise we have to perform an in-line move.
		else
		{
			return MoveInLine(move);
		}
	}

	public int UndoMove(Move move)
	{
		if (move.EnemyColumn == null)
		{
			return UndoMoveSideStep(move);
		}
		else
		{
			return UndoMoveInLine(move);
		}
	}

	// Helper for getting a space on the board.
	public char GetSpace(Vector loc)
	{
		return board[loc.x][loc.y];
	}

	// Helper method for checking if a move is valid.
	//Returns insance of Move if valid otherwise returns null.
	private Move CheckMove(Selection selection, string direction)
	{
		// If the move is being made along the axis of selection, then we need to check the Sumito condition.
		if (SameAxis(selection.Direction, direction))
		{
			// Debug.Log("doing sumito check");
			var enemyColor = (selection.Color == 'B') ? 'W' : 'B';
			var edgeIndex = selection.Column.Count - 1;
			var selectionEdge = (selection.Direction == direction) ? selection.Column[edgeIndex] : selection.Column[0];
			var enemyColumnStart = GetNeighborLocation(selectionEdge, direction);
			var enemyLimit = edgeIndex;
			var enemyColumn = new List<Vector>();
			if (Sumito(enemyColumnStart, direction, enemyColor, enemyLimit, enemyColumn))
			{
				return new Move(selection, enemyColumn, direction);
			}
		}

		// Otherwise we just have a side-step move, so we just make sure the target spaces are all valid and empty.
		// Debug.Log("doing regular check");
		foreach (var location in selection.Column)
		{
			var neighborLocation = GetNeighborLocation(location, direction);
			if (!ValidLocation(neighborLocation) || GetSpace(neighborLocation) != 'O') return null;
		}
		// Debug.Log("move checks out");
		return new Move(selection, null, direction);
	}

	// Helper method for checking if the Sumito condition holds.
	// Enemy pieces to be pushed are added to the List column.
	private bool Sumito(Vector start, string direction, char color, int bound, List<Vector> column)
	{
		// var column = new List<Vector>();
		var current = start;
		// We can keep going as long as the column so far is smaller than the bound.
		while (column.Count < bound)
		{
			// If we encounter the edge of the board then the column so far has to be nonempty.
			if (!ValidLocation(current)) return column.Count > 0;
			var space = GetSpace(current);
			// An empty space means we are good.
			if (space == 'O') return true;
			// A piece of opposite color means we are sandwiched - not good.
			if (space != color) return false;
			// Otherwise we encountered a piece of the desired color and can add it.
			column.Add(current);
			current = GetNeighborLocation(current, direction);
		}
		// If we did not return yet, then we have an opposing column of maximum possible size.
		// (This is at least 1, since we never call Sumito on a singleton selection.)
		// Thus the current space has to be empty or off the board.
		return !ValidLocation(current) || GetSpace(current) == 'O';
	}

	// Heper method for checking of two directions lie on the same axis.
	private static bool SameAxis(string direction1, string direction2)
	{
		if (direction1 == direction2) return true;

		// Sort the strings to simplify the logic.
		if (string.Compare(direction1, direction2) > 0)
		{
			var temp = direction1; direction1 = direction2; direction2 = temp;
		}
		if (direction1 == "E" && direction2 == "W") return true;
		if (direction1 == "NW" && direction2 == "SE") return true;
		if (direction1 == "NE" && direction2 == "SW") return true;
		return false;
	}

	// Helper method for making a sidestep move.
	private int MoveSideStep(Move move)
	{
		// For each location in the selection, get the corresponding target location,
		// set it to the selection color, and make the original location empty.
		foreach (var location in move.Selection.Column)
		{
			SetSpace(GetNeighborLocation(location, move.Direction), move.Selection.Color);
			SetSpace(location, 'O');
		}
		// No pieces are pushed off the board, so we return 0.
		return 0;
	}

	// Helper method for making an in-line move.
	private int MoveInLine(Move move)
	{
		// The location of the last selection piece to be moved has to be set to empty.
		// This location is either the first or last in the selection, depending on direction of motion.
		var emptyIndex = (move.Direction == move.Selection.Direction) ? 0 : move.Selection.Column.Count - 1;
		SetSpace(move.Selection.Column[emptyIndex], 'O');
		// Next, we must push a selection piece forward.
		// If the enemy column is empty, we must grab the location based on the leading location of the selection.
		int leadingIndex; Vector leadingTargetLocation;
		if (move.EnemyColumn.Count == 0)
		{
			leadingIndex = (emptyIndex == 0) ? move.Selection.Column.Count - 1 : 0;
			leadingTargetLocation = GetNeighborLocation(move.Selection.Column[leadingIndex], move.Direction);
			// Since there is no enemy column, and this is a valid move, the location must be a valid empty space.
			SetSpace(leadingTargetLocation, move.Selection.Color);
			return 0;
		}
		// Otherwise, we set the first location in the enemy column to the selection color.
		// In this case, we must also push the enemy column forward.
		else
		{
			SetSpace(move.EnemyColumn[0], move.Selection.Color);
			leadingIndex = move.EnemyColumn.Count - 1;
			leadingTargetLocation = GetNeighborLocation(move.EnemyColumn[leadingIndex], move.Direction);
			// If the location is off the board, that means an enemy piece is pushed off.
			if (!ValidLocation(leadingTargetLocation)) return 1;
			// Otherwise we set the location to the enemy color and return 0;
			else
			{
				var enemyColor = (move.Selection.Color == 'B') ? 'W' : 'B';
				SetSpace(leadingTargetLocation, enemyColor);
				return 0;
			}
		}
	}

	// Helper method for undoing a sidestep move.
	private int UndoMoveSideStep(Move move)
	{
		// For each location in the selection, we set that location to the selection color
		// and clear the corresponding location into which the move was made.
		foreach (var location in move.Selection.Column)
		{
			SetSpace(location, move.Selection.Color);
			SetSpace(GetNeighborLocation(location, move.Direction), 'O');
		}
		// Since a sidestep move never pushes anything off the board, there is no change in score.
		return 0;
	}

	// Helper method for undoing an in-line move.
	private int UndoMoveInLine(Move move)
	{
		// First we set the selection location that was emptied during the move back to the selection color.
		var emptyIndex = (move.Direction == move.Selection.Direction) ? 0 : move.Selection.Column.Count - 1;
		SetSpace(move.Selection.Column[emptyIndex], move.Selection.Color);
		// If the enemy column is empty, then we must clear the location into which the leading selection piece was moved.
		// Since no piece were displaced in this case, we return 0;
		int leadingIndex; Vector leadingTargetLocation;
		if (move.EnemyColumn.Count == 0)
		{
			leadingIndex = (emptyIndex == 0) ? move.Selection.Column.Count - 1 : 0;
			leadingTargetLocation = GetNeighborLocation(move.Selection.Column[leadingIndex], move.Direction);
			SetSpace(leadingTargetLocation, 'O');
			return 0;
		}
		// Otherwise we must restore the first location in the enemy column to the enemy color
		// and then clear the space (if any) into which the leading piece of the enemy column was pushed.
		else
		{
			var enemyColor = (move.Selection.Color == 'B') ? 'W' : 'B';
			SetSpace(move.EnemyColumn[0], enemyColor);
			leadingIndex = move.EnemyColumn.Count - 1;
			leadingTargetLocation = GetNeighborLocation(move.EnemyColumn[leadingIndex], move.Direction);
			// If location is invalid, that means an enemy piece was pushed off.
			// Thus undoing the move restores that piece and decreases the attacker's score.
			if (!ValidLocation(leadingTargetLocation)) return -1;
			// Otherwise no pieces were pushed off and we set the location back to empty.
			else
			{
				SetSpace(leadingTargetLocation, 'O');
				return 0;
			}
		}
	}

	// Helper method for getting the location of a neighbor of a cell in a particular direction.
	private static Vector GetNeighborLocation(Vector location, string direction)
	{
		// If direction is a throwaway value return a throwaway value as well.
		if (direction == "") return new Vector(-1, -1);
		// Get the direction vector for the upper half of the board.
		var delta = directionVectors[direction];
		// Adjust the vector based on the actual location on the board.
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

	// Helper method to check if a location is valid.
	private static bool ValidLocation(Vector loc)
	{
		return 0 <= loc.x && loc.x < height && 0 <= loc.y && loc.y < rowLengths[loc.x];
	}


	// Private methods for setting a space.
	private void SetSpace(Vector location, char space)
	{
		board[location.x][location.y] = space;
	}
}

// A struct for representing integral coordinates in a plane.
// Used to represent locations on the board as well vector directions on the board.
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

// Struct for keeping track of a selection.
public struct Selection
{
	// The locations that are part of the selection, in consecutive order.
	public ReadOnlyCollection<Vector> Column { get { return column.AsReadOnly(); } }

	// The direction of the selection, i.e. direction from first to last piece in Selection.
	public string Direction { get; private set; }

	// The color of the pieces in the selection.
	public char Color { get; private set; }

	// Private field backing Column.
	private List<Vector> column;

	public Selection(List<Vector> column, string direction, char color)
	{
		this.column = column;
		Direction = direction;
		Color = color;
	}
}

// Struct for keeping track of a move.
public class Move
{
	// The selection to be moved.
	public Selection Selection { get; private set; }

	// The enemy column to be pushed.
	public ReadOnlyCollection<Vector> EnemyColumn
	{
		get
		{
			if (enemyColumn == null) return null;
			else return enemyColumn.AsReadOnly();
		}
	}

	// The direction in which to move.
	public string Direction { get; private set; }

	// Private field backing EnemyColumn.
	private List<Vector> enemyColumn;

	// Whether or not the move is a sidestep move. If not, it is an in-line move and can be performed differently. 
	// public bool Sidestep { get; private set; }

	public Move(Selection selection, List<Vector> enemyColumn, string direction) //, bool sidestep)
	{
		Selection = selection;
		this.enemyColumn = enemyColumn;
		Direction = direction;
		// Sidestep = sidestep;
	}

}
