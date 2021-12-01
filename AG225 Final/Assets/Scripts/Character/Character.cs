using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HelpURL("https://www.youtube.com/watch?v=w9NmPShzPpE")]
public class Character : MonoBehaviour
{
    [Header("Character Attributes")]
    [SerializeField]
    protected float _baseMovespeed = 4f;
    [SerializeField]
    protected float _baseJumpForce = 100f;
    [SerializeField]
    protected uint _baseJumpCount = 2;
    public float BaseSpeed { get { return _baseMovespeed; } }


    #region movement component references
    [Header("Character Components")]
    [SerializeField]
    protected Rigidbody2D myRB;

    #endregion

    #region Movement Stored Variables
    private float xMovement;
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
        if(xMovement > 0.1f)
        {

        }
    }

    #region Movement

    public void XMovement(float moveValue)
    {
        xMovement = moveValue;
    }

    #endregion
}
