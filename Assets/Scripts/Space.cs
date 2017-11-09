using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
	public GamePiece piece;

    public void Clear()
    {
        Destroy(piece);
        piece = null;
    }

    public void SetPiece(Transform prefab)
    {
        if (piece != null) Destroy(this.piece);
        piece = Instantiate(prefab, transform.position, Quaternion.identity).GetComponent<GamePiece>();
    }
}
