namespace proboscidea_volcanium
{
    class CaveNode
    {
        public string Name { get; set; } = string.Empty;
        public float FlowRate { get; set; }
        public bool IsOpen { get; set; } = false;
        public List<CaveNode> Connections { get; set; } = new();
        public List<string> ConnectedCaveNames { get; set; } = new();

    }

    internal class Program
    {


        void Run( bool isTest)
        {
            Dictionary<string, CaveNode> nodes = File.ReadAllLines(isTest ? "input-test.txt" : "input.txt")
                .Select(z => z.Replace("Valve ", "").Replace("has flow rate=", "").Replace("tunnels lead to valves ", "").Replace("tunnel leads to valve ", ""))
                .Select(z => z.Split("; "))
                .Select(z =>
                {
                    string[] valveParts = z[0].Split(" ");
                    string valveName = valveParts[0];
                    int flowRate = int.Parse(valveParts[1]);

                    List<string> connections = z[1].Split(", ").ToList();

                    return new CaveNode()
                    {
                        Name = valveName,
                        FlowRate = flowRate,
                        ConnectedCaveNames = connections
                    };
                }).ToDictionary(z => z.Name, z => z);


            foreach(var kvp in nodes)
            {
                var node = kvp.Value;
                foreach (string connectedCaveName in node.ConnectedCaveNames)
                    node.Connections.Add(nodes[connectedCaveName]);
            }

            Console.WriteLine("Hello World");

        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run(true);
        }
    }
}