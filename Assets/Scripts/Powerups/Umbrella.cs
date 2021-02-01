
using UnityEngine;

public class Umbrella : Powerup
{
    public override void Start()
    {
        base.Start();
    }

    public override void OnPickedUp()
    {
        base.OnPickedUp();
        if (!AppManager.gameManager.player.isUmbrellaOpened || !AppManager.gameManager.player.isDead)
        {
            TrinaxManager.trinaxAudioManager.PlaySFX(TrinaxAudioManager.AUDIOS.UMBRELLA_PICKUP, TrinaxAudioManager.AUDIOPLAYER.SFX3);
            //Debug.Log("player opening umbrella!");
            AppManager.gameManager.player.OpenUmbrella();
            gameObject.SetActive(false);
        }
    }

}
