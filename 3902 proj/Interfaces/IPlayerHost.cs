using TransformersGame.Entities;

namespace TransformersGame.Interfaces
{
     internal interface IPlayerHost
    {
        IPlayer Player { get; set; }

        void RemovePlayerDecorator(PlayerDecorator decorator);
    }
}
