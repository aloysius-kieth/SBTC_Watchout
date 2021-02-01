using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CanvasController : MonoBehaviour {

    public CanvasGroup[] pageList;

    void Awake () {
        if (pageList.Length == 0) return;
        for (int i = 1; i < pageList.Length; ++i) {
            pageList[i].alpha = 0;
            pageList[i].gameObject.SetActive(false);
        }
    }

    public void OnStartup(int tobeActive, float duration, Action callback = null)
    {
        for (int i = 0; i < pageList.Length; i++)
        {
            if (tobeActive == i)
            {
                pageList[tobeActive].gameObject.SetActive(true);
                pageList[tobeActive].DOFade(1.0f, duration).OnComplete(()=> { callback?.Invoke(); });
            }
        }
    }

    public int getCanvasIndex() {
        int index = 0;
        for (; index < pageList.Length; ++index) {
            if (pageList[index].alpha > 0.5) {
                break;
            }
        }
        return index;
    }

    public void TransitToCanvas(int tobeActive, float duration, System.Action callback = null) {
        int oldCanvas = getCanvasIndex();
        if (oldCanvas == tobeActive) {
            Debug.LogWarning("Fading to same Canvas");
            return;
        }
        Button[] canvasButtons = pageList[tobeActive].GetComponentsInChildren<Button>();
        if (canvasButtons != null)
        {
            foreach (Button obj in canvasButtons)
            {
                //Debug.Log("button ");
                obj.interactable = false;
            }
        }
        else { Debug.Log("No buttons found in canvas!"); }

        Button[] oldCanvasButtons = pageList[oldCanvas].GetComponentsInChildren<Button>();
        if (oldCanvasButtons != null)
        {
            foreach (Button obj in oldCanvasButtons)
            {
                //Debug.Log("button ");
                obj.interactable = false;
            }
        }
        else { Debug.Log("No buttons found in oldCanvas!"); }

        pageList[oldCanvas].DOFade(0, duration).OnComplete(() => {
            pageList[oldCanvas].gameObject.SetActive(false);
            /*
            if (oldCanvas == gameStageIndex) {
                isGameMode = false;
                pageList[1].alpha = 0;
                pageList[1].gameObject.SetActive(false);
                //VuforiaBehaviour.Instance.enabled = true;
            }
            */
            pageList[tobeActive].gameObject.SetActive(true);


            pageList[tobeActive].DOFade(1, duration).OnComplete(() => {

                foreach (Button obj in canvasButtons)
                {
                    obj.interactable = true;
                }

                callback?.Invoke();
            });
        });

    }
}
