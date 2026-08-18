using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Vino.Fragment.Components
{
    [MaterialProperty("_BaseColor")]
    public struct FragmentColorOverride : IComponentData
    {
        public float4 Value;
    }
}