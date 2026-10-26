namespace TransformersGame.Interfaces
{
    
    interface IPlayerWeapon : IProjectile
    {
        void HitEnemy(IEnemy enemy);

        void HitSolid(IBlock block);
    }
}
