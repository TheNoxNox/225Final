using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float JumpCount { get { return _baseJumpCount; } }

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
    #endregion

    #region State Machine

    public CharacterStateMachine myStateMachine;

    #endregion

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
        
    }

    private void FixedUpdate()
    {
        if(Mathf.Abs(xMovement) > 0.1f)
        {
            myRB.AddForce(new Vector2((xMovement * BaseSpeed) / 50, 0),ForceMode2D.Impulse);
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
        myRB.AddForce(new Vector2(0, BaseJumpForce), ForceMode2D.Impulse);
    }

    #endregion
}
