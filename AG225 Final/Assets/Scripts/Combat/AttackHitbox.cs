using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public float AttackDamage = 7.5f;

    public SpriteRenderer mySprite;

    public Character me;

    [SerializeField]
    bool isActive = false;

    [SerializeField]
    private float attackDur = 0.15f;
    private float attackCD;

    private void Awake()
    {
        mySprite.enabled = false;
        attackCD = attackDur;
    }

    private void Update()
    {
        if (isActive)
        {
            attackCD -= Time.deltaTime;
            if(attackCD <= 0f)
            {
                Disable();
            }
        }
    }

    public void Activate()
    {
        mySprite.enabled = true;
        isActive = true;
        attackCD = attackDur;
    }

    private void Disable()
    {
        mySprite.enabled = false;
        isActive = false;
        me.canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
        {
            Character otherChar = collision.gameObject.GetComponent<Character>();

            if(otherChar && otherChar != me)
            {
                otherChar.DealDamage(AttackDamage, me);
            }
        }
    }
}
