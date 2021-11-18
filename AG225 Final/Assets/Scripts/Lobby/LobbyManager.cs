using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;


    public TMP_Text usernameTemp;

    public TMP_Text userListTemp;

    public List<PlayerCardDisplay> cardDisplays = new List<PlayerCardDisplay>();

    public List<LobbyPlayer> players = new List<LobbyPlayer>();

    public GameObject cardHolder;

    public GameObject playerCardPrefab;

    protected int playerCount = 0;

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

        Instantiate(gameInstancePrefab, Vector3.zero, Quaternion.identity);

        if (lobbyPlayerPrefab)
        {
            GameInstance.Instance.myLobbyPlayer = PhotonNetwork.Instantiate("LobbyPlayer", Vector3.zero, Quaternion.identity).GetComponent<LobbyPlayer>();
        }

        //GameInstance.OnPlayerJoin += PlayerJoin;
        //GameInstance.OnPlayerLeave += PlayerLeave;

        //usernameTemp.text = "Your username is " + PlayerNetworkManager.Instance.Username;

        //gameObject.GetPhotonView().RPC("ShowPlayerJoin", RpcTarget.AllBufferedViaServer, PlayerNetworkManager.Instance.Username);
    }

    [PunRPC]
    public void ShowPlayerJoin(string uName)
    {
        userListTemp.text += "\n" + uName;
    }

    #region host logic

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
