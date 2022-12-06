namespace tuning_trouble
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string line = File.ReadAllText("input.txt");

            for(int i = 4; i < line.Length; i++)
            {
                // get last 4 characters
                string code = line.Substring(i - 4, 4);
                if (code.ToCharArray().Distinct().Count() == code.Length)
                {
                    Console.WriteLine(i + "\t Found Marker Pos");
                    break;
                }
            }

            for (int i = 14; i < line.Length; i++)
            {
                // get last 4 characters
                string code = line.Substring(i - 14, 14);
                if (code.ToCharArray().Distinct().Count() == code.Length)
                {
                    Console.WriteLine(i + "\t Found Message Pos");
                    break;
                }
            }


            Console.WriteLine("Program End");
        }
    }
}