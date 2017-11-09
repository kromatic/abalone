using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserBoard : MonoBehaviour
{
	public Transform spacePrefab;
	public Transform blackPrefab;
	public Transform whitePrefab;
	public float paddingFactor = 1.1f;
	private List<List<Space>> userBoard;
	

	void Awake()
	{
		// create empty board
		userBoard = new List<List<Space>>(); 
		var spaceDiameter = spacePrefab.localScale.x * transform.localScale.x * paddingFactor;
		var spaceRadius = spaceDiameter / 2;
		var xOffset = Board.height / 2 * spaceDiameter;
		for (int i = 0; i < Board.height; i++)
		{
			var length = Board.rowLengths[i];
			var row = new List<Space>();
			var x = (Board.height - length) * spaceRadius  - xOffset;
			var y = (Board.height / 2 - i) * spaceDiameter;
			for (int j = 0; j < length; j++)
			{
				var position = new Vector3(x, y, 0);
				var space = Instantiate(spacePrefab, position, Quaternion.identity, this.transform).GetComponent<Space>();
				row.Add(space);
				x += spaceDiameter;
			}
			userBoard.Add(row);
		}
	}

	public void UpdateView(Board board)
	{
		int i = 0;
		foreach (var row in board.View())
		{
			for (int j = 0; j < row.Count; j++)
			{
				if (row[j] == 'O')
				{
					userBoard[i][j].Clear();
				}
				else
				{
					userBoard[i][j].SetPiece((row[j] == 'B') ? blackPrefab : whitePrefab);
				}
			}
			i++;
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
		anchor.MarkSelectable(0, "");
		res.Add(anchorLocation);
		foreach (var dir in directions.Keys)
		{
			var cur = anchorLocation;
			for(var d = 1; d < 3; d++)
			{
				// Debug.Log("trying to get neighbor");
				cur = GetNeighborLocation(cur, dir);
				if (!ValidLocation(cur)) break;
				var curSpace = GetSpace(cur);
				if (curSpace.Empty()) break;
				if (curSpace.piece.color != anchor.color) break;
				curSpace.piece.MarkSelectable(d, dir);
				res.Add(cur);
			}
		}
		return res;
	}

	public Space GetSpace(Vector loc)
	{
		return board[loc.x][loc.y];
	}

	public static List<Vector> GetColumn(Vector start, int distance, string dir)
	{
		var res = new List<Vector>();
		var cur = start;
		for (int d = 0; d <= distance; d++)
		{
			res.Add(cur);
			cur = GetNeighborLocation(cur, dir);
		}
		return res;
	}

	private static Vector GetNeighborLocation(Vector location, string dir)
	{
		if (!directions.ContainsKey(dir)) return new Vector(-1, -1);
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

	public Dictionary<string, List<Vector>> GetMoves(List<Vector> selection, string selectionDirection)
	{
		Debug.Log("selectionDirection: " + selectionDirection);
		var moves = new Dictionary<string, List<Vector>>();
		foreach(var dir in directions.Keys)
		{
			var enemyColumn = new List<Vector>();
			if (CheckMove(selection, selectionDirection, dir, enemyColumn))
			{
				moves.Add(dir, enemyColumn);
			}
		}
		return moves;
	}

	private bool CheckMove(List<Vector> selection, string selectionDirection, string dir, List<Vector> enemyColumn)
	{
		if (SameAxis(selectionDirection, dir))
		{
			Debug.Log("doing sumito check");
			var enemyColor = (GetSpace(selection[0]).piece.color == "black") ? "white" : "black";
			var selectionEdge = (selectionDirection == dir) ? selection[selection.Count - 1] : selection[0];
			var enemyColumnStart = GetNeighborLocation(selectionEdge, dir);
			return Sumito(enemyColumnStart, dir, enemyColor, selection.Count - 1, enemyColumn);
		}

		// if a side step move, just make sure target spaces are empty
		Debug.Log("doing regular check");
		foreach (var loc in selection)
		{
			var neighborLocation = GetNeighborLocation(loc, dir);
			if (!ValidLocation(neighborLocation) || !GetSpace(neighborLocation).Empty()) return false;
		}
		Debug.Log("move checks out");
		return true;
	}

	private bool Sumito(Vector start, string dir, string color, int bound, List<Vector> column)
	{
		var cur = start;
		while (column.Count < bound)
		{
			if (!ValidLocation(cur)) return column.Count > 0; // edge of board - ok
			var space = GetSpace(cur);
			if (space.Empty()) return true; // empty space - ok
			if (space.piece.color != color) return false; // enemy piece in sequence - not ok
			// remaining case: piece of this color
			column.Add(cur);
			cur = GetNeighborLocation(cur, dir);
		}
		// we got an opposing column of maximum possible size
		return !ValidLocation(cur) || GetSpace(cur).Empty();
	}

	private static bool SameAxis(string dir1, string dir2)
	{
		// sort strings if necessary
		Debug.Log(dir1);
		Debug.Log(dir2);
		if (string.Compare(dir1, dir2) > 0)
		{
			var temp = dir1; dir1 = dir2; dir2 = temp;
		}
		if (dir1 == dir2) return true;
		if (dir1 == "E" && dir2 == "W") return true;
		if (dir1 == "NW" && dir2 == "SE") return true;
		if (dir1 == "NE" && dir2 == "SW") return true;
		return false;
	}

	public int Move(List<Vector> selection, List<Vector> enemyColumn, string dir)
	{
		enemyColumn.Reverse();
		if (selection.Count > 1 && GetNeighborLocation(selection[0], dir) == selection[1]) // selection direction same as move
		{
			selection.Reverse();
		}
		int res = MoveColumn(enemyColumn, dir);
		MoveColumn(selection, dir);
		return res;
	}

	private int MoveColumn(List<Vector> column, string dir)
	{
		int res = 0;
		foreach(var loc in column)
		{
			res += MovePiece(loc, dir);
		}
		return res;
	}

	private int MovePiece(Vector loc, string dir)
	{
		var space = GetSpace(loc);
		var piece = space.piece;
		space.piece = null;
		var neighborLocation = GetNeighborLocation(loc, dir);
		if (!ValidLocation(neighborLocation))
		{
			GameObject.Destroy(piece.gameObject);
			return 1;
		}
		var neighbor = GetSpace(neighborLocation);
		piece.transform.position = neighbor.transform.position;
		piece.location = neighborLocation;
		// Debug.Log("new x" + neighborLocation.x.ToString());
		// Debug.Log("new y" + neighborLocation.y.ToString());
		// Debug.Log("one");
		piece.transform.parent = neighbor.transform;
		neighbor.piece = piece;
		return 0;
		// Debug.Log("two");
	}

}
