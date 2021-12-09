using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    public GameObject GameplayPlayerPrefab;

    public PlayerInput playerInput;

    public GameplayPlayer myPlayer;

    public int playerCount = 0;

    public List<GameplayPlayer> players;

    public List<GameplayCard> cards;

    public GameObject cardHolder;

    public GameObject playerCardPrefab;

    public TMP_Text timer;

    public MatchStats stats;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        if (GameplayPlayerPrefab)
        {
            GameInstance.Instance.myGameplayPlayer = PhotonNetwork.Instantiate("GameplayPlayer", Vector3.zero, Quaternion.identity).GetComponent<GameplayPlayer>();
            myPlayer = GameInstance.Instance.myGameplayPlayer;
        }
    }

    private void Update()
    {
        UpdateTimer();
    }

    public void PlayerLeave(GameplayPlayer player)
    {
        playerCount--;
        cards.Remove(player.myCard);
        Destroy(player.myCard.gameObject);
        players.Remove(player);
    }

    public GameplayCard AddPlayer(GameplayPlayer player)
    {
        playerCount++;

        players.Add(player);

        GameplayCard card = Instantiate(playerCardPrefab, cardHolder.transform).GetComponent<GameplayCard>();

        card.Username = player.Username;
        card.stockCount = player.Stock;
        card.score = player.Score;

        cards.Add(card);

        return card;
    }

    public void FlipCamera(bool isFlipped)
    {
        if (isFlipped) { Camera.main.transform.rotation = Quaternion.Euler(0, 0, 180); }
        else { Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0); }
    }

    private void UpdateTimer()
    {
        TimeSpan tSpan = TimeSpan.FromSeconds(stats.GameTime);

        string tString = tSpan.ToString(@"m\:ss");

        timer?.SetText(tString);
    }
}
