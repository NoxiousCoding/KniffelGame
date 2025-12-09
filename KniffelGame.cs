using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace KniffelConsole
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("====================================");
                Console.WriteLine("         K N I F F E L   🎲         ");
                Console.WriteLine("====================================");
                Console.ResetColor();

                Console.WriteLine("1. Neues Spiel starten");
                Console.WriteLine("2. Regeln anzeigen");
                Console.WriteLine("3. Beenden");
                Console.WriteLine("------------------------------------");
                Console.Write("Auswahl: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        KniffelGame.RunMultiplayerMode();
                        break;

                    case "2":
                        ShowRules();
                        break;

                    case "3":
                        Console.WriteLine("Spiel wird beendet...");
                        return;

                    default:
                        Console.WriteLine("Ungültige Eingabe! ENTER drücken...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        static void ShowRules()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("============== REGELN ==============");
            Console.ResetColor();
            Console.WriteLine("• 5 Würfel");
            Console.WriteLine("• Bis zu 3 Würfe pro Runde");
            Console.WriteLine("• Nach jedem Wurf darf man Würfel behalten");
            Console.WriteLine("• 13 Runden – jede Kategorie genau einmal");
            Console.WriteLine("• Kategorien: Einsen, Zweien, ... Kniffel (50 Punkte)");
            Console.WriteLine("• Bonus: 35 Punkte ab 63 Punkten in der oberen Sektion");
            Console.WriteLine("• Computer kann optional als Spieler teilnehmen");
            Console.WriteLine("\nENTER zum Zurückkehren...");
            Console.ReadLine();
        }
    }

    public static class KniffelGame
    {
        static Random rnd = new Random();

        public static void RunMultiplayerMode()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Mehrspieler-Modus gestartet!");
            Console.ResetColor();

            int numPlayers = 0;
            while (numPlayers < 1 || numPlayers > 4)
            {
                Console.Write("Anzahl menschlicher Spieler (1-4): ");
                int.TryParse(Console.ReadLine(), out numPlayers);
            }

            Console.Write("Computer als Spieler hinzufügen? (j/n): ");
            bool includeComputer = Console.ReadLine().Trim().ToLower() == "j";

            int totalPlayers = includeComputer ? numPlayers + 1 : numPlayers;

            string[] playerNames = new string[totalPlayers];
            ScoreCard[] scores = new ScoreCard[totalPlayers];

            for (int i = 0; i < numPlayers; i++)
            {
                Console.Write($"Name Spieler {i + 1}: ");
                playerNames[i] = Console.ReadLine().Trim();
                scores[i] = new ScoreCard();
            }

            if (includeComputer)
            {
                playerNames[totalPlayers - 1] = "Computer";
                scores[totalPlayers - 1] = new ScoreCard();
            }

            for (int round = 1; round <= 13; round++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n==== Runde {round}/13 ====");
                Console.ResetColor();

                for (int p = 0; p < totalPlayers; p++)
                {
                    Console.ForegroundColor = playerNames[p] == "Computer" ? ConsoleColor.Red : ConsoleColor.Green;
                    Console.WriteLine($"\n-- Zug: {playerNames[p]} --");
                    Console.ResetColor();

                    int[] dice = playerNames[p] == "Computer" ? PlayComputerRound() : PlayPlayerRound();

                    string category = playerNames[p] == "Computer" ? ChooseComputerCategory(scores[p], dice) : ChooseCategory(scores[p]);
                    int points = EvaluateCategory(category, dice);
                    ApplyScore(scores[p], category, points);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{playerNames[p]} belegt {category} mit {points} Punkten.");
                    Console.ResetColor();

                    scores[p].Print();
                    Console.WriteLine("ENTER für nächsten Spieler...");
                    Console.ReadLine();
                }
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n====== SPIEL ENDE ======");
            Console.ResetColor();

            var ranking = scores.Select((s, i) => new { Name = playerNames[i], Score = s.TotalScore })
                                .OrderByDescending(x => x.Score)
                                .ToArray();

            Console.WriteLine("=== Endstand ===");
            for (int i = 0; i < ranking.Length; i++)
            {
                Console.ForegroundColor = i == 0 ? ConsoleColor.Green : ConsoleColor.White;
                Console.WriteLine($"{i + 1}. {ranking[i].Name}: {ranking[i].Score} Punkte");
            }
            Console.ResetColor();
            Console.WriteLine("\nENTER zum Menü...");
            Console.ReadLine();
        }

        // -------------------------------
        // Spielerzug
        // -------------------------------
        static int[] PlayPlayerRound()
        {
            int[] dice = new int[5];
            bool[] keep = new bool[5];

            for (int roll = 1; roll <= 3; roll++)
            {
                for (int i = 0; i < 5; i++)
                    if (!keep[i])
                        dice[i] = rnd.Next(1, 7);

                Console.WriteLine($"\nWurf {roll}:");
                AnimateDice(dice);

                if (roll < 3)
                {
                    Console.Write("Welche Würfel behalten? (z.B. 1 3 5) ENTER = keine: ");
                    string input = Console.ReadLine().Trim();
                    keep = new bool[5];

                    if (input != "")
                    {
                        foreach (var part in input.Split())
                        {
                            if (int.TryParse(part, out int idx) && idx >= 1 && idx <= 5)
                                keep[idx - 1] = true;
                        }
                    }
                }
            }
            return dice;
        }

        // -------------------------------
        // Computerzug
        // -------------------------------
        static int[] PlayComputerRound()
        {
            int[] dice = new int[5];
            bool[] keep = new bool[5];

            for (int roll = 1; roll <= 3; roll++)
            {
                for (int i = 0; i < 5; i++)
                    if (!keep[i])
                        dice[i] = rnd.Next(1, 7);

                Console.WriteLine($"\nWurf {roll}:");
                AnimateDice(dice);

                if (roll < 3)
                    keep = DecideDiceToKeep(dice);
            }

            return dice;
        }

        // -------------------------------
        // Würfel-Animation mit roten Endwürfeln
        // -------------------------------
        static void AnimateDice(int[] dice)
        {
            int[] animDice = new int[5];
            for (int frame = 0; frame < 8; frame++)
            {
                for (int i = 0; i < 5; i++)
                    animDice[i] = rnd.Next(1, 7);

                Console.SetCursorPosition(0, Console.CursorTop);
                PrintDice(animDice); // Animation in Weiß
                Thread.Sleep(100);
                Console.WriteLine();
            }

            // Endgültige Würfel in Rot
            PrintDice(dice, ConsoleColor.Red);
        }

        // -------------------------------
        // KI-Logik
        // -------------------------------
        static bool[] DecideDiceToKeep(int[] dice)
        {
            bool[] keep = new bool[5];
            var counts = new int[7];
            foreach (int d in dice) counts[d]++;

            int maxCount = counts.Max();
            int mostFrequent = Array.IndexOf(counts, maxCount);

            for (int i = 0; i < 5; i++)
                if (dice[i] == mostFrequent) keep[i] = true;

            int[] unique = dice.Distinct().OrderBy(x => x).ToArray();
            if (unique.Length >= 4)
            {
                for (int i = 0; i < 5; i++)
                    if (unique.Contains(dice[i])) keep[i] = true;
            }

            for (int i = 0; i < 5; i++)
                if (dice[i] >= 5) keep[i] = true;

            return keep;
        }

        // -------------------------------
        // Kategorieauswahl Spieler
        // -------------------------------
        static string ChooseCategory(ScoreCard sc)
        {
            string[] categories = {
                "Einsen","Zweien","Dreien","Vieren","Fünfen","Sechsen",
                "Dreierpasch","Viererpasch","Full House",
                "Kleine Straße","Große Straße","Kniffel","Chance"
            };

            while (true)
            {
                for (int i = 0; i < categories.Length; i++)
                {
                    bool used = sc.IsUsed(categories[i]);
                    Console.ForegroundColor = used ? ConsoleColor.DarkGray : ConsoleColor.White;
                    Console.WriteLine($"{i + 1}. {categories[i]} {(used ? "(belegt)" : "")}");
                }
                Console.ResetColor();

                Console.Write("Kategorie #: ");
                if (int.TryParse(Console.ReadLine(), out int choice) &&
                    choice >= 1 && choice <= categories.Length)
                {
                    string cat = categories[choice - 1];
                    if (!sc.IsUsed(cat)) return cat;
                    Console.WriteLine("Kategorie bereits belegt!");
                }
            }
        }

        // -------------------------------
        // Kategorieauswahl Computer
        // -------------------------------
        static string ChooseComputerCategory(ScoreCard sc, int[] dice)
        {
            string[] categories = {
                "Einsen","Zweien","Dreien","Vieren","Fünfen","Sechsen",
                "Dreierpasch","Viererpasch","Full House",
                "Kleine Straße","Große Straße","Kniffel","Chance"
            };

            int bestScore = -1;
            string bestCat = "";

            foreach (var cat in categories)
            {
                if (!sc.IsUsed(cat))
                {
                    int pts = EvaluateCategory(cat, dice);
                    if (cat == "Kniffel" && pts == 50) pts += 20;
                    if ((cat == "Full House" || cat == "Große Straße") && pts > 0) pts += 10;

                    if (pts > bestScore)
                    {
                        bestScore = pts;
                        bestCat = cat;
                    }
                }
            }
            return bestCat;
        }

        // -------------------------------
        // Punkteberechnung
        // -------------------------------
        static int EvaluateCategory(string cat, int[] dice)
        {
            int[] count = new int[7];
            foreach (int d in dice) count[d]++;
            int sum = dice.Sum();
            int[] s = dice.OrderBy(x => x).ToArray();

            return cat switch
            {
                "Einsen" => count[1] * 1,
                "Zweien" => count[2] * 2,
                "Dreien" => count[3] * 3,
                "Vieren" => count[4] * 4,
                "Fünfen" => count[5] * 5,
                "Sechsen" => count[6] * 6,
                "Dreierpasch" => count.Any(c => c >= 3) ? sum : 0,
                "Viererpasch" => count.Any(c => c >= 4) ? sum : 0,
                "Full House" => (count.Contains(3) && count.Contains(2)) ? 25 : 0,
                "Kleine Straße" => (s.SequenceEqual(new[] { 1, 2, 3, 4, 5 }) || s.SequenceEqual(new[] { 2, 3, 4, 5, 6 })) ? 30 : 0,
                "Große Straße" => (s.SequenceEqual(new[] { 2, 3, 4, 5, 6 })) ? 40 : 0,
                "Kniffel" => count.Any(c => c == 5) ? 50 : 0,
                "Chance" => sum,
                _ => 0
            };
        }

        static void ApplyScore(ScoreCard score, string cat, int points)
        {
            switch (cat)
            {
                case "Einsen": score.Ones = points; break;
                case "Zweien": score.Twos = points; break;
                case "Dreien": score.Threes = points; break;
                case "Vieren": score.Fours = points; break;
                case "Fünfen": score.Fives = points; break;
                case "Sechsen": score.Sixes = points; break;
                case "Dreierpasch": score.ThreeOfAKind = points; break;
                case "Viererpasch": score.FourOfAKind = points; break;
                case "Full House": score.FullHouse = points; break;
                case "Kleine Straße": score.SmallStraight = points; break;
                case "Große Straße": score.LargeStraight = points; break;
                case "Kniffel": score.Yahtzee = points; break;
                case "Chance": score.Chance = points; break;
            }
        }

        // -------------------------------
        // ASCII-Würfel mit optionaler Punktfarbe
        // -------------------------------
        static void PrintDice(int[] dice, ConsoleColor? dotColor = null)
        {
            string[][] ascii = new string[6][]
            {
                new string[] { "╔═══╗", "║   ║", "║ • ║", "║   ║", "╚═══╝" },
                new string[] { "╔═══╗", "║•  ║", "║   ║", "║  •║", "╚═══╝" },
                new string[] { "╔═══╗", "║•  ║", "║ • ║", "║  •║", "╚═══╝" },
                new string[] { "╔═══╗", "║• •║", "║   ║", "║• •║", "╚═══╝" },
                new string[] { "╔═══╗", "║• •║", "║ • ║", "║• •║", "╚═══╝" },
                new string[] { "╔═══╗", "║• •║", "║• •║", "║• •║", "╚═══╝" }
            };

            for (int line = 0; line < 5; line++)
            {
                foreach (int d in dice)
                {
                    if (dotColor.HasValue)
                        Console.ForegroundColor = dotColor.Value;
                    else
                        Console.ResetColor();

                    Console.Write(ascii[d - 1][line] + " ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
    }

    public class ScoreCard
    {
        public int? Ones { get; set; }
        public int? Twos { get; set; }
        public int? Threes { get; set; }
        public int? Fours { get; set; }
        public int? Fives { get; set; }
        public int? Sixes { get; set; }
        public int? ThreeOfAKind { get; set; }
        public int? FourOfAKind { get; set; }
        public int? FullHouse { get; set; }
        public int? SmallStraight { get; set; }
        public int? LargeStraight { get; set; }
        public int? Yahtzee { get; set; }
        public int? Chance { get; set; }

        public int UpperScore => (Ones ?? 0) + (Twos ?? 0) + (Threes ?? 0) + (Fours ?? 0) + (Fives ?? 0) + (Sixes ?? 0);
        public int UpperBonus => UpperScore >= 63 ? 35 : 0;
        public int LowerScore => (ThreeOfAKind ?? 0) + (FourOfAKind ?? 0) + (FullHouse ?? 0) + (SmallStraight ?? 0) + (LargeStraight ?? 0) + (Yahtzee ?? 0) + (Chance ?? 0);
        public int TotalScore => UpperScore + UpperBonus + LowerScore;

        public bool IsUsed(string category)
        {
            return category switch
            {
                "Einsen" => Ones.HasValue,
                "Zweien" => Twos.HasValue,
                "Dreien" => Threes.HasValue,
                "Vieren" => Fours.HasValue,
                "Fünfen" => Fives.HasValue,
                "Sechsen" => Sixes.HasValue,
                "Dreierpasch" => ThreeOfAKind.HasValue,
                "Viererpasch" => FourOfAKind.HasValue,
                "Full House" => FullHouse.HasValue,
                "Kleine Straße" => SmallStraight.HasValue,
                "Große Straße" => LargeStraight.HasValue,
                "Kniffel" => Yahtzee.HasValue,
                "Chance" => Chance.HasValue,
                _ => false
            };
        }

        public void Print()
        {
            Console.WriteLine("\n===== PUNKTETABELLE =====");
            Console.WriteLine($"Einsen:         {Ones}");
            Console.WriteLine($"Zweien:         {Twos}");
            Console.WriteLine($"Dreien:         {Threes}");
            Console.WriteLine($"Vieren:         {Fours}");
            Console.WriteLine($"Fünfen:         {Fives}");
            Console.WriteLine($"Sechsen:        {Sixes}");
            Console.WriteLine($"Obere Sektion:  {UpperScore}");
            Console.WriteLine($"Bonus (63+):    {UpperBonus}");
            Console.WriteLine("\n--- Untere Sektion ---");
            Console.WriteLine($"Dreierpasch:     {ThreeOfAKind}");
            Console.WriteLine($"Viererpasch:     {FourOfAKind}");
            Console.WriteLine($"Full House:      {FullHouse}");
            Console.WriteLine($"Kleine Straße:   {SmallStraight}");
            Console.WriteLine($"Große Straße:    {LargeStraight}");
            Console.WriteLine($"Kniffel:         {Yahtzee}");
            Console.WriteLine($"Chance:          {Chance}");
            Console.WriteLine($"\nGESAMTPUNKTE:   {TotalScore}");
            Console.WriteLine("==========================\n");
        }
    }
}