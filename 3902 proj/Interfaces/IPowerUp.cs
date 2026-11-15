namespace ArmadilloGame.Interfaces
{
       interface IPowerUp : IItem
    {
        IPlayer Apply(IPlayer player, IPlayerHost host);
    }
}
