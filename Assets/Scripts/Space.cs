// Space is a MonoBehaviour that represents a space on the displayed board.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    // Reference to the piece located at this space, if any. Null if space is empty.
    public GamePiece piece;

    // The location of this space on the board.
    public Vector location;

    // Prefabs used for the black and white pieces.
    public Transform blackPrefab;
    public Transform whitePrefab;

    // Clear makes this space empty by destroying the game piece if it exists.
    public void Clear()
    {
        if (piece != null) Destroy(piece.gameObject);
        if (piece == null) Debug.Log("already null after destroying");
        piece = null;
    }

    // SetPiece sets a piece at this space.
    public void SetPiece(char color)
    {
        // If we already have a piece of this color set, we are done.
        if (piece != null && piece.color == color) return;

        // Otherwise we need to set a piece.

        // If a piece did exist (of the opposite color), then destroy it.
        if (piece != null) Destroy(piece.gameObject);
        // Now we need to create the new piece.
        var prefab = (color == 'B') ? blackPrefab : whitePrefab;
        piece = Instantiate(prefab, transform.position, Quaternion.identity, transform).GetComponent<GamePiece>();
    }
}
