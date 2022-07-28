using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    private BasePrefs _basePrefs;
    private List<GameObject> _buffEffects = new List<GameObject>();

    private void Start()
    {
        _basePrefs = BasePrefs.instance;
    }
    public void ActivateEffect(BuffClass buff)
    {
        if (buff.EffectOnUse)
        {
            GameObject obj = Instantiate(buff.EffectOnUse, transform.position, Quaternion.identity);
            obj.transform.SetParent(this.transform);
            StartCoroutine(DestroyObjWithDelay(obj, buff.EffectLifetime));
        }
        if (buff.EffectOnUsing)
        {
            GameObject obj = Instantiate(buff.EffectOnUsing, transform.position, Quaternion.identity);
            obj.transform.SetParent(this.transform);
            StartCoroutine(DestroyObjWithDelay(obj, buff.Time));
        }
    }
    public void DiactivateEffects()
    {
        foreach (GameObject buffObj in _buffEffects)
        {
            Destroy(buffObj);
        }
    }
    private IEnumerator DestroyObjWithDelay(GameObject obj, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        {
            Destroy(obj);
        }
    }

}