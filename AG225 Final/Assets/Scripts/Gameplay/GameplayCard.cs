using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayCard : PlayerCardDisplay
{
    public GameplayPlayer myPlayer;

    public TMP_Text scoreStockText;

    public int stockCount = 3;

    public int score = 0;

    public TMP_Text healthText;

    protected override void Update()
    {
        base.Update();
        switch (GameInstance.Instance._gamemode)
        {
            case Gamemode.Stock:
                scoreStockText.text = "Stock: " + myPlayer.Stock.ToString();
                break;
            case Gamemode.Timed:
                scoreStockText.text = "Score: " + myPlayer.Score.ToString();
                break;
        }
        healthText.text = "HP: " + myPlayer.PlayerHealth.ToString();
    }
}
