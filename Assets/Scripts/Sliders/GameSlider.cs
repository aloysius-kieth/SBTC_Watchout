using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSlider : AdminSlider
{
    public SLIDER_TYPE slider_type;
    public FALLING_TYPES type;

    public override void Init()
    {
        base.Init();

        // Refractor this 
        //if (slider_behaviour == SLIDER_BEHAVIOUR.TRAINING)
        //{
            sliderValText.text = slider.value.ToString();
            //UpdateObjectsValues();
        //}
    }

    public override void OnValueChanged(Slider slider)
    {
        base.OnValueChanged(slider);

        UpdateObjectsValues();
    }

    void UpdateObjectsValues()
    {
        // Affects game play properties
        if (ObjectPooler.Instance.pooledObjects.Count > 0)
        {
            for (int i = 0; i < ObjectPooler.Instance.pooledObjects.Count; i++)
            {
                if (ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>() != null)
                {
                    if (type == ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>().type)
                    {
                        if (slider_type == SLIDER_TYPE.DROP_SPEED)
                        {
                            ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>().timeScale = val;
                            //Debug.Log(val);
                        }
                        else if (slider_type == SLIDER_TYPE.DAMAGE)
                        {
                            ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>().damage = val;
                        }
                    }
                }
            }
        }
    }

}
