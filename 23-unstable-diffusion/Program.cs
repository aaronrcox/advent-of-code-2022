using Raylib_cs;

namespace unstable_diffusion
{
    public record Coord
    {
        public Coord() { }
        public Coord(int x, int y) { X = x; Y = y; }
        public Coord(Coord c) { X = c.X; Y = c.Y; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Elfe
    {
        public Coord Position { get; set; } = new();
        public Coord? ProposedPosition { get; set; } = null;
    }

    public enum EDirection
    {
        NORTH,
        SOUTH,
        WEST,
        EAST
    }

    public class ElfeGroup
    {
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }

        public List<Elfe> ElfeList { get; set; } = new List<Elfe>();
        public Dictionary<Coord, Elfe> ElfePositions { get; set; } = new();
        public Dictionary<Coord, int> ElfeProposals { get; set; } = new();

        public int RoundCount { get; set; } = 0;
        public int MovedElvesCount { get; set; } = -1;

        public void AddElfe(Coord pos)
        {
            Elfe elfe = new Elfe();
            ElfeList.Add(elfe);
            elfe.Position = pos;
            ElfePositions[pos] = elfe;
        }

        public void ProposeMovements()
        {

            Func<Elfe, bool>[] proposalFuncs = new Func<Elfe, bool>[4]
            {
                (Elfe elfe) => ProposeDirection(elfe, EDirection.NORTH),
                (Elfe elfe) => ProposeDirection(elfe, EDirection.SOUTH),
                (Elfe elfe) => ProposeDirection(elfe, EDirection.WEST),
                (Elfe elfe) => ProposeDirection(elfe, EDirection.EAST)
            };
            
            foreach (var elfe in ElfeList)
            {
                bool hasAdjecentElves = false;
                for (int y = -1; y < 2; y++)
                    for (int x = -1; x < 2; x++)
                        if (!(x == 0 && y == 0))
                            hasAdjecentElves = hasAdjecentElves | IsElfeAtPos(elfe.Position, x, y);

                if (hasAdjecentElves == false)
                    continue;

                for (int i=0; i<4; i++)
                    if (proposalFuncs[(i + (RoundCount % 4)) % 4](elfe))
                        break;
            }

            ElfeProposals = ElfeList
                .Where(z => z.ProposedPosition != null)
                .Select(z => z.ProposedPosition!)
                .GroupBy(z => z).ToDictionary(z => z.First(), z => z.Count());
        }

        public void DoMovements()
        {
            MovedElvesCount = 0;

            foreach (var elfe in ElfeList)
            {
                if (elfe.ProposedPosition != null && ElfeProposals[elfe.ProposedPosition] <= 1)
                {
                    elfe.Position = elfe.ProposedPosition!;
                    MovedElvesCount++;
                }
                    

                elfe.ProposedPosition = null;
            }

            ElfePositions = ElfeList.ToDictionary(elfe => elfe.Position, elfe => elfe);
            UpdateArea();

            RoundCount++;
        }


        Coord _isElfeAtPosLookupCache = new();
        private bool IsElfeAtPos(Coord pos, int xo, int yo)
        {
            _isElfeAtPosLookupCache.X = pos.X + xo;
            _isElfeAtPosLookupCache.Y = pos.Y + yo;

            return ElfePositions.ContainsKey(_isElfeAtPosLookupCache);
        }

        public bool ProposeDirection(Elfe elfe, EDirection dir)
        {
            switch(dir)
            {
                case EDirection.NORTH:
                    
                    if (!IsElfeAtPos(elfe.Position, 0, -1) && !IsElfeAtPos(elfe.Position, -1, -1) && !IsElfeAtPos(elfe.Position, 1, -1))
                    {
                        elfe.ProposedPosition = new(elfe.Position.X, elfe.Position.Y - 1);
                        return true;
                    }
                    return false;

                case EDirection.SOUTH:

                    if (!IsElfeAtPos(elfe.Position, 0, 1) && !IsElfeAtPos(elfe.Position, -1, 1) && !IsElfeAtPos(elfe.Position, 1, 1))
                    {
                        elfe.ProposedPosition = new(elfe.Position.X, elfe.Position.Y + 1);
                        return true;
                    }
                    return false;
                 
                case EDirection.WEST:

                    if (!IsElfeAtPos(elfe.Position, -1, 0) && !IsElfeAtPos(elfe.Position, -1, -1) && !IsElfeAtPos(elfe.Position, -1, 1))
                    {
                        elfe.ProposedPosition = new(elfe.Position.X - 1, elfe.Position.Y);
                        return true;
                    }
                        
                    return false;

                case EDirection.EAST:

                    if (!IsElfeAtPos(elfe.Position, 1, 0) && !IsElfeAtPos(elfe.Position, 1, -1) && !IsElfeAtPos(elfe.Position, 1, 1))
                    {
                        elfe.ProposedPosition = new(elfe.Position.X + 1, elfe.Position.Y);
                        return true;
                    }
                    return false;
            }

            throw new Exception("Should never get here");   
        }

        public void UpdateArea()
        {
            MinX = int.MaxValue; MaxX = int.MinValue;
            MinY = int.MaxValue; MaxY = int.MinValue;

            foreach (var elfe in ElfeList)
            {
                MinX = Math.Min(elfe.Position.X, MinX);
                MaxX = Math.Max(elfe.Position.X+1, MaxX);
                MinY = Math.Min(elfe.Position.Y, MinY);
                MaxY = Math.Max(elfe.Position.Y+1, MaxY);
            }
        }

        public void Draw(int ts)
        {
            int hs = ts / 2;

            foreach (var elfe in ElfeList)
            {
                Raylib.DrawRectangle(elfe.Position.X * ts - hs, elfe.Position.Y * ts - hs, ts, ts, Color.RED);
                if(elfe.ProposedPosition != null)
                {
                    Raylib.DrawCircle(elfe.ProposedPosition.X * ts, elfe.ProposedPosition.Y * ts, 3, Color.DARKBLUE);
                }
            }

            Raylib.DrawRectangleLinesEx(new(MinX * ts - hs, MinY * ts - hs, (MaxX - MinX) * ts, (MaxY - MinY) * ts), 2, Color.GRAY);
        }
    }

    internal class Program
    {
        void Run()
        {
            bool test = false;
            List<string> lines = File.ReadAllLines(test ? "input-test.txt" : "input.txt").ToList();

            ElfeGroup group = new();
            for(int y=0; y<lines.Count; y++)
            {
                for(int x=0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == '#')
                        group.AddElfe(new(x, y));
                }
            }
            group.UpdateArea();

            int windowWidth = 1280;
            int windowHeight = 720;
            Raylib.InitWindow(windowWidth, windowHeight, "unstable difusion");

            Camera2D camera = new Camera2D(new(windowWidth/2, windowHeight/2), new(), 0, 1);

            int turnCount = 0;
            float cxo = 0;
            float cyo = 0;

            while(!Raylib.WindowShouldClose())
            {
                if(Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) || group.MovedElvesCount != 0)
                {
                    // if (turnCount % 2 == 0) group.ProposeMovements();
                    // else group.DoMovements();

                    group.ProposeMovements();
                    turnCount += 1;
                    group.DoMovements();
                    turnCount += 1;
                }

                float dt = Raylib.GetFrameTime();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) cxo -= 100 * dt;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) cxo += 100 * dt;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) cyo -= 100 * dt;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) cyo += 100 * dt;

                int numX = group.MaxX - group.MinX;
                int numY = group.MaxY - group.MinY;

                int w = ((numX)) * 12;
                int h = ((numY)) * 12;

                camera.offset.X = (windowWidth / 2);
                camera.offset.Y = (windowHeight / 2);
                camera.target.X = w / 2;
                camera.target.Y = h / 2;
                float wz = (float)windowWidth / (float)(w);
                float hz = (float)windowHeight / (float)(h);
                float z = Math.Min(wz, hz);
                camera.zoom = z;

                camera.offset.X += cxo;
                camera.offset.Y += cyo;

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.RAYWHITE);

                Raylib.BeginMode2D(camera);
                group.Draw(12);
                Raylib.EndMode2D();

                Raylib.DrawText(group.RoundCount.ToString(), 10, 10, 22, Color.GRAY);
                Raylib.DrawText(((numX * numY) - group.ElfeList.Count).ToString(), 10, 42, 22, Color.GRAY);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
        

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }
    }
}