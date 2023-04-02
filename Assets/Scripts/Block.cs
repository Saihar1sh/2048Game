using ElRaccoone.Tweens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector2Int index { get; private set; }

    public NumBlock occupiedBlock;
    public bool isOccupied { get; private set; } = false;
    public int? numValue 
    { get { 
            if(isOccupied)
                return occupiedBlock.Value;
            return null;
          } 
    }

    //neighbours
    Block[] neighbourBlocks;
    Vector2Int?[] neighbourIndexes;

    public bool combined;

    public void Init(Vector2Int _index)
    {
        index = _index;
    }
    public Block FindTarget(int originalBlockValue, Direction dir, Block farNode = null)
    {
        Block dirNode = neighbourBlocks[(int)dir];
        if (dirNode)
        {
            // if already combined, return prev block
            if (dirNode != null && dirNode.combined)
                return this;
            // if two value equal return latest finded value.
            if (dirNode.numValue.HasValue && originalBlockValue!= 0)
            {
                if (dirNode.numValue == originalBlockValue)
                    return dirNode;

                if (dirNode.numValue != originalBlockValue)
                    return farNode;
            }
            return dirNode.FindTarget(originalBlockValue, dir, dirNode);
        }
        return farNode;
    }

    public NumBlock MergedBlockValueChange()
    {
        if (isOccupied)
        {
            int val = occupiedBlock.Value * 2;
            occupiedBlock.Init(val);
            return occupiedBlock;
        }
        return null;
    }

    public void OccupyThisBlock(NumBlock numBlock)
    {
        occupiedBlock = numBlock;
        numBlock.transform.SetPositionAndRotation(transform.position,Quaternion.identity);

        isOccupied = true;
        GridManager.Instance.RemoveBlockToUnOccupiedList(this);
    }

    public void SetNeighbours(Block[] _neighbours)
    {
        neighbourBlocks = _neighbours;
    }

/*    public Block GetTargetBlockInInputDirRecursion(Direction dir)
    {
        Block target = GetNeighbourBlockInInputDirection(dir);
        if(target)
        {
            while(target != null && !target.isOccupied)
            {
                Block block = target.GetNeighbourBlockInInputDirection(dir);
                if (block && !block.isOccupied)
                    target = block;
                else
                    return target;
            }
            Debug.LogWarning("",target);
        }
        return target;
    }
    private Block GetNeighbourBlockInInputDirection(Direction dir)
    {
        Block targetBlock = neighbourBlocks[(int)dir];

        if(!targetBlock)
            Debug.Log(index+" No target Block --"+dir,gameObject);

        Debug.Log("index: "+index+" "+targetBlock+" "+dir);
        
        return targetBlock;
    }

    public void MoveToTargetBlock(Block targetBlock, float timeInSecs)
    {
        MoveToTargetPosition(targetBlock.transform.position, timeInSecs);
        //occupiedBlock.transform.SetParent(targetBlock.transform);
        targetBlock.OccupyThisBlock(occupiedBlock);
        ResetOccupiedBlock();
        //occupiedBlock.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private void MoveToTargetPosition(Vector3 targetPosition, float timeInSecs)
    {
        if (isOccupied)
        {
            this.occupiedBlock.TweenPosition(targetPosition, timeInSecs);
            occupiedBlock.transform.SetPositionAndRotation(targetPosition, Quaternion.identity);

        }
    }
*/

    public void ResetOccupiedBlock()
    {
        isOccupied = false;
        occupiedBlock = null;
        GridManager.Instance.AddBlockToUnOccupiedList(this);
    }


    //testing purpose
    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.V))
            Debug.Log(index);
    }
}
