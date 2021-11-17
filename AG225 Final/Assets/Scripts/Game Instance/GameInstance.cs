using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class GameInstance : MonoBehaviour
{
    public static GameInstance Instance;

    #region host parameters

    public bool IsHost { get; private set; }



    #endregion

    #region host events

    public delegate void PlayerEvent(string playerID);

    public static event PlayerEvent OnPlayerJoin;
    public static event PlayerEvent OnPlayerLeave;

    #endregion

    protected List<PlayerInfo> players = new List<PlayerInfo>();

    #region personal information

    protected PlayerInfo myPlayerInfo;

    #endregion

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        IsHost = PhotonNetwork.IsMasterClient;

        PlayerNetworkManager.OnDisconnect += PlayerLeaveLogic;

        PlayerJoinLogic();
    }

    private void PlayerJoinLogic()
    {
        gameObject.GetPhotonView().RPC("AddPlayer", RpcTarget.AllBufferedViaServer,
            PlayerNetworkManager.Instance.Username, PlayerNetworkManager.Instance.UniqueID);
    }

    private void PlayerLeaveLogic()
    {
        gameObject.GetPhotonView().RPC("RemovePlayer", RpcTarget.AllBufferedViaServer, PlayerNetworkManager.Instance.UniqueID);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        IsHost = PhotonNetwork.IsMasterClient;
    }

    [PunRPC]
    public void AddPlayer(string uName, string uID)
    {
        myPlayerInfo = new PlayerInfo(uName, uID);
        players.Add(myPlayerInfo);
        OnPlayerJoin?.Invoke(myPlayerInfo.UserID);
    }

    [PunRPC]
    public void RemovePlayer(string uID)
    {
        foreach(PlayerInfo player in players)
        {
            if(player.UserID == uID)
            {
                players.Remove(player);
                OnPlayerLeave?.Invoke(myPlayerInfo.UserID);
            }
        }
    }

    #region information reference methods

    public string GetUsernameByID(string id)
    {
        foreach(PlayerInfo player in players)
        {
            if(player.UserID == id)
            {
                return player.Username;
            }
        }
        return "NO_PLAYER_FOUND";
    }

    #endregion
}
