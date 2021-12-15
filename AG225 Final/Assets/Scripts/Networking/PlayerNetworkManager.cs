using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlayerNetworkManager : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// The user's local instance of the Photon networking manager. Each user will have only one of these loaded throughout the entire application.
    /// </summary>
    public static PlayerNetworkManager Instance { get; private set; }

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
    #endregion

    #region user instance identification internals
    [Header("Instance-specific information")]
    [SerializeField]
    private string _gameVersion = "Alpha1";
    [SerializeField]
    private string _username;
    [SerializeField]
    private string _uniqueID;

    #endregion

    #region game scene string info

    public string lobby = "Lobby";

    #endregion

    #region photon variables
    RoomOptions _roomOptions = new RoomOptions();
    #endregion

    #region Photon callback delegates
    public delegate void NetworkManagerEvent();

    public static event NetworkManagerEvent RoomJoinFailed;
    public static event NetworkManagerEvent RoomCreateFailed;
    public static event NetworkManagerEvent ConnectedToMaster;
    public static event NetworkManagerEvent OnDisconnect;

    #endregion

    #region networking info
    /// <summary>
    /// Whether or not the client is connected to the master server network.
    /// </summary>
    public bool IsConnected { get { return _isConnected; } }
    /// <summary>
    /// A string containing the last error given by Photon for specific circumstances, for use with debugging.
    /// </summary>
    public string LastPhotonError { get { return _lastPhotonError; } }
    /// <summary>
    /// An internal boolean that controls various debug-mode-only features for development. SHOULD BE FALSE WHEN BUILT.
    /// </summary>
    public bool IsTestingMode { get { return _isTestingMode; } }

    [Header("Networking information.")]
    [SerializeField]
    private bool _isConnected = false;
    [SerializeField]
    private string _lastPhotonError = null;
    [SerializeField]
    protected bool _isTestingMode = false;

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
            Destroy(this.gameObject);
        }

        PhotonNetwork.SendRate = 120;
        PhotonNetwork.AutomaticallySyncScene = true;
        _roomOptions.MaxPlayers = 4;

        _uniqueID = Random.Range(10000, 100000).ToString();

        if (IsTestingMode)
        {
            Debug.LogWarning("WARNING: Game is in Testing mode. Switch this off for final builds.");
        }
    }

    private void Start()
    {
        Connect();
    }

    #region external data modification interfaces

    public void SetUsername(string uName)
    {
        if (!string.IsNullOrEmpty(uName))
        {
            _username = uName;         
        }
    }

    #endregion

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
        ConnectedToMaster?.Invoke();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomCreateFailed?.Invoke();
        base.OnCreateRoomFailed(returnCode, message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomJoinFailed?.Invoke();
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(lobby);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        OnDisconnect?.Invoke();
        base.OnDisconnected(cause);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
        base.OnLeftLobby();
    }
    #endregion
}


public static class PhotonViewHelper
{
    /// <summary>
    /// Extension method made for getting photonviews easier because calling gameobject.getphotonview all the time got annoying.
    /// </summary>
    /// <param name="mono"></param>
    /// <returns>The photonview attached to this behavior's gameobject, or null if it doesn't have one.</returns>
    public static PhotonView GetPhotonView(this MonoBehaviour mono)
    {
        return mono.gameObject.GetPhotonView();
    }

    /// <summary>
    /// Extension method to shortcut gameobject.getphotonview().ismine;
    /// </summary>
    /// <param name="mono"></param>
    /// <returns>True if photonview is owned by current player, false if not owned or doesn't exist.</returns>
    public static bool IsPhotonViewMine(this MonoBehaviour mono)
    {
        PhotonView view = mono.GetPhotonView();

        if (view)
        {
            return view.IsMine;
        }
        Debug.LogError("ERROR: Attempted to check if photonview on object " + mono.gameObject.name + " is mine, but view does not exist.");
        return false;
    }
}
