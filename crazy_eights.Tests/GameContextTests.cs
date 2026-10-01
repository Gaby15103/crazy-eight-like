using crazy_eights.GameEngine;
using crazy_eights.Models;
using crazy_eights.Strategies;
using Xunit;

namespace crazy_eights.Tests;

/// <summary>
/// Tests for GameContext - tracks game state and provides rich context to strategies.
/// </summary>
public class GameContextTests
{
    private MockFishingGame CreateMockGame(List<Player> players)
    {
        return new MockFishingGame(players);
    }

    [Fact]
    public void GameContext_Initializes_With_Correct_Properties()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones")
        };
        var awareness = new GameAwarenessTracker(players);
        var game = CreateMockGame(players);
        
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        
        Assert.Equal(2, context.AllPlayers.Count);
        Assert.NotNull(context.Awareness);
        Assert.True(context.IsClockwise);
        Assert.Equal(0, context.DrawPileSize);
    }

    [Fact]
    public void GameContext_Identifies_Danger_Player()
    {
        var player1 = new Player("1", "Alice", "Smith");
        var player2 = new Player("2", "Bob", "Jones");
        player2.AddCard(new Card(CardColor.Hearts, CardValue.King)); // Only 1 card
        
        var players = new List<Player> { player1, player2 };
        var awareness = new GameAwarenessTracker(players);
        var game = CreateMockGame(players);
        
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        context.SetCurrentPlayer(player1);
        
        Assert.True(context.IsAnyOpponentInDanger);
        Assert.NotNull(context.DangerPlayer);
        Assert.Equal("Bob", context.DangerPlayer.FirstName);
    }

    [Fact]
    public void GameContext_Returns_Null_When_No_Danger()
    {
        var player1 = new Player("1", "Alice", "Smith");
        player1.AddCard(new Card(CardColor.Hearts, CardValue.King));
        player1.AddCard(new Card(CardColor.Spades, CardValue.Queen));
        
        var player2 = new Player("2", "Bob", "Jones");
        player2.AddCard(new Card(CardColor.Hearts, CardValue.King));
        player2.AddCard(new Card(CardColor.Spades, CardValue.Queen));
        
        var players = new List<Player> { player1, player2 };
        var awareness = new GameAwarenessTracker(players);
        var game = CreateMockGame(players);
        
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        context.SetCurrentPlayer(player1);
        
        Assert.False(context.IsAnyOpponentInDanger);
        Assert.Null(context.DangerPlayer);
    }

    [Fact]
    public void GameContext_Computes_Opponents_List()
    {
        var player1 = new Player("1", "Alice", "Smith");
        var player2 = new Player("2", "Bob", "Jones");
        var player3 = new Player("3", "Charlie", "Brown");
        
        var players = new List<Player> { player1, player2, player3 };
        var awareness = new GameAwarenessTracker(players);
        var game = CreateMockGame(players);
        
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        context.SetCurrentPlayer(player1);
        
        Assert.Equal(2, context.Opponents.Count);
        Assert.DoesNotContain(player1, context.Opponents);
        Assert.Contains(player2, context.Opponents);
        Assert.Contains(player3, context.Opponents);
    }

    [Fact]
    public void GameContext_Updates_Top_Card()
    {
        var players = new List<Player> { new("1", "Alice", "Smith") };
        var awareness = new GameAwarenessTracker(players);
        var game = CreateMockGame(players);
        
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        
        var newCard = new Card(CardColor.Hearts, CardValue.Five);
        game.SimulateCardPlayed(newCard, "1");
        
        Assert.Equal(CardColor.Hearts, context.TopCard.Color);
        Assert.Equal(CardValue.Five, context.TopCard.Value);
    }

    [Fact]
    public void GameContext_Updates_Direction()
    {
        var players = new List<Player> { new("1", "Alice", "Smith") };
        var awareness = new GameAwarenessTracker(players);
        var game = CreateMockGame(players);
        
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        Assert.True(context.IsClockwise);
        
        game.SimulateDirectionChanged(false);
        
        Assert.False(context.IsClockwise);
    }

    [Fact]
    public void GameContext_Updates_Draw_Pile_Size()
    {
        var players = new List<Player> { new("1", "Alice", "Smith") };
        var awareness = new GameAwarenessTracker(players);
        var game = CreateMockGame(players);
        
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        Assert.Equal(0, context.DrawPileSize);
        
        game.SimulateDrawPileSizeChanged(25);
        
        Assert.Equal(25, context.DrawPileSize);
    }

    [Fact]
    public void GameContext_Fires_ContextChanged_Event()
    {
        var players = new List<Player> { new("1", "Alice", "Smith") };
        var awareness = new GameAwarenessTracker(players);
        var game = CreateMockGame(players);
        
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        
        var eventFired = false;
        context.OnContextChanged += (sender, args) =>
        {
            eventFired = true;
            Assert.Equal("DirectionChanged", args.ChangeType);
        };
        
        game.SimulateDirectionChanged(false);
        
        Assert.True(eventFired);
    }

    [Fact]
    public void GameContext_Updates_Awareness_On_Card_Play()
    {
        var player1 = new Player("1", "Alice", "Smith");
        var player2 = new Player("2", "Bob", "Jones");
        var players = new List<Player> { player1, player2 };
        
        var awareness = new GameAwarenessTracker(players);
        var game = CreateMockGame(players);
        
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        
        var card = new Card(CardColor.Hearts, CardValue.Five);
        game.SimulateCardPlayed(card, "1");
        
        var profile = awareness.GetPlayerProfile("1");
        Assert.NotNull(profile);
        Assert.Equal(1, profile.GetTotalCardsPlayed());
    }

    [Fact]
    public void GameContext_Updates_Awareness_On_Card_Draw()
    {
        var players = new List<Player> { new("1", "Alice", "Smith") };
        var awareness = new GameAwarenessTracker(players);
        var game = CreateMockGame(players);
        
        var context = new GameContext(players.AsReadOnly(), awareness, game);
        
        var card = new Card(CardColor.Spades, CardValue.King);
        game.SimulateCardDrawn(card);
        
        var probability = awareness.GetColorProbabilityInHand("1", CardColor.Spades, 1);
        Assert.True(probability >= 0 && probability <= 1);
    }
}
