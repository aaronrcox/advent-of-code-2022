using Raylib_cs;

namespace monkey_map
{

    public enum ETileType
    {
        EMPTY,
        WALKABLE,
        WALL
    }

    public record Coord
    {
        public Coord() { }
        public Coord(int x, int y) {  X  = x; Y = y; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
    }

    public class Map
    {
        public int Rows { get; set; }
        public int Cols { get; set; }

        public int TileSize { get; set; } = 12;
        public int HalfTileSize { get { return TileSize / 2; } }

        public Coord StartPos { get; set; } = new Coord();

        private Dictionary<Coord, ETileType> _map = new Dictionary<Coord, ETileType>();
        private Coord _getTileCoordCache = new Coord();

        public ETileType GetTile(int row, int col)
        {
            _getTileCoordCache.X = col;
            _getTileCoordCache.Y = row;
            if (_map.TryGetValue(_getTileCoordCache, out ETileType tile))
                return tile;
            return ETileType.EMPTY;
        }

        public void SetTile(ETileType val, int row, int col)
        {
            Coord coord = new Coord(col, row);
            _map[coord] = val;
        }

        public void Draw()
        {
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Cols; x++)
                {
                    var tile = GetTile(y, x);
                    int xPos = x * TileSize;
                    int yPos = y * TileSize;

                    Color col = Color.WHITE;
                    if (tile == ETileType.WALKABLE) col = Color.LIGHTGRAY;
                    if (tile == ETileType.WALL) col = Color.DARKGRAY;

                    Raylib.DrawRectangle(xPos, yPos, TileSize, TileSize, col);
                }
            }
        }

        public static Map FromFileLines(List<string> lines)
        {
            Map map = new Map();
            map.Rows = lines.Count;
            map.Cols = lines.Select(z => z.Length).Max();

            map.StartPos.X = lines[0].IndexOf('.');

            for(int y=0; y<lines.Count; y++)
            {
                string line = lines[y];
                for(int x=0; x<line.Length; x++)
                {
                    char val = line[x];
                    if (val == '.') map.SetTile(ETileType.WALKABLE, y, x);
                    if (val == '#') map.SetTile(ETileType.WALL, y, x);
                }
            }
            return map;
        }
    }

    public class Program
    {
        Camera2D camera = new Camera2D(new System.Numerics.Vector2(620, 360), new System.Numerics.Vector2(0, 0), 0, 1);

        Map map;

        Coord playerPos = new Coord(0, 0);
        Coord playerDir = new Coord(1, 0);

        string instructions = string.Empty;
        int instructionIndex = 0;

        int FacingVal
        {
            get
            {
                if (playerDir.X > 0 && playerDir.Y == 0) return 0;
                if (playerDir.X == 0 && playerDir.Y > 0) return 1;
                if (playerDir.X < 0 && playerDir.Y == 0) return 2;
                if (playerDir.X == 0 && playerDir.Y < 0) return 3;
                return 0;
            }
        }

        int PassCode 
        { 
            get
            {
                int y = playerPos.Y + 1;
                int x = playerPos.X + 1;
                return 1000 * y + 4 * x + FacingVal;
            } 
        }

        void Run()
        {
            

            bool runTest = false;
            List<string> lines = File.ReadAllLines(runTest ? "input-test.txt" : "input.txt").ToList();
            List<string> mapLines = lines.TakeWhile(line => !string.IsNullOrWhiteSpace(line)).ToList();
            instructions = lines.Last();

            map = Map.FromFileLines(mapLines);
            playerPos = map.StartPos;

            Raylib.InitWindow(1280, 720, "monkey-map");
            Raylib.SetTargetFPS(60);

            while(!Raylib.WindowShouldClose())
            {
                float dt = Raylib.GetFrameTime();

                camera.target.X = playerPos.X * map.TileSize;
                camera.target.Y = playerPos.Y * map.TileSize;

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT)) RotatePlayerLeft();
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT)) RotatePlayerRight();
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) MovePlayer(playerDir.X, playerDir.Y);
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) MovePlayer(-playerDir.X, -playerDir.Y);

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) || true) RunNextInstruction();

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.RAYWHITE);

                Raylib.DrawText(PassCode.ToString(), 10, 10, 22, Color.BLACK);

                Raylib.BeginMode2D(camera);
                
                map.Draw();
                DrawPlayer();
                

                Raylib.EndMode2D();
                Raylib.EndDrawing();
            }
        }

        void RunNextInstruction()
        {
            if (instructionIndex >= instructions.Length)
                return;

            char c = instructions[instructionIndex];
            if ( c == 'R' )
            {
                RotatePlayerRight();
                instructionIndex++;
            }
            else if( c == 'L' )
            {
                RotatePlayerLeft();
                instructionIndex++;
            }
            else
            {
                string ins = new string(instructions.Substring(instructionIndex).TakeWhile(z => !(z == 'R' || z == 'L')).ToArray());
                int count = int.Parse(ins);
                for(int i=0; i<count; i++)
                    MovePlayer(playerDir.X, playerDir.Y);
                instructionIndex += ins.Length;
            }
        }
        

        public void MovePlayer(int x, int y)
        {
            int xPos = playerPos.X;
            int yPos = playerPos.Y;

            int lastWalkableX = xPos;
            int lastWalkableY = yPos;

            do
            {
                ETileType tile = map.GetTile(yPos + y, xPos + x);
                if (tile == ETileType.WALKABLE)
                {
                    xPos = xPos + x; 
                    yPos = yPos + y;
                    if (xPos >= map.Cols) xPos = 0;
                    else if (yPos >= map.Rows) yPos = 0;
                    else if (xPos < 0) xPos = map.Cols - 1;
                    else if (yPos < 0) yPos = map.Rows - 1;
                    break;
                }
                if(tile == ETileType.EMPTY)
                {
                    xPos += x;
                    yPos += y;
                    if (xPos >= map.Cols) xPos = -1;
                    else if (yPos >= map.Rows) yPos = -1;
                    else if (xPos < 0) xPos = map.Cols;
                    else if (yPos < 0) yPos = map.Rows;
                }
                if(tile == ETileType.WALL)
                {
                    xPos = lastWalkableX;
                    yPos = lastWalkableY;
                    break;
                }
            
            } while (true);
            

            playerPos.X = xPos;
            playerPos.Y = yPos;
        }

        public void RotatePlayerLeft()
        {
            int x = playerDir.X;
            playerDir.X = playerDir.Y;
            playerDir.Y = -x;
        }

        public void RotatePlayerRight()
        {
            int x = playerDir.X;
            playerDir.X = -playerDir.Y;
            playerDir.Y = x;
        }

        public void DrawPlayer()
        {
            Raylib.DrawRectangle(playerPos.X * map.TileSize + 2, playerPos.Y * map.TileSize + 2, map.TileSize - 4, map.TileSize - 4, Color.RED);
            Raylib.DrawCircle(playerPos.X * map.TileSize + (map.HalfTileSize) + (playerDir.X * (map.HalfTileSize - 2)),
                              playerPos.Y * map.TileSize + (map.HalfTileSize) + (playerDir.Y * (map.HalfTileSize - 2)), 2, Color.BLACK);

        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }
    }
}