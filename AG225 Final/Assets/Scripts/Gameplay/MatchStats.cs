using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MatchStats : MonoBehaviour
{
    [SerializeField]
    protected float GameTimeMax = 60f;

    private float currentGameTime = 0f;

    public float GameTime { get { return currentGameTime; } }

    private void Awake()
    {
        currentGameTime = GameTimeMax;
    }

    // Update is called once per frame
    void Update()
    {
        currentGameTime -= Time.deltaTime;
    }
}
