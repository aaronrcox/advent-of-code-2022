namespace treetop_tree_house
{
    internal class Program
    {

        static List<int> GetValues(int rs, int re, int cs, int ce, List<List<int>> grid, Func<int, bool>? canContinueFunc = null)
        {
            List<int> values = new List<int>();
            GridForEach(rs, re, cs, ce, grid, val =>
            {
                if(canContinueFunc != null)
                {
                    if(canContinueFunc(val))
                    {
                        values.Add(val);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                        

                }
                else
                {
                    values.Add(val);
                    return true;
                }
            });
            
            return values;
        }

        static void GridForEach(int rs, int re, int cs, int ce, List<List<int>> grid, Func<int, bool> forEachFunc)
        {
            for (int y = rs; y <= re; y++)
            {
                for (int x = cs; x <= ce; x++)
                {
                    if (forEachFunc(grid[y][x]) == false)
                        return;
                }
            }
        }

        static void Main(string[] args)
        {
            List<List<int>> data = File.ReadAllLines("input-test.txt").Select(line => line.ToCharArray().Select(z => z - '0').ToList()).ToList();

            int rows = data.Count;
            int cols = data[0].Count;

            int outerVisibleCount = (rows * cols) - ((rows - 2) * (cols - 2));
            int innderVisibleCount = 0;
            for(int y=1; y<data.Count-1; y++)
            {
                for(int x=1; x < data[y].Count-1; x++)
                {
                    int height = data[y][x];
                    int leftTallest = GetValues(y, y, 0, x - 1, data).Max();
                    int rightTallest = GetValues(y, y, x + 1, cols-1, data).Max();
                    int upTallest = GetValues(0, y - 1, x, x, data).Max();
                    int downTallest = GetValues(y+1, rows-1, x, x, data).Max();

                    var isVisible = (int val) => val < height;
                    int upVisibleCount = GetValues(0, y - 1, x, x, data, isVisible).Count();
                    int leftVisibleCount = GetValues(y, y, 0, x - 1, data, isVisible).Count();
                    int rightVisibleCount = GetValues(y, y, x + 1, cols - 1, data, isVisible).Count();
                    int downVisibleCount = GetValues(y + 1, rows - 1, x, x, data, isVisible).Count();
                    int visibleScore = leftVisibleCount * rightVisibleCount * upVisibleCount * downVisibleCount;


                    if (leftTallest < height || rightTallest < height || upTallest < height || downTallest < height)
                    {
                        innderVisibleCount++;
                    }



                }
            }

            Console.WriteLine(outerVisibleCount + innderVisibleCount);

        }
    }
}