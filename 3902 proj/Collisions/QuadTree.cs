using System;
using Microsoft.Xna.Framework;
using TransformersGame.Interfaces;

namespace TransformersGame.Collisions
{

    // The main quadtree class
    internal class QuadTree
    {
        
        struct Node
        {
            public Point pos;
            public IGameObject data;

            public Node(IGameObject _data)
            {
                pos = new Point((int)_data.Position.X, (int)_data.Position.Y);
                data = _data;
            }
        }
        
        private Point topLeft;
        private Point botRight;

        private Node n;


        // Children of this tree
        private QuadTree topLeftTree;
        private QuadTree topRightTree;
        private QuadTree botLeftTree;
        private QuadTree botRightTree;

        public QuadTree()
        {
            topLeft = new Point(0, 0);
            botRight = new Point(0, 0);
            topLeftTree = null;
            topRightTree = null;
            botLeftTree = null;
            botRightTree = null;
            n = new Node();
        }

        public QuadTree(Point topL, Point botR)
        {
            topLeftTree = null;
            topRightTree = null;
            botLeftTree = null;
            botRightTree = null;
            topLeft = topL;
            botRight = botR;
            n = new Node();
        }

        public void Insert(IGameObject obj)
        {
            Node node = new Node(obj);
            
            // Current quad cannot contain it
            if (!InBoundary(node.pos))
                return;

            // We are at a quad of unit area
            // We cannot subdivide this quad further
            if (Math.Abs(topLeft.X - botRight.X) <= 1
                && Math.Abs(botRight.Y - topLeft.Y) <= 1)
            {
                if (n.Equals(default(Node)))
                    n = node;
                return;
            }

            if ((topLeft.X + botRight.X) / 2 >= node.pos.X)
            {
                // Indicates topLeftTree
                if ((topLeft.Y + botRight.Y) / 2 >= node.pos.Y)
                {
                    if (topLeftTree == null)
                        topLeftTree = new QuadTree(
                            new Point(topLeft.X, topLeft.Y),
                            new Point((topLeft.X + botRight.X) / 2,
                                    (topLeft.Y + botRight.Y) / 2));
                    topLeftTree.Insert(node.data);
                }

                // Indicates botLeftTree
                else
                {
                    if (botLeftTree == null)
                        botLeftTree = new QuadTree(
                            new Point(topLeft.X,
                                    (topLeft.Y + botRight.Y) / 2),
                            new Point((topLeft.X + botRight.X) / 2,
                                    botRight.Y));
                    botLeftTree.Insert(node.data);
                }
            }
            else
            {
                // Indicates topRightTree
                if ((topLeft.Y + botRight.Y) / 2 >= node.pos.Y)
                {
                    if (topRightTree == null)
                        topRightTree = new QuadTree(
                            new Point((topLeft.X + botRight.X) / 2,
                                    topLeft.Y),
                            new Point(botRight.X,
                                    (topLeft.Y + botRight.Y) / 2));
                    topRightTree.Insert(node.data);
                }

                // Indicates botRightTree
                else
                {
                    if (botRightTree == null)
                        botRightTree = new QuadTree(
                            new Point((topLeft.X + botRight.X) / 2,
                                    (topLeft.Y + botRight.Y) / 2),
                            new Point(botRight.X, botRight.Y));
                    botRightTree.Insert(node.data);
                }
            }
        }

        public IGameObject Search(Point p)
        {
            // Current quad cannot contain it
            if (!InBoundary(p))
                return default;

            // We are at a quad of unit length
            // We cannot subdivide this quad further
            if (!n.Equals(default(Node)))
                return n.data;

            if ((topLeft.X + botRight.X) / 2 >= p.X)
            {
                // Indicates topLeftTree
                if ((topLeft.Y + botRight.Y) / 2 >= p.Y)
                {
                    if (topLeftTree == null)
                        return default;
                    return topLeftTree.Search(p);
                }

                // Indicates botLeftTree
                else
                {
                    if (botLeftTree == null)
                        return default;
                    return botLeftTree.Search(p);
                }
            }
            else
            {
                // Indicates topRightTree
                if ((topLeft.Y + botRight.Y) / 2 >= p.Y)
                {
                    if (topRightTree == null)
                        return default;
                    return topRightTree.Search(p);
                }

                // Indicates botRightTree
                else
                {
                    if (botRightTree == null)
                        return default;
                    return botRightTree.Search(p);
                }
            }
        }

        // Check if current quadtree contains the point
        private bool InBoundary(Point p)
        {
            return p.X >= topLeft.X && p.X <= botRight.X
                    && p.Y >= topLeft.Y && p.Y <= botRight.Y;
        }
    }
}
