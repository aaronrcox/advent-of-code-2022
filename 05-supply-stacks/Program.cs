namespace supply_stacks
{

    public class Command
    {
        public int Quantity { get; set; }
        public int FromStackIndex { get; set; }
        public int ToStackIndex { get; set; }

        public void Execute_Task1(List<List<string>> supplyStacks)
        {
            for (int i = 0; i < Quantity; i++)
            {
                string value = supplyStacks[FromStackIndex].Last();
                supplyStacks[FromStackIndex].RemoveAt(supplyStacks[FromStackIndex].Count - 1);
                supplyStacks[ToStackIndex].Add(value);
            }
        }

        public void Execute_Task2(List<List<string>> supplyStacks)
        {
            List<string> toAdd = new();

            for (int i=0; i<Quantity; i++)
            {
                string value = supplyStacks[FromStackIndex].Last();
                toAdd.Add(value);
                supplyStacks[FromStackIndex].RemoveAt(supplyStacks[FromStackIndex].Count - 1);                
            }
            toAdd.Reverse();
            supplyStacks[ToStackIndex].AddRange(toAdd);

        }

        public static Command FromFileLine(string commandLine)
        {
            string[] tokens = commandLine.Split(' ');
            return new Command()
            {
                Quantity = int.Parse(tokens[1]),
                FromStackIndex = int.Parse(tokens[3]) - 1,
                ToStackIndex = int.Parse(tokens[5]) - 1,
            };
        }
    }
    public class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }

        List<Command> commands = new List<Command>();
        List<List<string>> supplyStacks = new List<List<string>>();

        void Run()
        {
            List<string> stackLines = File.ReadAllLines("input-stack.txt").ToList();
            List<string> commandLines = File.ReadAllLines("input-commands.txt").ToList();

            List<List<string>> supplyStacks_t1 = ParseCommandStacks(stackLines);
            List<List<string>> supplyStacks_t2 = ParseCommandStacks(stackLines);

            commands = commandLines.Select(z => Command.FromFileLine(z)).ToList();

            for(int i=0; i<commands.Count; i++)
            {
                commands[i].Execute_Task1(supplyStacks_t1);
                commands[i].Execute_Task2(supplyStacks_t2);
            }

            string topItems_t1 = string.Join("", supplyStacks_t1.Select(z => z.LastOrDefault()));
            string topItems_t2 = string.Join("", supplyStacks_t2.Select(z => z.LastOrDefault()));

            Console.WriteLine("Task1: " + topItems_t1);
            Console.WriteLine("Task2: " + topItems_t2);
        }

        List<List<string>> ParseCommandStacks(List<string> stackLines)
        {
            stackLines = stackLines.Select(z => z.Replace("    ", "-").Replace("     ", "-").Replace("[", "").Replace("]", "-").Replace(" ", "")).ToList();
            List<List<string>> stackItems = stackLines.Select(z => z.Split("-", StringSplitOptions.None).ToList()).ToList();

            return stackItems
                .SelectMany(inner => inner.Select((item, index) => new { item, index }))
                .GroupBy(i => i.index, i => i.item)
                .Select(z => z.Where(v => !string.IsNullOrWhiteSpace(v)).Reverse().ToList())
                .ToList();
        }
    }
}