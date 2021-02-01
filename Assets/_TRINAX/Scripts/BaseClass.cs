//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System.Threading.Tasks;

//[RequireComponent(typeof(CanvasController))]
//public abstract class BaseClass : MonoBehaviour
//{
//    // Component References
//    protected CanvasController canvasController;

//    public virtual async Task Start()
//    {
//        //await TrinaxGlobal.Instance.Init();

//        await new WaitUntil(() => TrinaxGlobal.Instance.isReady);

//        // Cache any component references
//        canvasController = GetComponent<CanvasController>();
//    }

//    public virtual void Init()
//    {
//        RefreshSettings(TrinaxGlobal.Instance.globalSettings, TrinaxGlobal.Instance.gameSettings, TrinaxGlobal.Instance.kinectSettings);
//    }

//    public virtual void RefreshSettings(GlobalSettings settings, GameSettings _gameSettings, KinectSettings _kinectSettings)
//    {
//        TrinaxGlobal.Instance.RefreshSettings(settings, _gameSettings, _kinectSettings);

//        Debug.Log("Settings refreshed!");
//    }
       
//}
