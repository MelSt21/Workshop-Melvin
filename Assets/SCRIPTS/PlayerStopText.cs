using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStopText : MonoBehaviour
{
    // Start is called before the first frame update
    public TopDownMovement player;

    public void SetMovement(TopDownMovement player)
    {
        if (player.controlEnabled)
        {
            player.controlEnabled = false;
        }
    }
}
