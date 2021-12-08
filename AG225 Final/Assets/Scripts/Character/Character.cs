using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[HelpURL("https://www.youtube.com/watch?v=w9NmPShzPpE")]
public class Character : MonoBehaviour
{
    [Header("Character Attributes")]
    [SerializeField]
    protected string _name = "TestCharacter";
    [SerializeField]
    protected float _baseMovespeed = 100f;
    //protected float _maxMoveSpeed =
    [SerializeField]
    protected float _baseJumpForce = 100f;
    [SerializeField]
    protected uint _baseJumpCount = 2;
    public string Name { get { return _name; } }
    public float BaseSpeed { get { return _baseMovespeed; } }
    public float BaseJumpForce { get { return _baseJumpForce; } }
    public float MaxJumpCount { get { return _baseJumpCount; } }

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

    private void Awake()
    {
        if (!myRB)
        {
            myRB = gameObject.GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        }
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
        if(Mathf.Abs(xMovement) > 0.1f)
        {
            float airMod = 1f;
            if (!IsTouchingGround) { airMod = 0.75f; }

            myRB.AddForce(new Vector2((xMovement * BaseSpeed * airMod) / 50, 0),ForceMode2D.Impulse);
            //Debug.Log(myRB.velocity.x);
        }
        //else
        //{
        //    myRB.velocity = new Vector2(myRB.velocity.x * 0.85f, myRB.velocity.y);
        //}
    }

    #region Movement

    public void XMovement(float moveValue)
    {
        xMovement = moveValue;
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


    #region Flip logic

    public void Flip()
    {
        if (this.GetPhotonView().IsMine)
        {
            this.GetPhotonView().RPC("FlipRPC", RpcTarget.AllBufferedViaServer);
        }
    }

    [PunRPC]
    public void FlipRPC()
    {
        myRB.gravityScale *= -1f;
        if (!IsFlipped)
        {
            transform.Rotate(new Vector3(0, 0, 180));
            _isFlipped = true;
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, -180));
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
