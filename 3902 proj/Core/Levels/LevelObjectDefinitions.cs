using Microsoft.Xna.Framework;
using ArmadilloGame.Entities;
using ArmadilloGame.Factories;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Core
{
    internal interface IPlatformObjectDefinition
    {
        IBlock Create(Vector2 position, int length);
    }

    internal interface ITriggerObjectDefinition
    {
        IPlayerTrigger Create(Vector2 position, float gravity);
    }

    internal sealed class MovingPlatformDefinition : IPlatformObjectDefinition
    {
        private const int TravelTiles = 4;
        private const float Speed = 70f;

        public IBlock Create(Vector2 position, int length)
        {
            Vector2 travel = new Vector2(TravelTiles * BlockSpriteFactory.BlockSize, 0);
            ISprite tile = BlockSpriteFactory.Instance.CreatePlatformTileSprite(false);
            return new MovingPlatform(position, length, travel, Speed, tile);
        }
    }

    internal sealed class ElevatorDefinition : IPlatformObjectDefinition
    {
        private const int TravelTiles = 6;
        private const float Speed = 60f;

        public IBlock Create(Vector2 position, int length)
        {
            Vector2 travel = new Vector2(0, -TravelTiles * BlockSpriteFactory.BlockSize);
            ISprite tile = BlockSpriteFactory.Instance.CreatePlatformTileSprite(true);
            return new MovingPlatform(position, length, travel, Speed, tile);
        }
    }

    internal sealed class SpringDefinition : ITriggerObjectDefinition
    {
        public IPlayerTrigger Create(Vector2 position, float gravity)
        {
            return new SpringPad(position, gravity);
        }
    }

    internal sealed class GoalDefinition : ITriggerObjectDefinition
    {
        public IPlayerTrigger Create(Vector2 position, float gravity)
        {
            return new Goal(position);
        }
    }
}
