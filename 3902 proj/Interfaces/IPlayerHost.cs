using TransformersGame.Entities;

namespace TransformersGame.Interfaces
{
    interface IPlayerHost
    {
        IPlayer Player { get; set; }

        void RemovePlayerDecorator(PlayerDecorator decorator);
    }
}
