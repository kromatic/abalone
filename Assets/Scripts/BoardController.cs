using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
	public Transform boardSpace;
	private Board board;
	// Use this for initialization
	void Start()
	{
		board = new Board();
		var rowLengths = new List<int> {5, 6, 7, 8, 9, 8, 7, 6, 5};
		for (int i = 0; i < 9; i++)
		{
			var len = rowLengths[i];
			var row = new BoardRow();
			var x = (9 - len) * 0.4f - 3.2f;
			var y = 2.772f - i*0.693f;
			for (int j = 0; j < len; j++)
			{
				var position = new Vector3(x, y, 0);
				var cell = Instantiate(boardSpace, position, Quaternion.identity, transform);
				row.Add(cell);
				x += 0.8f;
			}
			board.Add(row);
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}
}

class BoardRow : List<Transform>
{
	public BoardRow() : base() {}
}

class Board : List<BoardRow>
{
	public Board() : base() {}
}
