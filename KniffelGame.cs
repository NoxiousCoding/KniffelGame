using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static System.Formats.Asn1.AsnWriter;

namespace KniffelConsole
{
    // Neuer Enum für Spielmodi
    public enum GameMode
    {
        Classic,        // Original-Kniffel (13 Kategorien)
        BonusEdition,   // Mit Bonus-Kategorien (aktuelle Version)
        SpeedRun,       // Weniger Runden, schnelleres Spiel
        TeamMode,       // Teams statt Einzelspieler
        Tournament,     // Turniermodus mit mehreren Spielen
        Custom          // Benutzerdefinierte Regeln
    }

    // Neue Klasse für Spieleinstellungen
    public class GameSettings
    {
        public GameMode Mode { get; set; } = GameMode.BonusEdition;
        public int Rounds { get; set; } = 13;
        public bool AllowLocking { get; set; } = true;
        public bool UsePenalty { get; set; } = true;
        public bool UseUpperBonus { get; set; } = true;
        public int UpperBonusThreshold { get; set; } = 63;
        public int MaxLocks { get; set; } = 1;
        public int TeamCount { get; set; } = 2;
        public int TournamentGames { get; set; } = 3;
        public bool IncludeComputer { get; set; } = false;
        public KniffelGame.Difficulty ComputerDifficulty { get; set; } = KniffelGame.Difficulty.Medium;
        public bool AnimateDice { get; set; } = true;
        public bool UsePaar { get; set; } = true;
        public bool UseTwoPairs { get; set; } = true;
        public bool UseMiniFullHouse { get; set; } = true;
        public bool UseChancePlus { get; set; } = true;
        public bool UseLuckySeven { get; set; } = true;
        public bool UseAnyBonusCategories => UsePaar || UseTwoPairs || UseMiniFullHouse || UseChancePlus || UseLuckySeven;

        //Abwärtskompatibilität(optional)
        [Obsolete("Use individual category properties instead")]
        public bool UseBonusCategories
        {
            get => UseAnyBonusCategories;
            set
            {
                UsePaar = value;
                UseTwoPairs = value;
                UseMiniFullHouse = value;
                UseChancePlus = value;
                UseLuckySeven = value;
            }
        }
    }

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
                Console.WriteLine("          LOVE HER C ❤️           ");
                Console.WriteLine("==================================");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("1. Neues Spiel starten");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("2. Spielmodus auswählen");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("3. Regeln anzeigen");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("4. Beenden");
                Console.ResetColor();

                Console.WriteLine("------------------------------------");
                Console.Write("Auswahl: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        // Standardmodus starten
                        var settings = new GameSettings();
                        settings.Mode = GameMode.BonusEdition;
                        KniffelGame.RunGame(settings);
                        break;

                    case "2":
                        ShowGameModeSelection();
                        break;

                    case "3":
                        ShowRules();
                        break;

                    case "4":
                        Console.WriteLine("Spiel wird beendet...");
                        return;

                    default:
                        Console.WriteLine("Ungültige Eingabe! ENTER drücken...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        static void ShowGameModeSelection()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=========== SPIELMODI ===========");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("1. Klassischer Modus");
                Console.WriteLine("   - Original Kniffel mit 13 Kategorien");
                Console.WriteLine("   - Ohne Bonus-Sektion");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("2. Bonus Edition (Standard)");
                Console.WriteLine("   - Mit Bonus-Kategorien");
                Console.WriteLine("   - Paar, Zwei Paare, Mini Full House, Chance+, Lucky Seven");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("3. Speed Run");
                Console.WriteLine("   - Nur 8 Runden statt 13");
                Console.WriteLine("   - Schnelleres Spiel");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("4. Team Modus");
                Console.WriteLine("   - Spieler in Teams");
                Console.WriteLine("   - Gemeinsame Strategie");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("5. Turnier Modus");
                Console.WriteLine("   - Mehrere Spiele hintereinander");
                Console.WriteLine("   - Gesamtpunktzahl zählt");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("6. Benutzerdefiniert");
                Console.WriteLine("   - Eigene Regeln erstellen");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("7. Zurück zum Hauptmenü");
                Console.ResetColor();

                Console.WriteLine("------------------------------------");
                Console.Write("Auswahl: ");

                string input = Console.ReadLine().Trim();

                if (input == "7") return;

                GameSettings settings;

                switch (input)
                {
                    case "1":
                        settings = new GameSettings
                        {
                            Mode = GameMode.Classic,
                            Rounds = 13,
                            UsePaar = false,
                            UseTwoPairs = false,
                            UseMiniFullHouse = false,
                            UseChancePlus = false,
                            UseLuckySeven = false
                        };
                        break;

                    case "2":
                        settings = new GameSettings
                        {
                            Mode = GameMode.BonusEdition,
                            Rounds = 18
                        };
                        break;

                    case "3":
                        settings = new GameSettings
                        {
                            Mode = GameMode.SpeedRun,
                            Rounds = 8,
                            AllowLocking = false,
                            AnimateDice = false,
                            UsePaar = false,
                            UseTwoPairs = false,
                            UseMiniFullHouse = false,
                            UseChancePlus = false,
                            UseLuckySeven = false
                        };
                        break;

                    case "4":
                        settings = new GameSettings
                        {
                            Mode = GameMode.TeamMode
                        };
                        Console.Write("Anzahl Teams (2-4): ");
                        int teamCount;
                        while (!int.TryParse(Console.ReadLine(), out teamCount) || teamCount < 2 || teamCount > 4)
                        {
                            Console.Write("Bitte 2-4 eingeben: ");
                        }
                        settings.TeamCount = teamCount;
                        break;

                    case "5":
                        settings = new GameSettings
                        {
                            Mode = GameMode.Tournament
                        };
                        Console.Write("Anzahl Spiele im Turnier (1-5): ");
                        int gamesCount;
                        while (!int.TryParse(Console.ReadLine(), out gamesCount) || gamesCount < 1 || gamesCount > 5)
                        {
                            Console.Write("Bitte 1-5 eingeben: ");
                        }
                        settings.TournamentGames = gamesCount;
                        break;

                    case "6":
                        ConfigureAndRunCustomMode();
                        return;

                    default:
                        Console.WriteLine("Ungültige Eingabe! ENTER drücken...");
                        Console.ReadLine();
                        continue;
                }

                AskAdditionalSettings(settings);


                KniffelGame.RunGame(settings);
                break;
            }
        }

        static void AskAdditionalSettings(GameSettings settings)
        { 
            if (settings.Mode != GameMode.SpeedRun)
            {
                Console.Write("Punkte-Locking aktivieren? (j/n, Standard j): ");
                settings.AllowLocking = Console.ReadLine().Trim().ToLower() == "j";
                Console.Write("Strafpunkte aktivieren? (j/n, Standard j): ");
                settings.UsePenalty = Console.ReadLine().Trim().ToLower() == "j";
            }

            if (settings.Mode != GameMode.Classic)
            {
                Console.Write("Würfel-Animation aktivieren? (j/n, Standard j): ");
                settings.AnimateDice = Console.ReadLine().Trim().ToLower() == "j";
            }

            if (settings.Mode != GameMode.TeamMode)
            {
                Console.Write("Computer-Gegner hinzufügen? (j/n, Standard n): ");
                settings.IncludeComputer = Console.ReadLine().Trim().ToLower() == "j";

                if (settings.IncludeComputer)
                {
                    Console.Write("KI Schwierigkeit (1=Einfach, 2=Mittel, 3=Schwer, Standard 2): ");
                    int diffInput;
                    while (!int.TryParse(Console.ReadLine(), out diffInput) || diffInput < 1 || diffInput > 3)
                    {
                        Console.Write("Bitte 1-3 eingeben: ");
                    }
                    settings.ComputerDifficulty = diffInput switch
                    {
                        1 => KniffelGame.Difficulty.Easy,
                        2 => KniffelGame.Difficulty.Medium,
                        3 => KniffelGame.Difficulty.Hard,
                        _ => KniffelGame.Difficulty.Medium
                    };
                }
            }
        }

        static void ConfigureAndRunCustomMode()
        {
            var settings = new GameSettings
            {
                Mode = GameMode.Custom
            };
            ConfigureCustomMode(ref settings);
            KniffelGame.RunGame(settings);
        }

        static void ConfigureCustomMode(ref GameSettings settings)
        {
            Console.Clear();
            Console.WriteLine("=== Benutzerdefinierte Einstellungen ===");

            // Temporäre Variablen für die Eingabe
            int tempValue;
            string input;

            // Anzahl Runden
            Console.Write("Anzahl Runden (bis 18, Standard 13): ");
            input = Console.ReadLine();
            if (int.TryParse(input, out tempValue) && tempValue >= 5 && tempValue <= 20)
            {
                settings.Rounds = tempValue;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(input))
                    Console.WriteLine($"Ungültige Eingabe '{input}'. Verwende Standardwert: 13");
                settings.Rounds = 13;
            }

            // Locking
            Console.Write("Punkte-Locking aktivieren? (j/n, Standard j): ");
            input = Console.ReadLine().Trim().ToLower();
            settings.AllowLocking = string.IsNullOrEmpty(input) || input == "j";

            if (settings.AllowLocking)
            {
                Console.Write("Maximale Locks pro Spieler (1-3, Standard 1): ");
                input = Console.ReadLine();
                if (int.TryParse(input, out tempValue) && tempValue >= 1 && tempValue <= 3)
                {
                    settings.MaxLocks = tempValue;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(input))
                        Console.WriteLine($"Ungültige Eingabe '{input}'. Verwende Standardwert: 1");
                    settings.MaxLocks = 1;
                }
            }
            else
            {
                settings.MaxLocks = 0; // Keine Locks wenn Locking deaktiviert
            }

            // Strafpunkte
            Console.Write("Strafpunkte aktivieren? (j/n, Standard j): ");
            input = Console.ReadLine().Trim().ToLower();
            settings.UsePenalty = string.IsNullOrEmpty(input) || input == "j";

            // Oberer Bonus
            Console.Write("Obere Sektion Bonus aktivieren? (j/n, Standard j): ");
            input = Console.ReadLine().Trim().ToLower();
            settings.UseUpperBonus = string.IsNullOrEmpty(input) || input == "j";

            if (settings.UseUpperBonus)
            {
                Console.Write("Bonus-Schwelle (Standard 63): ");
                input = Console.ReadLine();
                if (int.TryParse(input, out tempValue) && tempValue >= 1)
                {
                    settings.UpperBonusThreshold = tempValue;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(input))
                        Console.WriteLine($"Ungültige Eingabe '{input}'. Verwende Standardwert: 63");
                    settings.UpperBonusThreshold = 63;
                }
            }
            else
            {
                settings.UpperBonusThreshold = 0; // Keine Schwelle wenn Bonus deaktiviert
            }

            // Bonus-Kategorien
            Console.WriteLine("\n=== BONUS-KATEGORIEN ===");
            Console.Write("Paar aktivieren? (j/n, Standard j): ");
            input = Console.ReadLine().Trim().ToLower();
            settings.UsePaar = string.IsNullOrEmpty(input) || input == "j";
            Console.Write("Zwei Paare aktivieren? (j/n, Standard j): ");
            input = Console.ReadLine().Trim().ToLower();
            settings.UseTwoPairs = string.IsNullOrEmpty(input) || input == "j";
            Console.Write("Mini Full House aktivieren? (j/n, Standart j) ");
            input = Console.ReadLine().Trim().ToLower();
            settings.UseMiniFullHouse = string.IsNullOrEmpty(input) || input == "j";
            Console.Write("Chance+ aktivieren? (j/n, Standard j): ");
            input = Console.ReadLine().Trim().ToLower();
            settings.UseChancePlus = string.IsNullOrEmpty(input) || input == "j";
            Console.Write("Lucky Seven aktivieren? (j/n, Standard j): ");
            input = Console.ReadLine().Trim().ToLower();
            settings.UseLuckySeven = string.IsNullOrEmpty(input) || input == "j";


            // Animation
            Console.Write("Würfel-Animation aktivieren? (j/n, Standard j): ");
            input = Console.ReadLine().Trim().ToLower();
            settings.AnimateDice = string.IsNullOrEmpty(input) || input == "j";

            // Computer-Gegner
            Console.Write("Computer-Gegner hinzufügen? (j/n, Standard n): ");
            input = Console.ReadLine().Trim().ToLower();
            settings.IncludeComputer = input == "j";

            if (settings.IncludeComputer)
            {
                Console.Write("KI Schwierigkeit (1=Einfach, 2=Mittel, 3=Schwer, Standard 2): ");
                input = Console.ReadLine();
                if (int.TryParse(input, out tempValue) && tempValue >= 1 && tempValue <= 3)
                {
                    settings.ComputerDifficulty = tempValue switch
                    {
                        1 => KniffelGame.Difficulty.Easy,
                        2 => KniffelGame.Difficulty.Medium,
                        3 => KniffelGame.Difficulty.Hard,
                        _ => KniffelGame.Difficulty.Medium
                    };
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(input))
                        Console.WriteLine($"Ungültige Eingabe '{input}'. Verwende Standardwert: Mittel");
                    settings.ComputerDifficulty = KniffelGame.Difficulty.Medium;
                }
            }
            else
            {
                //settings.ComputerDifficulty = KniffelGame.Difficulty.Medium; // Standard, wird aber nicht verwendet
            }

            // Zusammenfassung anzeigen
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== ZUSAMMENFASSUNG DER EINSTELLUNGEN ===");
            Console.ResetColor();

            Console.WriteLine($"\n📊 GRUNDEINSTELLUNGEN:");
            Console.WriteLine($"   • Runden: {settings.Rounds}");
            Console.WriteLine($"   • Würfel-Animation: {(settings.AnimateDice ? "✅ Aktiv" : "❌ Inaktiv")}");

            Console.WriteLine($"\n⚙️ REGELN:");
            Console.WriteLine($"   • Locking: {(settings.AllowLocking ? $"✅ Aktiv (max {settings.MaxLocks} Locks)" : "❌ Inaktiv")}");
            Console.WriteLine($"   • Strafpunkte: {(settings.UsePenalty ? "✅ Aktiv" : "❌ Inaktiv")}");
            Console.WriteLine($"   • Oberer Bonus: {(settings.UseUpperBonus ? $"✅ Aktiv (ab {settings.UpperBonusThreshold} Punkten)" : "❌ Inaktiv")}");

            Console.WriteLine($"\n🎲 BONUS-KATEGORIEN:");
            Console.WriteLine($"   • Paar: {(settings.UsePaar ? "✅" : "❌")}");
            Console.WriteLine($"   • Zwei Paare: {(settings.UseTwoPairs ? "✅" : "❌")}");
            Console.WriteLine($"   • Mini Full House: {(settings.UseMiniFullHouse ? "✅" : "❌")}");
            Console.WriteLine($"   • Chance+: {(settings.UseChancePlus ? "✅" : "❌")}");
            Console.WriteLine($"   • Lucky Seven: {(settings.UseLuckySeven ? "✅" : "❌")}");

            Console.WriteLine($"\n🤖 GEGNER:");
            Console.WriteLine($"   • Computer: {(settings.IncludeComputer ? $"✅ Aktiv ({settings.ComputerDifficulty})" : "❌ Inaktiv")}");

            Console.WriteLine($"\n🎮 SPIELMODUS: BENUTZERDEFINIERT");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\nDiese Einstellungen werden nun für das Spiel verwendet.");
            Console.ResetColor();

            Console.WriteLine("\nENTER zum Spiel starten...");
            Console.ReadLine();
        }

        static void ShowRules()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("============== REGELN ==============");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Magenta;
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
                    Console.WriteLine("\n=== SPIELMODI ===");
                    Console.WriteLine("• Klassisch: Original Kniffel");
                    Console.WriteLine("• Bonus Edition: Mit zusätzlichen Kategorien");
                    Console.WriteLine("• Speed Run: Nur 8 Runden, schnelleres Spiel");
                    Console.WriteLine("• Team Modus: Spieler in Teams");
                    Console.WriteLine("• Turnier: Mehrere Spiele, Gesamtpunktzahl");
                    Console.WriteLine("• Benutzerdefiniert: Eigene Regeln");
                    Console.WriteLine("\nENTER zum Zurückkehren...");
                    Console.ReadLine();
                    break;

                case "2":
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("=========== KATEGORIEN ===========");
                    Console.ResetColor();
                    Console.WriteLine("• Einsen bis Sechsen: Summe aller gewürfelten Augen dieser Zahl");
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
                    Console.WriteLine("• Paar: Mindestens ein Paar → Summe des höchsten Paars ");
                    Console.WriteLine("• Zwei Paare: Zwei verschiedene Paare → Summe beider Paare");
                    Console.WriteLine("• Mini-Full-House: 3 Gleiche + 2 unterschiedliche Würfel → 10 Punkte");
                    Console.WriteLine("• Chance+: Summe aller Würfel + 5 Bonuspunkte, wenn Summe ≥ 20");
                    Console.WriteLine("• Lucky Seven: Alle Würfel ergeben zusammen 7 → 20 Punkte");
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

        public enum Difficulty
        {
            Easy,
            Medium,
            Hard
        }

        // Haupt-Spielmethode mit Settings-Parameter
        public static void RunGame(GameSettings settings)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"=== {GetModeName(settings.Mode)} ===");
            Console.ResetColor();

            switch (settings.Mode)
            {
                case GameMode.Classic:
                    RunClassicMode(settings);
                    break;

                case GameMode.BonusEdition:
                    RunBonusEdition(settings);
                    break;

                case GameMode.SpeedRun:
                    RunSpeedRun(settings);
                    break;

                case GameMode.TeamMode:
                    RunTeamMode(settings);
                    break;

                case GameMode.Tournament:
                    RunTournament(settings);
                    break;

                case GameMode.Custom:
                    RunCustomMode(settings);
                    break;
            }
        }

        static string GetModeName(GameMode mode)
        {
            return mode switch
            {
                GameMode.Classic => "KLASSISCHER MODUS",
                GameMode.BonusEdition => "BONUS EDITION",
                GameMode.SpeedRun => "SPEED RUN",
                GameMode.TeamMode => "TEAM MODUS",
                GameMode.Tournament => "TURNIER MODUS",
                GameMode.Custom => "BENUTZERDEFINIERT",
                _ => "KNIFEEL"
            };
        }

        // Klassischer Modus
        static void RunClassicMode(GameSettings settings)
        {
            RunStandardGame(settings);
        }

        // Bonus Edition (Standard)
        static void RunBonusEdition(GameSettings settings)
        {
            RunStandardGame(settings);
        }

        // Speed Run Modus
        static void RunSpeedRun(GameSettings settings)
        {
            Console.WriteLine($"Speed Run - Nur {settings.Rounds} Runden!");
            RunStandardGame(settings);
        }

        // Team Modus
        static void RunTeamMode(GameSettings settings)
        {
            Console.WriteLine($"=== TEAM MODUS - {settings.TeamCount} TEAMS ===");

            // Team-Namen eingeben
            string[] teamNames = new string[settings.TeamCount];
            ScoreCard[] teamScores = new ScoreCard[settings.TeamCount];

            for (int i = 0; i < settings.TeamCount; i++)
            {
                Console.Write($"Name Team {i + 1}: ");
                teamNames[i] = Console.ReadLine().Trim();
                teamScores[i] = new ScoreCard(settings);
            }

            // Spieler pro Team
            int playersPerTeam = 2; // Standard: 2 Spieler pro Team
            if (settings.TeamCount == 2)
            {
                Console.Write("Spieler pro Team (1-3): ");
                while (!int.TryParse(Console.ReadLine(), out playersPerTeam) || playersPerTeam < 1 || playersPerTeam > 3)
                {
                    Console.Write("Bitte 1-3 eingeben: ");
                }
            }

            List<string> allPlayerNames = new List<string>();
            for (int t = 0; t < settings.TeamCount; t++)
            {
                for (int p = 0; p < playersPerTeam; p++)
                {
                    Console.Write($"Name Spieler {p + 1} in Team {teamNames[t]}: ");
                    allPlayerNames.Add($"{teamNames[t]}: {Console.ReadLine().Trim()}");
                }
            }

            // Spielablauf
            for (int round = 1; round <= settings.Rounds; round++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n==== RUNDE {round}/{settings.Rounds} ====");
                Console.ResetColor();

                // Jeder Spieler spielt einmal pro Runde
                for (int playerIndex = 0; playerIndex < allPlayerNames.Count; playerIndex++)
                {
                    string playerName = allPlayerNames[playerIndex];
                    int teamIndex = playerIndex / playersPerTeam;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n-- {playerName} --");
                    Console.ResetColor();

                    int[] dice = PlayPlayerRound(settings);

                    // Team-Strategie: Teammitglieder können sich beraten
                    if (playersPerTeam > 1)
                    {
                        Console.Write($"Team {teamNames[teamIndex]}-Beratung (ENTER wenn fertig): ");
                        Console.ReadLine();
                    }

                    ScoreCard teamScore = teamScores[teamIndex];
                    string category = ChooseCategory(teamScore, dice, settings, true);
                    int points = EvaluateCategory(category, dice, settings);

                    teamScore.SetScore(category, points, settings);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    if (category == teamScore.LockedCategory && teamScore.LockUsed)
                        Console.WriteLine($"{playerName} belegt {category} (LOCKED) mit {points * 2} Punkten für Team {teamNames[teamIndex]}");
                    else
                        Console.WriteLine($"{playerName} belegt {category} mit {points} Punkten für Team {teamNames[teamIndex]}");
                    Console.ResetColor();

                    // Team-Punktetabelle anzeigen
                    Console.WriteLine($"\n=== AKTUELLE TEAM-STÄNDE ===");
                    var teamRanking = teamScores.Select((s, i) => new { Name = teamNames[i], Score = s.TotalScore })
                                               .OrderByDescending(x => x.Score)
                                               .ToArray();
                    for (int t = 0; t < settings.TeamCount; t++)
                    {
                        Console.ForegroundColor = t == 0 ? ConsoleColor.Green : ConsoleColor.White;
                        Console.WriteLine($"{t + 1}. {teamRanking[t].Name}: {teamRanking[t].Score} Punkte");
                        Console.ResetColor();
                    }

                    Console.WriteLine("\nENTER für nächsten Spieler...");
                    Console.ReadLine();
                }
            }

            ShowFinalResultsTeam(teamNames, teamScores);
        }

        // Turnier Modus
        static void RunTournament(GameSettings settings)
        {
            Console.WriteLine($"=== TURNIER MODUS - {settings.TournamentGames} SPIELE ===");

            int numPlayers = 0;
            while (numPlayers < 1 || numPlayers > 4)
            {
                Console.Write("Anzahl menschlicher Spieler (1-4): ");
                int.TryParse(Console.ReadLine(), out numPlayers);
            }

            string[] playerNames = new string[numPlayers];
            int[] tournamentScores = new int[numPlayers];

            for (int i = 0; i < numPlayers; i++)
            {
                Console.Write($"Name Spieler {i + 1}: ");
                playerNames[i] = Console.ReadLine().Trim();
            }

            // Mehrere Spiele spielen
            for (int game = 1; game <= settings.TournamentGames; game++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n=== SPIEL {game}/{settings.TournamentGames} ===");
                Console.ResetColor();

                ScoreCard[] gameScores = new ScoreCard[numPlayers];
                for (int i = 0; i < numPlayers; i++)
                    gameScores[i] = new ScoreCard(settings);

                // Einzelnes Spiel durchführen
                for (int round = 1; round <= settings.Rounds; round++)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n--- RUNDE {round}/{settings.Rounds} ---");
                    Console.ResetColor();

                    for (int p = 0; p < numPlayers; p++)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n-- {playerNames[p]} --");
                        Console.ResetColor();

                        int[] dice = PlayPlayerRound(settings);
                        string category = ChooseCategory(gameScores[p], dice, settings, false);
                        int points = EvaluateCategory(category, dice, settings);
                        gameScores[p].SetScore(category, points, settings);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        if (category == gameScores[p].LockedCategory && gameScores[p].LockUsed)
                            Console.WriteLine($"{category}: {points * 2} Punkte (LOCKED)");
                        else
                            Console.WriteLine($"{category}: {points} Punkte");
                        Console.ResetColor();

                        if (round < settings.Rounds || p < numPlayers - 1)
                        {
                            Console.WriteLine("ENTER für nächsten Spieler...");
                            Console.ReadLine();
                        }
                    }
                }

                // Punkte für dieses Spiel zum Turnierstand addieren
                for (int p = 0; p < numPlayers; p++)
                {
                    tournamentScores[p] += gameScores[p].TotalScore;
                }

                // Zwischenstand anzeigen
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n=== TURNIER-ZWISCHENSTAND ===");
                Console.ResetColor();
                var ranking = tournamentScores.Select((s, i) => new { Name = playerNames[i], Score = s })
                                            .OrderByDescending(x => x.Score)
                                            .ToArray();
                for (int i = 0; i < ranking.Length; i++)
                {
                    Console.ForegroundColor = i == 0 ? ConsoleColor.Green :
                                             i == 1 ? ConsoleColor.Yellow :
                                             i == 2 ? ConsoleColor.Magenta : ConsoleColor.White;
                    Console.WriteLine($"{i + 1}. {ranking[i].Name}: {ranking[i].Score} Punkte");
                }
                Console.ResetColor();

                if (game < settings.TournamentGames)
                {
                    Console.WriteLine("\nENTER für nächstes Spiel...");
                    Console.ReadLine();
                }
            }

            // Finales Turnier-Ergebnis
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n====== TURNIER ENDE ======");
            Console.ResetColor();

            var finalRanking = tournamentScores.Select((s, i) => new { Name = playerNames[i], Score = s })
                                             .OrderByDescending(x => x.Score)
                                             .ToArray();

            Console.WriteLine("=== FINALE TURNIERPLATZIERUNG ===");
            for (int i = 0; i < finalRanking.Length; i++)
            {
                Console.ForegroundColor = i == 0 ? ConsoleColor.Green :
                                         i == 1 ? ConsoleColor.Yellow :
                                         i == 2 ? ConsoleColor.Magenta : ConsoleColor.White;
                Console.WriteLine($"{i + 1}. {finalRanking[i].Name}: {finalRanking[i].Score} Punkte");
            }
            Console.ResetColor();

            // Turniersieger ausrufen
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n🎉 TURNIERSIEGER: {finalRanking[0].Name} 🎉");
            Console.ResetColor();

            Console.WriteLine("\nENTER zum Menü...");
            Console.ReadLine();
        }

        // Benutzerdefinierter Modus
        static void RunCustomMode(GameSettings settings)
        {
            Console.WriteLine("=== BENUTZERDEFINIERTES SPIEL ===");
            Console.WriteLine($"• Runden: {settings.Rounds}");
            Console.WriteLine($"• Locking: {(settings.AllowLocking ? "Aktiv" : "Inaktiv")}");
            Console.WriteLine($"• Strafpunkte: {(settings.UsePenalty ? "Aktiv" : "Inaktiv")}");
            Console.WriteLine($"• Bonus-Kategorien: {(settings.UseAnyBonusCategories ? "Aktiv" : "Inaktiv")}");
            Console.WriteLine($"• Animation: {(settings.AnimateDice ? "Aktiv" : "Inaktiv")}");

            RunStandardGame(settings);
        }

        // Standard-Spielablauf (von den meisten Modi genutzt)
        static void RunStandardGame(GameSettings settings)
        {
            int numPlayers = 0;
            while (numPlayers < 1 || numPlayers > 4)
            {
                Console.Write("Anzahl menschlicher Spieler (1-4): ");
                int.TryParse(Console.ReadLine(), out numPlayers);
            }

            int totalPlayers = numPlayers;
            if (settings.IncludeComputer)
            {
                totalPlayers++;
            }

            string[] playerNames = new string[totalPlayers];
            ScoreCard[] scores = new ScoreCard[totalPlayers];

            for (int i = 0; i < numPlayers; i++)
            {
                Console.Write($"Name Spieler {i + 1}: ");
                playerNames[i] = Console.ReadLine().Trim();
                scores[i] = new ScoreCard(settings);
            }

            if (settings.IncludeComputer)
            {
                playerNames[totalPlayers - 1] = "🤖 Computer";
                scores[totalPlayers - 1] = new ScoreCard(settings);
            }

            for (int round = 1; round <= settings.Rounds; round++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n==== RUNDE {round}/{settings.Rounds} ====");
                Console.ResetColor();

                for (int p = 0; p < totalPlayers; p++)
                {
                    Console.ForegroundColor = playerNames[p] == "🤖 Computer" ? ConsoleColor.Red : ConsoleColor.Green;
                    Console.WriteLine($"\n-- ZUG: {playerNames[p]} --");
                    Console.ResetColor();

                    int[] dice = playerNames[p] == "🤖 Computer" ?
                        PlayComputerRound(settings) : PlayPlayerRound(settings);

                    string category = playerNames[p] == "🤖 Computer" ?
                        ChooseComputerCategory(scores[p], dice, settings.ComputerDifficulty, settings) :
                        ChooseCategory(scores[p], dice, settings, false);

                    int points = EvaluateCategory(category, dice, settings);

                    scores[p].SetScore(category, points, settings);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    if (category == scores[p].LockedCategory && scores[p].LockUsed)
                        Console.WriteLine($"{playerNames[p]} belegt {category} (LOCKED) mit {points * 2} Punkten.");
                    else
                        Console.WriteLine($"{playerNames[p]} belegt {category} mit {points} Punkten.");
                    Console.ResetColor();

                    scores[p].Print(settings);
                    Console.WriteLine("ENTER für nächsten Spieler...");
                    Console.ReadLine();
                }
            }

            ShowFinalResults(playerNames, scores, settings);
        }

        // Finale Ergebnisse für Standardspiel
        static void ShowFinalResults(string[] playerNames, ScoreCard[] scores, GameSettings settings)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n====== SPIEL ENDE ======");
            Console.ResetColor();

            var ranking = scores.Select((s, i) => new { Name = playerNames[i], Score = s.TotalScore })
                                .OrderByDescending(x => x.Score)
                                .ToArray();

            Console.WriteLine("=== ENDSTAND ===");
            for (int i = 0; i < ranking.Length; i++)
            {
                Console.ForegroundColor = i == 0 ? ConsoleColor.Green :
                                         i == 1 ? ConsoleColor.Yellow :
                                         i == 2 ? ConsoleColor.Magenta : ConsoleColor.White;
                Console.WriteLine($"{i + 1}. {ranking[i].Name}: {ranking[i].Score} Punkte");
            }
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n🎉 SIEGER: {ranking[0].Name} 🎉");
            Console.ResetColor();

            // Besondere Auszeichnungen
            Console.WriteLine("\n=== BESONDERE AUSZEICHNUNGEN ===");

            // Höchste Einzelrunde
            var highestSingleScore = scores.SelectMany(s => s.GetAllScores())
                                          .OrderByDescending(x => x.Value)
                                          .FirstOrDefault();
            if (highestSingleScore.Key != null)
            {
                Console.WriteLine($"🏆 Höchste Einzelwertung: {highestSingleScore.Key} ({highestSingleScore.Value} Punkte)");
            }

            // Meiste Locks (wenn aktiv)
            if (settings.AllowLocking)
            {
                var mostLocks = scores.Select((s, i) => new { Name = playerNames[i], Locks = s.LocksUsed })
                                     .OrderByDescending(x => x.Locks)
                                     .FirstOrDefault();
                if (mostLocks != null && mostLocks.Locks > 0)
                {
                    Console.WriteLine($"🔒 Meiste Locks: {mostLocks.Name} ({mostLocks.Locks}x)");
                }
            }

            Console.WriteLine("\nENTER zum Menü...");
            Console.ReadLine();
        }

        // Finale Ergebnisse für Teamspiel
        static void ShowFinalResultsTeam(string[] teamNames, ScoreCard[] teamScores)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n====== TEAM-SPIEL ENDE ======");
            Console.ResetColor();

            var ranking = teamScores.Select((s, i) => new { Name = teamNames[i], Score = s.TotalScore })
                                   .OrderByDescending(x => x.Score)
                                   .ToArray();

            Console.WriteLine("=== FINALE TEAM-STÄNDE ===");
            for (int i = 0; i < ranking.Length; i++)
            {
                Console.ForegroundColor = i == 0 ? ConsoleColor.Green :
                                         i == 1 ? ConsoleColor.Yellow :
                                         i == 2 ? ConsoleColor.Magenta : ConsoleColor.White;
                Console.WriteLine($"{i + 1}. {ranking[i].Name}: {ranking[i].Score} Punkte");
            }
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n🏆 GEWINNENDE MANNSCHAFT: {ranking[0].Name} 🏆");
            Console.ResetColor();

            Console.WriteLine("\nENTER zum Menü...");
            Console.ReadLine();
        }

        // -------------------------------
        // Spielerzug
        // -------------------------------
        static int[] PlayPlayerRound(GameSettings settings)
        {
            int[] dice = new int[5];
            bool[] keep = new bool[5];

            for (int roll = 1; roll <= 3; roll++)
            {
                for (int i = 0; i < 5; i++)
                    if (!keep[i])
                        dice[i] = rnd.Next(1, 7);

                Console.WriteLine($"\nWurf {roll}:");
                if (settings.AnimateDice)
                    AnimateDice(dice);
                else
                    PrintDice(dice, ConsoleColor.Red);

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
        static int[] PlayComputerRound(GameSettings settings)
        {
            int[] dice = new int[5];
            bool[] keep = new bool[5];

            for (int roll = 1; roll <= 3; roll++)
            {
                for (int i = 0; i < 5; i++)
                    if (!keep[i])
                        dice[i] = rnd.Next(1, 7);

                Console.WriteLine($"\nWurf {roll}:");
                if (settings.AnimateDice)
                    AnimateDice(dice);
                else
                    PrintDice(dice, ConsoleColor.Red);

                if (roll < 3)
                    keep = DecideDiceToKeep(dice, settings.ComputerDifficulty);
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
        static bool[] DecideDiceToKeep(int[] dice, Difficulty difficulty = Difficulty.Medium)
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
        static string ChooseCategory(ScoreCard sc, int[] dice, GameSettings settings, bool isTeamMode)
        {
            List<string> categories = new List<string>
            {
                "Einsen", "Zweien", "Dreien", "Vieren", "Fünfen", "Sechsen",
                "Dreierpasch", "Viererpasch", "Full House",
                "Kleine Straße", "Große Straße", "Kniffel", "Chance"
            };

            // Bonus-Kategorien basierend auf Einstellungen hinzufügen
            if (settings.UsePaar) categories.Add("Paar");
            if (settings.UseTwoPairs) categories.Add("Zwei Paare");
            if (settings.UseMiniFullHouse) categories.Add("Mini Full House");
            if (settings.UseChancePlus) categories.Add("Chance+");
            if (settings.UseLuckySeven) categories.Add("Lucky Seven");

            while (true)
            {
                Console.WriteLine("\n=== VERFÜGBARE KATEGORIEN ===");
                for (int i = 0; i < categories.Count; i++)
                {
                    string cat = categories[i];
                    bool used = sc.IsUsed(cat);
                    bool locked = sc.LockedCategory == cat;

                    if (locked)
                        Console.ForegroundColor = ConsoleColor.Cyan; // gelockte Kategorie
                    else if (used)
                        Console.ForegroundColor = ConsoleColor.DarkGray; // belegte Kategorie
                    else
                        Console.ForegroundColor = ConsoleColor.White; // verfügbar

                    Console.WriteLine($"{i + 1}. {cat} {(used ? "(belegt)" : locked ? "(LOCKED)" : "")}");
                }
                Console.ResetColor();

                // Zeige mögliche Punkte für jede Kategorie an
                Console.WriteLine("\n=== MÖGLICHE PUNKTE ===");
                foreach (string cat in categories.Where(c => !sc.IsUsed(c)))
                {
                    int points = EvaluateCategory(cat, dice, settings);
                    if (points > 0)
                    {
                        Console.WriteLine($"{cat}: {points} Punkte");
                    }
                }

                Console.Write($"\nKategorie # (1-{categories.Count}): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int choice) &&
                    choice >= 1 && choice <= categories.Count)
                {
                    string selectedCat = categories[choice - 1];
                    if (sc.IsUsed(selectedCat))
                    {
                        Console.WriteLine("Kategorie bereits belegt!");
                        continue;
                    }

                    int points = EvaluateCategory(selectedCat, dice, settings);

                    // Locking-Logik
                    if (settings.AllowLocking && !sc.LockUsed && sc.LocksUsed < settings.MaxLocks && points > 0)
                    {
                        if (!isTeamMode)
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
                        else
                        {
                            // Im Team-Modus fragen
                            Console.Write("Team-Entscheidung: Kategorie locken? (j/n): ");
                            if (Console.ReadLine().Trim().ToLower() == "j")
                            {
                                sc.LockedCategory = selectedCat;
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine($"Kategorie {selectedCat} ist jetzt TEAM-LOCKED!");
                                Console.ResetColor();
                            }
                        }
                    }

                    if (settings.UsePenalty && points == 0)
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
        static string ChooseComputerCategory(ScoreCard sc, int[] dice, Difficulty difficulty, GameSettings settings)
        {
            List<string> categories = new List<string>
            {
                "Einsen", "Zweien", "Dreien", "Vieren", "Fünfen", "Sechsen",
                "Dreierpasch", "Viererpasch", "Full House",
                "Kleine Straße", "Große Straße", "Kniffel", "Chance"
            };

            // Bonus-Kategorien basierend auf Einstellungen hinzufügen
            if (settings.UsePaar) categories.Add("Paar");
            if (settings.UseTwoPairs) categories.Add("Zwei Paare");
            if (settings.UseMiniFullHouse) categories.Add("Mini Full House");
            if (settings.UseChancePlus) categories.Add("Chance+");
            if (settings.UseLuckySeven) categories.Add("Lucky Seven");

            string selectedCat = null;
            int bestEval = int.MinValue;
            int bestRealPoints = 0;

            var openCategories = categories.Where(c => !sc.IsUsed(c)).ToList();

            if (openCategories.Count == 0)
                return categories.First(); // Fallback

            if (difficulty == Difficulty.Easy)
            {
                // Zufällige Auswahl mit leichter Priorisierung
                foreach (var cat in openCategories)
                {
                    int pts = EvaluateCategory(cat, dice, settings);
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

                // Zufälliges Locken mit niedriger Wahrscheinlichkeit
                if (settings.AllowLocking && !sc.LockUsed && sc.LocksUsed < settings.MaxLocks && rnd.NextDouble() < 0.15)
                {
                    sc.LockedCategory = selectedCat;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"🤖 Computer LOCKT (zufällig) die Kategorie {selectedCat}!");
                    Console.ResetColor();
                }
            }
            else
            {
                // Mittel/Schwierig: Intelligente Auswahl
                foreach (var cat in openCategories)
                {
                    int pts = EvaluateCategory(cat, dice, settings);
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
                            "Einsen" => pts > 0 ? 2 : 0,
                            "Zweien" => pts > 0 ? 2 : 0,
                            "Dreien" => pts > 0 ? 2 : 0,
                            "Vieren" => pts > 0 ? 2 : 0,
                            "Fünfen" => pts > 0 ? 2 : 0,
                            "Sechsen" => pts > 0 ? 2 : 0,
                            "Paar" => pts > 0 ? 8 : 0,
                            "Dreierpasch" => pts > 0 ? 12 : 0,
                            "Viererpasch" => pts > 0 ? 12 : 0,
                            "Chance" => pts > 0 ? 8 : 0,
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
                            "Paar" => pts > 0 ? 15 : 0,
                            "Dreierpasch" => pts > 0 ? 20 : 0,
                            "Viererpasch" => pts > 0 ? 25 : 0,
                            "Chance" => pts > 0 ? 15 : 0,
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

                // Sicherheitsfallback
                if (selectedCat == null)
                    selectedCat = openCategories.First();

                // Locking-Entscheidung
                if (settings.AllowLocking && !sc.LockUsed && sc.LocksUsed < settings.MaxLocks)
                {
                    bool shouldLock = false;

                    switch (difficulty)
                    {
                        case Difficulty.Medium:
                            shouldLock = (selectedCat == "Kniffel" && bestRealPoints == 50) ||
                                         (selectedCat == "Full House" && bestRealPoints >= 25) ||
                                         (selectedCat == "Große Straße" && bestRealPoints >= 40) ||
                                         (bestRealPoints >= 30);
                            break;
                        case Difficulty.Hard:
                            shouldLock = (selectedCat == "Kniffel" && bestRealPoints == 50) ||
                                         (selectedCat == "Full House" && bestRealPoints >= 25) ||
                                         (selectedCat == "Große Straße" && bestRealPoints >= 40) ||
                                         (selectedCat == "Lucky Seven" && bestRealPoints == 7) ||
                                         (selectedCat == "Chance+" && bestRealPoints >= 25) ||
                                         (bestRealPoints >= 25);
                            break;
                    }

                    if (shouldLock)
                    {
                        sc.LockedCategory = selectedCat;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"🤖 Computer LOCKT die Kategorie {selectedCat}!");
                        Console.ResetColor();
                    }
                }
            }

            // Strafpunkte für Computer
            if (settings.UsePenalty && bestRealPoints == 0)
                sc.Penalty += 5;

            return selectedCat;
        }

        // -------------------------------
        // Punkteberechnung
        // -------------------------------
        static int EvaluateCategory(string cat, int[] dice, GameSettings settings)
        {
            int baseScore = EvaluateCategoryBase(cat, dice);

            // Strafpunkte-Logik
            if (settings.UsePenalty && baseScore == 0)
            {
                return 0;
            }

            return baseScore;
        }

        static int EvaluateCategoryBase(string cat, int[] dice)
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
                    int pairCount = count[i] / 2;
                    for (int j = 0; j < pairCount; j++)
                    {
                        pairs.Add(i);
                    }
                }

                if (pairs.Count >= 2)
                {
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
                "Dreierpasch" => count.Any(c => c >= 3) ? sum : 0,
                "Viererpasch" => count.Any(c => c >= 4) ? sum : 0,
                "Full House" => (count.Contains(3) && count.Contains(2)) ? 25 : 0,
                "Kleine Straße" => HasRun(4) ? 30 : 0,
                "Große Straße" => HasRun(5) ? 40 : 0,
                "Kniffel" => count.Any(c => c == 5) ? 50 : 0,
                "Chance" => sum,
                "Paar" => EvaluatePair(count),
                "Zwei Paare" => EvaluateTwoPairs(count),
                "Mini Full House" => count.Contains(3) && count.Count(c => c == 2) == 0 ? 10 : 0,
                "Chance+" => sum >= 20 ? sum + 5 : sum,
                "Lucky Seven" => sum == 7 ? 20 : 0,
                _ => 0
            };
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
        private GameSettings Settings { get; set; }
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
        public int? Paar { get; set; }
        public int? TwoPairs { get; set; }
        public int? MiniFullHouse { get; set; }
        public int? ChancePlus { get; set; }
        public int? LuckySeven { get; set; }

        public string LockedCategory { get; set; } = null;
        public bool LockUsed { get; set; } = false;
        public int LocksUsed { get; set; } = 0;
        public int Penalty { get; set; } = 0;

        public ScoreCard(GameSettings settings = null)
        {
            Settings = settings ?? new GameSettings();
        }

        public int UpperScore => (Ones ?? 0) + (Twos ?? 0) + (Threes ?? 0) + (Fours ?? 0) + (Fives ?? 0) + (Sixes ?? 0);
        public int UpperBonus => Settings.UseUpperBonus && UpperScore >= Settings.UpperBonusThreshold ? 35 : 0;
        public int LowerScore => (ThreeOfAKind ?? 0) + (FourOfAKind ?? 0) + (FullHouse ?? 0) + (SmallStraight ?? 0) + (LargeStraight ?? 0) + (Yahtzee ?? 0) + (Chance ?? 0);

        public int BonusScore => 
        (Settings.UsePaar ? (Paar ?? 0) : 0) +
        (Settings.UseTwoPairs ? (TwoPairs ?? 0) : 0) +
        (Settings.UseMiniFullHouse ? (MiniFullHouse ?? 0) : 0) +
        (Settings.UseChancePlus ? (ChancePlus ?? 0) : 0) +
        (Settings.UseLuckySeven ? (LuckySeven ?? 0) : 0);

        public int TotalScore => UpperScore + UpperBonus + LowerScore + BonusScore - Penalty;

        public Dictionary<string, int?> GetAllScores()
        {
            var scores = new Dictionary<string, int?>
            {
                { "Einsen", Ones },
                { "Zweien", Twos },
                { "Dreien", Threes },
                { "Vieren", Fours },
                { "Fünfen", Fives },
                { "Sechsen", Sixes },
                { "Dreierpasch", ThreeOfAKind },
                { "Viererpasch", FourOfAKind },
                { "Full House", FullHouse },
                { "Kleine Straße", SmallStraight },
                { "Große Straße", LargeStraight },
                { "Kniffel", Yahtzee },
                { "Chance", Chance }
            };


                if (Settings.UsePaar) scores.Add("Paar", Paar);
                if (Settings.UseTwoPairs) scores.Add("Zwei Paare", TwoPairs);
                if (Settings.UseMiniFullHouse) scores.Add("Mini Full House", MiniFullHouse);
                if (Settings.UseChancePlus) scores.Add("Chance+", ChancePlus);
                if (Settings.UseLuckySeven) scores.Add("Lucky Seven", LuckySeven);

                return scores;
        }

        private string LockIcon(string category)
        {
            if (LockedCategory == category && LockUsed)
                return "🔒";
            return "🔓";
        }

        public List<string> GetOpenCategories()
        {
            List<string> allCategories = new List<string>
            {
                "Einsen", "Zweien", "Dreien", "Vieren", "Fünfen", "Sechsen",
                "Paar", "Dreierpasch", "Viererpasch", "Full House",
                "Kleine Straße", "Große Straße", "Kniffel", "Chance"
            };

            // Nur aktivierte Bonus-Kategorien hinzufügen
            if (Settings.UsePaar) allCategories.Add("Paar");
            if (Settings.UseTwoPairs) allCategories.Add("Zwei Paare");
            if (Settings.UseMiniFullHouse) allCategories.Add("Mini Full House");
            if (Settings.UseChancePlus) allCategories.Add("Chance+");
            if (Settings.UseLuckySeven) allCategories.Add("Lucky Seven");

            return allCategories.Where(c => !IsUsed(c)).ToList();
        }

        public void SetScore(string category, int points, GameSettings settings)
        {
            if (!string.IsNullOrEmpty(LockedCategory) && category == LockedCategory && !LockUsed && settings.AllowLocking)
            {
                points *= 2;
                LockUsed = true;
                LocksUsed++;
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
                case "Paar": Paar = points; break;
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
                "Paar" => Paar.HasValue,
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
            List<string> categories = new List<string>
            {
                "Einsen", "Zweien", "Dreien", "Vieren", "Fünfen", "Sechsen",
                "Dreierpasch", "Viererpasch", "Full House",
                "Kleine Straße", "Große Straße", "Kniffel", "Chance"
            };

            if (Settings.UseAnyBonusCategories)
            {
                categories.AddRange(new[]
                {
                    "Paar", "Zwei Paare", "Mini Full House", "Chance+", "Lucky Seven"
                });
            }

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
            Console.Write($"{left.PadRight(colWidth)} ");

            if (value.HasValue)
            {
                if (category == LockedCategory && LockUsed)
                    Console.ForegroundColor = ConsoleColor.Cyan;

                Console.WriteLine($"{value}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("-");
            }
        }

        public void Print(GameSettings settings)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n===== PUNKTETABELLE =====");
            Console.WriteLine("\n--- Obere Sektion ---");
            Console.ResetColor();
            PrintLine("Einsen", Ones);
            PrintLine("Zweien", Twos);
            PrintLine("Dreien", Threes);
            PrintLine("Vieren", Fours);
            PrintLine("Fünfen", Fives);
            PrintLine("Sechsen", Sixes);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            PrintLine("Obere Sektion", UpperScore);
            if (settings.UseUpperBonus)
            {
                PrintLine($"Bonus ({settings.UpperBonusThreshold}+)", UpperBonus);
            }
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n--- Untere Sektion ---");
            Console.ResetColor();
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

            if (settings.UseAnyBonusCategories)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n--- Bonus Sektion ---");
                Console.ResetColor();
                if (settings.UsePaar) PrintLine("Paar", Paar);
                if (settings.UseTwoPairs) PrintLine("Zwei Paare", TwoPairs);
                if (settings.UseMiniFullHouse) PrintLine("Mini Full House", MiniFullHouse);
                if (settings.UseChancePlus) PrintLine("Chance+", ChancePlus);
                if (settings.UseLuckySeven) PrintLine("Lucky Seven", LuckySeven);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                PrintLine("Bonus Sektion", BonusScore);
                Console.ResetColor();
            }

            Console.WriteLine("==========================\n");
            if (settings.UsePenalty && Penalty > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                PrintLine("Strafpunkte", Penalty);
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            PrintLine("GESAMTPUNKTE", TotalScore);
            Console.ResetColor();

            // Lock-Status anzeigen
            if (settings.AllowLocking)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"\n🔒 Locks verwendet: {LocksUsed}/{settings.MaxLocks}");
                if (LockedCategory != null && !LockUsed)
                    Console.Write($" (Lock verfügbar für: {LockedCategory})");
                Console.WriteLine();
                Console.ResetColor();
            }

            Console.WriteLine("==========================\n");
        }
    }
}