using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using Vino.Fragment.Components;

namespace Vino.Global.Gravity.Jobs
{
    [BurstCompile]
    public partial struct CalculateSingularityGravityJob : IJobEntity
    {
        public int activeCount;

        public float3 blackHolePos;
        public float GM;
        public float epsilonSq;
        public float deltaTme;
        public uint randomSeed;
        public float spawnRadius;
        public float initialSpeed;
        public int ringPercentage;

        public float eventHorizonSq;
        public float eventHorizonRadius; //在 System 中计算：math.sqrt(eventHorizonSq)
        public float safeMaxSpeed;       //在 System 中计算：(eventHorizonRadius * 0.8f) / deltaTme
        public float safeMaxSpeedSq;     //在 System 中计算：safeMaxSpeed * safeMaxSpeed

        public float minRingRadius;      //System计算：eventHorizonRadius * ringInnerRatio
        public float maxRingRadius;      //System计算：spawnRadius * ringOuterRatio

        public float dragMultiplierRing; //System计算：math.exp(-(particleDrag * 0.02f) * deltaTme)
        public float dragMultiplierChaos;//System计算：math.exp(-(particleDrag * 2.0f) * deltaTme)


        [BurstCompile]
        public void Execute([EntityIndexInQuery] int entityIndex,
                        RefRW<FragmentData> fragment,
                        RefRW<FragmentVelocityColor> shaderVel,
                        ref LocalTransform t)
        {
            if (entityIndex >= activeCount)
            {
                t.Scale = 0f;
                return;
            }
            t.Scale = 1.0f;
            var r = blackHolePos - t.Position;
            var distSq = math.lengthsq(r);

            //优化点：抛弃昂贵的 math.hash，直接用 index 取模，足以满足视觉随机性
            bool isRingParticle = (entityIndex % 100) < ringPercentage;

            if (distSq < eventHorizonSq)
            {
                uint seed = (uint)entityIndex + randomSeed + 1;
             
                if (seed == uint.MaxValue)
                {
                    seed = uint.MaxValue - 1;
                }
                var random = Unity.Mathematics.Random.CreateFromIndex(seed);
                //isRingParticle = random.NextFloat() < (ringPercentage * 0.01f);
                float3 newPosition;
                float3 newVelocity;

                if (isRingParticle)
                {
                    float angle = random.NextFloat(0f, math.PI * 2f);
                    float yThickness = random.NextFloat(-0.1f, 0.1f);
                    float3 diskDir = math.normalize(new float3(math.cos(angle), yThickness, math.sin(angle)));

                    float ra = random.NextFloat();
                    //优化：尽量少用 pow，求立方可以直接连乘 ra * ra * ra，效率更高
                    float exponentialT = ra * ra * ra;

                    float3 up = new float3(0, 1, 0);
                    float3 tangent = math.normalize(math.cross(up, diskDir));

                    float speedNoise = random.NextFloat(1.2f, 1.8f);
                    newVelocity = tangent * initialSpeed * speedNoise;

                    float randomDist = math.lerp(minRingRadius, maxRingRadius, exponentialT);
                    newPosition = blackHolePos + diskDir * randomDist;

                    
                }
                else
                {
                    float3 randomDir = math.normalize(random.NextFloat3(-1f, 1f));
                    float ra = random.NextFloat();

                    float3 arbitraryUp = (math.abs(randomDir.y) < 0.9f) ? new float3(0, 1, 0) : new float3(1, 0, 0);
                    float3 semiTangent = math.normalize(math.cross(arbitraryUp, randomDir));

                    float speedNoise = random.NextFloat(0.2f, 0.6f);
                    newVelocity = semiTangent * initialSpeed * speedNoise;

                    //优化：ra * ra 替代 math.pow(ra, 2.0f)
                    float randomDist = math.lerp(spawnRadius * 0.6f, spawnRadius * 1.5f, ra * ra);
                    newPosition = blackHolePos + randomDir * randomDist;

                    
                }

                fragment.ValueRW.Velocity = newVelocity;

                t.Position = newPosition;

                float initialSpeedValue = math.length(newVelocity);
                shaderVel.ValueRW.Value = initialSpeedValue;
                //t.Scale = (initialSpeedValue * 0.001f) + 0.1f;

                if (initialSpeedValue > 0.1f)
                {
                    t.Rotation = quaternion.LookRotationSafe(newVelocity, new float3(0, 1, 0));
                }

                return;
            }

            //原代码：var a = GM * math.pow(distSq + epsilonSq, -1.5f) * r;
            //x^(-1.5) = rsqrt(x) * (1.0 / x)
            float distPlusEpsilon = distSq + epsilonSq;
            float invDist = math.rsqrt(distPlusEpsilon);
            var a = GM * invDist * (1.0f / distPlusEpsilon) * r;

            float3 currentVel = fragment.ValueRO.Velocity;
            currentVel += a * deltaTme;

            //优化点：直接使用 System 传进来的预计算乘数，省去 15万次 math.exp() 计算
            currentVel *= isRingParticle ? dragMultiplierRing : dragMultiplierChaos;

            //优化点：使用 System 预计算的 safeMaxSpeedSq
            if (math.lengthsq(currentVel) > safeMaxSpeedSq)
            {
                currentVel = math.normalize(currentVel) * safeMaxSpeed;
            }

            //fragment.ValueRW.Velocity = currentVel;
            //t.Position += currentVel * deltaTme;

            //float currentSpeed = math.length(currentVel);
            //t.Scale = (currentSpeed * 0.001f) + 0.1f;
            fragment.ValueRW.Velocity = currentVel;
            float currentSpeed = math.length(currentVel);
            shaderVel.ValueRW.Value = currentSpeed;
            //float speedMagnitude = math.length(currentVel);
            //shaderVel.ValueRW.Value = new float3(speedMagnitude, speedMagnitude, speedMagnitude);

            t.Position += currentVel * deltaTme;

            if (currentSpeed > 0.01f)
            {
                t.Rotation = quaternion.LookRotationSafe(currentVel, new float3(0, 1, 0));
            }
        }
    }
}