using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    public string testLevelName = "TestArena";

    public List<LobbyPlayer> players = new List<LobbyPlayer>();

    #region player card info

    [Header("Player Card Info")]
    public List<PlayerCardDisplay> cardDisplays = new List<PlayerCardDisplay>();

    public GameObject cardHolder;

    public GameObject playerCardPrefab;

    #endregion

    #region lobby info display

    [Header("Lobby Info Display")]
    public TMP_Text playerCounter;

    public TMP_Text gamemodeDisp;

    public GameObject hostUI;

    public Button startButton;

    #endregion

    #region lobby info

    protected int playerCount = 0;
    public int PlayerCount { get { return playerCount; } }

    #endregion

    [Header("Instantiation prefabs")]
    public GameObject gameInstancePrefab;
    public GameObject lobbyPlayerPrefab;

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

        PhotonNetwork.CurrentRoom.IsOpen = true;

        //Instantiate(gameInstancePrefab, Vector3.zero, Quaternion.identity);

        

        //GameInstance.OnPlayerJoin += PlayerJoin;
        //GameInstance.OnPlayerLeave += PlayerLeave;

        //usernameTemp.text = "Your username is " + PlayerNetworkManager.Instance.Username;

        //gameObject.GetPhotonView().RPC("ShowPlayerJoin", RpcTarget.AllBufferedViaServer, PlayerNetworkManager.Instance.Username);
    }

    private void Start()
    {
        if (lobbyPlayerPrefab)
        {
            GameInstance.Instance.myLobbyPlayer = PhotonNetwork.Instantiate("LobbyPlayer", Vector3.zero, Quaternion.identity).GetComponent<LobbyPlayer>();
        }
    }

    private void Update()
    {
        UpdatePlayerCount();

        if(GameInstance.Instance._gamemode == Gamemode.Stock)
        {
            gamemodeDisp.text = "Current Gamemode: \n" + "Stock"; 
        }
        else
        {
            gamemodeDisp.text = "Current Gamemode: \n" + "Timed";
        }

        if (GameInstance.Instance.IsHost && !hostUI.activeSelf)
        {
            hostUI.SetActive(true);
        }
        else if(!GameInstance.Instance.IsHost && hostUI.activeSelf)
        {
            hostUI.SetActive(false);
        }

        if(playerCount < 2) { startButton.interactable = false; }
        else { startButton.interactable = true; }
    }

    #region UI update methods

    private void UpdatePlayerCount()
    {
        playerCounter.text = "Players: " + playerCount;
    }

    #endregion

    #region host logic

    public void StartGame()
    {
        if (GameInstance.Instance.IsHost)
        {
            Debug.Log("Joining using test string. Change this to desired level for finished product.");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(testLevelName);
        }
    }

    public void ChangeGamemode(string gm)
    {
        GameInstance.Instance.GetPhotonView().RPC("SetGamemode", RpcTarget.AllBufferedViaServer, gm);
    }

    //public void PlayerJoin(string playerID)
    //{
    //    gameObject.GetPhotonView().RPC("SendHostJoinNotification", RpcTarget.AllBufferedViaServer, playerID);
    //    Debug.Log("PlayerJoin called");
    //}

    //public void PlayerLeave(string playerID)
    //{
    //    gameObject.GetPhotonView().RPC("SendHostLeaveNotification", RpcTarget.AllBufferedViaServer, playerID);
    //}

    #endregion


    #region Game start methods

    #endregion


    #region Menu Options

    public void LeaveLobby()
    {
        GameInstance.Instance.myLobbyPlayer.gameObject.GetPhotonView().RPC("PlayerLeave", RpcTarget.AllBufferedViaServer);

        PhotonNetwork.LeaveRoom();
    }

    #endregion


    #region information update methods

    public PlayerCardDisplay AddPlayer(LobbyPlayer player)
    {
        playerCount++;

        PlayerCardDisplay pCard = Instantiate(playerCardPrefab, cardHolder.transform).GetComponent<PlayerCardDisplay>();

        pCard.Username = player.Username;
        pCard.PlayerID = player.UserID;

        cardDisplays.Add(pCard);

        return pCard;
    }

    public void RemovePlayer(LobbyPlayer player)
    {
        playerCount--;

        cardDisplays.Remove(player.myCard);
        Destroy(player.myCard.gameObject);
        players.Remove(player);
    }

    //[PunRPC]
    //private void AddPlayerCard(string playerID, string playerName)
    //{
    //    playerCount++;

    //    PlayerCardDisplay pCard = Instantiate(playerCardPrefab, cardHolder.transform).GetComponent<PlayerCardDisplay>();

    //    pCard.Username = playerName;
    //    pCard.PlayerID = playerID;

    //    if (pCard) { cardDisplays.Add(pCard); }
    //}

    //[PunRPC]
    //private void RemovePlayerCard(string playerID, int indexToRemove)
    //{


    //    playerCount--;
    //}

    #endregion


    #region host methods

    //[PunRPC]
    //private void SendHostJoinNotification(string playerID)
    //{
    //    Debug.Log("SendHostJoin called");
    //    if (GameInstance.Instance.IsHost)
    //    {
    //        gameObject.GetPhotonView().RPC("AddPlayerCard", RpcTarget.AllBufferedViaServer, 
    //            playerID, GameInstance.Instance.GetUsernameByID(playerID));
    //        Debug.Log("Instance is host.");
    //    }
    //}

    //[PunRPC]
    //private void SendHostLeaveNotification(string playerID)
    //{
    //    if (GameInstance.Instance.IsHost)
    //    {

    //    }
    //}

    #endregion
}
