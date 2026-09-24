using System.Text;
using crazy_eights.GameEngine;
using crazy_eights.GameEngine.Menu;
using crazy_eights.Models;
using crazy_eights.Strategies;

namespace crazy_eights;

class Program
{
    /// <summary>
    /// Method main pour lancer le programme et lancer le menu pour pouvoir changer des configurations d'une partie
    /// </summary>
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        bool keepRunning = true;

        while (keepRunning)
        {
            var startOption = new ActionMenuItem("Lancer une nouvelle partie (Défaut)",
                async () => { await InitializeAndStartGameAsync(playerCount: 2, initialHandSize: 5); });

            var customGameOption = new ActionMenuItem("Configurer et lancer une partie", async () =>
            {
                var playerCountSelector = new IntegerSelectorMenuItem("Nombre de joueurs", 2, 2, 4);
                var handSizeSelector = new IntegerSelectorMenuItem("Cartes en main au début", 5, 3, 8);

                var confirmOption = new ActionMenuItem("Valider et lancer",
                    async () =>
                    {
                        await InitializeAndStartGameAsync(playerCountSelector.Value, handSizeSelector.Value);
                    });

                var backOption = new ActionMenuItem("Retour au menu principal", () => { });

                List<MenuItem> configItems = new List<MenuItem>
                {
                    playerCountSelector,
                    handSizeSelector,
                    confirmOption,
                    backOption
                };

                Menu configMenu = new Menu(configItems, "CONFIGURATION DE LA PARTIE");
                configMenu.Run();
            });

            var exitOption = new ActionMenuItem("Quitter", () =>
            {
                keepRunning = false;
                Console.WriteLine("Au revoir !");
            }, isExit: true);

            List<MenuItem> mainItems = new List<MenuItem>
            {
                startOption,
                customGameOption,
                exitOption
            };

            Menu mainMenu = new Menu(mainItems, "Paire de Cartes - MENU PRINCIPAL");
            mainMenu.Run();
        }
    }

    /// <summary>
    /// Méthode mutualisée pour configurer, instancier et lancer une partie de FishingGame.
    /// </summary>
    private static async Task InitializeAndStartGameAsync(int playerCount, int initialHandSize)
    {
        Console.Clear();
        Console.WriteLine(
            $"Lancement de la partie avec {playerCount} joueurs et {initialHandSize} cartes chacun...\n");

        List<Player> players = new List<Player>();
        string[] names = { "Alice", "Bob", "Charlie", "Diana", "Eve", "Frank" };
        string[] lastNames = { "Smith", "Dupont", "Martin", "Bernard", "Petit", "Durand" };

        Random rand = new Random();

        for (int i = 0; i < playerCount; i++)
        {
            var player = new Player((i + 1).ToString(), names[i], lastNames[i]);
            int strategyChoice = rand.Next(0, 3);
            IPlayerStrategy baseStrategy = strategyChoice switch
            {
                0 => new ActionCardPriorityStrategy(),
                1 => new MaxColorStrategy(),
                2 => new MinimizingPointsStrategy(),
                _ => new RandomStrategy()
            };
            player.Strategy = new DangerAwareStrategyDecorator(
                innerStrategy: baseStrategy,
                isAnyOpponentInDanger: () => false 
            );

            players.Add(player);
        }

        GameConfig config = new GameConfig
        {
            InitialHandSize = initialHandSize,
            PlayerCount = playerCount
        };

        FishingGame game = new FishingGame(players, config);
        GameTuiView tuiView = new GameTuiView();
        tuiView.Initialize();

        game.OnMessageLogged += (sender, e) =>
        {
            Player currentPlayer = game.CurrentPlayer;
            tuiView.UpdateBoard(game.Board, currentPlayer.FirstName, e, currentPlayer.Hand,
                currentPlayer.Strategy.Name);
        };

        game.OnOneCardLeft += (sender, player) =>
        {
            tuiView.UpdateBoard(game.Board, player.FirstName,
                new ($"ALERTE UNO : {player.FirstName} n'a plus qu'une carte !", MessageType.System), player.Hand, player.Strategy.Name);
        };

        game.OnGameEnded += (sender, winner) =>
        {
            foreach (var p in game.Board.Players)
            {
                p.Score = p.Hand.Sum(c => c.Points);
            }
            
            var rankedPlayers = game.Board.Players.OrderBy(p => p.Score).ToList();
            
            StringBuilder endMessage = new StringBuilder();
            endMessage.AppendLine($"FIN DE PARTIE ! Gagnant : {winner.FirstName} {winner.LastName}\n");
            endMessage.AppendLine("--- CLASSEMENT FINAL ---");

            for (int i = 0; i < rankedPlayers.Count; i++)
            {
                var rankedPlayer = rankedPlayers[i];
                string positionSuffix = i == 0 ? "1er 🏆" : $"{i + 1}e";
                endMessage.AppendLine($"{positionSuffix} : {rankedPlayer.FirstName} {rankedPlayer.LastName} - {rankedPlayer.Score} points ({rankedPlayer.Hand.Count} cartes en main)");
            }
            
            tuiView.UpdateBoard(game.Board, winner.FirstName,
                new (endMessage.ToString(), MessageType.System), winner.Hand, winner.Strategy.Name);
            
            Console.ReadKey();
            tuiView.Shutdown();
        };

        _ = Task.Run(async () =>
        {
            await Task.Delay(500);
            await game.StartGameAsync();
        });

        tuiView.Run();
    }
}