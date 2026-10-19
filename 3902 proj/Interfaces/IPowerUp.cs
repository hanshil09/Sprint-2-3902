namespace TransformersGame.Interfaces
{
       internal interface IPowerUp : IItem
    {
        IPlayer Apply(IPlayer player, IPlayerHost host);
    }
}
