using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class LobbyManager : MonoBehaviour
{
    public TMP_Text usernameTemp;

    public TMP_Text userListTemp;

    public List<PlayerCardDisplay> cardDisplays = new List<PlayerCardDisplay>();

    protected int playerCount = 0;

    private void Awake()
    {
        GameInstance.OnPlayerJoin += PlayerJoin;
        GameInstance.OnPlayerLeave += PlayerLeave;

        usernameTemp.text = "Your username is " + PlayerNetworkManager.Instance.Username;

        gameObject.GetPhotonView().RPC("ShowPlayerJoin", RpcTarget.AllBufferedViaServer, PlayerNetworkManager.Instance.Username);
    }

    [PunRPC]
    public void ShowPlayerJoin(string uName)
    {
        userListTemp.text += "\n" + uName;
    }

    #region host logic

    public void PlayerJoin(string playerID)
    {
        gameObject.GetPhotonView().RPC("SendHostJoinNotification", RpcTarget.AllBufferedViaServer, playerID);
    }

    public void PlayerLeave(string playerID)
    {
        gameObject.GetPhotonView().RPC("SendHostLeaveNotification", RpcTarget.AllBufferedViaServer, playerID);
    }

    #endregion


    #region information update methods
    [PunRPC]
    private void AddPlayerCard(string playerID, string playerName)
    {
        playerCount++;


    }

    [PunRPC]
    private void RemovePlayerCard(string playerID, int indexToRemove)
    {


        playerCount--;
    }

    #endregion


    #region host methods

    [PunRPC]
    private void SendHostJoinNotification(string playerID)
    {
        if (GameInstance.Instance.IsHost)
        {
            gameObject.GetPhotonView().RPC("AddPlayerCard", RpcTarget.AllBufferedViaServer, 
                playerID, GameInstance.Instance.GetUsernameByID(playerID));
        }
    }

    [PunRPC]
    private void SendHostLeaveNotification(string playerID)
    {
        if (GameInstance.Instance.IsHost)
        {

        }
    }

    #endregion
}
