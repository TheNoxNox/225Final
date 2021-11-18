using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyPlayer : MonoBehaviour
{
    protected string _username = "DEFAULT_USER";
    public string Username { get { return _username; } }

    protected string _userID = "00000";
    public string UserID { get { return _userID; } }

    protected string _characterID = "000";
    public string CharacterID { get { return _characterID; } }

    public PlayerCardDisplay myCard;

    private void Awake()
    {
        gameObject.GetPhotonView().RPC("PlayerJoin", RpcTarget.AllBufferedViaServer,
            PlayerNetworkManager.Instance.Username, PlayerNetworkManager.Instance.UniqueID);
    }

    [PunRPC]
    private void PlayerJoin(string username, string id)
    {
        _username = username;
        _userID = id;
        LobbyManager.Instance.AddPlayer(this);
    }

    [PunRPC]
    private void PlayerLeave()
    {
        LobbyManager.Instance.RemovePlayer(this);
    }
}
