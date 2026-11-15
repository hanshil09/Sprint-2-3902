using ArmadilloGame.Entities;

namespace ArmadilloGame.Interfaces
{
    interface IPlayerHost
    {
        IPlayer Player { get; set; }

        void RemovePlayerDecorator(PlayerDecorator decorator);
    }
}
