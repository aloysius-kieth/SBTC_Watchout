using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public ShadowSprite sprite;

    public void SetShadowOnGround()
    {
        GameObject shadow = ObjectPooler.Instance.GetPooledObject("Shadow");
        if (shadow != null)
        {
            shadow.transform.position = new Vector2(transform.position.x, shadow.transform.position.y);
            shadow.SetActive(true);
            sprite = shadow.GetComponent<ShadowSprite>();
        }
        else
        {
            Debug.LogWarning(shadow + " does not exist!");
            return;
        }
    }

    public void Deactivate()
    {
        sprite.gameObject.SetActive(false);
    }
}
