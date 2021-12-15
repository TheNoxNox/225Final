using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

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

    public float PlayerHealth { get; private set; }

    public GameplayCard myCard;

    #endregion

    #region spawning & respawning

    private GameObject spawnPoint;

    [SerializeField]
    protected bool isPlayerDead = false;

    [SerializeField]
    protected float SpawnCooldown = 4.5f;
    protected float _spawnCDremaining = 0f;

    #endregion

    public Slider flipCDSlider;

    private bool flipOnCooldown = false;

    [SerializeField]
    private float flipCooldownMax = 6f;
    private float flipTimer = 0;

    public bool playerOut = false;

    private bool gameDone => GameplayManager.Instance.GameDone;

    private void Awake()
    {
        if (isViewMine)
        {
            this.GetPhotonView().RPC("PlayerJoin", RpcTarget.AllBufferedViaServer, 
                PlayerNetworkManager.Instance.Username, PlayerNetworkManager.Instance.UniqueID,GameInstance.Instance.characterName);
            flipTimer = flipCooldownMax;
            flipCDSlider = GameplayManager.Instance.CDslider;
        }
        
    }

    private void Update()
    {
        if (isPlayerDead && !playerOut)
        {
            _spawnCDremaining -= Time.deltaTime;
            if(_spawnCDremaining <= 0f)
            {
                SpawnCharacter();
            }
        }

        if (flipOnCooldown)
        {
            flipTimer += Time.deltaTime;
            if(flipTimer >= flipCooldownMax)
            {
                flipTimer = flipCooldownMax;
                flipOnCooldown = false;
            }
        }

        flipCDSlider.value = ((flipTimer / flipCooldownMax) * 100);
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
            myCharacter = PhotonNetwork.Instantiate(CharacterName, spawnPoint.transform.position, Quaternion.identity)?.GetComponent<Character>();
            myCharacter.GetPhotonView().RPC("SetNametag", RpcTarget.AllBufferedViaServer, Username);
            this.GetPhotonView().RPC("UpdatePlayerHealthRPC", RpcTarget.AllBuffered, myCharacter.HitpointsCurrent);
            MyCharacter.PlayerDamageTaken += UpdatePlayerCard;
            MyCharacter.PlayerDeath += PlayerOnDeath;
            myCharacter.PlayerGetKill += GetKill;
        }
        isPlayerDead = false;
    }

    [PunRPC]
    public void SetPlayerNumber(int pNumber)
    {
        playerNum = pNumber + 1;
        spawnPoint = GameplayManager.Instance.spawnPts[pNumber];
    }

    public void UpdatePlayerCard(float playerHealth)
    {
        this.GetPhotonView().RPC("UpdatePlayerHealthRPC", RpcTarget.AllBuffered, playerHealth);
    }

    public void PlayerOnDeath()
    {
        if(GameInstance.Instance._gamemode == Gamemode.Stock)
        {
            _stock -= 1;
            this.GetPhotonView().RPC("UpdatePlayerScoreOrStockRPC", RpcTarget.AllBuffered, Stock);
        }
        else
        {           
            myCharacter?.lastHitCharacter?.GetPhotonView().RPC("GetKillRPC", RpcTarget.AllBuffered);
        }
        PhotonNetwork.Destroy(myCharacter.gameObject);
        isPlayerDead = true;
        _spawnCDremaining = SpawnCooldown;

        if(Stock <= 0)
        {
            playerOut = true;
            this.GetPhotonView().RPC("PlayerOut", RpcTarget.AllBufferedViaServer);
        }
    }

    [PunRPC]
    public void PlayerOut()
    {
        playerOut = true;
    }

    public void GetKill()
    {
        _score += 1;
        this.GetPhotonView().RPC("UpdatePlayerScoreOrStockRPC", RpcTarget.AllBuffered, Score);
    }

    [PunRPC]
    public void UpdatePlayerHealthRPC(float playerHealth)
    {
        PlayerHealth = playerHealth;
    }

    [PunRPC]
    public void UpdatePlayerScoreOrStockRPC(int scoreOrStock)
    {
        if (GameInstance.Instance._gamemode == Gamemode.Stock)
        {
            _stock = scoreOrStock;
        }
        else
        {
            _score = scoreOrStock;
        }
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
        _stock = 0;
        _score = 0;
        this.GetPhotonView().RPC("PlayerOut", RpcTarget.AllBufferedViaServer);

        this.GetPhotonView().RPC("UpdatePlayerScoreOrStockRPC", RpcTarget.AllBuffered, Stock);

        GameplayManager.Instance.PlayerLeave(this);
    }

    [PunRPC]
    public void GameEndDisplay(string winnerName, int winnerScoreStock, bool isTie)
    {
        GameplayManager.Instance.winScreen.SetActive(true);
        if (isTie)
        {
            GameplayManager.Instance.winnerName.text = "NONE: TIE";
            GameplayManager.Instance.winnerScoreStock.text = "";
        }
        else
        {
            GameplayManager.Instance.winnerName.text = winnerName;
            if(GameInstance.Instance._gamemode == Gamemode.Stock) { GameplayManager.Instance.winnerScoreStock.text = "Stock: " + winnerScoreStock; }
            else { GameplayManager.Instance.winnerScoreStock.text = "Score: " + winnerScoreStock; }   
        }
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
        if (myCharacter && !gameDone)
        {
            myCharacter.XMovement(axisMovement);
        }
    }

    public void JumpCharacter()
    {
        if (myCharacter && !gameDone)
        {
            myCharacter.Jump();
        }
    }

    public void FlipCharacter()
    {
        if (myCharacter && !flipOnCooldown && !gameDone)
        {
            flipOnCooldown = true;
            flipTimer = 0;
            myCharacter.Flip();
        }
    }

    public void AttackCharacter(Vector2 attackDir)
    {
        if (myCharacter && !gameDone)
        {
            myCharacter.PlayerAttack(attackDir);
        }
    }

    #endregion
}
