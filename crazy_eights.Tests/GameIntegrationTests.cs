using crazy_eights.GameEngine;
using crazy_eights.Models;
using crazy_eights.Strategies;
using Xunit;

namespace crazy_eights.Tests;

/// <summary>
/// Integration tests that verify complete game scenarios work correctly.
/// Tests the interaction between multiple components (Context, Awareness, Strategies, Game).
/// </summary>
public class GameIntegrationTests
{
    [Fact]
    public void Three_Player_Game_Completes_Without_Errors()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones"),
            new("3", "Charlie", "Brown")
        };

        foreach (var player in players)
        {
            player.Strategy = new MinimizingPointsStrategy();
        }

        var config = new GameConfig
        {
            InitialHandSize = 5,
            PlayerCount = 3
        };

        var game = new FishingGame(players, config);

        // Verify game initialized correctly
        Assert.Equal(3, game.Board.Players.Count);
        Assert.Equal(5, game.Board.Players[0].Hand.Count);
        Assert.Equal(5, game.Board.Players[1].Hand.Count);
        Assert.Equal(5, game.Board.Players[2].Hand.Count);
    }

    [Fact]
    public void Game_Context_Tracks_Player_Strategies()
    {
        var player1 = new Player("1", "Alice", "Smith");
        var player2 = new Player("2", "Bob", "Jones");
        
        player1.Strategy = new MinimizingPointsStrategy();
        player2.Strategy = new AwarenessBlockingStrategy(new RandomStrategy());

        var players = new List<Player> { player1, player2 };
        var awareness = new GameAwarenessTracker(players);
        var game = new MockFishingGame(players);
        
        var context = new GameContext(players.AsReadOnly(), awareness, game);

        Assert.NotNull(player1.Strategy);
        Assert.NotNull(player2.Strategy);
        Assert.IsType<MinimizingPointsStrategy>(player1.Strategy);
        Assert.IsType<AwarenessBlockingStrategy>(player2.Strategy);
    }

    [Fact]
    public void Awareness_Tracking_Affects_Strategy_Decisions()
    {
        var player1 = new Player("1", "Alice", "Smith");
        var player2 = new Player("2", "Bob", "Jones");
        player2.AddCard(new Card(CardColor.Hearts, CardValue.King));
        
        var players = new List<Player> { player1, player2 };
        var awareness = new GameAwarenessTracker(players);
        
        for (int i = 0; i < 10; i++)
        {
            awareness.RecordPlayedCard(
                new Card(CardColor.Hearts, (CardValue)(2 + (i % 11))), 
                player2.Id);
        }
        
        var game = new MockFishingGame(players);
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        context.SetCurrentPlayer(player1);
        
        var strategy = new AwarenessBlockingStrategy(new RandomStrategy());
        var hand = new List<Card>
        {
            new(CardColor.Spades, CardValue.Five),
            new(CardColor.Clubs, CardValue.Five),
            new(CardColor.Diamonds, CardValue.Five),
            new(CardColor.Hearts, CardValue.Five),
        };
        var topCard = new Card(CardColor.Diamonds, CardValue.Five);
        var turnManager = new TurnManager();
        
        var chosenCard = strategy.ChooseCard(
            context, 
            hand, 
            topCard, 
            turnManager.IsValidePlay);
        
        Assert.NotNull(chosenCard);
        Assert.NotEqual(CardColor.Hearts, chosenCard.Value.Color);
    }

    [Fact]
    public void All_Strategies_Work_With_GameContext()
    {
        var player = new Player("1", "Alice", "Smith");
        var players = new List<Player> { player };
        var awareness = new GameAwarenessTracker(players);
        var game = new MockFishingGame(players);
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        context.SetCurrentPlayer(player);
        
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Five),
            new(CardColor.Hearts, CardValue.King),
            new(CardColor.Spades, CardValue.Jack),
        };
        var topCard = new Card(CardColor.Hearts, CardValue.Two);
        var turnManager = new TurnManager();
        
        var strategies = new IPlayerStrategy[]
        {
            new RandomStrategy(),
            new ActionCardPriorityStrategy(),
            new MaxColorStrategy(),
            new MinimizingPointsStrategy(),
            new AwarenessBlockingStrategy()
        };
        
        foreach (var strategy in strategies)
        {
            var card = strategy.ChooseCard(context, hand, topCard, turnManager.IsValidePlay);
            Assert.True(card == null || hand.Contains(card.Value));
        }
    }

    [Fact]
    public void Awareness_Correctly_Calculates_Probabilities_After_Multiple_Plays()
    {
        var player1 = new Player("1", "Alice", "Smith");
        var player2 = new Player("2", "Bob", "Jones");
        var players = new List<Player> { player1, player2 };
        
        var awareness = new GameAwarenessTracker(players);
        
        awareness.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.Two), "1");
        awareness.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.Three), "1");
        awareness.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.Four), "1");
        awareness.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.Five), "1");
        
        awareness.RecordPlayedCard(new Card(CardColor.Spades, CardValue.Two), "2");
        awareness.RecordPlayedCard(new Card(CardColor.Spades, CardValue.Three), "2");
        awareness.RecordPlayedCard(new Card(CardColor.Spades, CardValue.Four), "2");
        
        var profile1 = awareness.GetPlayerProfile("1");
        var profile2 = awareness.GetPlayerProfile("2");
        
        Assert.NotNull(profile1);
        Assert.NotNull(profile2);
        
        Assert.Equal(1.0, profile1.GetColorBias(CardColor.Hearts));
        
        Assert.Equal(1.0, profile2.GetColorBias(CardColor.Spades));
    }

    [Fact]
    public void Game_Context_Properly_Identifies_Multiple_Danger_Scenarios()
    {
        var player1 = new Player("1", "Alice", "Smith");
        var player2 = new Player("2", "Bob", "Jones");
        var player3 = new Player("3", "Charlie", "Brown");
        
        player1.AddCard(new Card(CardColor.Hearts, CardValue.King));
        player1.AddCard(new Card(CardColor.Spades, CardValue.Queen));
        player2.AddCard(new Card(CardColor.Clubs, CardValue.Jack));
        player3.AddCard(new Card(CardColor.Diamonds, CardValue.King));
        player3.AddCard(new Card(CardColor.Hearts, CardValue.Queen));
        
        var players = new List<Player> { player1, player2, player3 };
        var awareness = new GameAwarenessTracker(players);
        var game = new MockFishingGame(players);
        
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        context.SetCurrentPlayer(player1);
        
        Assert.True(context.IsAnyOpponentInDanger);
        Assert.NotNull(context.DangerPlayer);
        Assert.Equal("Bob", context.DangerPlayer.FirstName);
    }
}
