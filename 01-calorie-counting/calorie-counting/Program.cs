
using System.Text.RegularExpressions;

namespace CalorieCounting
{
    class Program
    {
        public static void Main(string[] args)
        {
            var caloryGroups = GetCaloryGroupsFromFile("input-test.txt")!;

            PrintTotalCalories(caloryGroups);
            PrintTopThreeCalories(caloryGroups);

        }

        private static List<List<int>> GetCaloryGroupsFromFile(string filename)
        {
            string input = File.ReadAllText(filename);
            return input.Split("\r\n\r\n").Select(z => z.Split("\r\n").Where(val => !string.IsNullOrEmpty(val)).Select(int.Parse).ToList()).ToList()!;
        }

        private static void PrintTotalCalories(List<List<int>> caloryGroups)
        {
            int largestTotal = 0;
            int largestTotalIndex = 0;
            for (int i = 0; i < caloryGroups.Count; i++)
            {
                int groupTotal = caloryGroups[i].Sum();
                if (groupTotal > largestTotal)
                {
                    largestTotal = groupTotal;
                    largestTotalIndex = i;
                }
            }

            Console.WriteLine("Total Calories: " + largestTotal);
            Console.WriteLine("Position: " + (largestTotalIndex + 1));
        }

        private static void PrintTopThreeCalories(List<List<int>> caloryGroups)
        {

            int totalCalariesOfTopThree = caloryGroups
                .Select(z => z.Sum())
                .OrderByDescending(z => z)
                .Take(3)
                .Sum();

            Console.WriteLine("Total Calories of top three: " + totalCalariesOfTopThree);

        }
        
    }
}
