using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Vino.Fragment.Components
{
    [MaterialProperty("_Velocity")]
    public struct FragmentVelocityColor : IComponentData
    {
        //public float3 Value;
        public float Value;
    }
}