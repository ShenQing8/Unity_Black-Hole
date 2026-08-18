using Unity.Entities;
using Unity.Mathematics;

namespace Vino.Global.Gravity.Component
{
    public struct GravityConfigData : IComponentData
    {
        public float G;
        public float ParticleDrag;//阻力
        public float epsilon;
        public float SpawnRadius;
        public float InitialSpeed;
    }

    public struct GravityConfigTag : IComponentData { }
}