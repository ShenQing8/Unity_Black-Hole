using Unity.Entities;

namespace Vino.Spawner.Component
{
    public struct SpawnerData : IComponentData
    {
        public Entity prefab;
        public int count;
        public int targetActiveCount;
        public bool needRespawn;
    }
    public struct SpawnerTag : IComponentData { }
}