using Unity.Entities;
using UnityEngine;
using Vino.Global.Gravity.Component;
namespace Vino.Global.Gravity.Authoring
{
    public class GravityConfigAuthoring : MonoBehaviour
    {
        public float G;
        public float ParticleDrag;//阻力
        public float epsilon;
        public float SpawnRadius;
        public float InitialSpeed;
        public class Baker : Baker<GravityConfigAuthoring>
        {
            public override void Bake(GravityConfigAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent<GravityConfigTag>(entity);
                AddComponent(entity, new GravityConfigData
                {
                    G = authoring.G,
                    ParticleDrag = authoring.ParticleDrag,
                    epsilon = authoring.epsilon,
                    SpawnRadius = authoring.SpawnRadius,
                    InitialSpeed = authoring.InitialSpeed
                });
            }
        }
    }
}