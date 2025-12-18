using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Channels;

namespace KniffelConsole
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Kniffel von NoxiousCoding";

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==================================");
                Console.WriteLine("         K N I F F E L 🎲         ");
                Console.WriteLine("         BY NOXIOUSCODING         ");
                Console.WriteLine("==================================");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("1. Neues Spiel starten");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("2. Regeln anzeigen");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("3. Beenden");
                Console.ResetColor();
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
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("============== REGELN ==============");
            Console.ResetColor();
            Console.ForegroundColor= ConsoleColor.Magenta;
            Console.WriteLine("1. Spielregeln anzeigen");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("2. Kategorien-Erklärungen");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("3. Zurück zum Hauptmenü");
            Console.ResetColor();
            Console.WriteLine("------------------------------------");
            Console.Write("Auswahl: ");

            string input = Console.ReadLine().Trim();
            switch (input)
            {
                case "1":
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("============== SPIELREGELN ==============");
                    Console.ResetColor();
                    Console.WriteLine("• 5 Würfel");
                    Console.WriteLine("• Bis zu 3 Würfe pro Runde");
                    Console.WriteLine("• Nach jedem Wurf darf man Würfel behalten");
                    Console.WriteLine("• 13 Runden – jede Kategorie genau einmal");
                    Console.WriteLine("• Kategorien: Einsen, Zweien, ... Kniffel (50 Punkte)");
                    Console.WriteLine("• Bonus: 35 Punkte ab 63 Punkten in der oberen Sektion");
                    Console.WriteLine("• Du kannst eine Kategorie einmal pro Spiel locken und die Punkte damit verdoppeln,\n🔓 können gelockt werden,\n🔒 gelockte Kategorie");
                    Console.WriteLine("• Computer kann optional als Spieler teilnehmen");
                    Console.WriteLine("\nENTER zum Zurückkehren...");
                    Console.ReadLine();
                    break;

                case "2":
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("=========== KATEGORIEN ===========");
                    Console.ResetColor();
                    Console.WriteLine("• Einsen bis Sechsen: Summe aller gewürfelten Augen dieser Zahl");
                    Console.WriteLine("• Paar: Mindestens ein Paar → Summe des höchsten Paars ");
                    Console.WriteLine("• Dreierpasch: Mindestens drei gleiche Würfel, Summe aller Würfel");
                    Console.WriteLine("• Viererpasch: Mindestens vier gleiche Würfel, Summe aller Würfel");
                    Console.WriteLine("• Full House: 3 gleiche + 2 gleiche → 25 Punkte");
                    Console.WriteLine("• Kleine Straße: Vier aufeinanderfolgende Würfel → 30 Punkte");
                    Console.WriteLine("• Große Straße: Fünf aufeinanderfolgende Würfel → 40 Punkte");
                    Console.WriteLine("• Kniffel: Fünf gleiche Würfel → 50 Punkte");
                    Console.WriteLine("• Chance: Summe aller Würfel");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("=========== BONUS-KATEGORIEN ===========");
                    Console.ResetColor();
                    Console.WriteLine("• Zwei Paare: Zwei verschiedene Paare → 15 Punkte");
                    Console.WriteLine("• Mini-Full-House: 3 Gleiche + 2 unterschiedliche Würfel (kein Paar) → 10 Punkte");
                    Console.WriteLine("• Chance+: Summe aller Würfel + 5 Bonuspunkte, wenn Summe ≥ 20");
                    Console.WriteLine("• Lucky Seven: Alle Würfel ergeben zusammen 7 → + 20 Punkte");
                    Console.WriteLine("\nENTER zum Zurückkehren...");
                    Console.ReadLine();
                    break;

                case "3":
                    return;

                default:
                    Console.WriteLine("Ungültige Eingabe! ENTER drücken...");
                    Console.ReadLine();
                    break;
            }

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

            Difficulty compDifficulty = Difficulty.Medium;

            if (includeComputer)
            {
                Console.Write("Schwierigkeit des Computers (1=Einfach, 2=Mittel, 3=Schwer): ");
                int diffInput = int.Parse(Console.ReadLine());
                compDifficulty = diffInput switch
                {
                    1 => Difficulty.Easy,
                    2 => Difficulty.Medium,
                    3 => Difficulty.Hard,
                    _ => Difficulty.Medium
                };
            }

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

                    string category = playerNames[p] == "Computer" ? ChooseComputerCategory(scores[p], dice, compDifficulty) : ChooseCategory(scores[p], dice);
                    int points = EvaluateCategory(category, dice);

                    scores[p].SetScore(category, points);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    if(category == scores[p].LockedCategory && scores[p].LockUsed)
                        Console.WriteLine($"{playerNames[p]} belegt {category} (LOCKED) mit {points * 2} Punkten.");
                    else
                        Console.WriteLine($"{playerNames[p]} belegt {category} mit {points} Punkten.");
                    Console.ResetColor();

                    scores[p].Print();
                    Console.WriteLine("ENTER für nächsten Spieler...");
                    Console.ReadLine();
                }
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
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
        // KI-Schwierigkeitsgrad
        // -------------------------------
        public enum Difficulty
        {
            Easy,
            Medium,
            Hard
        }

        // -------------------------------
        // KI-Logik
        // -------------------------------
        static bool[] DecideDiceToKeep(int[] dice,Difficulty difficulty = Difficulty.Medium)
        {
            bool[] keep = new bool[5];
            var counts = new int[7];
            foreach (int d in dice) counts[d]++;

            int maxCount = counts.Max();
            int mostFrequent = Array.IndexOf(counts, maxCount);

            if (difficulty == Difficulty.Easy)
            {
                // Easy: zufällige Würfel behalten
                for (int i = 0; i < 5; i++)
                    keep[i] = rnd.NextDouble() < 0.5;
            }

            else if (difficulty == Difficulty.Medium)
            {
                // Medium: behalte die häufigsten Würfel
                for (int i = 0; i < 5; i++)
                    if (dice[i] == mostFrequent) keep[i] = true;
            }

            else if (difficulty == Difficulty.Hard)
            {
                // Hard: Optimale KI
                // 1. Priorität: Kniffel, Full House, Große Straße
                if (maxCount >= 3)
                {
                    for (int i = 0; i < 5; i++)
                        if (dice[i] == mostFrequent) keep[i] = true;
                }

                // 2. Kleine/Große Straße: aufeinanderfolgende Würfel behalten
                int[] unique = dice.Distinct().OrderBy(x => x).ToArray();
                if (unique.Length >= 4)
                {
                    for (int i = 0; i < 5; i++)
                        if (unique.Contains(dice[i])) keep[i] = true;
                }

                // 3. Bonus-Kategorie Lucky Seven
                int sum = dice.Sum();
                if (sum < 7)
                {
                    for (int i = 0; i < 5; i++)
                        if (dice[i] <= 2) keep[i] = true;
                }

                // 4. Chance+: Würfel >= 4 behalten
                for (int i = 0; i < 5; i++)
                    if (dice[i] >= 4) keep[i] = true;
            }   

            return keep;
        }

        // -------------------------------
        // Kategorieauswahl Spieler
        // -------------------------------
        static string ChooseCategory(ScoreCard sc, int[] dice)
        {
            string[] categories = {
                "Einsen","Zweien","Dreien","Vieren","Fünfen","Sechsen",
                "Paar","Dreierpasch","Viererpasch","Full House",
                "Kleine Straße","Große Straße","Kniffel","Chance",
                "Zwei Paare","Mini Full House","Chance+", "Lucky Seven"
            };

            while (true)
            {
                for (int i = 0; i < categories.Length; i++)
                {
                    bool used = sc.IsUsed(categories[i]);
                    bool locked = sc.LockedCategory == categories[i];
                    if (locked)
                        Console.ForegroundColor = ConsoleColor.Cyan; // gelockte Kategorie
                    else if (used)
                        Console.ForegroundColor = ConsoleColor.DarkGray; // belegte Kategorie
                    else
                        Console.ForegroundColor = ConsoleColor.White; // verfügbar

                    Console.WriteLine($"{i + 1}. {categories[i]} {(used ? "(belegt)" : locked ? "(LOCKED)" : "")}");
                }
                Console.ResetColor();


                Console.Write("Kategorie #: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int choice) &&
                    choice >= 1 && choice <= categories.Length)
                {
                    string selectedCat = categories[choice - 1];
                    if (sc.IsUsed(selectedCat))
                    {
                        Console.WriteLine("Kategorie bereits belegt!");
                        continue;
                    }
                    int points = EvaluateCategory(selectedCat, dice);
                    if (!sc.LockUsed)
                    {
                        Console.Write("Möchtest du diese Kategorie locken und Punkte verdoppeln? (j/n): ");
                        if (Console.ReadLine().Trim().ToLower() == "j")
                        {
                            sc.LockedCategory = selectedCat;
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"Kategorie {selectedCat} ist jetzt LOCKED!");
                            Console.ResetColor();
                        }
                    }


                    if (points == 0)
                    {
                        // Strafpunkte vergeben
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Falsche Kategorie gewählt! Du erhältst 5 Strafpunkte!");
                        Console.ResetColor();
                        sc.Penalty += 5;

                    }


                    return selectedCat; // gültige Kategorie
                }
                else
                {
                    Console.WriteLine("Ungültige Eingabe!");
                }


            }
        }

        // -------------------------------
        // Kategorieauswahl Computer
        // -------------------------------
        static string ChooseComputerCategory(ScoreCard sc, int[] dice, Difficulty difficulty)
        {
            string[] categories = {
                "Einsen","Zweien","Dreien","Vieren","Fünfen","Sechsen",
                "Paar","Dreierpasch","Viererpasch","Full House",
                "Kleine Straße","Große Straße","Kniffel","Chance",
                "Zwei Paare","Mini Full House","Chance+","Lucky Seven"
            };

            string selectedCat = null;
            int bestEval = int.MinValue;
            int bestRealPoints = 0;

            if (difficulty == Difficulty.Easy)
            {
                foreach (var cat in sc.GetOpenCategories())
                {
                    int pts = EvaluateCategory(cat, dice);
                    int priority = cat switch
                    {
                        "Kniffel" => pts == 50 ? 20 : 0,
                        "Full House" => pts > 0 ? 10 : 0,
                        "Große Straße" => pts > 0 ? 10 : 0,
                        "Kleine Straße" => pts > 0 ? 5 : 0,
                        "Chance+" => pts >= 25 ? 3 : 0,
                        "Mini Full House" => pts > 0 ? 2 : 0,
                        "Lucky Seven" => pts == 7 ? 5 : 0,
                        "Zwei Paare" => pts > 0 ? 2 : 0,
                        "Einsen" => pts > 0 ? 1 : 0,
                        "Zweien" => pts > 0 ? 1 : 0,
                        "Dreien" => pts > 0 ? 1 : 0,
                        "Vieren" => pts > 0 ? 1 : 0,
                        "Fünfen" => pts > 0 ? 1 : 0,
                        "Sechsen" => pts > 0 ? 1 : 0,
                        "Paar" => pts > 0 ? 3 : 0,
                        "Dreierpasch" => pts > 0 ? 5 : 0,
                        "Viererpasch" => pts > 0 ? 5 : 0,
                        "Chance" => pts > 0 ? 2 : 0,
                        _ => 0
                    };
                    int eval = pts + priority;
                    if (eval > bestEval)
                    {
                        bestEval = eval;
                        selectedCat = cat;
                        bestRealPoints = pts;
                    }
                }

                // Zufälliges Locken mit niedriger Wahrscheinlichkeit (z.B. 15%)
                if (!sc.LockUsed && sc.LockedCategory == null && rnd.NextDouble() < 0.15)
                {
                    sc.LockedCategory = selectedCat;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"🤖 Computer LOCKT (zufällig) die Kategorie {selectedCat}!");
                    Console.ResetColor();
                }
            }
            else
            {

                foreach (var cat in sc.GetOpenCategories())
                {
                    if (sc.IsUsed(cat)) continue;
                    {
                        int pts = EvaluateCategory(cat, dice);
                        int priority = 0;

                        if (difficulty == Difficulty.Medium)
                        {
                            priority = cat switch
                            {
                                "Kniffel" => pts == 50 ? 30 : 0,
                                "Full House" => pts > 0 ? 15 : 0,
                                "Große Straße" => pts > 0 ? 15 : 0,
                                "Kleine Straße" => pts > 0 ? 10 : 0,
                                "Chance+" => pts >= 25 ? 5 : 0,
                                "Mini Full House" => pts > 0 ? 5 : 0,
                                "Lucky Seven" => pts == 7 ? 10 : 0,
                                "Zwei Paare" => pts > 0 ? 5 : 0,
                                "Einsen" => pts > 0 ? 1 : 0,
                                "Zweien" => pts > 0 ? 1 : 0,
                                "Dreien" => pts > 0 ? 1 : 0,
                                "Vieren" => pts > 0 ? 1 : 0,
                                "Fünfen" => pts > 0 ? 1 : 0,
                                "Sechsen" => pts > 0 ? 1 : 0,
                                "Paar" => pts > 0 ? 5 : 0,
                                "Dreierpasch" => pts > 0 ? 10 : 0,
                                "Viererpasch" => pts > 0 ? 10 : 0,
                                "Chance" => pts > 0 ? 5 : 0,
                                _ => 0
                            };
                        }
                        else if (difficulty == Difficulty.Hard)
                        {
                            priority = cat switch
                            {
                                "Kniffel" => pts == 50 ? 50 : 0,
                                "Full House" => pts > 0 ? 25 : 0,
                                "Große Straße" => pts > 0 ? 25 : 0,
                                "Kleine Straße" => pts > 0 ? 20 : 0,
                                "Chance+" => pts >= 25 ? 15 : 0,
                                "Mini Full House" => pts > 0 ? 10 : 0,
                                "Lucky Seven" => pts == 7 ? 20 : 0,
                                "Zwei Paare" => pts > 0 ? 10 : 0,
                                "Einsen" => pts > 0 ? 5 : 0,
                                "Zweien" => pts > 0 ? 5 : 0,
                                "Dreien" => pts > 0 ? 5 : 0,
                                "Vieren" => pts > 0 ? 5 : 0,
                                "Fünfen" => pts > 0 ? 5 : 0,
                                "Sechsen" => pts > 0 ? 5 : 0,
                                "Paar" => pts > 0 ? 10 : 0,
                                "Dreierpasch" => pts > 0 ? 15 : 0,
                                "Viererpasch" => pts > 0 ? 20 : 0,
                                "Chance" => pts > 0 ? 10 : 0,
                                _ => 0
                            };
                        }
                        int eval = pts + priority;

                        if (eval > bestEval)
                        {
                            bestEval = eval;
                            selectedCat = cat;
                            bestRealPoints = pts;
                        }
                    }
                }
                // Sicherheitsfallback
                if (selectedCat == null)
                   selectedCat = categories.First(c => !sc.IsUsed(c));

                if (!sc.LockUsed && sc.LockedCategory == null && difficulty != Difficulty.Easy)
                {
                    bool shouldLock = difficulty switch
                    {
                        Difficulty.Medium => bestRealPoints >= 25, // alle Kategorien mit mindestens 25 Punkten
                        Difficulty.Hard => bestRealPoints >= 40,   // alle Kategorien mit mindestens 40 Punkten
                        _ => false
                    };

                    /*switch(difficulty)
                    {
                        case Difficulty.Easy:
                            if (bestScore >= 40) shouldLock = true;
                            break;
                            case Difficulty.Medium:
                            if ((selectedCat == "Kniffel" && bestScore == 50) || 
                                    (selectedCat == "Full House" && bestScore >= 25) || 
                                    (selectedCat == "Große Straße" && bestScore >= 40) ||
                                    (bestScore >= 30))
                                shouldLock = true;
                            break;
                            case Difficulty.Hard:
                            if ((selectedCat == "Kniffel" && bestScore == 50) ||
                                    (selectedCat == "Full House" && bestScore >= 25) ||
                                    (selectedCat == "Große Straße" && bestScore >= 40) ||
                                    (selectedCat == "Lucky Seven" && bestScore == 7) ||
                                    (selectedCat == "Chance+" && bestScore >= 25) ||
                                    (bestScore >= 20))
                                shouldLock = true;
                            break;
                    }*/

                    if (shouldLock)
                    {
                        sc.LockedCategory = selectedCat;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"🤖 Computer LOCKT die Kategorie {selectedCat}!");
                        Console.ResetColor();
                    }
                }
            }


            // Punkte berechnen und eintragen
            int points = EvaluateCategory(selectedCat, dice);

            if (points == 0)
                sc.Penalty += 5; // Strafpunkte für Computer ebenfalls

            return selectedCat;
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
            int[] distinct = dice.Distinct().OrderBy(x => x).ToArray();


            // Hilfsfunktion:
            bool HasRun(int len)
            {
                if (distinct.Length < len) return false;
                int run = 1;
                for (int i = 1; i < distinct.Length; i++)
                {
                    if (distinct[i] == distinct[i - 1] + 1)
                    {
                        run++;
                        if (run >= len) return true;
                    }
                    else
                    {
                        run = 1;
                    }
                }
                return false;
            }

            static int EvaluatePair(int[] count)
            {
                for (int i = 6; i >= 1; i--)
                    if (count[i] >= 2)
                        return i * 2;
                return 0;
            }

            static int EvaluateTwoPairs(int[] count)
            {
                List<int> pairs = new List<int>();

                for (int i = 6; i >= 1; i--)
                {
                    int pairCount = count[i] / 2; // Wie viele Paare möglich sind
                    for (int j = 0; j < pairCount; j++)
                    {
                        pairs.Add(i);
                    }
                }

                if (pairs.Count >= 2)
                {
                    // Summe der **zwei höchsten Paare**
                    return pairs[0] * 2 + pairs[1] * 2;
                }
                return 0;
            }




            return cat switch
            {
                "Einsen" => count[1] * 1,
                "Zweien" => count[2] * 2,
                "Dreien" => count[3] * 3,
                "Vieren" => count[4] * 4,
                "Fünfen" => count[5] * 5,
                "Sechsen" => count[6] * 6,
                //"Paar" => count.Any(c => c >= 2) ? sum : 0,
                "Paar" => EvaluatePair(count),
                "Dreierpasch" => count.Any(c => c >= 3) ? sum : 0,
                "Viererpasch" => count.Any(c => c >= 4) ? sum : 0,
                "Full House" => (count.Contains(3) && count.Contains(2)) ? 25 : 0,
                //"Kleine Straße" => (s.Distinct().SequenceEqual(new[] { 1, 2, 3, 4}) || s.Distinct().SequenceEqual(new[] { 2, 3, 4, 5}) || s.Distinct().SequenceEqual(new[] {3, 4 , 5, 6})) ? 30 : 0, 
                //"Große Straße" => (s.Distinct().SequenceEqual(new[] { 1, 2, 3, 4, 5 }) || s.Distinct().SequenceEqual(new[] {2, 3, 4, 5, 6})) ? 40 : 0,
                // kleine Straße: irgendeine Lauffolge von 4 aufeinanderfolgenden Zahlen in den distinct-Werten
                "Kleine Straße" => HasRun(4) ? 30 : 0,
                // große Straße: Lauffolge von 5
                "Große Straße" => HasRun(5) ? 40 : 0,
                "Kniffel" => count.Any(c => c == 5) ? 50 : 0,
                "Chance" => sum,

                //"Zwei Paare" => count.Count(c => c >= 2) >= 2 ? 15 : 0,
                "Zwei Paare" => EvaluateTwoPairs(count),
                //"Mini Full House" => (count.Count(c => c == 2) == 2) ? 10 : 0,
                "Mini Full House" => count.Contains(3) && count.Count(c => c == 2) == 0 ? 10 : 0,

                "Chance+" => sum >= 20 ? sum + 5 : sum,
                "Lucky Seven" => sum == 7 ? 20 : 0,

                _ => 0
            };
        }

        // BACKUP - ApplyScore Funktion nicht mehr benötigt
        /*static void ApplyScore(ScoreCard score, string cat, int points)
        {
            switch (cat)
            {
                case "Einsen": score.Ones = points; break;
                case "Zweien": score.Twos = points; break;
                case "Dreien": score.Threes = points; break;
                case "Vieren": score.Fours = points; break;
                case "Fünfen": score.Fives = points; break;
                case "Sechsen": score.Sixes = points; break;
                case "Paar": score.Pair = points; break;
                case "Dreierpasch": score.ThreeOfAKind = points; break;
                case "Viererpasch": score.FourOfAKind = points; break;
                case "Full House": score.FullHouse = points; break;
                case "Kleine Straße": score.SmallStraight = points; break;
                case "Große Straße": score.LargeStraight = points; break;
                case "Kniffel": score.Yahtzee = points; break;
                case "Chance": score.Chance = points; break;

                case "Zwei Paare": score.TwoPairs = points; break;
                case "Mini Full House": score.MiniFullHouse = points; break;
                case "Chance+": score.ChancePlus = points; break;
                case "Lucky Seven": score.LuckySeven = points; break;



            }
        }*/

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
        public int? Pair { get; set; }
        public int? ThreeOfAKind { get; set; }
        public int? FourOfAKind { get; set; }
        public int? FullHouse { get; set; }
        public int? SmallStraight { get; set; }
        public int? LargeStraight { get; set; }
        public int? Yahtzee { get; set; }
        public int? Chance { get; set; }
        public int? TwoPairs { get; set; }
        public int? MiniFullHouse { get; set; }
        public int? ChancePlus { get; set; }
        public int? LuckySeven { get; set; }

        public string LockedCategory { get; set; } = null;
        public bool LockUsed { get; set; } = false;

        public int Penalty { get; set; } = 0;

        public int UpperScore => (Ones ?? 0) + (Twos ?? 0) + (Threes ?? 0) + (Fours ?? 0) + (Fives ?? 0) + (Sixes ?? 0);
        public int UpperBonus => UpperScore >= 63 ? 35 : 0;
        public int LowerScore => (Pair ?? 0) + (ThreeOfAKind ?? 0) + (FourOfAKind ?? 0) + (FullHouse ?? 0) + (SmallStraight ?? 0) + (LargeStraight ?? 0) + (Yahtzee ?? 0) + (Chance ?? 0); 

        public int BonusScore => (TwoPairs ?? 0) + (MiniFullHouse ?? 0) + (ChancePlus ?? 0) + (LuckySeven ?? 0);

        public int TotalScore => UpperScore + UpperBonus + LowerScore + BonusScore - Penalty;

        private string LockIcon(string category)
        {
            if (LockedCategory == category && LockUsed)
                return "🔒";
            return "🔓";
        }

        public List<string> GetOpenCategories()
        {
            string[] allcategories = {
                "Einsen", "Zweien", "Dreien", "Vieren", "Fünfen", "Sechsen",
                "Paar", "Dreierpasch", "Viererpasch", "Full House",
                "Kleine Straße", "Große Straße", "Kniffel", "Chance",
                "Zwei Paare", "Mini Full House", "Chance+", "Lucky Seven"
            };
            return allcategories.Where(c => !IsUsed(c)).ToList();
        }   


        public void SetScore(string category, int points)
        {
            if (!string.IsNullOrEmpty(LockedCategory) && category == LockedCategory && !LockUsed)
            {
                points *= 2;
                LockUsed = true;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"In Kategorie {category} wurden die Punkte verdoppelt.");
                Console.ResetColor();
            }

            switch (category)
            {
                case "Einsen": Ones = points; break;
                case "Zweien": Twos = points; break;
                case "Dreien": Threes = points; break;
                case "Vieren": Fours = points; break;
                case "Fünfen": Fives = points; break;
                case "Sechsen": Sixes = points; break;
                case "Paar": Pair = points; break;
                case "Dreierpasch": ThreeOfAKind = points; break;
                case "Viererpasch": FourOfAKind = points; break;
                case "Full House": FullHouse = points; break;
                case "Kleine Straße": SmallStraight = points; break;
                case "Große Straße": LargeStraight = points; break;
                case "Kniffel": Yahtzee = points; break;
                case "Chance": Chance = points; break;
                case "Zwei Paare": TwoPairs = points; break;
                case "Mini Full House": MiniFullHouse = points; break;
                case "Chance+": ChancePlus = points; break;
                case "Lucky Seven": LuckySeven = points; break;
                default: break;
            }
        }



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
                "Paar" => Pair.HasValue,
                "Dreierpasch" => ThreeOfAKind.HasValue,
                "Viererpasch" => FourOfAKind.HasValue,
                "Full House" => FullHouse.HasValue,
                "Kleine Straße" => SmallStraight.HasValue,
                "Große Straße" => LargeStraight.HasValue,
                "Kniffel" => Yahtzee.HasValue,
                "Chance" => Chance.HasValue,

                "Zwei Paare" => TwoPairs.HasValue,
                "Mini Full House" => MiniFullHouse.HasValue,
                "Chance+" => ChancePlus.HasValue,
                "Lucky Seven" => LuckySeven.HasValue,


                _ => false
            };
        }
       
        private int getMaxCategoryLength()
            {
            string[] categories = {
                "Einsen","Zweien","Dreien","Vieren","Fünfen","Sechsen",
                "Paar","Dreierpasch","Viererpasch","Full House",
                "Kleine Straße","Große Straße","Kniffel","Chance",
                "Zwei Paare","Mini Full House","Chance+","Lucky Seven"
            };
            int maxlen = 0;
            foreach (var cat in categories)
            {
                int len = (LockIcon(cat) + " " + cat + ":").Length;
                if (len > maxlen)
                    maxlen = len;
            }
            return maxlen;
        }


        // Hilfsfunktion zum Ausdrucken der Punktetabelle
        private void PrintLine(string category, int? value)
        {
            int colWidth = getMaxCategoryLength() + 2; 
            string left = $"{LockIcon(category)} {category}:";
            Console.WriteLine($"{left.PadRight(colWidth)} {value}");
        }
        public void Print()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n===== PUNKTETABELLE =====");
            Console.WriteLine("\n--- Obere Sektion ---");
            Console.ResetColor();
            PrintLine("Zweien", Twos);
            PrintLine("Dreien", Threes);
            PrintLine("Vieren", Fours);
            PrintLine("Fünfen", Fives);
            PrintLine("Sechsen", Sixes);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            PrintLine("Obere Sektion", UpperScore);
            PrintLine("Bonus (63+)", UpperBonus);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n--- Untere Sektion ---");
            Console.ResetColor();
            PrintLine("Paar", Pair);
            PrintLine("Dreierpasch", ThreeOfAKind);
            PrintLine("Viererpasch", FourOfAKind);
            PrintLine("Full House", FullHouse);
            PrintLine("Kleine Straße", SmallStraight);
            PrintLine("Große Straße", LargeStraight);
            PrintLine("Kniffel", Yahtzee);
            PrintLine("Chance", Chance);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            PrintLine("Untere Sektion", LowerScore);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n--- Bonus Sektion ---");
            Console.ResetColor();
            PrintLine("Zwei Paare", TwoPairs);
            PrintLine("Mini Full House", MiniFullHouse);
            PrintLine("Chance+", ChancePlus);
            PrintLine("Lucky Seven", LuckySeven);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            PrintLine("Bonus Sektion", BonusScore);
            Console.ResetColor();
            Console.WriteLine("==========================\n");
            Console.ForegroundColor = ConsoleColor.Red;
            PrintLine("Strafpunkte", Penalty);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            PrintLine("GESAMTPUNKTE", TotalScore);
            Console.ResetColor();
            Console.WriteLine("==========================\n");
        }
    }
}