using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
	public GamePiece piece;
    public Vector location;

    public void Clear()
    {
        Destroy(piece);
        piece = null;
    }

    public void SetPiece(Transform prefab)
    {
        if (piece != null) Destroy(this.piece);
        piece = Instantiate(prefab, transform.position, Quaternion.identity, transform).GetComponent<GamePiece>();
        // piece.transform.localScale = prefab.localScale * transform.localScale;
        piece.location = location;
    }
}
