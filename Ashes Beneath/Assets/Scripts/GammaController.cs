using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class GammaController : MonoBehaviour
{
    public Slider gammaSlider;

    public void SetGamma(float value)
    {
        RenderSettings.ambientIntensity = value;
        Debug.Log("Gamma set to: " + value);
    }
    void Start()
    {
        // Load saved gamma value or default to 0.1
        float gamma = PlayerPrefs.GetFloat("GammaValue", 0.1f);
        gammaSlider.value = gamma;
        RenderSettings.ambientIntensity = gamma;

        // Add listener to slider
        gammaSlider.onValueChanged.AddListener(UpdateGamma);
    }

    public void UpdateGamma(float value)
    {
        RenderSettings.ambientIntensity = value;

        // Save setting
        PlayerPrefs.SetFloat("GammaValue", value);
    }
}
