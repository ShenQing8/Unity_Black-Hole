using UnityEngine;
using UnityEngine.UI;

public class BGMController : MonoBehaviour
{
    [Header("音乐文件")]
    public AudioClip bgmClip; // 现在只需要一个音乐文件

    [Header("UI 控制")]
    public Slider volumeSlider;

    private AudioSource _bgmSource; // 只需要一个音频播放器

    void Start()
    {
        // 动态创建并配置 AudioSource
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.clip = bgmClip;
        _bgmSource.loop = true; // 开启引擎底层无限循环

        // 初始化音量
        float initialVolume = volumeSlider != null ? volumeSlider.value : 0.5f;
        SetVolume(initialVolume);

        // 监听滑条事件
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // 只要有文件就直接播放
        if (bgmClip != null)
        {
            _bgmSource.Play();
        }
    }

    // 控制音量的函数
    public void SetVolume(float vol)
    {
        if (_bgmSource != null)
        {
            _bgmSource.volume = vol;
        }
    }
}