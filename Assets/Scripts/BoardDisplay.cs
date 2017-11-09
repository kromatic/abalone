using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDisplay : MonoBehaviour
{
	public Transform spacePrefab;
	public Transform blackPrefab;
	public Transform whitePrefab;
	public float paddingFactor = 1.1f;
	private Board board;
	private Game game;
	private List<List<Space>> boardDisplay;
	private Vector anchorLocation;
	private List<Vector> selectables;
	private List<Vector> selection;

	void Awake()
	{
		game = GameObject.Find("Game").GetComponent<Game>();
		// create empty board
		boardDisplay = new List<List<Space>>(); 
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
			boardDisplay.Add(row);
		}
	}

	public void UpdateView()
	{
		int i = 0;
		foreach (var row in board.View())
		{
			for (int j = 0; j < row.Count; j++)
			{
				if (row[j] == 'O')
				{
					boardDisplay[i][j].Clear();
				}
				else
				{
					boardDisplay[i][j].SetPiece((row[j] == 'B') ? blackPrefab : whitePrefab);
				}
			}
			i++;
		}
	}

	public void Anchor(Vector anchorLocation)
	{
		this.anchorLocation = anchorLocation;
		selectables = new List<Vector>();
		foreach (var pair in board.GetSelectables(anchorLocation))
		{
			var location = pair.Key; var direction = pair.Value;
			MarkSelectable(location, direction);
			selectables.Add(location);
		}
	}

	private void MarkSelectable(Vector location, string direction)
	{
		GetSpace(location).piece.MarkSelectable(direction);
	}

	private Space GetSpace(Vector location)
	{
		return boardDisplay[location.x][location.y];
	}

	public void CompleteSelection(Vector targetLocation, string direction)
	{
		selection = Board.GetColumn(anchorLocation, targetLocation, direction);
		foreach (var pair in board.GetMoves(selection, direction))
		{
			var moveDirection = pair.Key; var enemyColumn = pair.Value;
			var moveButton = GameObject.Find("Move" + moveDirection);
			moveButton.Enable(enemyColumn);
		}
	}

}
