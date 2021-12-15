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
    public Character MyCharacter { get { return myCharacter; } }

    #region player identification variables
    protected string _username = "DEFAULT_USER";
    public string Username { get { return _username; } }

    protected string _userID = "00000";
    public string UserID { get { return _userID; } }
    [SerializeField]
    protected string _characterName = "TestCharacter";
    public string CharacterName { get { return _characterName; } }

    public int playerNum;
    #endregion

    #region game score variables

    private int _score = 0;
    public int Score { get { return _score; } }

    private int _stock = 3;
    public int Stock { get { return _stock; } }

    public GameplayCard myCard;

    #endregion

    private GameObject spawnPoint;

    private void Awake()
    {
        if (isViewMine)
        {
            this.GetPhotonView().RPC("PlayerJoin", RpcTarget.AllBufferedViaServer, 
                PlayerNetworkManager.Instance.Username, PlayerNetworkManager.Instance.UniqueID,GameInstance.Instance.characterName);            
        }
        
    }

    [PunRPC]
    public void SpawnCharacterRPC()
    {
        SpawnCharacter();
    }

    public void SpawnCharacter()
    {
        if (isViewMine)
        {
            myCharacter = PhotonNetwork.Instantiate(CharacterName, spawnPoint.transform.position, Quaternion.identity).GetComponent<Character>();
            myCharacter.GetPhotonView().RPC("SetNametag", RpcTarget.AllBufferedViaServer, Username);
        }
    }

    [PunRPC]
    public void SetPlayerNumber(int pNumber)
    {
        playerNum = pNumber + 1;
        spawnPoint = GameplayManager.Instance.spawnPts[pNumber];
    }

    [PunRPC]
    public void PlayerJoin(string username, string id, string charName)
    {
        _username = username;
        _userID = id;
        _characterName = charName;

        myCard = GameplayManager.Instance.AddPlayer(this);
        myCard.myPlayer = this;
    }

    public void PlayerLeave()
    {
        GameplayManager.Instance.PlayerLeave(this);
    }

    private void OnDestroy()
    {
        //GameplayManager.Instance.PlayerLeave(this);
    }

    #region Character Control Methods

    [PunRPC]
    public void CharacterDie()
    {

    }


    public void MoveCharacter(float axisMovement)
    {
        if (myCharacter)
        {
            myCharacter.XMovement(axisMovement);
        }
    }

    public void JumpCharacter()
    {
        if (myCharacter)
        {
            myCharacter.Jump();
        }
    }

    public void FlipCharacter()
    {
        if (myCharacter)
        {
            myCharacter.Flip();
        }
    }

    public void AttackCharacter(Vector2 attackDir)
    {
        if (myCharacter)
        {
            myCharacter.PlayerAttack(attackDir);
        }
    }

    #endregion
}
