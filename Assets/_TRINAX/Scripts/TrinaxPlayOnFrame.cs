using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

/// <summary>
/// Used for executing stuff on animation frames
/// </summary>
public class TrinaxPlayOnFrame : MonoBehaviour
{
    public GameObject backInstuctionsBtn;
    public GameObject nextInstructionBtn;
    //public Animator instructionsNextBtnAnim;

    public void PlayInstructionsBtnAnim()
    {
        nextInstructionBtn.transform.DOScale(Vector3.zero, 0.0f).OnComplete(() => {
            nextInstructionBtn.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f).OnComplete(() => { nextInstructionBtn.transform.DOScale(Vector3.one, 0.1f); });
        });

        backInstuctionsBtn.transform.DOScale(Vector3.zero, 0.0f).OnComplete(() => {
            backInstuctionsBtn.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f).OnComplete(() => { backInstuctionsBtn.transform.DOScale(Vector3.one, 0.1f); });
        });
    }

    //public void SetInstructionsBtnGrpOn()
    //{
    //    instructionBtnGrp.SetActive(true);
    //}

    //public void SetInstructionsBtnGrpOff()
    //{
    //    instructionBtnGrp.SetActive(false);
    //}

    //public void ChangeInstructionsBtnImage(string animName)
    //{
    //    instructionsNextBtnAnim.SetTrigger(animName);
    //}
}
