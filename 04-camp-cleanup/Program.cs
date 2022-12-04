namespace Camp_Cleanup
{

    class Range
    {
        int Start { get; set; } = 0;
        int End { get; set; } = 0;

        public static bool Contains(Range a, Range b)
        {
            return (a.Start <= b.Start && a.End >= b.End)  || (b.Start <= a.Start && b.End >= a.End);
        }

        public static bool Overlap(Range a, Range b)
        {
            int sMin = Math.Min(a.Start, b.Start);
            int eMin = Math.Min(a.End, b.End);

            int sMax = Math.Max(a.Start, b.Start);
            int eMax = Math.Max(a.End, b.End);

            return sMax <= eMin && eMin >= sMax;
        }

        public static Range FromString(string line)
        {
            List<int> values = line.Split('-').Select(z => int.Parse(z)).ToList();
            return new Range()
            {
                Start = values.First(),
                End = values.Last()
            };
        }
    }

    class RangePair
    {
        public Range First { get; set; } = new Range();
        public Range Second { get; set; } = new Range();

        public bool Contains()
        {
            return Range.Contains(First, Second);
        }

        public bool Overlap()
        {
            return Range.Overlap(First, Second);
        }

        public static RangePair FromString(string line)
        {
            List<Range> ranges = line.Split(',').Select(z => Range.FromString(z)).ToList();
            return new RangePair()
            {
                First = ranges.First(),
                Second = ranges.Last()
            };
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {

            List<RangePair> rangePairs = File.ReadAllLines("input.txt").Select(RangePair.FromString).ToList();
            List<RangePair> fullOverlap = rangePairs.Where(z => z.Contains()).ToList();

            List<RangePair> partialOverlap = rangePairs.Where(z => z.Overlap()).ToList();

            Console.WriteLine(fullOverlap.Count);
            Console.WriteLine(partialOverlap.Count);

        }
    }
}