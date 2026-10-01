using crazy_eights.GameEngine;
using crazy_eights.Models;
using crazy_eights.Strategies;
using Xunit;

namespace crazy_eights.Tests;

/// <summary>
/// Tests for GameAwarenessTracker - tracks game history and calculates probabilities.
/// </summary>
public class GameAwarenessTrackerTests
{
    [Fact]
    public void GameAwarenessTracker_Records_Played_Cards()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones")
        };
        var tracker = new GameAwarenessTracker(players);
        
        var card1 = new Card(CardColor.Hearts, CardValue.Five);
        var card2 = new Card(CardColor.Spades, CardValue.King);
        
        tracker.RecordPlayedCard(card1, "1");
        tracker.RecordPlayedCard(card2, "2");
        
        var profile1 = tracker.GetPlayerProfile("1");
        Assert.NotNull(profile1);
        Assert.Equal(1, profile1.GetTotalCardsPlayed());
    }

    [Fact]
    public void GameAwarenessTracker_Calculates_Color_Bias()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones")
        };
        var tracker = new GameAwarenessTracker(players);
        
        tracker.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.Two), "1");
        tracker.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.Three), "1");
        tracker.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.Four), "1");
        
        var profile = tracker.GetPlayerProfile("1");
        Assert.NotNull(profile);
        
        var heartsBias = profile.GetColorBias(CardColor.Hearts);
        Assert.Equal(1.0, heartsBias);
        
        var spadesBias = profile.GetColorBias(CardColor.Spades);
        Assert.Equal(0.0, spadesBias);
    }

    [Fact]
    public void GameAwarenessTracker_Calculates_Color_Probability_In_Hand()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones")
        };
        var tracker = new GameAwarenessTracker(players);
        
        tracker.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.Two), "1");
        tracker.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.Three), "1");
        
        var probability = tracker.GetColorProbabilityInHand("2", CardColor.Hearts, 1);
        
        Assert.True(probability >= 0 && probability <= 1);
    }

    [Fact]
    public void GameAwarenessTracker_Predicts_Best_Blocking_Color()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones")
        };
        var tracker = new GameAwarenessTracker(players);
        
        for (int i = 0; i < 5; i++)
        {
            tracker.RecordPlayedCard(new Card(CardColor.Hearts, (CardValue)i), "2");
        }
        
        var blockingColor = tracker.PredictBestBlockingColor("2", 1);
        
        Assert.NotEqual(CardColor.Hearts, blockingColor);
    }

    [Fact]
    public void GameAwarenessTracker_Records_Drawn_Cards()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith")
        };
        var tracker = new GameAwarenessTracker(players);
        
        var card = new Card(CardColor.Diamonds, CardValue.Seven);
        tracker.RecordDrawnCard(card);
        
        var probability = tracker.GetColorProbabilityInHand("1", CardColor.Diamonds, 1);
        
        Assert.True(probability >= 0 && probability <= 1);
    }

    [Fact]
    public void GameAwarenessTracker_Handles_Unknown_Players()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith")
        };
        var tracker = new GameAwarenessTracker(players);
        
        var profile = tracker.GetPlayerProfile("999");
        Assert.Null(profile);
    }

    [Fact]
    public void GameAwarenessTracker_Initializes_All_Players()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones"),
            new("3", "Charlie", "Brown")
        };
        var tracker = new GameAwarenessTracker(players);
        
        Assert.NotNull(tracker.GetPlayerProfile("1"));
        Assert.NotNull(tracker.GetPlayerProfile("2"));
        Assert.NotNull(tracker.GetPlayerProfile("3"));
    }

    [Fact]
    public void PlayerProfile_Tracks_Card_Values()
    {
        var players = new List<Player> { new("1", "Alice", "Smith") };
        var tracker = new GameAwarenessTracker(players);
        
        tracker.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.As), "1");
        tracker.RecordPlayedCard(new Card(CardColor.Spades, CardValue.As), "1");
        tracker.RecordPlayedCard(new Card(CardColor.Clubs, CardValue.King), "1");
        
        var profile = tracker.GetPlayerProfile("1");
        Assert.NotNull(profile);
        
        var aceBias = profile.GetValueBias(CardValue.As);
        Assert.Equal(2.0 / 3.0, aceBias);
    }
}
