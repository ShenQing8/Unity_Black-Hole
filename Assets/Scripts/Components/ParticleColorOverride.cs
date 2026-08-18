using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Vino.Fragment.Components
{
    [MaterialProperty("_MyParticleColor")]
    public struct ParticleColorOverride : IComponentData
    {
        public float4 Value;
    }
}