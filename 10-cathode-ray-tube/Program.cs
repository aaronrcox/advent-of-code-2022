namespace cathode_ray_tube
{
    internal class Program
    {
        int cycles = 0;
        int xRegister = 1;

        List<int> xValuePerCycle = new List<int>();
        List<char[]> display = new();

        void Run()
        {
            string[] lines = File.ReadAllLines("input.txt");
            for (int i = 0; i < lines.Length; i++)
                RunInstruction(i, lines[i]);

            Console.WriteLine("---");

            for (int i = 1; i <= xValuePerCycle.Count; i++)
                 Console.WriteLine($"{i}:\t value: {xValuePerCycle[i-1]} \tStrength: {i * xValuePerCycle[i-1]}");
            
            Console.WriteLine("---");

            Console.WriteLine("Answer: " + (
                (20*xValuePerCycle[20-1]) + 
                (60*xValuePerCycle[60-1]) + 
                (100*xValuePerCycle[100-1]) + 
                (140*xValuePerCycle[140-1]) + 
                (180*xValuePerCycle[180-1]) + 
                (220*xValuePerCycle[220-1]))
            );
            Console.WriteLine("---");

            foreach (var displayLine in display)
                Console.WriteLine(displayLine);
        }

        void NextCycle(string commandStr)
        {
            xValuePerCycle.Add(xRegister);
            // Console.WriteLine($"{commandStr}\t\tIndex: {xValuePerCycle.Count}\tvalue: {xValuePerCycle.Last()} \tstrength: {xValuePerCycle.Count * xValuePerCycle.Last()}");

            int row = (xValuePerCycle.Count - 1) / 40;
            int col = (xValuePerCycle.Count - 1) % 40;

            while (display.Count - 1 < row)
                display.Add("........................................".ToCharArray());

            if (xRegister >= col - 1 && xRegister <= col + 1)
            {
                display[row][col] = '#';
            }
        }

        void RunInstruction(int index, string instruction)
        {
            string[] parts = instruction.Split(' ');
            if (parts[0] == "noop")
            {
                NextCycle("noop 1");
            }
            else if (parts[0] == "addx")
            {
                NextCycle("addx 1");   
                NextCycle("addx 2");

                int val = int.Parse(parts[1]);
                xRegister += val;
            }
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }
    }
}