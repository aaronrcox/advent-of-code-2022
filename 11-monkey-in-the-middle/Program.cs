using System.ComponentModel;
using System.Numerics;

namespace monkey_in_the_middle
{

    class Monkey
    {
        public int Index { get; set; } = 0;
        // public List<BigInteger> Items { get; set; } = new();
        public LinkedList<BigInteger> Items { get; set; } = new();

        public Func<BigInteger, BigInteger>? OperationFunc { get; set; } = null;

        public long TestFuncDivisiblyByValue { get; set; }
        public Func<BigInteger, bool>? TestFunc { get; set; } = null;
        public int ThrowTrueTarget { get; set; } = 0;
        public int ThrowFalseTarget { get; set; } = 0;

        public long InspectedItemCount = 0;

        public void TakeTurn(List<Monkey> monkies, long denominator)
        {
            while(Items.Count > 0)
            {
                // Inspect Item
                InspectedItemCount++;
                Items.First.Value = OperationFunc!(Items.First.Value) % denominator;
                
                if (TestFunc!(Items.First.Value)) ThrowTo(monkies[ThrowTrueTarget]); // ThrowTrueFunc!(this, monkies);
                else ThrowTo(monkies[ThrowFalseTarget]);
            }
        }

        public void ThrowTo(Monkey target)
        {
            
            var value = Items.First.Value;
            Items.RemoveFirst();

            target.Items.AddLast(value);
        }

        public static Monkey FromFileData(string data)
        {
            int monkeyIndex = 0;
            LinkedList<BigInteger> items = new();
            Func<BigInteger, BigInteger>? operationFunc = null;
            Func<BigInteger, bool>? testFunc = null;
            long testFuncDivisiblyByValue = 0;

            int throwTrueTarget = 0;
            int throwFalseTarget = 0;

            List<string> lines = data.Split("\r\n").Select(z => z.Trim()).ToList();

            int currentLineIndex = 0;
            while(currentLineIndex < lines.Count)
            {
                string line = lines[currentLineIndex];
                
                if(line.StartsWith("Monkey "))
                {
                    monkeyIndex = int.Parse(line.Replace("Monkey ", "").Replace(":", ""));
                }
                else if(line.StartsWith("Starting items: "))
                {
                    items = new(line.Replace("Starting items: ", "").Split(", ").Select(BigInteger.Parse));
                }
                else if(line.StartsWith("Operation:"))
                {
                    string[] token = line.Replace("Operation: ", "").Split(" ");

                    int value = 0;
                    
                    if (token[3] == "*" && token[4] == "old") operationFunc = (old) => old * old;
                    else if (token[3] == "*" && int.TryParse(token[4], out value)) operationFunc = (old) => old * value;
                    else if (token[3] == "+" && int.TryParse(token[4], out value)) operationFunc = (old) => old + value;

                }
                else if(line.StartsWith("Test:"))
                {
                    int.TryParse(line.Replace("Test: divisible by ", ""), out int divisibleByVal);
                    testFuncDivisiblyByValue = divisibleByVal;
                    testFunc = (val) => val % divisibleByVal == 0;

                    for (int i = 0; i < 2; i++)
                    {
                        currentLineIndex++;
                        line = lines[currentLineIndex];
                        if (line.StartsWith("If true: throw to monkey "))
                        {
                            throwTrueTarget = int.Parse(line.Replace("If true: throw to monkey ", ""));
                        }
                        if (line.StartsWith("If false: throw to monkey "))
                        {
                            throwFalseTarget = int.Parse(line.Replace("If false: throw to monkey ", ""));
                        }
                    }
                }

                currentLineIndex += 1;
            }

            return new Monkey()
            {
                Index = monkeyIndex,
                Items = items,
                OperationFunc = operationFunc,
                TestFuncDivisiblyByValue = testFuncDivisiblyByValue,
                TestFunc = testFunc,
                ThrowTrueTarget = throwTrueTarget,
                ThrowFalseTarget = throwFalseTarget
            };
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText("input.txt");
            List<Monkey> monkies = text.Split("\r\n\r\n").Select(data => Monkey.FromFileData(data)).ToList();

            long denominator = monkies.Select(z => z.TestFuncDivisiblyByValue).Aggregate(1, (long a, long b) => a * b);

            for(int i=0; i<10000; i++)
            {
                Console.WriteLine("Turn: " + i);
                foreach (var m in monkies)
                    m.TakeTurn(monkies, denominator);
            }

            var inspectedItemsCount = monkies.Select(m => m.InspectedItemCount).OrderByDescending(z => z).ToList();

            Console.WriteLine(inspectedItemsCount[0] * inspectedItemsCount[1]);

            
        }
    }
}