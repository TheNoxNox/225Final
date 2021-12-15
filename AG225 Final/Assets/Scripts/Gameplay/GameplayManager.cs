using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;

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

    public List<GameObject> spawnPts;

    public GameObject cardHolder;

    public GameObject playerCardPrefab;

    public TMP_Text timer;

    public MatchStats stats;

    public Slider CDslider;

    bool _gameIsStarted = false;

    public bool GameDone = false;

    public GameObject winScreen;

    public TMP_Text winnerName;

    public TMP_Text winnerScoreStock;

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
        if (!_gameIsStarted && GameInstance.Instance.IsHost)
        {
            if(playerCount == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                
                BeginGame();
                _gameIsStarted = true;
            }
        }

        UpdateTimer();

        if(stats.GameTime <= 0)
        {
            EndGame();
        }
    }

    private void BeginGame()
    {
        foreach (GameplayPlayer player in players)
        {
            player.GetPhotonView().RPC("SpawnCharacterRPC", RpcTarget.All);

        }
    }

    private void EndGame()
    {
        Time.timeScale = 0;

        if (GameInstance.Instance.IsHost)
        {
            List<GameplayPlayer> winnerCandidates = new List<GameplayPlayer>();
            foreach (GameplayPlayer player in players)
            {
                if (!player.playerOut)
                {
                    winnerCandidates.Add(player);
                }
            }

            GameplayPlayer winner = winnerCandidates[0] ?? null;
            bool isTie = false;

            if (GameInstance.Instance._gamemode == Gamemode.Stock)
            {             
                foreach (GameplayPlayer player in winnerCandidates)
                {
                    if (player.Stock > winner.Stock)
                    {
                        winner = player;
                        isTie = false;
                    }
                    else if (player.Stock == winner.Stock)
                    {
                        isTie = true;
                    }
                }

                foreach (GameplayPlayer player in players)
                {
                    player.GetPhotonView().RPC("GameEndDisplay", RpcTarget.All, winner.Username, winner.Stock, isTie);
                }
            }
            else
            {
                foreach(GameplayPlayer player in winnerCandidates)
                {
                    if(player.Score > winner.Score)
                    {
                        winner = player;
                        isTie = false;
                    }
                    else if(player.Score == winner.Score)
                    {
                        isTie = true;
                    }
                }

                foreach(GameplayPlayer player in players)
                {
                    player.GetPhotonView().RPC("GameEndDisplay", RpcTarget.All, winner.Username, winner.Score, isTie);
                }
            }
        }

        

        
    }

    public void DisplayWinner(string name, int scoreStock)
    {

    }

    public void BackToLobby()
    {
        SceneManager.LoadScene("Lobby");
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
        if (GameInstance.Instance.IsHost)
        {
            player.GetPhotonView().RPC("SetPlayerNumber", RpcTarget.AllBufferedViaServer, playerCount);
        }

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
