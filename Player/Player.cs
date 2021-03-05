using System.Collections.Generic;
using UnityEngine;

namespace Script.Player
{
    public class Player
    {
        private int playerID;
        private Color playerColor;
        private List<GameObject> units;

        public Player(int playerID, Color playerColor)
        {
            this.playerID = playerID;
            this.playerColor = playerColor;
            units = new List<GameObject>();
        }

        public void SetUnits(List<GameObject> units)
        {
            this.units = units;
        }

        public List<GameObject> GetUnits()
        {
            return units;
        }

        public void AddUnit(GameObject entity)
        {
            units.Add(entity);
        }

        public Color GetColor()
        {
            return playerColor;
        }
    }
}



