using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectCollider : MonoBehaviour
{
    public FallingObject obj;
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject != null && col.gameObject.tag == "Floor" && !obj.hitUmberlla && !obj.onGroundHit)
        {
            if (obj.type == FALLING_TYPES.Banana)
            {
                TrinaxManager.trinaxAudioManager.PlaySFX(TrinaxAudioManager.AUDIOS.BANANA, TrinaxAudioManager.AUDIOPLAYER.SFX4);
            }
            else if (obj.type == FALLING_TYPES.Bottle)
            {
                int rand = Random.Range((int)TrinaxAudioManager.AUDIOS.BOTTLE_1, (int)TrinaxAudioManager.AUDIOS.BOTTLE_4 + 1);
                TrinaxManager.trinaxAudioManager.PlaySFX((TrinaxAudioManager.AUDIOS)rand, TrinaxAudioManager.AUDIOPLAYER.SFX4);
            }
            else if (obj.type == FALLING_TYPES.Flowerpot)
            {
                TrinaxManager.trinaxAudioManager.PlaySFX(TrinaxAudioManager.AUDIOS.FLOWERPOT, TrinaxAudioManager.AUDIOPLAYER.SFX4);
            }
            else if (obj.type == FALLING_TYPES.Microwave)
            {
                TrinaxManager.trinaxAudioManager.PlaySFX(TrinaxAudioManager.AUDIOS.MICROWAVE, TrinaxAudioManager.AUDIOPLAYER.SFX4);
            }
            else if (obj.type == FALLING_TYPES.Newspaper)
            {
                TrinaxManager.trinaxAudioManager.PlaySFX(TrinaxAudioManager.AUDIOS.NEWSPAPER, TrinaxAudioManager.AUDIOPLAYER.SFX4);
            }
            else if (obj.type == FALLING_TYPES.Slipper)
            {
                TrinaxManager.trinaxAudioManager.PlaySFX(TrinaxAudioManager.AUDIOS.SLIPPER, TrinaxAudioManager.AUDIOPLAYER.SFX4);
            }
            else if (obj.type == FALLING_TYPES.Sock)
            {
                TrinaxManager.trinaxAudioManager.PlaySFX(TrinaxAudioManager.AUDIOS.SOCK, TrinaxAudioManager.AUDIOPLAYER.SFX4);
            }
            obj.OnGroundHit();
        }
        else if(col.gameObject.tag == "Floor" && obj.hitUmberlla)
        {
            obj.gameObject.SetActive(false);
        }

        //if (col.gameObject.tag == "Player")
        //    obj.OnHitPlayer();
    }

}
