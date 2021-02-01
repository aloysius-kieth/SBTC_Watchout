
public class FirstAid : Powerup
{
    public override void Start()
    {
        base.Start();
    }

    public override void OnPickedUp()
    {
        base.OnPickedUp();
        if (!AppManager.gameManager.player.isDead)
        {
            AppManager.gameManager.player.PlayerRecoverHP(TrinaxGlobal.Instance.gameSettings.firstAidRecover);
            gameObject.SetActive(false);
        }
    }
}
