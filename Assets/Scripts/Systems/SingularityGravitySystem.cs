using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SocialPlatforms;
using Vino.BlackHole.Components;
using Vino.Fragment.Components;
using Vino.Global.Gravity.Component;
using Vino.Global.Gravity.Jobs;
using Vino.Spawner.Component;

namespace Vino.Global.Gravity.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct SingularityGravitySystem : ISystem
    {
        //private JobHandle myHandle;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BlackHoleData>();
            state.RequireForUpdate<GravityConfigData>();

        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            float deltaTime = SystemAPI.Time.DeltaTime;
            uint randomSeed = math.hash((double2)SystemAPI.Time.ElapsedTime) + 1;

            var blackHoleData = SystemAPI.GetSingleton<BlackHoleData>();
            var gravityConfig = SystemAPI.GetSingleton<GravityConfigData>();

            var blackHoleEntity = SystemAPI.GetSingletonEntity<BlackHoleData>();
            var blackHolePos = SystemAPI.GetComponent<LocalTransform>(blackHoleEntity).Position;

            float GM = gravityConfig.G * blackHoleData.Mass;
            float epsilonSq = gravityConfig.epsilon * gravityConfig.epsilon;
            float eventHorizonSq = blackHoleData.eventHorizon * blackHoleData.eventHorizon; // 消失判定距离平方

            float eventHorizonRadius = math.sqrt(eventHorizonSq);
            float safeMaxSpeed = (eventHorizonRadius * 0.8f) / deltaTime;
            int currentActiveCount = SystemAPI.GetSingleton<SpawnerData>().targetActiveCount;
            var calculateSingularityGravityJob = new CalculateSingularityGravityJob
            {
                activeCount = currentActiveCount,

                GM = GM,
                blackHolePos = blackHolePos,
                epsilonSq = epsilonSq,
                deltaTme = deltaTime,
                initialSpeed = gravityConfig.InitialSpeed,
                spawnRadius = gravityConfig.SpawnRadius,
                eventHorizonSq = eventHorizonSq,
                randomSeed = randomSeed,
                ringPercentage = blackHoleData.RingPercentage,


                eventHorizonRadius = eventHorizonRadius,
                safeMaxSpeed = safeMaxSpeed,
                safeMaxSpeedSq = safeMaxSpeed * safeMaxSpeed,

                //预计算内外环真实半径
                minRingRadius = eventHorizonRadius * blackHoleData.RingInnerRatio,
                maxRingRadius = gravityConfig.SpawnRadius * blackHoleData.RingOuterRatio,

                //预计算阻力的指数衰减乘数
                dragMultiplierRing = math.exp(-(gravityConfig.ParticleDrag * 0.02f) * deltaTime),
                dragMultiplierChaos = math.exp(-(gravityConfig.ParticleDrag * 2.0f) * deltaTime)
            };

            state.Dependency = calculateSingularityGravityJob.ScheduleParallel(state.Dependency);
        }
    }
}