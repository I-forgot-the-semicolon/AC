using UnityEngine;

namespace Script.Player
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private int playerOwner;
        [SerializeField] private Vector3 position;
        
        public int GetPlayerOwner()
        {
            return playerOwner;
        }

        public void SetPlayerOwner(int playerOwner)
        {
            this.playerOwner = playerOwner;
        }

        public Vector3 GetEntityPos()
        {
            return position;
        }

        public void SetEntityPos(Vector3 position)
        {
            this.position = position;
        }
    }
}

