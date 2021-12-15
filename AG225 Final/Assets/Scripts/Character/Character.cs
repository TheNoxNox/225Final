using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

[HelpURL("https://www.youtube.com/watch?v=w9NmPShzPpE")]
public class Character : MonoBehaviour
{
    [Header("Character Attributes")]
    [SerializeField]
    protected float _hitpointMax = 100f;
    [SerializeField]
    protected float _hitpointCurrent = 100f;
    [SerializeField]
    protected float _baseMovespeed = 100f;
    [SerializeField]
    protected float _baseJumpForce = 100f;
    [SerializeField]
    protected uint _baseJumpCount = 2;
    public string Name { get { return gameObject.name; } }
    public float BaseSpeed { get { return _baseMovespeed; } }
    public float BaseJumpForce { get { return _baseJumpForce; } }
    public float MaxJumpCount { get { return _baseJumpCount; } }
    public float HitpointsCurrent { get { return _hitpointCurrent; } }

    #region State Machine Info

    #endregion

    #region movement component references
    [Header("Character Components")]
    [SerializeField]
    protected Rigidbody2D myRB;
    public Rigidbody2D OwnRigidBody { get { return myRB; } }

    #endregion

    #region Movement Stored Variables
    private float xMovement;
    private uint currentJumps = 0;
    private bool facingRight = true;
    public bool IsTouchingGround { get; private set; }
    #endregion

    #region State Machine

    public CharacterStateMachine myStateMachine;

    #endregion

    [Header("Misc Fields")]
    [SerializeField]
    protected LayerMask platformMask;
    public BoxCollider2D groundCheckBox;
    private bool _isFlipped = false;
    public bool IsFlipped { get { return _isFlipped; } }
    public SpriteRenderer sprite;
    public TMP_Text nametag;

    #region attack hitboxes
    public bool canAttack = true;

    public AttackHitbox upAttack;
    public AttackHitbox leftAttack;
    public AttackHitbox rightAttack;
    public AttackHitbox downAttack;

    #endregion

    private void Awake()
    {
        if (!myRB)
        {
            myRB = gameObject.GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        }
        _hitpointCurrent = _hitpointMax;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        IsTouchingGround = IsGrounded();
    }

    private void FixedUpdate()
    {
        if (this.GetPhotonView().IsMine)
        {
            if (Mathf.Abs(xMovement) > 0.1f)
            {
                float airMod = 1f;
                if (!IsTouchingGround) { airMod = 0.75f; }
                float flipMod = 1f;
                if (IsFlipped) { flipMod = -1f; }

                myRB.AddForce(new Vector2((xMovement * BaseSpeed * airMod * flipMod) / 50, 0), ForceMode2D.Impulse);
                //Debug.Log(myRB.velocity.x);
            }
        }
        
        //else
        //{
        //    myRB.velocity = new Vector2(myRB.velocity.x * 0.85f, myRB.velocity.y);
        //}
    }

    public void DealDamage(float damage, Character source)
    {

    }

    #region Movement

    public void XMovement(float moveValue)
    {
        xMovement = moveValue;
        if(moveValue < -0.1f && facingRight) 
        {
            //transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
            this.GetPhotonView().RPC("SpriteFlip", RpcTarget.All, true);
            facingRight = false; 
        }
        else if(moveValue > 0.1f && !facingRight) 
        {
            //transform.localScale = new Vector3(transform.localScale.x * -1f,transform.localScale.y,transform.localScale.z); 
            this.GetPhotonView().RPC("SpriteFlip", RpcTarget.All, false);
            facingRight = true; 
        }
    }

    [PunRPC]
    public void SpriteFlip(bool flip)
    {
        sprite.flipX = flip;
    }

    public void PlayerAttack(Vector2 attackDir)
    {
        float xDir = attackDir.x;
        float yDir = attackDir.y;
        if (canAttack)
        {
            if (xDir <= -0.1f)
            {
                this.GetPhotonView().RPC("DoAttack", RpcTarget.AllViaServer, 0);
                canAttack = false;
            }
            else if(xDir >= 0.1f)
            {
                this.GetPhotonView().RPC("DoAttack", RpcTarget.AllViaServer, 1);
                canAttack = false;
            }
            else if(yDir >= 0.1f)
            {
                this.GetPhotonView().RPC("DoAttack", RpcTarget.AllViaServer, 2);
                canAttack = false;
            }
            else if(yDir <= 0.1f && !IsTouchingGround)
            {
                this.GetPhotonView().RPC("DoAttack", RpcTarget.AllViaServer, 3);
                canAttack = false;
            }
        }
       
    }

    [PunRPC]
    public void DoAttack(int direction)
    {
        switch (direction)
        {
            case 0:
                leftAttack.Activate();
                break;
            case 1:
                rightAttack.Activate();
                break;
            case 2:
                upAttack.Activate();
                break;
            case 3:
                downAttack.Activate();
                break;
        }
    }

    public void Jump()
    {
        if(currentJumps < MaxJumpCount)
        {
            myStateMachine.currentJumpState.Jump();
        }
    }

    public void DoJump()
    {
        myRB.velocity = new Vector2(myRB.velocity.x, myRB.velocity.y * 0.05f);
        myRB.AddForce(new Vector2(0, BaseJumpForce * transform.up.y), ForceMode2D.Impulse);
        currentJumps++;
    }

    public void ResetJumps()
    {
        currentJumps = 0;
    }

    //https://www.youtube.com/watch?v=c3iEl5AwUF8
    private bool IsGrounded()
    {
        RaycastHit2D groundCastHit = Physics2D.BoxCast(groundCheckBox.bounds.center, groundCheckBox.bounds.size, 0f, Vector2.down, 1f, platformMask);
        return groundCastHit.collider != null;
    }

    #endregion

    [PunRPC]
    public void SetNametag(string name)
    {
        if (this.GetPhotonView().IsMine)
        {
            nametag.text = "YOU";
            nametag.color = Color.red;
        }
        else
        {
            nametag.text = name;
        }
    }

    #region Flip logic

    public void Flip()
    {
        if (this.GetPhotonView().IsMine)
        {
            GameplayManager.Instance.FlipCamera(!IsFlipped);
            this.GetPhotonView().RPC("FlipRPC", RpcTarget.AllBufferedViaServer);
        }
    }

    [PunRPC]
    public void FlipRPC()
    {
        myRB.gravityScale *= -1f;
        if (!IsFlipped)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            _isFlipped = true;
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            _isFlipped = false;
        }
    }

    #endregion


    #region RPC State Machine Sync Methods

    /// <summary>
    /// RPC for setting jump state across the network.
    /// </summary>
    /// <param name="stateName">This should be EXACTLY the name, no capitals, of the state following "Jump_" (Ex. "grounded")</param>
    [PunRPC]
    public void SetJumpState(string stateName)
    {
        switch (stateName)
        {
            case "liftoff":
                myStateMachine.ChangeJumpState(myStateMachine.Jump_Liftoff);
                break;
            case "grounded":
                myStateMachine.ChangeJumpState(myStateMachine.Jump_Grounded);
                break;
            case "landing":
                myStateMachine.ChangeJumpState(myStateMachine.Jump_Landing);
                break;
            case "jumping":
                myStateMachine.ChangeJumpState(myStateMachine.Jump_Jumping);
                break;
            case "falling":
                myStateMachine.ChangeJumpState(myStateMachine.Jump_Falling);
                break;
        }
    }

    #endregion
}
