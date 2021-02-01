
using UnityEngine;

public static class ObjectPropertyManager
{
    public static void PopulateObjectProperties()
    {
        Debug.Log("Populating default values to falling objects...");
        // In future change this to objects added to a global list instead of objectpooler
        if (ObjectPooler.Instance.pooledObjects.Count > 0)
        {
            for (int i = 0; i < ObjectPooler.Instance.pooledObjects.Count; i++)
            {
                FallingObject obj = ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>();
                if (obj != null)
                {
                    if (TrinaxGlobal.Instance.scene == SCENE.MAIN)
                        GetFallingObjectProperties(obj);
                    else
                        GetTrainingFallingObjectProperties(obj);
                }
            }
        }
        else return;
    }

    public static void GetFallingObjectProperties(FallingObject obj)
    {
        for (int i = 0; i < TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj.Count; i++)
        {
            if (obj.type.ToString() == TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj[i].name)
            {
                obj.timeScale = TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj[i].objectProperties.dropSpeed;
                obj.damage = TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj[i].objectProperties.damage;
            }
        }
    }


    public static void GetTrainingFallingObjectProperties(FallingObject obj)
    {
        for (int i = 0; i < TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj.Count; i++)
        {
            if (obj.type.ToString() == TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj[i].name)
            {
                for (int j = 0; j < SliderManager.Instance.trainingGameSliders.Count; j++)
                {
                    if (SliderManager.Instance.trainingGameSliders[j].slider_type == SLIDER_TYPE.DROP_SPEED)
                    {
                        obj.timeScale = SliderManager.Instance.trainingGameSliders[j].val;
                    }
                    else if(SliderManager.Instance.trainingGameSliders[j].slider_type == SLIDER_TYPE.DAMAGE)
                    {
                        obj.damage = SliderManager.Instance.trainingGameSliders[j].val;
                    }
                }
            }
        }
    }
}
