using Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : LazySingleton<GameLoop>
{
    [SerializeField] int initialNumberBlockCount = 4;

    [SerializeField] Vector2Int gridSize;

    [SerializeField] NumberBlockManager numberBlockManager;

    protected override void Awake()
    {
        base.Awake();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        EventsService.Instance.AddSubscribersToOnInputDirectionEvent(MoveBlocksToInputDirection);

        //.SetupGrid(gridSize);
        //StartGame();
    }
    private void StartGame()
    {
        for (int i = 0; i < initialNumberBlockCount; i++)
        {
            NumBlock numBlock = numberBlockManager.CreateNumberBlockAccordingToProbability();
            if (numBlock)
            {
                //Block block = gridManager.GetRandomUnOccupiedBlock();
               // block.OccupyThisBlock(numBlock);

            }
            else
                Debug.Log("el");
        }
    }

    public void MoveBlocksToInputDirection(SwipeInputValues swipeInput)
    {
        Direction dir = Direction.none;
        switch (swipeInput)
        {
            case SwipeInputValues.Null:
                break;
            case SwipeInputValues.SwipeLeft:
                dir = Direction.left;
                break;
            case SwipeInputValues.SwipeRight:
                dir = Direction.right;
                break;
            case SwipeInputValues.SwipeUp:
                dir = Direction.up;
                break;
            case SwipeInputValues.SwipeDown:
                dir = Direction.down;
                break;
            default:
                break;
        }

        if (dir == Direction.none)
            return;
        Debug.Log("gameLoop dir:" + dir);
        Board.Instance.MoveTo(dir);
    }
    private void Update()
    {
        Board.Instance.UpdateState();
        
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(EventsService.Instance)
        EventsService.Instance.RemoveSubscribersToOnBlockOccupiedEvent(MoveBlocksToInputDirection);

    }
}
