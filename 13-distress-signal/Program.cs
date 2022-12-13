

using System.Collections;
using System.Collections.Generic;

namespace DistressSignal
{
    enum SingnalValueNodeType
    {
        ARRAY,
        VALUE,
        NONE
    }

    class SignalValueNodeComparer : IComparer<SignalValueNode>
    {
        public int Compare(SignalValueNode x, SignalValueNode y)
        {
            return SignalValueNode.Compare(x, y);
        }
    }

    class SignalValueNode
    {
        public string Data { get; set; } = string.Empty;

        public int? Value { get; set; }

        public string TrackingId { get; set; } = string.Empty;

        public SingnalValueNodeType ValueType
        {
            get
            {
                if(Value.HasValue) return SingnalValueNodeType.VALUE;
                if (Children != null) return SingnalValueNodeType.ARRAY;
                return SingnalValueNodeType.NONE;
            }
        }


        public SignalValueNode? ValueAsNode 
        { 
            get
            {
                return Value.HasValue ? new SignalValueNode($"[{Value}]") : null;
            }
        }

        public SignalValueNode[]? Children { get; set; }

        public SignalValueNode(string val, string trackingId = "")
        {
            TrackingId = trackingId;
            Data = val;
            if(Data.StartsWith('['))
            {
                SignalValueNode[] children = Parse(Data);
                Children = children;
            }
            if (int.TryParse(Data, out int value))
            {
                Value = value;
            }
        }

        public static int Compare(SignalValueNode left, SignalValueNode right)
        {
            if(left.ValueType == SingnalValueNodeType.VALUE && right.ValueType == SingnalValueNodeType.VALUE)
                return left.Value == right.Value ? 0 : left.Value < right.Value ? -1 : 1;

            if (left.ValueType == SingnalValueNodeType.NONE && right.ValueType == SingnalValueNodeType.NONE)
                return 0;

            if (left.ValueType == SingnalValueNodeType.NONE && right.ValueType != SingnalValueNodeType.NONE)
                return 1;

            if (left.ValueType != SingnalValueNodeType.NONE && right.ValueType == SingnalValueNodeType.NONE)
                return -1;

            if(left.ValueType == SingnalValueNodeType.ARRAY && right.ValueType == SingnalValueNodeType.ARRAY)
            {
                int leftLen = left.Children!.Length;
                int rightLen = right.Children!.Length;
                for(int i=0; i< leftLen && i < rightLen; i++)
                {
                    int result = Compare(left.Children![i], right.Children![i]);
                    if (result != 0) return result;
                }

                return leftLen == rightLen ? 0 : leftLen < rightLen ? -1 : 1;

            }

            if (left.ValueType == SingnalValueNodeType.VALUE && right.ValueType == SingnalValueNodeType.ARRAY)
                left = left.ValueAsNode!;

            if (left.ValueType == SingnalValueNodeType.ARRAY && right.ValueType == SingnalValueNodeType.VALUE)
                right = right.ValueAsNode!;

            return Compare(left, right);
        }

        public static SignalValueNode[] Parse(string str)
        {
            // make sure there are no spaces in the string
            str = str.Replace(" ", "");

            // remove the start / end bracket from the string
            if (str.StartsWith('[')) str = str.Substring(1, str.Length - 2);

            List<string> values = new List<string>();

            int curPos = 0;
            for(int i=0; i<str.Length; i++)
            {
                if(str[i] == ',')
                {
                    string val = str.Substring(curPos, i - curPos);
                    values.Add(val);
                    curPos = i + 1;
                }
                if (str[i] == '[')
                {
                    int closeBracketPos = FindMatchingClosingBracketIndex(str, i);
                    string val = str.Substring(i, closeBracketPos - i + 1);
                    values.Add(val);
                    i = closeBracketPos + 1;
                    curPos = i + 1;
                }
            }

            // add last value
            if(curPos < str.Length)
                values.Add(str.Substring(curPos));

            return values.Select(val => new SignalValueNode(val)).ToArray();
        }

        public static int FindMatchingClosingBracketIndex(string str, int sPos)
        {
            int bracketCount = 1;
            for(int i=sPos+1; i<str.Length; i++)
            {
                if (str[i] == '[') bracketCount++;
                if (str[i] == ']') bracketCount--;
                if (bracketCount == 0) return i;
            }

            return -1;
        }

        
    }

    class SignalPair
    {
        public int Index { get; set; }
        public SignalValueNode Left { get; set; }
        public SignalValueNode Right { get; set; }

        public bool IsInOrder
        {
            get { return SignalValueNode.Compare(Left, Right) <= 0; }
        }

        public SignalPair(int index, string[] pairs)
        {
            Index = index;
            Left = new(pairs[0]);
            Right = new(pairs[1]);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            SignalValueNode nodes0l = new SignalValueNode("[1,1,3,1,1]");
            SignalValueNode nodes0r = new SignalValueNode("[1,1,5,1,1]");
            int compare0 = SignalValueNode.Compare(nodes0l, nodes0r); // Expected: -1


            SignalValueNode nodes1l = new SignalValueNode("[[1],[2,3,4]]");
            SignalValueNode nodes1r = new SignalValueNode("[[1],4]");
            int compare1 = SignalValueNode.Compare(nodes1l, nodes1r); // Expected: -1


            SignalValueNode nodes2l = new SignalValueNode("[9]");
            SignalValueNode nodes2r = new SignalValueNode("[[8,7,6]]");
            int compare2 = SignalValueNode.Compare(nodes2l, nodes2r);// Expected: 1


            SignalValueNode nodes3l = new SignalValueNode("[[4,4],4,4]");
            SignalValueNode nodes3r = new SignalValueNode("[[4,4],4,4,4]");
            int compare3 = SignalValueNode.Compare(nodes3l, nodes3r); // Expected -1


            SignalValueNode nodes4l = new SignalValueNode("[7,7,7,7]");
            SignalValueNode nodes4r = new SignalValueNode("[7,7,7]");
            int compare4 = SignalValueNode.Compare(nodes4l, nodes4r); // Expected 1

            SignalValueNode nodes5l = new SignalValueNode("[]");
            SignalValueNode nodes5r = new SignalValueNode("[3]");
            int compare5 = SignalValueNode.Compare(nodes5l, nodes5r); // Expected -1

            SignalValueNode nodes6l = new SignalValueNode("[[[]]]");
            SignalValueNode nodes6r = new SignalValueNode("[[]]");
            int compare6 = SignalValueNode.Compare(nodes6l, nodes6r); // Expected 1

            SignalValueNode nodes7l = new SignalValueNode("[1,[2,[3,[4,[5,6,7]]]],8,9]");
            SignalValueNode nodes7r = new SignalValueNode("[1,[2,[3,[4,[5,6,0]]]],8,9]");
            int compare7 = SignalValueNode.Compare(nodes7l, nodes7r); // Expected 1

            // Part 1
            List<SignalPair> signalPairs = File.ReadAllText("input.txt").Split("\r\n\r\n").Select((pairstr, index) => new SignalPair(index + 1, pairstr.Split("\r\n"))).ToList();
            int result1 = signalPairs.Where(z => z.IsInOrder).Select(z => z.Index).Sum();
            Console.WriteLine(result1);

            // Part 2
            List <SignalValueNode> packets = File.ReadAllText("input.txt").Split("\r\n").Where(z => !string.IsNullOrWhiteSpace(z)).Select(z => new SignalValueNode(z)).ToList();
            packets.Add(new SignalValueNode("[[2]]", "Divider 1"));
            packets.Add(new SignalValueNode("[[6]]", "Divider 2"));
            packets.Sort(new SignalValueNodeComparer());

            int divider1Index = packets.FindIndex(z => z.TrackingId == "Divider 1") + 1;
            int divider2Index = packets.FindIndex(z => z.TrackingId == "Divider 2") + 1;

            int result2 = divider1Index * divider2Index;
            Console.WriteLine(result2);
        }
    }
}