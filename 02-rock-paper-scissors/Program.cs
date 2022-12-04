

namespace RockPaperScissors
{
    public enum MoveType
    {
        UNKNWON,
        ROCK,
        PAPER,
        SCISSORS,
    }

    public enum RoundResult
    {        
        LOSE,
        TIE,
        WIN,
        UNKNWON,
    }

    public static class MoveTypeExtensions
    {
        public static MoveType MoveTypeFromString(this string move)
        {
            return move switch
            {
                "A" => MoveType.ROCK,
                "B" => MoveType.PAPER,
                "C" => MoveType.SCISSORS,
                _ => MoveType.UNKNWON,
            };
        }

        public static RoundResult RoundResultFromString(this string result)
        {
            return result switch
            {
                "X" => RoundResult.LOSE,
                "Y" => RoundResult.TIE,
                "Z" => RoundResult.WIN,
                _ => RoundResult.UNKNWON,
            };
        }


    }

    public struct Round
    {
        public Round(string[] moves)
        {
            P1Move = moves[0].MoveTypeFromString();
            TargetResult = moves[1].RoundResultFromString();
            P2Move = MoveType.UNKNWON;

            if(TargetResult == RoundResult.WIN)
            {
                if (P1Move == MoveType.ROCK) P2Move = MoveType.PAPER;
                if (P1Move == MoveType.PAPER) P2Move = MoveType.SCISSORS;
                if (P1Move == MoveType.SCISSORS) P2Move = MoveType.ROCK;
            }
            if(TargetResult == RoundResult.LOSE)
            {
                if (P1Move == MoveType.ROCK) P2Move = MoveType.SCISSORS;
                if (P1Move == MoveType.PAPER) P2Move = MoveType.ROCK;
                if (P1Move == MoveType.SCISSORS) P2Move = MoveType.PAPER;
            }
            if(TargetResult == RoundResult.TIE)
            {
                P2Move = P1Move;
            }

        }

        public MoveType P1Move { get; set; } 
        public MoveType P2Move {get; set; }
        public RoundResult TargetResult { get; set; }
        
        public RoundResult GetWinResult
        {
            get
            {
                if (P2Move == P1Move) return RoundResult.TIE;

                if (P2Move == MoveType.ROCK && P1Move == MoveType.SCISSORS) return RoundResult.WIN;
                if (P2Move == MoveType.PAPER && P1Move == MoveType.ROCK) return RoundResult.WIN;
                if (P2Move == MoveType.SCISSORS && P1Move == MoveType.PAPER) return RoundResult.WIN;
                
                return RoundResult.LOSE;
            }
            
        }

        public int GetRoundScore
        {
            get
            {
                int winMultiplier = (int)GetWinResult;
                int moveScore = (int)P2Move;
                return (winMultiplier * 3) + moveScore;
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            List<Round> rounds = File.ReadAllLines("input.txt").Select(line => new Round(line.Split(' '))).ToList();
            List<int> roundScores = rounds.Select(z => z.GetRoundScore).ToList();
            int totalScore = roundScores.Sum();

            Console.WriteLine(totalScore);
        }

        
    }
}

