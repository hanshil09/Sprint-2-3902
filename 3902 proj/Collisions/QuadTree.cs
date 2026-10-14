using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Controllers;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.UI;

namespace TransformersGame.Collisions
{

    // The main quadtree class
    public class QuadTree
    {
        public struct Node
        {
            public Point pos;
            public int data;

            public Node(Point pos, int data)
            {
                this.pos = pos;
                this.data = data;
            }
        }
        
        private Point topLeft;
        private Point botRight;

        // Contains details of node
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
            n = new Node(); // Initialize to default Node
            topLeftTree = null;
            topRightTree = null;
            botLeftTree = null;
            botRightTree = null;
        }

        public QuadTree(Point topL, Point botR)
        {
            n = new Node(); // Initialize to default Node
            topLeftTree = null;
            topRightTree = null;
            botLeftTree = null;
            botRightTree = null;
            topLeft = topL;
            botRight = botR;
        }

        public void Insert(Node node)
        {
            if (node.Equals(default(Node)))
                return;

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
                    topLeftTree.Insert(node);
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
                    botLeftTree.Insert(node);
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
                    topRightTree.Insert(node);
                }

                // Indicates botRightTree
                else
                {
                    if (botRightTree == null)
                        botRightTree = new QuadTree(
                            new Point((topLeft.X + botRight.X) / 2,
                                    (topLeft.Y + botRight.Y) / 2),
                            new Point(botRight.X, botRight.Y));
                    botRightTree.Insert(node);
                }
            }
        }

        public Node Search(Point p)
        {
            // Current quad cannot contain it
            if (!InBoundary(p))
                return default(Node);

            // We are at a quad of unit length
            // We cannot subdivide this quad further
            if (!n.Equals(default(Node)))
                return n;

            if ((topLeft.X + botRight.X) / 2 >= p.X)
            {
                // Indicates topLeftTree
                if ((topLeft.Y + botRight.Y) / 2 >= p.Y)
                {
                    if (topLeftTree == null)
                        return default(Node);
                    return topLeftTree.Search(p);
                }

                // Indicates botLeftTree
                else
                {
                    if (botLeftTree == null)
                        return default(Node);
                    return botLeftTree.Search(p);
                }
            }
            else
            {
                // Indicates topRightTree
                if ((topLeft.Y + botRight.Y) / 2 >= p.Y)
                {
                    if (topRightTree == null)
                        return default(Node);
                    return topRightTree.Search(p);
                }

                // Indicates botRightTree
                else
                {
                    if (botRightTree == null)
                        return default(Node);
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
