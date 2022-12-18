using Raylib_cs;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace pyroclastic_flow
{

    interface IGrid
    {
        long MaxYPos { get; }
        void AddRock(int x, long y);
        bool HasRock(int x, long y);

    }

    public class Grid1 : IGrid
    {
        public long MaxYPos { get; private set; } = 0;
        Dictionary<long, char[]> rocks = new();

        private long lastRowCleared = 0;

        public void AddRock(int x, long y)
        {
            MaxYPos = Math.Max(MaxYPos, y);

            if (!rocks.TryGetValue(y, out var row))
            {
                row = new char[7];
                rocks.Add(y, row);
            }

            row[x] = '#';

            if (IsRowFull(y))
            {
                // Console.WriteLine($"Clearing rows: {lowestRockPos} to {y}");
                for (long rowIndex = lastRowCleared; rowIndex < y; rowIndex++)
                    rocks.Remove(rowIndex);
                lastRowCleared = y;
            }
        }

        public bool HasRock(int x, long y)
        {
            if (y > MaxYPos || x < 0 || x >= 7)
                return false;

            return rocks[y][x] == '#';
        }

        public bool IsRowFull(long y)
        {
            int count = 0;
            if (rocks.TryGetValue(y, out var row))
            {
                for (int i = 0; i < 7; i++)
                {
                    if (row[i] == '#')
                        count++;
                }
            }

            return count == 7;
        }
    }

    public class Grid2 : IGrid
    {
        public long MaxYPos { get; private set; } = 0;

        Dictionary<long, int> rocks = new();

        private long lastRowCleared = 0;

        public void AddRock(int x, long y)
        {
            MaxYPos = Math.Max(MaxYPos, y);

            rocks.TryGetValue(y, out var row);
            rocks[y] = row | (1 << x);

            if (IsRowFull(y))
            {
                // Console.WriteLine($"Clearing rows: {lastRowCleared} to {y}");
                for (long rowIndex = lastRowCleared; rowIndex < y; rowIndex++)
                    rocks.Remove(rowIndex);

                lastRowCleared = y;
            }
        }

        public bool HasRock(int x, long y)
        {
            if (y > MaxYPos || x < 0 || x >= 7)
                return false;
            
            return ((rocks[y] >> x) & 1) == 1;
        }

        public bool IsRowFull(long y)
        {
            if (rocks.TryGetValue(y, out var row))
            {
                return row == 127;
            }

            return false;
        }
    }

    public class Grid3 : IGrid
    {
        public long MaxYPos { get; private set; } = 0;

        const int NUM_ROWS = 134217728;
        const int NUM_ROWS_MOD = 134217727;
        int[] rocks = new int[NUM_ROWS];

        private long lastRowCleared = 0;

        public void AddRock(int x, long y)
        {

            MaxYPos = Math.Max(MaxYPos, y);

            int yIndex = (int)(y & NUM_ROWS_MOD);

            rocks[yIndex] = rocks[yIndex] | (1 << x);

            if (rocks[yIndex] == 127)
            {
                //Console.WriteLine($"Clearing rows: {lastRowCleared} to {y}");
                for (long rowIndex = lastRowCleared; rowIndex < y; rowIndex++)
                    rocks[rowIndex % NUM_ROWS] = 0;

                lastRowCleared = y;
            }
        }

        public bool HasRock(int x, long y)
        {
            //if (y > MaxYPos)
            //    return false;

            return ((rocks[y & NUM_ROWS_MOD] >> x) & 1) == 1;
        }

        public bool IsRowFull(long y)
        {
            return rocks[y & NUM_ROWS_MOD] == 127;
        }
    }

    class Vec2i
    {
        public Vec2i() { }
        public Vec2i(int x, long y) { this.x = x; this.y = y; }

        public int x = 0;
        public long y = 0;
    }

    internal class Program
    {
        Color backgroundColor = new Color(0x41, 0x35, 0x2E, 0xFF);
        // Color caveBackgroundColor = new Color(0x96, 0x7b, 0x6a, 0xFF);
        Color caveBackgroundColor = new Color(0x6C, 0x58, 0x4C, 0xFF);
        Color fallingRockColor = new Color(0xc1, 0x9d, 0x88, 0xFF);
        Color solidRockColor = new Color(0x96, 0x7b, 0x6a, 0xFF);

        public int windowWidth = 480;
        public int windowHeight = 640;

        IGrid rocks = new Grid3();


        int nextRockIndex = 0;
        List<Vec2i> fallingRocks = new(5){ new(), new(), new(), new(), new()};
        int numTIlesInFallingRocks = 0;

        long fallingRockCount = 0;


        float stepCooldownReset = 0.001f;
        float nextStepCooldown = 0.1f;

        long stepCount = 0;

        int nextInstruction = 0;
        List<char> instructions = new();

        float yOffset = 0;
        float targetYOffset = 0;

        long totalFalingBlocks = 2022;
        
        void Run()
        {
            Raylib.InitWindow(windowWidth, windowHeight, "Pyroclastic Flow");

            bool test = false;
            if (test == false) totalFalingBlocks = 1000000000000;
            else totalFalingBlocks = 2022;
            instructions = File.ReadAllText(test ? "input-test.txt" : "input.txt").ToCharArray().ToList();

            for (int i = 0; i < 7; i++)
                rocks.AddRock(i, 0);

            sw.Start();

            SpwanNextRock();


           while (fallingRockCount <= totalFalingBlocks)
           {
               UpdateStep();
           }
           
           Console.WriteLine("-----------------------------------");
           Console.WriteLine("Heighest block: " + (rocks.MaxYPos));
           Console.WriteLine("-----------------------------------");


            while (!Raylib.WindowShouldClose())
            {

                nextStepCooldown -= Raylib.GetFrameTime();
                if(nextStepCooldown < 0 && fallingRockCount <= totalFalingBlocks && Raylib.IsKeyDown(KeyboardKey.KEY_SPACE))
                {
                    nextStepCooldown = stepCooldownReset;
                    UpdateStep();
                }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT)) MoveRocks(-1, 0);
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT)) MoveRocks(1, 0);
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) MoveRocks(0, -1);

                if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) targetYOffset -= 1;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) targetYOffset += 1;


                // float yOffsetDir = (yOffset - targetYOffset) * stepCooldownReset;
                yOffset = targetYOffset;

                // Color.DARKBROWN
                Raylib.BeginDrawing();
                Raylib.ClearBackground(backgroundColor);
                DrawRocks();
               
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        void UpdateStep()
        {
            char instruction = instructions[nextInstruction];
            nextInstruction += 1;
            if (nextInstruction == instructions.Count)
                nextInstruction = 0;


            int dir = instruction == '<' ? -1 : 1;
            MoveRocks(dir, 0);
            MoveRocks(0, -1);

            //stepCount += 1;

            //if (stepCount % 2 == 0)
            //{
            //    MoveRocks(0, -1);
            //}
                
            //else
            //{
                
            //}
        }

        void DrawRocks()
        {
            int tileSize = 32;
            int xPos = (windowWidth / 2) - (tileSize * 7 / 2);

            Raylib.DrawRectangle(xPos, 0, tileSize * 7, windowHeight, caveBackgroundColor);

            // Draw the solid rocks
            for(long y=0; y<=rocks.MaxYPos; y++)
            {
                for(int x=0; x<7; x++)
                {
                    if(rocks.HasRock(x, y))
                    {
                        Raylib.DrawRectangle(xPos + x * tileSize, (int)(windowHeight - y * tileSize - tileSize) + (int)yOffset, tileSize, tileSize, solidRockColor);
                    }
                }
            }

            // Draw the falling rocks
            for(int i=0; i< numTIlesInFallingRocks; i++)
            {
                long xp = fallingRocks[i].x;
                long yp = fallingRocks[i].y;
                Raylib.DrawRectangle((int)(xPos + xp * tileSize), (int)((windowHeight - yp * tileSize - tileSize) + yOffset), tileSize, tileSize, fallingRockColor);
            }

            Color gridLinesColor = Color.WHITE;
            gridLinesColor.a = (int)(255.0f * 0.1f);
            for (int i=0; i<7; i++)
            {
                Raylib.DrawLine(xPos + i * tileSize, 0, xPos + i * tileSize, windowHeight, gridLinesColor);
            }

            for (int i = 0; i < windowHeight/tileSize; i++)
            {
                Raylib.DrawLine(xPos, windowHeight - tileSize - (i*tileSize), xPos + 7 * tileSize, windowHeight - tileSize - (i * tileSize), gridLinesColor);
            }

            Raylib.DrawText($"Height: {rocks.MaxYPos}", 10, 10, 14, solidRockColor);
            Raylib.DrawText($"Count: {fallingRockCount}", 10, 24, 14, solidRockColor);
        }

        Stopwatch sw = new Stopwatch();
        int progressStep = 0;
        void SpwanNextRock()
        {
            
            var setRockPos = (int i, int x, long y) => { fallingRocks[i].x = x; fallingRocks[i].y = y; };
            fallingRockCount += 1;
            progressStep += 1;

            if (progressStep == 10000000)
            {
                
                double complete = (fallingRockCount / (double)totalFalingBlocks)*100;
                double percentageLeft = 100 - complete;
                Console.WriteLine($"Complete: {complete.ToString("#.000")}\tBlocks: {fallingRockCount}\tTotal Height: {rocks.MaxYPos}\tTime:{sw.Elapsed.TotalSeconds}\t");
                progressStep = 0;

            }
                

            // fallingRocks.Clear();

            long yPos = rocks.MaxYPos + 4;

            if (nextRockIndex == 0)
            {
                numTIlesInFallingRocks = 4;
                setRockPos(0, 2, yPos);
                setRockPos(1, 3, yPos);
                setRockPos(2, 4, yPos);
                setRockPos(3, 5, yPos);
            }
            if (nextRockIndex == 1)
            {
                numTIlesInFallingRocks = 5;
                setRockPos(0, 3, yPos + 2);
                setRockPos(1, 4, yPos + 1);
                setRockPos(2, 2, yPos + 1);
                setRockPos(3, 3, yPos + 1);
                setRockPos(4, 3, yPos);
            }
            if (nextRockIndex == 2)
            {
                numTIlesInFallingRocks = 5;
                setRockPos(0, 4, yPos + 2);
                setRockPos(1, 4, yPos+1);
                setRockPos(2, 2, yPos);
                setRockPos(3, 3, yPos);
                setRockPos(4, 4, yPos);
            }
            if (nextRockIndex == 3)
            {
                numTIlesInFallingRocks = 4;
                setRockPos(0, 2, yPos + 3);
                setRockPos(1, 2, yPos+2);
                setRockPos(2, 2, yPos+1);
                setRockPos(3, 2, yPos);
            }
            if (nextRockIndex == 4)
            {
                numTIlesInFallingRocks = 4;
                setRockPos(0, 3, yPos + 1);
                setRockPos(1, 2, yPos + 1);
                setRockPos(2, 3, yPos);
                setRockPos(3, 2, yPos);
            }

            nextRockIndex++;
            nextRockIndex = nextRockIndex % 5;
        }

        void MoveRocks(int xd, int yd)
        {
            for (int i = 0; i < numTIlesInFallingRocks; i++)
            {
                int xp = fallingRocks[i].x;
                long yp = fallingRocks[i].y;
                int nextXp = xp + xd;
                long nextYp = yp + yd;

                if (nextXp < 0 || nextXp >= 7)
                    return;


                if(rocks.HasRock(nextXp, nextYp))
                {
                    
                    if (yd != 0)
                    {
                        for (int j = 0; j < numTIlesInFallingRocks; j++)
                        {
                            var rock = fallingRocks[j];
                            rocks.AddRock(rock.x, rock.y);
                        }
                        SpwanNextRock();
                        
                    }
                    return;
                }
            }

            
            
                for (int i = 0; i < numTIlesInFallingRocks; i++)
                {
                    var rock = fallingRocks[i];
                    rock.x += xd;
                    rock.y += yd;
                }
            
        }


        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }
    }
}