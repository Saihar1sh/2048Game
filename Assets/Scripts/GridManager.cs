using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] int initialNumberBlockCount = 4;

    [SerializeField] NumberBlockManager numberBlockManager;

    [SerializeField] Vector2 blocksPlacementOffset = Vector2.one;

    [SerializeField] Block blockPrefab;

    List<Block> gridBlocksList;

    List<Block> unOccupiedgridBlocksList;

    private void Awake()
    {
        gridBlocksList = new List<Block>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetupGrid(Vector2Int.one * 5);

        StartGame();
    }

    private void StartGame()
    {
        for (int i = 0; i < initialNumberBlockCount; i++)
        { 
            NumBlock numBlock = numberBlockManager.CreateNumberBlockAccordingToProbability();
            if (numBlock)
            {
                Block block = GetRandomUnOccupiedBlock();
                block.OccupyThisBlock(numBlock);
                unOccupiedgridBlocksList.Remove(block);

            }
            else
                Debug.Log("el");
        }
    }

    private void SetupGrid(Vector2Int gridSize)
    {
        Vector2Int currentIndex = Vector2Int.zero;

        //horizontal layout //Top to Bottom
        for (int j = gridSize.y-1; j >= 0; j--)
            for (int i = 0; i < gridSize.x; i++)
            {
                currentIndex.Set(i, j);

                Block _block = Instantiate(blockPrefab, transform);
                _block.transform.SetLocalPositionAndRotation((currentIndex * blocksPlacementOffset), Quaternion.identity);
                _block.Init(currentIndex);
                gridBlocksList.Add(_block);
            }

        SetNeighboursForBlocks(gridSize);

        unOccupiedgridBlocksList = gridBlocksList;
    }

    private void SetNeighboursForBlocks(Vector2Int gridSize)
    {
        for (int i = 0; i < gridBlocksList.Count; i++)
        {
            gridBlocksList[i].AssignNeighbours(GetBlockFromList(i - gridSize.x), GetBlockFromList(i + gridSize.x), GetBlockFromList(i - 1), GetBlockFromList(i + 1));
        }
    }

    Block GetBlockFromList(int index)
    {
        if (index < 0 || index>=gridBlocksList.Count || index % 5 is 1 or 0)
            return null;

        if (gridBlocksList[index])
            return gridBlocksList[index];
        else
            return null;
    }

    public Block GetRandomUnOccupiedBlock()
    {
        int randomNum = Random.Range(0, unOccupiedgridBlocksList.Count - 1);
        return unOccupiedgridBlocksList[randomNum];
    }
}

