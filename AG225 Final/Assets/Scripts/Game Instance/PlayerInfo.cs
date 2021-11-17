using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public PlayerInfo(string username, string userID)
    {
        _username = username;
        _userID = userID;
    }

    protected string _username = "DEFAULT_USER";
    public string Username { get { return _username; } }

    protected string _userID = "00000";
    public string UserID { get { return _userID; } }

    protected string _characterID = "000";
    public string CharacterID { get { return _characterID; } }

}
