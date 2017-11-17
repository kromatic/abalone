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
	private bool showingSelectables;
	private bool showingSelection;
	private List<Vector> selectables;
	private List<Vector> selection;
	public bool flipBoard;

	void Awake()
	{
		game = GameObject.Find("Game").GetComponent<Game>();
		showingSelectables = showingSelection = flipBoard = false;
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
				var space = Instantiate(spacePrefab, position, Quaternion.identity, transform).GetComponent<Space>();
				space.location = new Vector(i, j);
				row.Add(space);
				x += spaceDiameter;
			}
			boardDisplay.Add(row);
		}
	}

	void Start()
	{
		board = game.board;
		UpdateView();
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
		if (showingSelectables) ClearSelectables();
		else if (showingSelection) ClearSelection();
		this.anchorLocation = anchorLocation;
		selectables = new List<Vector>();
		foreach (var pair in board.GetSelectables(anchorLocation))
		{
			var location = pair.Key; var direction = pair.Value;
			if (location == anchorLocation) MarkAnchor(location);
			else MarkSelectable(location, direction);
			selectables.Add(location);
		}
		showingSelectables = true;
	}

	private void MarkAnchor(Vector location)
	{
		GetSpace(location).piece.MarkAnchor();
	}

	private void MarkSelectable(Vector location, string direction)
	{
		GetSpace(location).piece.MarkSelectable(direction);
	}

	private void Select(Vector location)
	{
		GetSpace(location).piece.Select();
	}

	private Space GetSpace(Vector location)
	{
		return boardDisplay[location.x][location.y];
	}

	public void CompleteSelection(Vector targetLocation, string direction)
	{
		ClearSelectables();
		selection = Board.GetColumn(anchorLocation, targetLocation, direction);
		foreach (var location in selection)
		{
			Select(location);
		}
		showingSelection = true;
		foreach (var pair in board.GetMoves(selection, direction))
		{
			var moveDirection = pair.Key; var enemyColumn = pair.Value;
			var moveButton = GameObject.Find("Move" + moveDirection).GetComponent<MoveButton>();
			moveButton.Enable(selection, direction, enemyColumn);
		}
	}

	public void ClearSelection()
	{
		showingSelection = false;	
		ClearPieces(selection);
	}

	private void ClearSelectables()
	{
		showingSelectables = false;
		ClearPieces(selectables);
	}

	private void ClearPieces(List<Vector> locations)
	{
		foreach (var location in locations)
		{
			GetSpace(location).piece.Clear();
		}
	}

	public void ChangeFlipSetting()
	{
		flipBoard = !flipBoard;
		if (game.currentPlayer == 'W') FlipBoard();
	}

	public void FlipBoard()
	{
		StartCoroutine(FlipBoardCoroutine());
	}

	private IEnumerator FlipBoardCoroutine(int numberRotations = 50)
	{
		var angle = 180.0f / numberRotations;
		for (int count = 0; count < numberRotations; count++)
		{
			transform.Rotate(0, 0, angle);
			yield return null;
		}
		// transform.Rotate(0, 0, 180);
	}

}
