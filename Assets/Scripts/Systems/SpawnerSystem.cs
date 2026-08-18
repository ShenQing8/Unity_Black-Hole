using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Vino.Fragment.Components;
using Vino.Spawner.Component;

namespace Vino.Fragment.Systems
{
    [BurstCompile]
    public partial struct SpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnerData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //var spawner = SystemAPI.GetSingleton<SpawnerData>();
            //var instances = state.EntityManager.Instantiate(spawner.prefab, spawner.count, Allocator.Temp);

            //instances.Dispose();
            //state.Enabled = false;

            if (!SystemAPI.TryGetSingletonRW<SpawnerData>(out var spawnerRW)) return;

            // 检查你的 bool 开关是否为 true (ValueRO 代表只读访问，性能更好)
            if (spawnerRW.ValueRO.needRespawn)
            {
                // 执行瞬间爆兵
                var instances = state.EntityManager.Instantiate(spawnerRW.ValueRO.prefab, spawnerRW.ValueRO.count, Allocator.Temp);
                instances.Dispose();

                // 【完事后关闭开关】(ValueRW 代表写入访问)
                spawnerRW.ValueRW.needRespawn = false;
            }
        }

    }
}