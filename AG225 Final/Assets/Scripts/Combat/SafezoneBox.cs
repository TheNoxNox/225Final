using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafezoneBox : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        Character deadChar = collision?.gameObject.GetComponent<Character>();

        deadChar?.DealDamage(deadChar.HitpointsCurrent);
    }
}
