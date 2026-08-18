using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BlackHoleUIController : MonoBehaviour
{
    [Header("主控制面板")]
    public CanvasGroup controlPanelGroup;
    public Button eyeButton;

    [Header("颜色设置子面板 (新增)")]
    public CanvasGroup colorPanelGroup;
    public Button colorSettingButton;  

    [Header("动效与沉浸式设置")]
    public float fadeSpeed = 5f;
    public float idleTimeToHide = 2.0f;
    public float appearSpeedMultiplier = 4f;

    private bool _isPanelOpen = false;
    private bool _isColorPanelOpen = false;
    private float _eyeAlpha = 0f;
    private float _idleTimer = 0f;
    private Vector2 _lastMousePos;

    void Start()
    {
        controlPanelGroup.alpha = 0;
        controlPanelGroup.interactable = false;
        controlPanelGroup.blocksRaycasts = false;

        if (colorPanelGroup != null)
        {
            colorPanelGroup.alpha = 0;
            colorPanelGroup.interactable = false;
            colorPanelGroup.blocksRaycasts = false;
        }

        eyeButton.onClick.AddListener(ToggleMainPanel);

        if (colorSettingButton != null)
        {
            colorSettingButton.onClick.AddListener(ToggleColorPanel);
        }

        if (Mouse.current != null)
        {
            _lastMousePos = Mouse.current.position.ReadValue();
        }
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 currentMousePos = Mouse.current.position.ReadValue();

        if (Vector2.Distance(currentMousePos, _lastMousePos) > 1.0f)
        {
            _idleTimer = 0f;
            _lastMousePos = currentMousePos;
        }
        else
        {
            _idleTimer += Time.deltaTime;
        }

        if (_isPanelOpen)
        {
            _eyeAlpha = 1f;
            Cursor.visible = true;
        }
        else
        {
            if (_idleTimer > idleTimeToHide)
            {
                _eyeAlpha = 0f;
                Cursor.visible = false;
            }
            else
            {
                _eyeAlpha = 1f;
                Cursor.visible = true;
            }
        }

        UpdateVisuals();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    void UpdateVisuals()
    {
        var eyeColor = eyeButton.image.color;
        float currentSpeed = (_eyeAlpha > eyeColor.a) ? fadeSpeed * appearSpeedMultiplier : fadeSpeed;
        eyeColor.a = Mathf.Lerp(eyeColor.a, _eyeAlpha, Time.deltaTime * currentSpeed);
        eyeButton.image.color = eyeColor;

        float targetMainAlpha = _isPanelOpen ? 1f : 0f;
        controlPanelGroup.alpha = Mathf.Lerp(controlPanelGroup.alpha, targetMainAlpha, Time.deltaTime * fadeSpeed);

        if (colorPanelGroup != null)
        {
            float targetColorAlpha = _isColorPanelOpen ? 1f : 0f;
            colorPanelGroup.alpha = Mathf.Lerp(colorPanelGroup.alpha, targetColorAlpha, Time.deltaTime * fadeSpeed);
        }
    }

    void ToggleMainPanel()
    {
        _isPanelOpen = !_isPanelOpen;

        controlPanelGroup.interactable = _isPanelOpen;
        controlPanelGroup.blocksRaycasts = _isPanelOpen;

        if (!_isPanelOpen && _isColorPanelOpen)
        {
            _isColorPanelOpen = false;
            if (colorPanelGroup != null)
            {
                colorPanelGroup.interactable = false;
                colorPanelGroup.blocksRaycasts = false;
            }
        }

        _idleTimer = 0f;
    }

    void ToggleColorPanel()
    {
        _isColorPanelOpen = !_isColorPanelOpen;

        if (colorPanelGroup != null)
        {
            colorPanelGroup.interactable = _isColorPanelOpen;
            colorPanelGroup.blocksRaycasts = _isColorPanelOpen;
        }

        _idleTimer = 0f;
    }

    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}