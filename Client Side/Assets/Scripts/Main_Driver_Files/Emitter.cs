using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour
{
    public Material material; // Directly reference the material
    public Color emissionColor;
    public float maxEmissionIntensity;
    public float minEmissionIntensity;
    public float cycleDuration = 5.0f;

    private float timeElapsed = 0.0f;
    public bool isEffectRunning = true;

    private Color originalEmissionColor;

    void Start()
    {
        if (material == null)
        {
            Debug.LogError("Material not assigned!");
            return;
        }
        originalEmissionColor = emissionColor;
    }

    void Update()
    {
        if (isEffectRunning)
        {
            Controller();
        }
    }

    void Controller()
    {
        timeElapsed += Time.deltaTime;
        float t = Mathf.PingPong(timeElapsed, cycleDuration) / cycleDuration;

        float modifiedMinEmissionIntensity = Mathf.Pow(minEmissionIntensity, 1.0f);
        float modifiedMaxEmissionIntensity = Mathf.Pow(maxEmissionIntensity, 1.5f);

        float currentEmissionIntensity = Mathf.Lerp(modifiedMinEmissionIntensity, modifiedMaxEmissionIntensity, t);

        material.SetColor("_EmissionColor", emissionColor * currentEmissionIntensity);
    }

    public void StartEffect()
    {
        isEffectRunning = true;
    }

    public void StopEffect()
    {
        isEffectRunning = false;

        material.SetColor("_EmissionColor", emissionColor * Mathf.Pow(2.0F, 3f));
    }

    public void ChangeEmissionColor(Color newEmissionColor)
    {
        emissionColor = newEmissionColor;
        timeElapsed += Time.deltaTime;
        float t = Mathf.PingPong(timeElapsed, cycleDuration) / cycleDuration;

        float currentEmissionIntensity = Mathf.Lerp(minEmissionIntensity, maxEmissionIntensity, t);
        material.SetColor("_EmissionColor", emissionColor * Mathf.Pow(2.0F, 3.5f));
    }

    public void ResetEmissionColor()
    {
        emissionColor = originalEmissionColor;
        material.SetColor("_EmissionColor", emissionColor * Mathf.Pow(1.5F, 3.5f));
    }

    public void HideEmission()
    {
        material.DisableKeyword("_EMISSION");
    }

    public void showEmission()
    {
        material.EnableKeyword("_EMISSION");
    }
}
