using UnityEngine;
using TMPro;

public class TrinaxCanvas : MonoBehaviour
{
    public TrinaxAdminPanel adminPanel;
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI kinectUserText;
    [HideInInspector]
    public Reporter reporter;

    private void Awake()
    {
        if (TrinaxManager.trinaxCanvas == null || TrinaxManager.trinaxCanvas != this)
        {
            TrinaxManager.trinaxCanvas = this;
        }
    }

    private void Start()
    {
#if !UNITY_EDITOR
        HideDebugText(true);
#endif
        reporter = FindObjectOfType<Reporter>();
    }

    private void Update()
    {
        ToggleAdminPanel(adminPanel);
        fpsText.text = "FPS: " + reporter.fps.ToString("F1");
    }

    public void HideDebugText(bool _hide)
    {
        if (_hide)
        {
            fpsText.color = Color.clear;
            kinectUserText.color = Color.clear;
        }
        else
        {
            fpsText.color = Color.black;
            kinectUserText.color = Color.black;
        }
    }

    void ToggleAdminPanel(TrinaxAdminPanel _aP)
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            StartCoroutine(_aP.DoSuccessResult(true));
            _aP.gameObject.SetActive(!_aP.gameObject.activeSelf);
            HideDebugText(!_aP.gameObject.activeSelf);
            if (reporter == null) return;
            else
            {
                if (reporter.show)
                {
                    reporter.show = !reporter.show;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F10) && _aP.gameObject.activeSelf)
        {
            if (reporter == null) return;
            reporter.show = !reporter.show;
            if (reporter.show)
            {
                reporter.doShow();
            }
        }

    }

}
