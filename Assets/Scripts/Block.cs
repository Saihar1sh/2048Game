using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    Vector2Int index;

    NumBlock occupiedBlock;
    bool isOccupied = false;

    //neighbours
    Block upperBlock;
    Block lowerBlock;
    Block leftBlock;
    Block rightBlock;

    public void Init(Vector2Int _index)
    {
        index = _index;

    }

    public void AssignNeighbours(Block _upperBlock,Block _lowerBlock,Block _leftBlock,Block _rightBlock)
    {
        upperBlock = _upperBlock;
        lowerBlock = _lowerBlock;
        leftBlock  = _leftBlock;
        rightBlock = _rightBlock;
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
