using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ElRaccoone.Tweens;

namespace Game.Core
{
    public class Board : LazySingleton<Board>
    {
        public State state = State.WAIT;

        public List<NumBlock> realNodeList = new();
        public List<Block> blockData = new();
        public Dictionary<Vector2Int, Block> blockMap = new();
        public Vector2Int gridSize = new(5,5);
        public Vector2 cellSpacing = new(.25f, .25f);

        public GameObject emptyNodePrefab;
        public GameObject nodePrefab;
        public Transform emptyNodeRect;
        public Transform realNodeRect;

        public void OnGameOver()
        {
            Debug.Log("Game Over!!!!");
        }
        private void CreateBoard()
        {
            /* first initialize empty Node rect*/
            realNodeList.Clear();
            blockMap.Clear();
            blockData.Clear();

            Vector2Int currentIndex = new(-1,-1);

            /* and, empty node create for get grid point*/
            for (int j = 0; j < gridSize.y; j++)
            {
                for (int i = 0; i < gridSize.x; i++)
                {
                    var instantiatePrefab = GameObject.Instantiate(emptyNodePrefab, emptyNodeRect.transform, false);
                    currentIndex.Set(i, j);
                    Vector2Int left = currentIndex + Vector2Int.left;
                    Vector2Int down = currentIndex + Vector2Int.down;
                    Vector2Int right = currentIndex + Vector2Int.right;
                    Vector2Int up = currentIndex + Vector2Int.up;
                    Vector2Int[] v = new Vector2Int[4] {Vector2Int.one *-1, Vector2Int.one * -1, Vector2Int.one * -1, Vector2Int.one * -1 };
                    if (IsValid(right)) v[(int)Direction.right] = right;
                    if (IsValid(down)) v[(int)Direction.down] = down;
                    if (IsValid(left)) v[(int)Direction.left] = left;
                    if (IsValid(up)) v[(int)Direction.up] = up;
                    Block node = new(v);
                    node.index = currentIndex;
                    node.nodeRectObj = instantiatePrefab;
                    blockData.Add(node);
                    node.position = (Vector2.one + cellSpacing) * currentIndex;
                    instantiatePrefab.name = node.index.ToString();
                    instantiatePrefab.transform.SetLocalPositionAndRotation(node.position,Quaternion.identity);
                    this.blockMap.Add(currentIndex, node);
                }
            }

            foreach (Block item in blockData)
            {
                item.AssignNeighbours();
            }
        }

        private bool IsValid(Vector2Int point)
        {
            if (point.x == -1 || point.x == gridSize.x || point.y == -1 || point.y == gridSize.y)
                return false;

            return true;
        }
        private void CreateBlock(int x, int y)
        {
            if (blockMap[new Vector2Int(x, y)].numberBlock != null) return;

            GameObject realNodeObj = Instantiate(nodePrefab, realNodeRect.transform, false);
            var node = blockMap[new Vector2Int(x, y)];
            var pos = node.position;
            realNodeObj.transform.localPosition = pos;
            realNodeObj.transform.TweenLocalScale(new Vector3(1.32f, 1.32f, 1.32f), 0.15f).SetEaseLinear().SetPingPong();
            var nodeObj = realNodeObj.GetComponent<NumBlock>();
            this.realNodeList.Add(nodeObj);
            nodeObj.InitializeFirstValue();
            node.value = nodeObj.value;
            node.numberBlock = nodeObj;
        }

        public void Combine(Block from, Block to)
        {
            //   Debug.Log($"TRY COMBINE {from.point} , {to.point}");
            to.value = to.value * 2;
            from.value = null;
            if (from.numberBlock != null)
            {
                from.numberBlock.CombineToNode(from, to);
                from.numberBlock = null;
                to.combined = true;
            }
        }

        public void Move(Block from, Block to)
        {
            //        Debug.Log($"TRY MOVE {from.point} , {to.point}");
            to.value = from.value;
            from.value = null;
            if (from.numberBlock != null)
            {
                from.numberBlock.MoveToNode(from, to);
                if (from.numberBlock != null)
                {
                    to.numberBlock = from.numberBlock;
                    from.numberBlock = null;
                    Debug.Log(to.numberBlock != null);
                }
            }
        }
        public void MoveTo(Direction direction)
        {
            StartCoroutine(MoveTo_Coroutine(direction));
        }
        /// <summary>
        /// Move Blocks by User Input.
        /// </summary>
        /// <param name="dir"></param>
        public IEnumerator MoveTo_Coroutine(Direction dir)
        {
            yield return new WaitWhile(()=> { return state != State.WAIT; });

            Vector2Int currentIndex = new(-1, -1);
            switch (dir)
            {
                case Direction.left:
                    for (int j = 0; j < gridSize.y; j++)
                    {
                        for (int i = 1; i < gridSize.x; i++)
                        {
                            currentIndex.Set(i, j);
                            MoveLogic(currentIndex,dir);
                        }
                    }

                    break;
                case Direction.right:
                    for (int j = 0; j < gridSize.y; j++)
                    {
                        for (int i = (gridSize.x - 2); i >= 0; i--)
                        {
                            currentIndex.Set(i, j);
                            MoveLogic(currentIndex,dir);
                        }
                    }

                    break;
                case Direction.up:
                    for (int j = gridSize.y - 2; j >= 0; j--)
                    {
                        for (int i = 0; i < gridSize.x; i++)
                        {
                            currentIndex.Set(i, j);
                            MoveLogic(currentIndex,dir);
                        }
                    }

                    break;
                case Direction.down:
                    for (int j = 1; j < gridSize.y; j++)
                    {
                        for (int i = 0; i < gridSize.x; i++)
                        {
                            currentIndex.Set(i, j);
                            MoveLogic(currentIndex,dir);
                        }
                    }

                    break;
                case Direction.none:
                    break;
                default:
                    break;
            }

            foreach (var data in realNodeList)
            {
                if (data.target != null)
                {
                    state = State.PROCESSING;
                    data.StartMoveAnimation();
                }
            }
            Show();
            if (IsGameOver())
            {
                OnGameOver();
            }
            //UpdateState();
        }

        private void MoveLogic(Vector2Int currentIndex,Direction direction)
        {
            var node = blockMap[currentIndex];
            if (node.value != null)
            {
                var dirBlock = node.FindTarget(node.value.Value, direction);
                if (dirBlock != null)
                {
                    if (node.value.HasValue && dirBlock.value.HasValue)
                    {
                        if (node.value == dirBlock.value)
                        {
                            Combine(node, dirBlock);
                        }
                    }
                    else if (dirBlock != null && dirBlock.value.HasValue == false)
                    {
                        Move(node, dirBlock);
                    }
                    else if (dirBlock == null)
                        return;
                }
            }
        }

        /// <summary>
        /// if can't combine anymore then game over!!!!
        /// </summary>
        /// <returns></returns>
        public bool IsGameOver()
        {
            bool gameOver = true;
            blockData.ForEach(x =>
            {
                for (int i = 0; i < x.neighbours.Length; i++)
                {
                    if (x.numberBlock == null) gameOver = false;
                    if (x.neighbours[i] == Vector2Int.one*-1)
                        continue;

                    var nearNode = blockMap[x.neighbours[i]];
                    if (x.value != null && nearNode.value != null)
                        if (x.value == nearNode.value)
                        {
                            gameOver = false;
                        }
                }
            });

            return gameOver;
        }
        private void CreateRandom()
        {
            var emptys = blockData.FindAll(x => x.numberBlock == null);
            if (emptys.Count == 0)
            {
                if (IsGameOver())
                {
                    OnGameOver(); ;
                }
            }
            else
            {
                var rand = UnityEngine.Random.Range(0, emptys.Count);
                var pt = emptys[rand].index;
                CreateBlock(pt.x, pt.y);
            }
        }
        protected override void Awake()
        {
            base.Awake();
            CreateBoard();

        }

        public void UpdateState()
        {
            bool targetAllNull = true;
            foreach (var data in realNodeList)
            {
                if (data.target != null)
                {
                    targetAllNull = false;
                    break;
                }
            }

            if (targetAllNull)
            {
                if (state == State.PROCESSING)
                {
                    var removeList = realNodeList.FindAll((block) => block.needDestroy == true);
                    foreach (var item in removeList)
                    {
                        realNodeList.Remove(item);
                        GameObject.Destroy(item.gameObject);
                    }
                    state = State.END;
                }
            }

            if (state == State.END)
            {
                blockData.ForEach(x => x.combined = false);
                //            Debug.Log("init nodes, try move!");
                state = State.WAIT;
                CreateRandom();
            }
        }

        private void Show()
        {
            string v = null;
            for (int i = gridSize.y - 1; i >= 0; i--)
            {
                for (int j = 0; j < gridSize.x; j++)
                {
                    var p = blockMap[new Vector2Int(j, i)].value;
                    string t = p.ToString();
                    if (p.HasValue == false)
                    {
                        t = "N";
                    }
                    if (p == 0) t = "0";

                    v += t + " ";
                }
                v += "\n";
            }
            Debug.Log(v);
        }
/*        private void Update()
        {
            UpdateState();

            if (Input.GetKeyUp(KeyCode.Space))
            {
                Show();
            }
        }
*/
        private void Start()
        {
            CreateRandom();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
    public enum State
    {
        WAIT, PROCESSING, END
    }

}


