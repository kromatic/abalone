using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
	public GamePiece piece;
    public Transform blackPrefab;
	public Transform whitePrefab;
    public Vector location;

    public void Clear()
    {
        if (piece != null) { print("destroying piece"); Destroy(piece.gameObject); }
        piece = null;
    }

    public void SetPiece(Transform prefab)
    {
        // can make this a bit more efficient
        if (piece != null) Destroy(piece.gameObject);
        piece = Instantiate(prefab, transform.position, Quaternion.identity, transform).GetComponent<GamePiece>();
        // piece.transform.localScale = prefab.localScale * transform.localScale;
        piece.location = location;
    }
}
