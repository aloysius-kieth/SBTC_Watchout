using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : FallingObject
{
    public bool applyStun = true;
    [Header("Stun value (e.g divide by 2 is half speed")]
    public float stunVal = 2;
    public override void ApplyStatusEffect(Player player)
    {
        base.ApplyStatusEffect(player);
        if(applyStun)
        {
            if (player.isStunned)
                player.ApplyStun(stunVal);
        }

    }
}
