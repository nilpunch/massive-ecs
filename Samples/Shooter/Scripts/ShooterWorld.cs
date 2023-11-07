using System;
using System.Linq;
using Massive.Samples.Misc;
using UnityEngine;

namespace Massive.Samples.Shooter
{
    public class ShooterWorld : MonoBehaviour
    {
        [SerializeField] private float _simulationSpeed = 1;
        [SerializeField] private int _frames = 120;
        [SerializeField] private int _resimulate = 120;

        [Header("Game Settings")]
        [SerializeField] private Character _characterPrefab;
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private int _bulletsCapacity = 200;

        private WorldTimeline _worldTimeline;

        public static WorldState<BulletState> BulletWorld { get; private set; }
        public static WorldTime WorldTime { get; private set; }

        private void Awake()
        {
            BulletWorld = new WorldState<BulletState>(_frames, _bulletsCapacity);
            WorldTime = new WorldTime(60);

            // Create character states
            CharacterState[] characters = FindObjectsOfType<CharacterSpawnPoint>().Select(spawnPoint => spawnPoint.GetState()).ToArray();
            var characterWorld = new WorldState<CharacterState>(_frames, characters.Length);
            foreach (CharacterState characterState in characters)
                characterWorld.Reserve(characterState);

            var characterSimulation = new WorldSimulation<CharacterState>(characterWorld,
                new AvailableEntities<CharacterState>(
                    new PrefabFactory<Character, CharacterState>(_characterPrefab),
                    characterWorld.StatesCapacity));

            var bulletsSimulation = new WorldSimulation<BulletState>(BulletWorld,
                new AvailableEntities<BulletState>(
                    new PrefabFactory<Bullet, BulletState>(_bulletPrefab),
                    BulletWorld.StatesCapacity));

            characterSimulation.SaveFrame();
            bulletsSimulation.SaveFrame();

            _worldTimeline = new WorldTimeline(
                new WorldStateGroup(new IWorldState[]
                {
                    characterSimulation,
                    bulletsSimulation
                }),
                new SimulationGroup(new ISimulation[]
                {
                    characterSimulation,
                    bulletsSimulation
                }));
        }

        private float _elapsedTime;

        private void Update()
        {
            _elapsedTime += Time.deltaTime * _simulationSpeed;

            var currentFrame = Mathf.Max(Mathf.FloorToInt(_elapsedTime * WorldTime.FramesPerSecond), 0);

            var resimulateTick = Mathf.Max(currentFrame - _resimulate, 0);
            _worldTimeline.UpdateEarliestApprovedFrame(resimulateTick);
            _worldTimeline.FastForwardToFrame(currentFrame);
        }
    }
}
