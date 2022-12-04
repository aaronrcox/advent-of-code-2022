namespace Rucksack_Reorganization
{
    public class Program
    {

        class Rulestack
        {
            public Rulestack(string input)
            {
                Input = input;
                int hStrPos = input.Length / 2;
                Compartment1 = input.Substring(0, hStrPos);
                Compartment2 = input.Substring(hStrPos);
            }

            public string Input { get; set; }
            public string Compartment1 { get; set; }
            public string Compartment2 { get; set; }

            public char GetMatchingComponent()
            {
                for(int i = 0; i < Compartment1.Length; i++)
                {
                    if(Compartment2.Contains(Compartment1[i]))
                        return Compartment1[i];
                }
                return ' ';
            }

            public static int GetComponentScore(char component)
            {
                if (char.IsLower(component))
                {
                    return component - 'a' + 1;
                }
                if(char.IsUpper(component))
                {
                    return component - 'A' + 27;
                }

                return 0;
            }
        }

        class RuleStackGroup
        {
            public List<Rulestack> Stack { get; set; } = new List<Rulestack>();
            public int GetStackBadge()
            {
                for(char c = 'a'; c <= 'z'; c++)
                {
                    if (Stack.All(z => z.Input.Contains(c)))
                        return Rulestack.GetComponentScore(c);
                }

                for(char c = 'A'; c <= 'Z'; c++)
                {
                    if (Stack.All(z => z.Input.Contains(c)))
                        return Rulestack.GetComponentScore(c);
                }

                return 0;
            }
        }

        static void Main(string[] args)
        {
            

            List<string> lines = File.ReadAllLines("input.txt").ToList();

            for(int j=0; j<100; j++)
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                List<Rulestack> rulestacks = lines
                    .Select(z => new Rulestack(z))
                    .ToList();



                // Ruleset Scores
                List<char> compartment = rulestacks.Select(z => z.GetMatchingComponent()).ToList();
                List<int> componentScores = compartment.Select(z => Rulestack.GetComponentScore(z)).ToList();
                // Console.WriteLine(componentScores.Sum());



                // Group Scores
                List<RuleStackGroup> ruleStackGroups = new List<RuleStackGroup>();
                for (int i = 0; i < rulestacks.Count; i += 3)
                {
                    ruleStackGroups.Add(new RuleStackGroup());
                    ruleStackGroups.Last().Stack.Add(rulestacks[i]);
                    ruleStackGroups.Last().Stack.Add(rulestacks[i + 1]);
                    ruleStackGroups.Last().Stack.Add(rulestacks[i + 2]);
                }


                List<int> groupScores = ruleStackGroups.Select(z => z.GetStackBadge()).ToList();
                //Console.WriteLine(groupScores.Sum());


                watch.Stop();
                Console.WriteLine("Elapsed Time:" + watch.ElapsedMilliseconds);
            }

            

            Console.WriteLine("hello world");
        }
    }
}