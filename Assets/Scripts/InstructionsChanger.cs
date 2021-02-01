using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public enum INSTRUCTIONS_PAGES
{
    PAGE1,
    PAGE2,
    PAGE3,
}

public class InstructionsChanger : MonoBehaviour
{
    public INSTRUCTIONS_PAGES state = INSTRUCTIONS_PAGES.PAGE1;

    public GameObject[] pages;

    int current = 0;

    public Button nextInstructionsBtn;
    public Button backInstructionsBtn;

    public Sprite startBtnImage;
    public Sprite nextBtnImage;

    private void Start()
    {
        ChangePage(current);

        nextInstructionsBtn.transform.localScale = Vector3.zero;
        backInstructionsBtn.transform.localScale = Vector3.zero;

        nextInstructionsBtn.onClick.AddListener(() => { ChangePage(1); });
        backInstructionsBtn.onClick.AddListener(() => { ChangePage(-1); });
    }

    IEnumerator Scale()
    {
        while(true)
        {
            nextInstructionsBtn.transform.DOScale(Vector3.one * 1.2f, 0.3f);
            yield return new WaitForSeconds(0.3f);
            nextInstructionsBtn.transform.DOScale(Vector3.one, 0.3f);
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void ChangePage(int dir)
    {
        current = current + dir;
        if (current == 2)
        {
            nextInstructionsBtn.transform.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
            {
                nextInstructionsBtn.image.sprite = startBtnImage;
                nextInstructionsBtn.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f).OnComplete(() =>
                {
                    nextInstructionsBtn.transform.DOScale(Vector3.one, 0.1f).OnComplete(() =>
                    {
                        StartCoroutine("Scale");
                    });
                });
            });
        }
        else if(dir == -1 && current == 1)
        {
            StopCoroutine("Scale");
            //nextInstructionsBtn.GetComponent<Animator>().SetTrigger("Idle");
            nextInstructionsBtn.transform.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
            {
                nextInstructionsBtn.image.sprite = nextBtnImage;
                nextInstructionsBtn.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f).OnComplete(() =>
                {
                    nextInstructionsBtn.transform.DOScale(Vector3.one, 0.1f);
                });
            });
        }

        if(current >= pages.Length)
        {
            AppManager.uiManager.ToGame();
        }
        else if (current < 0)
        {
            AppManager.uiManager.ToScreensaver();
        }
        else
        {
            for (int i = 0; i < pages.Length; i++)
            {
                if (i == current)
                {
                    pages[i].SetActive(true);
                }
                else
                {
                    pages[i].SetActive(false);
                }
            }
        }
    }

    private void OnDisable()
    {
        StopCoroutine("Scale");
        nextInstructionsBtn.transform.localScale = Vector3.zero;
        backInstructionsBtn.transform.localScale = Vector3.zero;
        nextInstructionsBtn.image.sprite = nextBtnImage;
        current = 0;
        ChangePage(current);
    }
}
