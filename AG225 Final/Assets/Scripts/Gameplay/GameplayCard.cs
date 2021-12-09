using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayCard : PlayerCardDisplay
{
    public TMP_Text scoreStockText;

    public int stockCount = 3;

    public int score = 0;

    protected override void Update()
    {
        base.Update();
        switch (GameInstance.Instance._gamemode)
        {
            case Gamemode.Stock:
                scoreStockText.text = "Stock: " + stockCount.ToString();
                break;
            case Gamemode.Timed:
                scoreStockText.text = "Score: " + score.ToString();
                break;
        }
    }
}
