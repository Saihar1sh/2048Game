using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : LazySingleton<GridManager>
{
    public Dictionary<Vector2Int, Block> blocksMap = new Dictionary<Vector2Int, Block>();

    [SerializeField] Vector2 blocksPlacementOffset = Vector2.one;

    [SerializeField] Block blockPrefab;

    List<Block> unOccupiedgridBlocksList;

    private Vector2Int gridSize;

    private State state;

    protected override void Awake()
    {
        base.Awake();
        //unOccupiedgridBlocksList = new();
    }

    public void SetupGrid(Vector2Int _gridSize)
    {
        Vector2Int currentIndex = Vector2Int.zero;
        gridSize = _gridSize;

        //horizontal layout //Top to Bottom
        for (int j = 0; j < gridSize.y; j++)
            for (int i = 0; i < gridSize.x; i++)
            {
                currentIndex.Set(i, j);

                Block _block = Instantiate(blockPrefab, transform);
                _block.transform.SetLocalPositionAndRotation((currentIndex * blocksPlacementOffset), Quaternion.identity);
                blocksMap.Add(currentIndex, _block);
                _block.Init(currentIndex);
            }

        SetNeighboursForBlocks();
        unOccupiedgridBlocksList = new List<Block>(blocksMap.Values);
    }
    private bool IsValidIndex(Vector2Int index)
    {
        if (index.x < 0 || index.x >= gridSize.x || index.y < 0 || index.y >= gridSize.y)
            return false;
        else
            return true;
    }
    private void SetNeighboursForBlocks()
    {
        foreach (Block item in blocksMap.Values)
        {
            Vector2Int _index = item.index;
            Block[] neighbourBlocks = new Block[4];

            Vector2Int left = _index + Vector2Int.left;
            Vector2Int right = _index + Vector2Int.right;
            Vector2Int up = _index + Vector2Int.up;
            Vector2Int down = _index + Vector2Int.down;

            neighbourBlocks[(int)Direction.left] = IsValidIndex(left) ? blocksMap[left] : null;
            neighbourBlocks[(int)Direction.right] = IsValidIndex(right) ? blocksMap[right] : null;
            neighbourBlocks[(int)Direction.up] = IsValidIndex(up) ? blocksMap[up] : null;
            neighbourBlocks[(int)Direction.down] = IsValidIndex(down) ? blocksMap[down] : null;

            item.SetNeighbours(neighbourBlocks);
        }
    }
    public void MoveTo(Direction dir)
    {
        switch (dir)
        {
            case Direction.left:
                for (int j = 0; j < gridSize.y; j++)
                {
                    for (int i = 1; i < gridSize.x; i++)
                    {
                        Vector2Int index = new Vector2Int(i, j);
                        BlockMovement(index, dir);
                    }
                }
                break;
            case Direction.right:
                for (int j = 0; j < gridSize.y; j++)
                {
                    for (int i = (gridSize.x - 2); i >= 0; i--)
                    {
                        Vector2Int index = new Vector2Int(i, j);
                        BlockMovement(index,dir);
                    }
                }
                break;
            case Direction.up:
                for (int j = gridSize.y - 2; j >= 0; j--)
                {
                    for (int i = 0; i < gridSize.x; i++)
                    {
                        Vector2Int index = new Vector2Int(i, j);
                        BlockMovement(index, dir);
                    }
                }
                break;
            case Direction.down:
                for (int j = 1; j < gridSize.y; j++)
                {
                    for (int i = 0; i < gridSize.x; i++)
                    {
                        Vector2Int index = new Vector2Int(i, j);
                        BlockMovement(index, dir);
                    }
                }

                break;
            case Direction.none:
                break;
            default:
                break;
        }

        foreach (var data in blocksMap.Values)
        {
            if (data.occupiedBlock != null)
            {
                state = State.Processing;
                data.occupiedBlock.StartMoveAnimation();
            }
        }
        
        /*        Show();
                if (IsGameOver())
                {
                    OnGameOver();
                }
        */
    }

    private void BlockMovement(Vector2Int _index, Direction dir)
    {
        Block node = blocksMap[_index];
        if (node.isOccupied)
        {
            Block dirNode = node.FindTarget(node.numValue.Value,dir);
            if (dirNode != null)
            {
                if (node.numValue.HasValue && dirNode.numValue.HasValue)
                {
                    if (node.numValue == dirNode.numValue)
                    {
                        Combine(node, dirNode);
                    }
                }
                else if (dirNode != null && dirNode.numValue.HasValue == false)
                {
                    Move(node, dirNode);
                }
                else if (dirNode == null)
                    return;
            }
        }

    }
    public void Combine(Block from, Block to)
    {
        //   Debug.Log($"TRY COMBINE {from.point} , {to.point}");
        //to.value = to.value * 2;
        //from.value = null;
        if (from.occupiedBlock != null)
        {
            from.occupiedBlock.CombineToBlock(from, to);
            from.ResetOccupiedBlock();
            to.combined = true;
        }
    }

    public void Move(Block from, Block to)
    {
        //        Debug.Log($"TRY MOVE {from.point} , {to.point}");
        //to.value = from.value;
        //from.value = null;
        if (from.isOccupied)
        {
            from.occupiedBlock.MoveToBlock(from, to);
            if (from.occupiedBlock != null)
            {
                to.occupiedBlock = from.occupiedBlock;

                from.ResetOccupiedBlock();
                Debug.Log(to.occupiedBlock != null);
            }
        }
    }


    /*    public void MoveNumberBlocksInInputDirection(Direction dir)
        {
            foreach (Block item in blocksMap.Values)
            {
                if(item.isOccupied)
                {
                   Block target = item.FindTarget(item,dir);
                    if (target)
                        BlockMovement(target, 1f);
                    else
                        Debug.Log("there is no block in dir:" + dir, item);
                }
            }
        }
    */
    public void AddBlockToUnOccupiedList(Block block)
    {
        if(!unOccupiedgridBlocksList.Contains(block))
        {
            unOccupiedgridBlocksList.Add(block);
        }
        else
        {
            Debug.LogWarning("Block is already in list", block);
        }
    }
    public void RemoveBlockToUnOccupiedList(Block block)
    {
        if (block == null)
            return;
        if(unOccupiedgridBlocksList.Contains(block))
        {
            unOccupiedgridBlocksList.Remove(block);
        }
        else
        {
            Debug.LogWarning("Block is not in list", block);
        }
    }
    public Block GetRandomUnOccupiedBlock()
    {
        int randomNum = Random.Range(0, unOccupiedgridBlocksList.Count - 1);
        return unOccupiedgridBlocksList[randomNum];
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

public enum State
{
    Wait,
    Processing,
    End
}