using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerPhotonManager : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// The user's local instance of the Photon networking manager. Each user will have only one of these loaded throughout the entire application.
    /// </summary>
    public static PlayerPhotonManager Instance { get; private set; }

    #region user instance identification public references
    /// <summary>
    /// The version of the game that this user is currently running
    /// </summary>
    public string GameVersion { get { return _gameVersion; } }
    /// <summary>
    /// The username for this player, contained within their local instance to be sent to the client host on lobby connect.
    /// </summary>
    public string Username { get { return _username; } }
    /// <summary>
    /// A unique ID that is generated randomly each time a user enters the application, to allow for distinguishing players with the same names.
    /// </summary>
    public string UniqueID { get { return _uniqueID; } }
    /// <summary>
    /// Whether or not the client is connected to the master server network.
    /// </summary>
    public bool IsConnected { get { return _isConnected; } }
    #endregion

    #region user instance identification internals
    [Header("Instance-specific information")]
    [SerializeField]
    private string _gameVersion = "Alpha1";
    [SerializeField]
    private string _username;
    [SerializeField]
    private string _uniqueID;
    [SerializeField]
    private bool _isConnected = false;

    #endregion

    #region photon variables
    RoomOptions _roomOptions = new RoomOptions();
    #endregion

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        PhotonNetwork.AutomaticallySyncScene = true;
        _roomOptions.MaxPlayers = 4;
    }

    private void Start()
    {
        Connect();
    }

    #region networking methods
    private void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = _gameVersion;
        }
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CreateRoom()
    {
        Debug.Log("[PhotonManager][CreateRoom] Attempting to create room. . .");
        PhotonNetwork.CreateRoom(null, _roomOptions);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("[PhotonMan][JoinRandRoom]");
    }
    #endregion

    #region Photon callbacks
    public override void OnConnectedToMaster()
    {
        _isConnected = true;
    }
    #endregion
}
