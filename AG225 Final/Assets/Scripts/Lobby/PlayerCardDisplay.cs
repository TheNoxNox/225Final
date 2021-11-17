using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCardDisplay : MonoBehaviour
{
    public bool IsEmpty { get; private set; }

    public string Username { get; set; }

    public string CharacterID { get; set; }

    public Image characterImg;
    public TMP_Text characterName;

    public TMP_Text usernameField;

    private void Awake()
    {
        Username = "Waiting For Player...";
        CharacterID = "000";
    }

    private void Update()
    {
        
    }
}
