using UnityEngine;

namespace Script.Map_Generation
{
    public enum ChunkType
    {
        NonWalkable,
        Free,
        Gold,
        Iron,
        Trees
    }
    
    public class Chunk
    {
        private readonly Vector3 pos;
        private readonly Vector2Int indexVector;
        private ChunkType chunkType;
        private GameObject parentGameObject;

        private float resourceAmount;
        
        public Chunk(Vector3 pos, Vector2Int indexVector)
        {
            this.pos = pos;
            this.indexVector = indexVector;
            parentGameObject = null;
        }
        
        public Chunk(Vector2 pos, Vector2Int indexVector)
        {
            this.pos = new Vector3(pos.x, 0, pos.y);
            this.indexVector = indexVector;
            parentGameObject = null;
        }

        public void SetParentGameObject(GameObject gameObject)
        {
            parentGameObject = gameObject;
        }

        public GameObject GetParentGameObject()
        {
            return parentGameObject;
        }
        
        public Vector3 GetPos()
        {
            return pos;
        }
        
        public Vector2 GetPosV2()
        {
            return new Vector2(pos.x, pos.z);
        }

        public Vector2Int GetIndexes()
        {
            return indexVector;
        }
        
        public void SetChunkType(ChunkType type)
        {
            chunkType = type;
        }
        
        public ChunkType GetChunkType()
        {
            return chunkType;
        }

        public float GetResourceAmount()
        {
            return resourceAmount;
        }

        public void SetResourceAmount(float amount)
        {
            resourceAmount = amount;
        }
        
        
    }
}