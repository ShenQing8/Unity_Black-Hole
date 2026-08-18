using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderInputSync : MonoBehaviour
{
    [Header("UI 组件")]
    public Slider targetSlider;
    public TMP_InputField targetInputField;

    void Start()
    {
        if (targetSlider == null || targetInputField == null)
        {
            Debug.LogWarning("滑条或输入框未赋值！", this);
            return;
        }

        UpdateInputFromSlider(targetSlider.value);

        targetSlider.onValueChanged.AddListener(UpdateInputFromSlider);

        targetInputField.onEndEdit.AddListener(UpdateSliderFromInput);
    }

    private void UpdateInputFromSlider(float value)
    {
        if (targetSlider.wholeNumbers)
        {
            targetInputField.text = value.ToString("F0");
        }
        else
        {
            targetInputField.text = value.ToString("F2");
        }
    }

    private void UpdateSliderFromInput(string textValue)
    {
        if (float.TryParse(textValue, out float result))
        {
            result = Mathf.Clamp(result, targetSlider.minValue, targetSlider.maxValue);

            targetSlider.value = result;

            UpdateInputFromSlider(targetSlider.value);
        }
        else
        {
            UpdateInputFromSlider(targetSlider.value);
        }
    }
}