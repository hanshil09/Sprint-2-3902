

struct Point
{
    public int x;
    public int y;

    public Point(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}

struct Node
{
    public Point pos;
    public int data;

    public Node(Point _pos, int _data)
    {
        pos = _pos;
        data = _data;
    }
}

// The main quadtree class
class Quad
{
    private Point topLeft;
    private Point botRight;

    // Contains details of node
    private Node n;

    // Children of this tree
    private Quad topLeftTree;
    private Quad topRightTree;
    private Quad botLeftTree;
    private Quad botRightTree;

    public Quad()
    {
        topLeft = new Point(0, 0);
        botRight = new Point(0, 0);
        n = new Node(); // Initialize to default Node
        topLeftTree = null;
        topRightTree = null;
        botLeftTree = null;
        botRightTree = null;
    }

    public Quad(Point topL, Point botR)
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
        if (Math.Abs(topLeft.x - botRight.x) <= 1
            && Math.Abs(topLeft.y - botRight.y) <= 1)
        {
            if (n.Equals(default(Node)))
                n = node;
            return;
        }

        if ((topLeft.x + botRight.x) / 2 >= node.pos.x)
        {
            // Indicates topLeftTree
            if ((topLeft.y + botRight.y) / 2 >= node.pos.y)
            {
                if (topLeftTree == null)
                    topLeftTree = new Quad(
                        new Point(topLeft.x, topLeft.y),
                        new Point((topLeft.x + botRight.x) / 2,
                                  (topLeft.y + botRight.y) / 2));
                topLeftTree.Insert(node);
            }

            // Indicates botLeftTree
            else
            {
                if (botLeftTree == null)
                    botLeftTree = new Quad(
                        new Point(topLeft.x,
                                  (topLeft.y + botRight.y) / 2),
                        new Point((topLeft.x + botRight.x) / 2,
                                  botRight.y));
                botLeftTree.Insert(node);
            }
        }
        else
        {
            // Indicates topRightTree
            if ((topLeft.y + botRight.y) / 2 >= node.pos.y)
            {
                if (topRightTree == null)
                    topRightTree = new Quad(
                        new Point((topLeft.x + botRight.x) / 2,
                                  topLeft.y),
                        new Point(botRight.x,
                                  (topLeft.y + botRight.y) / 2));
                topRightTree.Insert(node);
            }

            // Indicates botRightTree
            else
            {
                if (botRightTree == null)
                    botRightTree = new Quad(
                        new Point((topLeft.x + botRight.x) / 2,
                                  (topLeft.y + botRight.y) / 2),
                        new Point(botRight.x, botRight.y));
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

        if ((topLeft.x + botRight.x) / 2 >= p.x)
        {
            // Indicates topLeftTree
            if ((topLeft.y + botRight.y) / 2 >= p.y)
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
            if ((topLeft.y + botRight.y) / 2 >= p.y)
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
        return p.x >= topLeft.x && p.x <= botRight.x
                && p.y >= topLeft.y && p.y <= botRight.y;
    }
}
