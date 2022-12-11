using System.Transactions;

namespace rope_bridge
{

    public class RopeBridge
    {

        int cMin = 0;
        int cMax = 0;
        int rMin = 0;
        int rMax = 0;

        List<string> snake = new List<string>();

        Dictionary<string, int> visitedPositions = new();

        public RopeBridge()
        {
            SetValue(0, 0, '.');
            SetValue(-2, -2, '.');
            SetValue(2, 2, '.');

            visitedPositions["0 0"] = 1;

            for (int i = 0; i < 10; i++)
                snake.Add("0 0");
        }

        public void SetValue(int rowIndex, int colIndex, char value)
        {
            rMin = Math.Min(rMin, rowIndex);
            cMin = Math.Min(cMin, colIndex);
            rMax = Math.Max(rMax, rowIndex);
            cMax = Math.Max(cMax, colIndex);
        }

        public void Print()
        {
            int visitedCount = visitedPositions.Count;
            Console.WriteLine($"Tail Visited Count: {visitedCount} ");
            for (int r = rMin; r <= rMax; r++)
            {
                for (int c = cMin; c <= cMax; c++)
                {
                    bool containsHeadOrTailPos = false;
                    for (int i = 0; i < snake.Count; i++)
                    {
                        GetSnakePos(i, out int xPos, out int yPos);
                        if (r == yPos && c == xPos)
                        {
                            if (i == 0) Console.Write('H');
                            else Console.Write($"{i}");
                            containsHeadOrTailPos = true;
                            break;
                        }
                    }

                    if (containsHeadOrTailPos == false)
                    {
                        string key = $"{c} {r}";
                        // if (visitedPositions.ContainsKey(key)) Console.Write('#');
                        // else Console.Write('.');
                        Console.Write('.');
                    }

                }
                Console.WriteLine();
            }
        }

        void GetSnakePos(int i, out int xPos, out int yPos)
        {
            string[] parts = snake[i].Split(" ");
            xPos = int.Parse(parts[0]);
            yPos = int.Parse(parts[1]);
        }

        void SetSnakePos(int i, int xPos, int yPos)
        {
            string key = $"{xPos} {yPos}";
            snake[i] = key;

            if (i == snake.Count - 1)
            {
                if (visitedPositions.ContainsKey(key) == false)
                    visitedPositions[key] = 0;

                visitedPositions[key] += 1;
            }
        }

        public void MoveHead(int xDir, int yDir)
        {
            MoveSnakePart(0, xDir, yDir);
            //GetSnakePos(0, out var headXPos, out var headYPos);


            //int currentXPos = headXPos;
            //int currentYPos = headYPos;

            //headXPos += xDir;
            //headYPos += yDir;

            //SetSnakePos(0, headXPos, headYPos);

            //rMin = Math.Min(rMin, headYPos);
            //cMin = Math.Min(cMin, headXPos);
            //rMax = Math.Max(rMax, headYPos);
            //cMax = Math.Max(cMax, headXPos);

            //GetSnakePos(1, out var tailXPos, out var tailYPos);

            //int xDif = headXPos - tailXPos;
            //int yDif = headYPos - tailYPos;
            //int dist = (int)MathF.Sqrt(xDif * xDif + yDif * yDif);

            //if (dist > 1)
            //{
            //    GetSnakePos(1, out int previousXPos, out int previousYPos);
            //    SetSnakePos(1, currentXPos, currentYPos);


            //    for (int i=2; i<snake.Count; i++)
            //    {
            //        GetSnakePos(i, out currentXPos, out currentYPos);
            //        SetSnakePos(i, previousXPos, previousYPos);
            //        previousXPos = currentXPos;
            //        previousYPos = currentYPos;
            //    }
            //    // MoveSnakePart(i + 1, currentXPos - tailXPos, currentYPos - tailYPos, 1);

            //}
        }

        public void MoveSnakePart(int i, int xDir, int yDir)
        {
            GetSnakePos(i, out var currentXPos, out var currentYPos);

            int newXPos = currentXPos + xDir;
            int newYPos = currentYPos + yDir;

            SetSnakePos(i, currentXPos + xDir, currentYPos + yDir);

            rMin = Math.Min(rMin, newYPos);
            cMin = Math.Min(cMin, newXPos);
            rMax = Math.Max(rMax, newYPos);
            cMax = Math.Max(cMax, newXPos);

            MoveTailTowardParent(i + 1);

            // GetSnakePos(i + 1, out var tailXPos, out var tailYPos);
            // 
            // int xDif = headXPos - tailXPos;
            // int yDif = headYPos - tailYPos;
            // int dist = (int)MathF.Sqrt(xDif * xDif + yDif * yDif);
            // 
            // if (dist > minDist)
            // {
            //     // MoveSnakePart(i + 1, xDir, yDir, 1);
            //     MoveSnakePart(i + 1, (currentXPos - tailXPos) * Math.Abs(xDir), (currentYPos - tailYPos) * Math.Abs(yDir), 1);
            // 
            // }
        }

        public void MoveTailTowardParent(int i)
        {
            GetSnakePos(i - 1, out var parentXPos, out var parentYPos);
            GetSnakePos(i, out var currentXPos, out var currentYPos);

            float xDif = parentXPos - currentXPos;
            float yDif = parentYPos - currentYPos;
            float dist = MathF.Sqrt(xDif * xDif + yDif * yDif);
            int xDir = (int)(xDif > 0 ? MathF.Ceiling(xDif / dist) : MathF.Floor(xDif / dist));
            int yDir = (int)(yDif > 0 ? MathF.Ceiling(yDif / dist) : MathF.Floor(yDif / dist));

            if (dist > 1)
            {
                SetSnakePos(i, currentXPos + xDir, currentYPos + yDir);
                // SetSnakePos(i, oldParentXPos, oldParentYPos);

                if(i+1 < snake.Count)
                    MoveTailTowardParent(i + 1);
            }

        }

    }

    internal class Program
    {
        static void Main(string[] args)
        {
            RopeBridge bridge = new RopeBridge();

            List<string> instructions = File.ReadAllLines("input-test.txt").ToList();
            foreach (string instruction in instructions)
            {
                string[] parts = instruction.Split(" ");
                char dir = parts[0][0];
                int count = int.Parse(parts[1]);
                for (int i = 0; i < count; i++)
                {
                    if (dir == 'U') bridge.MoveHead(0, -1);
                    if (dir == 'D') bridge.MoveHead(0, 1);
                    if (dir == 'L') bridge.MoveHead(-1, 0);
                    if (dir == 'R') bridge.MoveHead(1, 0);
                }

            }

            bridge.Print();

            Console.WriteLine("Complete - Press enter to continue");
            Console.ReadLine();
            Console.Clear();

            bridge.Print();

            while (true)
            {
                if(Console.KeyAvailable)
                {
                    Console.Clear();

                    var key = Console.ReadKey(true);
                    if (key.KeyChar == 'w' || key.KeyChar == 'W') bridge.MoveHead( 0,-1);
                    if (key.KeyChar == 's' || key.KeyChar == 'S') bridge.MoveHead( 0, 1);
                    if (key.KeyChar == 'a' || key.KeyChar == 'A') bridge.MoveHead(-1, 0);
                    if (key.KeyChar == 'd' || key.KeyChar == 'D') bridge.MoveHead( 1, 0);
                    
                    bridge.Print();
                }
            }
            
        }
    }
}