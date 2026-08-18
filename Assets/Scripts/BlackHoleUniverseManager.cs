using TMPro;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Vino.BlackHole.Components;
using Vino.Global.Gravity.Component;
using Vino.Spawner.Component;

public class BlackHoleUniverseManager : MonoBehaviour
{
    [Header("物理控制 (ECS)")]
    public Slider countSlider;
    public Slider massSlider;
    public Slider dragSlider;
    public Slider rangeSlider;

    [Header("视觉控制 (Shader Global)")]
    public Slider brightnessSlider;
    public Slider globalScaleSlider;

    [Header("UI 颜色控制 (RGB Sliders)")]

    public Slider fastR, fastG, fastB;
    public Slider slowR, slowG, slowB;

    [Header("全局粒子上限控制 (重启宇宙)")]
    public TMP_InputField maxLimitInputField;

    private EntityManager _entityManager;
    private Entity _spawnerEntity;
    private Entity _gravityEntity;
    private Entity _blackHoleEntity;
    private bool _isInitialized = false;

    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (maxLimitInputField != null)
        {
            maxLimitInputField.onEndEdit.AddListener(OnMaxLimitChanged);
        }
    }

    void Update()
    {
        if (!_isInitialized)
        {
            if (!TryFindEntities()) return;
        }

        SyncPhysicsToECS();
        SyncVisualsToGPU();
    }

    private bool TryFindEntities()
    {
        var spawnerQuery = _entityManager.CreateEntityQuery(typeof(SpawnerData));
        var gravityQuery = _entityManager.CreateEntityQuery(typeof(GravityConfigData));
        var blackHoleQuery = _entityManager.CreateEntityQuery(typeof(BlackHoleData));

        if (!spawnerQuery.IsEmpty && !gravityQuery.IsEmpty && !blackHoleQuery.IsEmpty)
        {
            _spawnerEntity = spawnerQuery.GetSingletonEntity();
            _gravityEntity = gravityQuery.GetSingletonEntity();
            _blackHoleEntity = blackHoleQuery.GetSingletonEntity();

            InitializeSlidersWithInspectorValues();
            _isInitialized = true;
            return true;
        }
        return false;
    }

    private void InitializeSlidersWithInspectorValues()
    {
        var sData = _entityManager.GetComponentData<SpawnerData>(_spawnerEntity);

        countSlider.maxValue = sData.count;
        countSlider.value = sData.count;


        _isInitialized = true;
        SyncPhysicsToECS();
        SyncVisualsToGPU();

    }

    private void SyncPhysicsToECS()
    {
        if (!_isInitialized) return;

        var sData = _entityManager.GetComponentData<SpawnerData>(_spawnerEntity);
        sData.targetActiveCount = (int)countSlider.value;
        _entityManager.SetComponentData(_spawnerEntity, sData);

        var bData = _entityManager.GetComponentData<BlackHoleData>(_blackHoleEntity);
        bData.Mass = massSlider.value;
        _entityManager.SetComponentData(_blackHoleEntity, bData);

        var gData = _entityManager.GetComponentData<GravityConfigData>(_gravityEntity);
        gData.SpawnRadius = rangeSlider.value;
        gData.ParticleDrag = dragSlider.value;
        _entityManager.SetComponentData(_gravityEntity, gData);
    }

    private void SyncVisualsToGPU()
    {
        Shader.SetGlobalFloat("_GlobalBrightness", brightnessSlider.value);
        Shader.SetGlobalFloat("_GlobalScale", globalScaleSlider.value);

        Color currentFastColor = new Color(fastR.value, fastG.value, fastB.value, 1.0f);
        Color currentSlowColor = new Color(slowR.value, slowG.value, slowB.value, 1.0f);

        Shader.SetGlobalColor("_FastColor", currentFastColor);
        Shader.SetGlobalColor("_SlowColor", currentSlowColor);
    }

    public void OnMaxLimitChanged(string inputText)
    {
        if (!_isInitialized) return;

        if (int.TryParse(inputText, out int newMaxCount))
        {
            newMaxCount = Mathf.Max(newMaxCount, 100);

            var particleQuery = _entityManager.CreateEntityQuery(typeof(Vino.Fragment.Components.FragmentTag));
            _entityManager.DestroyEntity(particleQuery);

            var sData = _entityManager.GetComponentData<SpawnerData>(_spawnerEntity);
            sData.count = newMaxCount;
            sData.targetActiveCount = newMaxCount;
            sData.needRespawn = true;
            _entityManager.SetComponentData(_spawnerEntity, sData);
            //Debug.Log($"宇宙已重启！新的粒子上限为：{newMaxCount}");
        }
        else
        {
            if (countSlider != null)
            {
                maxLimitInputField.text = countSlider.maxValue.ToString();
            }
        }
    }
}