using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2 blocksPlacementOffset = Vector2.one;

    [SerializeField] Block blockPrefab;

    List<Block> gridBlocksList;

    private void Awake()
    {
        gridBlocksList = new List<Block>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetupGrid(Vector2Int.one * 5);
    }
    private void SetupGrid(Vector2Int gridSize)
    {
        Vector2Int currentIndex = Vector2Int.zero;
        for (int i = 0; i < gridSize.x; i++)
            for (int j = 0; j < gridSize.y; j++)
            {
                currentIndex.Set(i, j);

                Block _block = Instantiate(blockPrefab,transform);
                _block.transform.SetLocalPositionAndRotation((currentIndex * blocksPlacementOffset), Quaternion.identity);
                _block.Init(currentIndex);
                gridBlocksList.Add(_block);
            }
    }
    public Block GetRandomBlock()
    {
        int randomNum = Random.Range(0, gridBlocksList.Count - 1);
        return gridBlocksList[randomNum];
    }
}

