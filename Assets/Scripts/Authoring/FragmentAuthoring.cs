using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Vino.Fragment.Components;
using Unity.Rendering;

namespace Vino.Fragment.Authorings
{
    public class FragmentAuthoring : MonoBehaviour
    {
        public float Mass = 1.0f;
        public float3 Velocity;

        //[Header("Color Mapping (粒子变色映射)")]
        //public float MinColorSpeed = 5f;
        //public float MaxColorSpeed = 40f;

        //[ColorUsage(showAlpha: true, hdr: true)]
        //public Color SlowColor = new Color(1.5f, 0.1f, 0f, 1f);

        //[ColorUsage(showAlpha: true, hdr: true)]
        //public Color FastColor = new Color(0.5f, 3f, 8f, 1f);
        public class Backer : Baker<FragmentAuthoring>
        {
            public override void Bake(FragmentAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<FragmentTag>(entity);
                AddComponent(entity, new FragmentData
                {
                    Mass = authoring.Mass,
                    Velocity = authoring.Velocity,
                    //MinColorSpeed = authoring.MinColorSpeed,
                    //MaxColorSpeed = authoring.MaxColorSpeed,
                    //SlowColor = new float4(authoring.SlowColor.r, authoring.SlowColor.g, authoring.SlowColor.b, authoring.SlowColor.a),
                    //FastColor = new float4(authoring.FastColor.r, authoring.FastColor.g, authoring.FastColor.b, authoring.FastColor.a)
                });
                AddComponent(entity, new FragmentVelocityColor { Value = math.length(authoring.Velocity) });
            }
        }
    }
}