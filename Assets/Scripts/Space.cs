using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
	public GamePiece piece;

    public bool Empty()
    {
        return piece == null;
    }
}
