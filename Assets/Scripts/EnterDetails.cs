using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EnterDetails : MonoBehaviour
{
    [Header("Inputfields")]
    public TMP_InputField nameIF;
    //public TMP_InputField emailIF;
    //public TMP_InputField contactIF;

    [Header("Buttons")]
    public Button submitDetailsBtn;

    private void Start()
    {
        submitDetailsBtn.onClick.AddListener(() => { OnSubmitDetails(); });
    }

    private void OnEnable()
    {
        nameIF.text = "";
        //emailIF.text = "";
        //contactIF.text = "";
    }

    void OnSubmitDetails()
    {
        bool validInput = true;
        submitDetailsBtn.interactable = false;
        if (!CredentialsValidator.validateName(nameIF.text))
        {
            validInput = false;
            submitDetailsBtn.interactable = false;

            nameIF.image.color = Color.red;
            nameIF.transform.DOShakePosition(0.5f, new Vector3(50, 0, 0), 5, 90, true, true).OnComplete(() =>
            {
                nameIF.image.color = Color.white;
                submitDetailsBtn.interactable = true;
            });
        }

        //if (!CredentialsValidator.validateEmail(emailIF.text))
        //{
        //    validInput = false;
        //    submitDetailsBtn.interactable = false;

        //    emailIF.image.color = Color.red;
        //    emailIF.transform.DOShakePosition(0.5f, new Vector3(50, 0, 0), 5, 90, true, true).OnComplete(() =>
        //    {
        //        emailIF.image.color = Color.white;
        //        submitDetailsBtn.interactable = true;
        //    });
        //}

        //if (!CredentialsValidator.validateMobile(contactIF.text))
        //{
        //    validInput = false;
        //    submitDetailsBtn.interactable = false;

        //    contactIF.image.color = Color.red;
        //    contactIF.transform.DOShakePosition(0.5f, new Vector3(50, 0, 0), 5, 90, true, true).OnComplete(() =>
        //    {
        //        contactIF.image.color = Color.white;
        //        submitDetailsBtn.interactable = true;
        //    });
        //}

        if (validInput)
        {
            TrinaxManager.trinaxAudioManager.PlayUISFX(TrinaxAudioManager.AUDIOS.VALID, TrinaxAudioManager.AUDIOPLAYER.UI_SFX);

            //GameManager.Instance.userData.name = nameIF.text;
            //GameManager.Instance.userData.mobileno = "";
            //GameManager.Instance.userData.email = "";
            //GameManager.Instance.userData.score = ScoreManager.Instance.Score.ToString();

            //await APICalls.RunAddGameResult();
            PlayerInfo pInfo = new PlayerInfo()
            {
                name = nameIF.text,
                score = ScoreManager.Instance.Score,
            };

            AppManager.leaderboardManager.Save(pInfo);

            AppManager.uiManager.ToScreensaver();
            AppManager.gameManager.screensaver.state = SCREENSAVER_STATE.LEADER;
            //APICalls.RunEndInteraction().WrapErrors();
        }
        else
        {
            TrinaxManager.trinaxAudioManager.PlayUISFX(TrinaxAudioManager.AUDIOS.INVALID, TrinaxAudioManager.AUDIOPLAYER.UI_SFX);
        }
    }

    [ContextMenu("AUTOFILLFORM")]
    void AUTOFILLFORM()
    {
        string[] randomNames = { "Tom", "Dick", "Harry" };
        int index = Random.Range(0, randomNames.Length);
        string name = randomNames[index];

        nameIF.text = name;
    }

}
