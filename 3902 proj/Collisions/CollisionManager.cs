using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Collisions
{
    public class CollisionManager
    {
        private QuadTree tree;
        private int worldWidth;
        private int worldHeight;
        private List<IGameObject> objects;
        public CollisionManager(int maxWidth, int maxHeight)
        {
            worldWidth = maxWidth;
            worldHeight = maxHeight;
            tree = new QuadTree(
                                new Point(0),
                                new Point(worldWidth, worldHeight)
                                );
        }
        public void BuildTree(List<IGameObject> objs)
        {
            objects = objs;
            foreach(IGameObject ob in objects)
            {
                tree.Insert(ob);
            }
        }
        private IGameObject PotentialCollision(IGameObject obj)
        {
            return tree.Search(new Point((int)obj.Position.X, (int)obj.Position.Y));
        }
        public void DetectCollision()
        {
            List<IGameObject> nearbyObjects = null;
            foreach(IGameObject ob in objects)
            {
                nearbyObjects.Add(PotentialCollision(ob));
                foreach(IGameObject closeObject in nearbyObjects)
                {
                    if(ob != closeObject && closeObject != default && CheckCollision(ob, closeObject))
                    {
                        HandleCollision(ob, closeObject);
                    }
                }
            }

        }
        static private bool CheckCollision(IGameObject objectA, IGameObject objectB)
        {
            bool collided = false;
            if(objectA.Position.X > objectB.Position.X -2 && objectA.Position.X < objectB.Position.X +2)
            {
                if(objectA.Position.Y > objectB.Position.Y -2 && objectA.Position.Y < objectB.Position.Y +2)
                {
                    collided = true;
                }
            }
            return collided;
        }

        static private void HandleCollision(IGameObject objectA, IGameObject objectB)
        {
            if(objectA is Player)
            {
                if(objectB is Block)
                {
                    
                }
                if(objectB is EnemyProjectile)
                {
                    
                }
                if(objectB is Enemy)
                {
                    
                }
            }
            if(objectA is Enemy)
            {
                if(objectB is Block)
                {
                    
                }
            }
            //TODO: finish filling in all handling cases
        }
    }
}