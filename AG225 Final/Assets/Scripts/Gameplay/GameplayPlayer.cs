using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameplayPlayer : MonoBehaviour
{
    PhotonView myView => this.GetPhotonView();

    bool isViewMine => this.IsPhotonViewMine();

    [SerializeField]
    protected Character myCharacter;

    #region player identification variables
    protected string _username = "DEFAULT_USER";
    public string Username { get { return _username; } }

    protected string _userID = "00000";
    public string UserID { get { return _userID; } }
    [SerializeField]
    protected string _characterName = "TestCharacter";
    public string CharacterName { get { return _characterName; } }
    #endregion

    private void Awake()
    {
        if (isViewMine)
        {
            if (PlayerNetworkManager.Instance.IsTestingMode)
            {
                myCharacter = PhotonNetwork.Instantiate(CharacterName, Vector3.zero, Quaternion.identity).GetComponent<Character>();
            }
        }
        
    }

    #region Character Control Methods

    public void MoveCharacter(float axisMovement)
    {
        if (myCharacter)
        {
            myCharacter.XMovement(axisMovement);
        }
    }

    #endregion
}
