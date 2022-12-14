
using Raylib_cs;
using System.Security.Principal;

namespace regolith_reservoir
{
    record Vec2i
    {
        public int x;
        public int y;

        public Vec2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vec2i(string coord)
        {
            string[] parts = coord.Split(",");
            x = int.Parse(parts[0]);
            y = int.Parse(parts[1]);
        }

    }

    class Program
    {
        int minX = 0;
        int maxX = 0;
        int minY = 0;
        int maxY = 0;
        int width = 0;
        int height = 0;
        // char[,] grid = new char[0,0];
        Dictionary<Vec2i, char> tiles = new();

        Vec2i? currentSandTile = null;
        int sandAtRestCount = 0;
        bool done = false;

        int tileSize = 4;

        void Run()
        {

            LoadFileData();

            Raylib.InitWindow(width * tileSize, height * tileSize, "Regolith Resevoir");
            Raylib.SetTargetFPS(60);

            while(!Raylib.WindowShouldClose())
            {
                //if(Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                for(int i=0; i<50; i++)
                    UpdateFrame();

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.RAYWHITE);
                DrawGrid();
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        void LoadFileData()
        {
            bool test = false;
            string filename = test ? "input-test.txt" : "input.txt";
            List<List<Vec2i>> rockPaths = File.ReadAllLines(filename).Select(line => line.Split(" -> ").Select(coord => new Vec2i(coord)).ToList()).ToList();
            if (filename == "input-test.txt") tileSize = 20;
            if (filename == "input.txt") tileSize = 5;


            minX = rockPaths.SelectMany(z => z).Min(coord => coord.x) - 1;
            maxX = rockPaths.SelectMany(z => z).Max(coord => coord.x) + 2;
            minY = rockPaths.SelectMany(z => z).Min(coord => coord.y) - 1;
            maxY = rockPaths.SelectMany(z => z).Max(coord => coord.y) + 3;
            minY = Math.Min(0, minY);
            minX = Math.Min(500, minX);
            maxX = Math.Max(500, maxX);

            int sandPos = 500 - minX;

            width = maxX - minX;
            height = maxY - minY;
            // grid = new char[height, width];

            //for (int y = 0; y < height; y++)
            //    for (int x = 0; x < width; x++)
            //        grid[y, x] = '.';

            AddTile('+', sandPos, 0);

            for (int i = 0; i < rockPaths.Count; i++)
            {
                Vec2i startPos = rockPaths[i][0];
                startPos.x -= minX;
                startPos.y -= minY;
                for (int j = 1; j < rockPaths[i].Count; j++)
                {
                    Vec2i endPos = rockPaths[i][j];
                    endPos.x -= minX;
                    endPos.y -= minY;

                    int xDir = startPos.x == endPos.x ? 0 : startPos.x > endPos.x ? -1 : 1;
                    int yDir = startPos.y == endPos.y ? 0 : startPos.y > endPos.y ? -1 : 1;

                    for (int x = startPos.x; x <= endPos.x; x++)
                        AddTile('#', x, startPos.y); // grid[startPos.y, x] = '#';

                    for (int x = startPos.x; x >= endPos.x; x--)
                        AddTile('#', x, startPos.y); //grid[startPos.y, x] = '#';

                    for (int y = startPos.y; y <= endPos.y; y++)
                        AddTile('#', startPos.x, y);//grid[y, startPos.x] = '#';

                    for (int y = startPos.y; y >= endPos.y; y--)
                        AddTile('#', startPos.x, y); //grid[y, startPos.x] = '#';

                    startPos = endPos;

                }
            }
        }

        void AddTile(char t, int x, int y)
        {
            // grid[y, x] = t;
            tiles[new Vec2i(x, y)] = t;
        }

        Vec2i getTileVecCache = new Vec2i(0, 0);
        char GetTile(int x, int y)
        {
            getTileVecCache.x = x;
            getTileVecCache.y = y;
            return tiles.GetValueOrDefault(getTileVecCache, '.');
        }

        void UpdateFrame()
        {
            if (done)
                return;

            if (currentSandTile == null)
            {
                currentSandTile = new Vec2i(500 - minX, 0);
                return;
            }

            if (OutOfBounds(currentSandTile))
            {
                done = true;
            }
            if (CanSandMove(currentSandTile, 0, 1))
            {
                currentSandTile.y += 1;
            }
            else if (CanSandMove(currentSandTile, -1, 1))
            {
                currentSandTile.x -= 1;
                currentSandTile.y += 1;
            }
            else if (CanSandMove(currentSandTile, 1, 1))
            {
                currentSandTile.x += 1;
                currentSandTile.y += 1;
            }
            else
            {
                if (currentSandTile.x == 500 - minX && currentSandTile.y == 0)
                    done = true;

                AddTile('O', currentSandTile.x, currentSandTile.y);
                currentSandTile = null;
                sandAtRestCount += 1;
            }

        }

        bool OutOfBounds(Vec2i pos, int xd = 0, int yd = 0)
        {
            // if (pos.x + xd < 0) return true;
            // if (pos.x + xd >= width) return true;
            if (pos.y + yd < 0) return true;
            if (pos.y + yd >= height) return true;

            return false;
        }

        bool CanSandMove(Vec2i pos, int xd, int yd)
        {
            if (OutOfBounds(pos, xd, yd))
                return true;

            if (pos.y + yd >= height - 1)
                return false;

            char val = GetTile(pos.x + xd, pos.y + yd); // grid[pos.y + yd, pos.x + xd];



            return !(val == '#' || val == 'O');
        }

        void DrawGrid()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color col = Color.WHITE;
                    if (GetTile(x, y) == '#' || y == height-1) col = Color.BROWN;
                    else if (GetTile(x, y) == '.') col = Color.SKYBLUE;
                    else if (GetTile(x, y) == 'O') col = Color.YELLOW;
                    else if (GetTile(x, y) == '+') col = Color.BEIGE;

                    Raylib.DrawRectangle(x * tileSize, y * tileSize, tileSize, tileSize, col);
                }       
            }

            if(currentSandTile != null)
            {
                Raylib.DrawRectangle(currentSandTile.x * tileSize, currentSandTile.y * tileSize, tileSize, tileSize, Color.ORANGE);
            }

            if(done)
                Raylib.DrawText(sandAtRestCount.ToString(), 10, 10, 12, Color.BLACK);
        }

        static void Main(string[] args)
        {
            
            Program p = new Program();
            p.Run();
        }
    }
}