namespace hill_climbing_algorithm
{
    public class PathFindNode
    {
        public PathFindNode(PathFindNode? parent, int pos, char val)
        {
            parentNode = parent;
            this.pos = pos;
            this.val = val;
        }

        public PathFindNode? parentNode;
        public int pos;
        public char val;

        public int StepsToStart
        {
            get
            {
                if (parentNode == null)
                    return 0;

                return parentNode.StepsToStart + 1;
            }
        }
    }
    internal class Program
    {

        int rows = 0;
        int cols = 0;
        int startPos = 0;
        List<int> allStartPositions = new();
        int targetPos = 0;
        List<List<char>> grid = new();

        List<PathFindNode> open = new();
        List<PathFindNode> closed = new();
        
        public void Run()
        {
            ParseFile(File.ReadAllLines("input.txt"));
            PathFindNode target = PathFind(startPos);

            if (target == null) Console.WriteLine("No path found");
            else Console.WriteLine(target?.StepsToStart);


            // Part 2
            // -----------------
            int leastSteps = int.MaxValue;
            foreach(int sPos in allStartPositions)
            {
                open.Clear();
                closed.Clear();
                PathFindNode target2 = PathFind(sPos);
                if(target2 != null)
                {
                    int steps = target2.StepsToStart;
                    if (steps < leastSteps)
                        leastSteps = steps;
                }

            }
            Console.WriteLine(leastSteps);




            Console.WriteLine("Hello World");
        }

        char GetGridValue(int index)
        {
            if (index < 0)
                return ' ';

            int ri = index / cols;
            int ci = index % cols;
            return grid[ri][ci];
        }

        public PathFindNode? PathFindStep()
        {
            var currentNode = open[open.Count - 1];
            open.RemoveAt(open.Count - 1);
            closed.Add(currentNode);

            if(currentNode.pos == targetPos)
            {
                return currentNode;
            }

            List<int> children = GetChildren(currentNode.pos);
            for(int i=0; i<children.Count; i++)
            {
                var childPos = children[i];
                var childNode = new PathFindNode(currentNode, childPos, GetGridValue(childPos));

                var existingChildNode = FindInOpenOrClosed(childPos);
                if(existingChildNode != null)
                {
                    if(childNode.StepsToStart < existingChildNode.StepsToStart)
                        existingChildNode.parentNode = currentNode;
                }
                else
                {
                    open.Add(childNode);
                }
            }

            open = open.OrderByDescending(z => z.StepsToStart).ToList();
            return null;

        }

        bool InBounds(int row, int col)
        {
            return row >= 0 && row < rows && col >= 0 && col < cols;
        }

        bool CanWalk(int fromPos, int toPos)
        {
            if (fromPos < 0 || toPos < 0)
                return false;

            char fromVal = GetGridValue(fromPos);
            char toVal = GetGridValue(toPos);

            int dif = toVal - fromVal;
            return dif <= 1;
        }

        int RowColToIndex(int row, int col)
        {
            return row * cols + col;
        }

        public List<int> GetChildren(int pos)
        {
            int ri = pos / cols;
            int ci = pos % cols;

            int lPos = InBounds(ri, ci - 1) ? RowColToIndex(ri, ci - 1) : -1;
            int rPos = InBounds(ri, ci + 1) ? RowColToIndex(ri, ci + 1) : -1;
            int uPos = InBounds(ri - 1, ci) ? RowColToIndex(ri - 1, ci) : -1;
            int dPos = InBounds(ri + 1, ci) ? RowColToIndex(ri + 1, ci) : -1;

            List<int> children = new();
            if (CanWalk(pos, lPos)) children.Add(lPos);
            if (CanWalk(pos, rPos)) children.Add(rPos);
            if (CanWalk(pos, uPos)) children.Add(uPos);
            if (CanWalk(pos, dPos)) children.Add(dPos);

            return children;
        }

        public PathFindNode PathFind(int starLocation)
        {
            PathFindNode? targetNode = null;
            open.Add(new PathFindNode(null, starLocation, GetGridValue(starLocation)));
            while(open.Count > 0)
            {
                var foundNode = PathFindStep();
                if (foundNode != null)
                {
                    targetNode = foundNode;
                    break;
                }
            }

            return targetNode;
        }

        PathFindNode? FindInOpenOrClosed(int pos)
        {
            var openNode = open.FirstOrDefault(z => z.pos == pos);
            if (openNode != null) return openNode;

            var closedNode = closed.FirstOrDefault(z => z.pos == pos);
            if (closedNode != null) return closedNode;

            return null;
        }

        public void ParseFile(string[] lines)
        {
            grid = lines.Select(z => z.ToCharArray().ToList()).ToList();
            rows = grid.Count;
            cols = grid[0].Count;

            for (int ri = 0; ri < rows; ri++)
            {
                for (int ci = 0; ci < cols; ci++)
                {
                    if (grid[ri][ci] == 'S')
                    {
                        startPos = ri * cols + ci;
                        grid[ri][ci] = 'a';
                    }
                    if (grid[ri][ci] == 'E')
                    {
                        targetPos = ri * cols + ci;
                        grid[ri][ci] = 'z';
                    }

                    if (grid[ri][ci] == 'a')
                    {
                        allStartPositions.Add(ri * cols + ci);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }
    }
}