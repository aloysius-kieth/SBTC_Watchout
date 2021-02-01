using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class RedVignetteOverlay : MonoBehaviour
{
    public AnimationCurve curve;
    private PostProcessingBehaviour redVignette;

    void Awake()
    {
        redVignette = GetComponent<PostProcessingBehaviour>();
    }

    private void OnDisable()
    {
        redVignette.enabled = false;
    }

    private void OnEnable()
    {
        redVignette.enabled = true;
    }

    void Update()
    {
        VignetteModel.Settings vignette = redVignette.profile.vignette.settings;
        float val = curve.Evaluate(Time.time);
        vignette.intensity = val;

        redVignette.profile.vignette.settings = vignette;
    }
}
