using System.Transactions;

namespace beacon_exclusion_zone
{
    record Vec2i
    {
        public int x;
        public int y;

        public Vec2i()
        {

        }

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

        public static int ManhattanDistance(Vec2i a, Vec2i b)
        {
            return Math.Abs(b.x - a.x) + Math.Abs(b.y - a.y);
        }
    }

    class Sensor
    {
        public Vec2i Pos { get; set; } = new();
        public Vec2i BeaconPos { get; set; } = new();

        public float DistToBeacon { get; set; }

    }

    class Program
    {
        bool test = false;
        int readRow;
        int maxAreaSize;

        Dictionary<int, Dictionary<int, char>> map = new();
        Vec2i areaMin = new(int.MaxValue, int.MaxValue);
        Vec2i areaMax = new(int.MinValue, int.MinValue);

        void Run()
        {
            readRow = test ? 10 : 2000000;
            maxAreaSize = test ? 20 : 4000000;

            List<Sensor> sensors = File.ReadAllLines(test ? "input-test.txt" : "input.txt")
                .Select(line => line.Replace("Sensor at ", "").Replace(" closest beacon is at ", "").Replace("x=", "").Replace("y=", "").Replace(", ", ","))
                .Select(line => line.Split(":").Select(z => z.Trim()).ToList())
                .Select(coords => new Sensor()
                {
                    Pos = new Vec2i(coords[0]),
                    BeaconPos = new Vec2i(coords[1]),
                    DistToBeacon = Vec2i.ManhattanDistance(new Vec2i(coords[1]), new Vec2i(coords[0]))
                }).ToList();

            //foreach (var sensor in sensors)
            //{
            //    AddToMap('S', sensor.Pos.x, sensor.Pos.y);
            //    AddToMap('B', sensor.BeaconPos.x, sensor.BeaconPos.y);

            //    int dist = Vec2i.ManhattanDistance(sensor.Pos, sensor.BeaconPos);

            //    {
            //        for (int i = 0; i <= dist; i++)
            //        {
            //            FillMapAreaP1('#', sensor.Pos.x - (dist - i), sensor.Pos.y - i, sensor.Pos.x + (dist - i), sensor.Pos.y - i);
            //            FillMapAreaP1('#', sensor.Pos.x - (dist - i), sensor.Pos.y + i, sensor.Pos.x + (dist - i), sensor.Pos.y + i);
            //        }
            //    }


            //}

            //PrintMap();
            //Console.WriteLine("-----------------");

            //int count = 0;
            //for (int x = areaMin.x; x <= areaMax.x; x++)
            //{
            //    char c = GetMapValue(x, readRow);
            //    if (c == '#') count += 1;
            //    if (c == 'S') count += 1;
            //}

            //Console.WriteLine();
            //Console.WriteLine("Part 1: " + count);
            //Console.WriteLine("-----------------");

            

            int rowsProcessed = 0;
            long result = 0;

            //Campy.Parallel.For(maxAreaSize, (y) =>
            //{
            //    for (int x = 0; x < maxAreaSize; x++)
            //    {
            //        var visibleSensor = sensors.FirstOrDefault(s =>
            //        {
            //            int dist = Math.Abs(x - s.Pos.x) + Math.Abs(y - s.Pos.y);
            //            return dist <= s.DistToBeacon;
            //        });

            //        if (visibleSensor != null)
            //        {
            //            if (x < visibleSensor.Pos.x)
            //            {
            //                int xDist = Math.Abs(x - visibleSensor.Pos.x);
            //                x += (xDist * 2) - 1;
            //            }
            //        }
            //        else
            //        {
            //            result = x * maxAreaSize + y;
            //            Console.WriteLine($"x:{x} y:{y}: f:{result}");
            //        }
            //    };

            //    rowsProcessed++;
            //    if (rowsProcessed % 1000 == 0)
            //    {
            //        Console.WriteLine("Checked: " + (rowsProcessed / (float)maxAreaSize) * 100 + "%");
            //    }
            //});

            Console.WriteLine("Part2 Complete: " + result);
            
            

            Parallel.For(0, maxAreaSize, (i, loop) =>
            {
                bool shouldBreak = false;

                if (shouldBreak)
                    loop.Stop();

                for (int x = i; x < maxAreaSize; x++)
                {
                    Sensor? visibleSensor = null;
                    for(int j=0; j<sensors.Count; j++)
                    {
                        int xDif = x - sensors[j].Pos.x;
                        int yDif = j - sensors[j].Pos.y;
                        int dist = (xDif >= 0 ? xDif : -xDif) + (yDif >= 0 ? yDif : -yDif);
                        if(dist <= sensors[j].DistToBeacon)
                        {
                            visibleSensor = sensors[j];
                            break;
                        }
                    }
                    
                    if (visibleSensor != null)
                    {
                        if (x < visibleSensor.Pos.x)
                        {
                            int xDist = Math.Abs(x - visibleSensor.Pos.x);
                            x += (xDist * 2) - 1;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"x:{x} y:{i}: f:{x * maxAreaSize + i}");
                        shouldBreak = true;
                        loop.Stop();
                        break;
                    }
                };

                for (int y = i; y < maxAreaSize; y++)
                {

                    Sensor? visibleSensor = null;
                    for (int j = 0; j < sensors.Count; j++)
                    {
                        int xDif = i - sensors[j].Pos.x;
                        int yDif = y - sensors[j].Pos.y;
                        int dist = (xDif >= 0 ? xDif : -xDif) + (yDif >= 0 ? yDif : -yDif);
                        if (dist <= sensors[j].DistToBeacon)
                        {
                            visibleSensor = sensors[j];
                            break;
                        }
                    }


                    if (visibleSensor != null)
                    {
                        if (y < visibleSensor.Pos.y)
                        {
                            int yDist = Math.Abs(y - visibleSensor.Pos.y);
                            y += (yDist * 2) - 1;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"x:{i} y:{y}: f:{i * maxAreaSize + y}");
                        shouldBreak = true;
                        loop.Stop();
                        break;
                        
                    }
                };


                rowsProcessed++;
                if (rowsProcessed % 100 == 0)
                {
                    Console.WriteLine("Checked: " + (rowsProcessed / (float)maxAreaSize) * 100 + "%");
                }


            }); ;



            Console.WriteLine("Complete");
            Console.ReadLine();
        }

        

        void PrintMap()
        {
            if (test == false)
                return;

            int xMin = Math.Max(areaMin.x, areaMin.x);
            int yMin = Math.Max(areaMin.y, areaMin.y);
            int xMax = Math.Min(areaMax.x, areaMax.x);
            int yMax = Math.Min(areaMax.y, areaMax.y);

            for(int y=yMin; y<=yMax; y++)
            {
                for(int x=xMin; x<=xMax; x++)
                {
                    Console.Write(GetMapValue(x, y));
                }
                Console.WriteLine();
            }
        }

        void FillMapAreaP1(char c, int sx, int sy, int ex, int ey)
        {
            if (test || (sy == readRow || ey == readRow))
            {
                for (int y = sy; y <= ey; y++)
                    for (int x = sx; x <= ex; x++)
                        AddToMap(c, x, y, false);
            }
            
        }

        void AddToMap(char c, int x, int y, bool overrideExisting = true)
        {
            if (!map.TryGetValue(x, out var row))
            {
                row = new Dictionary<int, char>();
                map.Add(x, row);
            }

            if (overrideExisting)
            {
                row[y] = c;
            }
            else
            {
                if (row.ContainsKey(y) == false)
                    row[y] = c;
            }

            areaMin.x = Math.Min(areaMin.x, x);
            areaMin.y = Math.Min(areaMin.y, y);
            areaMax.x = Math.Max(areaMax.x, x);
            areaMax.y = Math.Max(areaMax.y, y);
        }

        char GetMapValue(int x, int y)
        {
            if(map.TryGetValue(x, out var row))
            {
                if (row.TryGetValue(y, out char value))
                    return value;
            }

            return '.';
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();

            

        }
    }
}

