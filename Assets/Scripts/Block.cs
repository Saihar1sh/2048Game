using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    Vector2Int index;

    NumBlock occupiedBlock;
    bool isOccupied = false;

    public void Init(Vector2Int _index)
    {
        index = _index;

    }
    public void OccupyThisBlock(NumBlock numBlock)
    {
        occupiedBlock = numBlock;
        numBlock.transform.SetParent(this.transform);
        numBlock.transform.SetLocalPositionAndRotation(Vector3.zero,Quaternion.identity);
        isOccupied = true;
    }

    public void Reset()
    {
        isOccupied = false;
    }
}
