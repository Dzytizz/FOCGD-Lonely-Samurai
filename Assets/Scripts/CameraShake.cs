using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Vector3 OriginalPos;
    public AnimationCurve curve;
    public void Start()
    {
        OriginalPos = transform.localPosition;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 pos = transform.localPosition;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float strength = 1.0f;
            if (curve != null)
            {
                strength = curve.Evaluate(elapsed / duration);
            }
            float x = Random.Range(-1f, 1f) * magnitude * strength;
            float y = Random.Range(-1f, 1f) * magnitude * strength;

            pos += new Vector3(x, y, 0);
            transform.position = pos;
            
            yield return null;
        }
  
        transform.position = this.OriginalPos;
    }
}
