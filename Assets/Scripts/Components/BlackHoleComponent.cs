using Unity.Entities;
using Unity.Mathematics;

namespace Vino.BlackHole.Components
{
    public struct BlackHoleData : IComponentData
    {
        public float Mass;
        public float Radius;
        public float eventHorizon;
        public float RingInnerRatio;
        public float RingOuterRatio;
        public int RingPercentage;
    }
    public struct BlackHoleTag : IComponentData { }
}