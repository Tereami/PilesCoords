using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, List<int>> keysAndMarks = new Dictionary<string, List<int>>();
            keysAndMarks.Add("A", new List<int>() { 1, 2, 4, 5, 6, 7, 11, 13});
            keysAndMarks.Add("B", new List<int>() { 3, 8, 9, 10, 12 });

            foreach(var kvp in keysAndMarks)
            {
                List<int> marks = kvp.Value;
                string range = marks[0].ToString();
                if (marks[1] == marks[0] + 1) range += "-";
                if (marks[1] != marks[0] + 1) range += ", ";

                for (int i = 1; i < marks.Count -1; i++)
                {
                    int curMark = marks[i];
                    if(marks[i + 1] != curMark + 1)
                    {
                        range += curMark + ", ";
                        continue;
                    }
                    else if (marks[i - 1] != curMark - 1)
                    {
                        range += curMark + "-";
                        continue;
                    }
                }
                range += marks[marks.Count - 1];

                Console.WriteLine(range);
            }

            Console.ReadKey();
        }
    }
}
