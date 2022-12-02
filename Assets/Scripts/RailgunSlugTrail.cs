using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunSlugTrail : MonoBehaviour
{
    LineRenderer line;
    public float fadeOutSpeed;
    float alpha = 255.0f;
    public Gradient gradient;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        gradient = new Gradient();
    }

    // Update is called once per frame
    void Update()
    {
        alpha -= Time.deltaTime * fadeOutSpeed;
        float alphaNormalized = alpha / 255.0f;
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alphaNormalized, 0.0f), new GradientAlphaKey(alphaNormalized, 1.0f) }
            );
        line.colorGradient = gradient;
        /*Color c = new Color(0.0f, 255.0f, 0.0f, alpha);
        line.SetColors(c, c);*/

        if (alpha <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
