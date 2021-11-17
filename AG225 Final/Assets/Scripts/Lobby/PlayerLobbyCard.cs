using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLobbyCard
{
    public PlayerLobbyCard(string uName, string uID)
    {
        _username = uName;
        _userID = uID;
    }

    protected string _username = "DEFAULT_USERNAME";
    public string Username { get { return _username; } }

    protected string _userID = "00000";

    protected string _characterID = "000";
    public string CharacterID { get { return _characterID; } }

    public void SetCharacter(string charID)
    {
        _characterID = charID;
    }

    public bool IsPlayer(string otherPlayerID)
    {
        return _userID == otherPlayerID;
    }
}
