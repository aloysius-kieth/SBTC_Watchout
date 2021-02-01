using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

// Inputs relating to debugging, put them here!
public class TrinaxHandleInputs : MonoBehaviour
{
    private TrinaxAdminPanel aP;
    private bool isReady = false;
    //bool hideText = false;

    async void Start()
    {
        isReady = false;
        await new WaitUntil(() => TrinaxGlobal.Instance.isReady);

        isReady = true;

        aP = TrinaxManager.trinaxCanvas.adminPanel;
    }

    private void Update()
    {
        if (!isReady) return;

        if (aP != null && aP.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Debug.Log("Going back to scene 0");
                TrinaxHelperMethods.ChangeLevel((int)SCENE.MAIN, () =>
                {
                    TrinaxManager.trinaxCanvas.adminPanel.gameObject.SetActive(false);
                });
            }
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            //hideText = !hideText;
            //TrinaxCanvas.Instance.HideDebugText(hideText);
            Cursor.visible = !Cursor.visible;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            TrinaxGlobal.Instance.IsAppPaused = !TrinaxGlobal.Instance.IsAppPaused;
        }

        if (TrinaxGlobal.Instance.IsAppPaused)
        {
            Time.timeScale = 0;
        }
        else Time.timeScale = 1;
    }
}
