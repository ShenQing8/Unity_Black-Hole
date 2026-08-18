using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Vino.BlackHole.Components;
using UnityEngine;

namespace Vino.BlackHole.Authoring 
{
    public class BlackHoleAuthoring : MonoBehaviour
    {
        public float Mass;
        public float Radius;
        public float eventHorizon;

        [Header("Accretion Disk (吸积盘设置)")]
        public float RingInnerRatio = 5.0f;
        [Range(0.1f, 1.0f)] public float RingOuterRatio = 0.9f;
        [Range(0, 100)] public int RingPercentage = 75;

        private void OnValidate()
        {
            if (Radius <= 0.1f) Radius = 0.1f;
            transform.localScale = new Vector3(Radius * 2f, Radius * 2f, Radius * 2f);
        }
        public class Backer : Baker<BlackHoleAuthoring>
        {
            public override void Bake(BlackHoleAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<BlackHoleTag>(entity);
                AddComponent(entity, new BlackHoleData
                {
                    Mass = authoring.Mass,
                    Radius = authoring.Radius,
                    eventHorizon = authoring.eventHorizon,
                    RingInnerRatio = authoring.RingInnerRatio,
                    RingOuterRatio = authoring.RingOuterRatio,
                    RingPercentage = authoring.RingPercentage
                });
            }
        }
    }
}
