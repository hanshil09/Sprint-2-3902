namespace TransformersGame.Interfaces
{
    
    internal interface IPlayerWeapon : IProjectile
    {
        void HitEnemy(IEnemy enemy);

        void HitSolid(IBlock block);
    }
}
