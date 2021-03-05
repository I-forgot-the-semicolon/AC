using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnHelper : MonoBehaviour
{
    [SerializeField] private int Turn;
    private int lastTurn;
    [SerializeField] private TextMeshProUGUI TurnText;
    
    private void Start()
    {
        lastTurn = 0;
        Turn = 1;
    }

    private void Update()
    {
        if (lastTurn != Turn)
        {
            TurnText.text = "Turno: " + Turn;
            lastTurn = Turn;
        }
    }

    public void NextTurn()
    {
        
    }
}
