
using System; 
using UnityEngine;
using UnityEngine.UI;
//using Game.Core;

namespace Game.Core
{
    [System.Serializable]
    public class Block
    {
        public Block(Vector2Int[] _neighbours)
        {
            neighbours = _neighbours;
        }
        [NonSerialized] public GameObject nodeRectObj;
        public NumBlock numberBlock;
        public int? value = null;
        public Vector2Int index;
        public Vector2 position;
        public bool combined = false;
        public Vector2Int[] neighbours = null;

        [NonSerialized] private Block[] neighbourBlocks;


        public void AssignNeighbours()
        {
            neighbourBlocks = new Block[neighbours.Length];
            for (int i = 0; i < neighbours.Length; i++)
            {
                if(neighbours[i] != Vector2Int.one*-1)
                    neighbourBlocks[i] = Board.Instance.blockMap[neighbours[i]];
            }
        }

        public Block FindTarget(int originalNodeValue, Direction dir, Block farNode = null)
        {
            if (neighbourBlocks[(int)dir] != null)
            {
                var dirNode = neighbourBlocks[(int)dir];
                // if already combined, return prev block
                if (dirNode != null && dirNode.combined)
                    return this;
                // if two value equal return latest finded value.
                if (dirNode.value.HasValue && originalNodeValue != 0)
                {
                    if (dirNode.value == originalNodeValue)
                        return dirNode;

                    if (dirNode.value != originalNodeValue)
                        return farNode;
                }
                return dirNode.FindTarget(originalNodeValue, dir, dirNode);
            }
            return farNode;
        }
    }
}