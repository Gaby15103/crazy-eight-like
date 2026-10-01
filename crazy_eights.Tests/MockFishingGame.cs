using crazy_eights.GameEngine;
using crazy_eights.Models;

namespace crazy_eights.Tests;

/// <summary>
/// Mock FishingGame for testing purposes.
/// Allows simulation of game events without running a full game loop.
/// </summary>
public class MockFishingGame : FishingGame
{
    public MockFishingGame(List<Player> players)
        : base(players, new GameConfig { InitialHandSize = 5, PlayerCount = players.Count }, shuffleDeck: false)
    {
    }

    /// <summary>
    /// Simulates a player playing a card and fires the OnCardPlayed event.
    /// </summary>
    public void SimulateCardPlayed(Card card, string playerId)
    {
        RaiseCardPlayed(card, playerId);
    }

    /// <summary>
    /// Simulates a card being drawn and fires the OnCardDrawn event.
    /// </summary>
    public void SimulateCardDrawn(Card card)
    {
        RaiseCardDrawn(card);
    }

    /// <summary>
    /// Simulates a direction change and fires the OnDirectionChanged event.
    /// </summary>
    public void SimulateDirectionChanged(bool isClockwise)
    {
        RaiseDirectionChanged(isClockwise);
    }

    /// <summary>
    /// Simulates a draw pile size change and fires the OnDrawPileSizeChanged event.
    /// </summary>
    public void SimulateDrawPileSizeChanged(int size)
    {
        RaiseDrawPileSizeChanged(size);
    }

    /// <summary>
    /// Simulates a player change and fires the OnCurrentPlayerChanged event.
    /// </summary>
    public void SimulateCurrentPlayerChanged(Player player)
    {
        RaiseCurrentPlayerChanged(player);
    }
}