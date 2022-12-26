namespace full_of_hot_air
{
    internal class Program
    {


        static long FromSNAFU(string value)
        {
            long result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                int x = "=-012".IndexOf(value.Substring(value.Length - 1 - i, 1)) - 2;
                result += x * (long)Math.Pow(5, i);
            }
            return result;
        }

        static string ToSNAFU(long value)
        {
            string result = "";
            while (value > 0)
            {
                result += "012=-"[(int)(value % 5)];
                value -= (((value + 2) % 5) - 2);
                value /= 5;
            }


            return result;
        }

        static void TestToSNAFU(int value, string expected)
        {
            string snafu = ToSNAFU(value);
            Console.WriteLine($"[{(snafu == expected ? "pass" : "fail")}] \tvalue: {value} \t\t to {snafu} \t\texpected {expected}");
        }

        static void TestFromSNAFU(string value, long expected)
        {
            long number = FromSNAFU(value);
            Console.WriteLine($"[{(number == expected ? "pass" : "fail")}] \tvalue: {value} \t\t to {number} \t\texpected {expected}");
        }


        static void Main(string[] args)
        {
            TestToSNAFU(1, "1");
            TestToSNAFU(2, "2");
            TestToSNAFU(3, "1=");
            TestToSNAFU(4, "1-");
            TestToSNAFU(5, "10");
            TestToSNAFU(6, "11");
            TestToSNAFU(7, "12");
            TestToSNAFU(8, "2=");
            TestToSNAFU(9, "2-");
            TestToSNAFU(10, "20");
            TestToSNAFU(15, "1=0");
            TestToSNAFU(20, "1-0");
            TestToSNAFU(2022, "1=11-2");
            TestToSNAFU(12345, "1-0---0");
            TestToSNAFU(314159265, "1121-1110-1=0");

            Console.WriteLine("-----------------------");

            TestFromSNAFU("1=-0-2", 1747);
            TestFromSNAFU("12111",  906);
            TestFromSNAFU("2=0=",  198);
            TestFromSNAFU("21",   11);
            TestFromSNAFU("2=01",  201);
            TestFromSNAFU("111",   31);
            TestFromSNAFU("20012", 1257);
            TestFromSNAFU("112",   32);
            TestFromSNAFU("1=-1=",  353);
            TestFromSNAFU("1-12",  107);
            TestFromSNAFU("12",    7);
            TestFromSNAFU("1=",    3);
            TestFromSNAFU("122",   37);

            Console.WriteLine("-------------------------");

            bool isTest = false;
            List<string> lines = File.ReadAllLines(isTest ? "input-test.txt" : "input.txt").ToList();
            long total = lines.Select(FromSNAFU).Sum();
            Console.WriteLine($"Total: {total} \t\t snafu:{ToSNAFU(total)}");

        }
    }
}