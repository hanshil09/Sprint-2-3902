namespace TransformersGame.Interfaces
{
       interface IPowerUp : IItem
    {
        IPlayer Apply(IPlayer player, IPlayerHost host);
    }
}
