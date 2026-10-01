using crazy_eights.GameEngine;
using crazy_eights.Models;
using crazy_eights.Strategies;
using Xunit;
namespace crazy_eights.Tests;

public class StrategyTests
{
    private GameContext CreateMockGameContext(List<Player> players, GameAwarenessTracker? awareness = null)
    {
        awareness ??= new GameAwarenessTracker(players);
        var game = new MockFishingGame(players);
        return new GameContext(players.AsReadOnly(), awareness, game);
    }
    #region ActionCardPriorityStrategy Tests
    [Fact]
    public void ActionCardPriorityStrategy_Chooses_Action_Card_First()
    {
        var strategy = new ActionCardPriorityStrategy();
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Five),
            new(CardColor.Hearts, CardValue.As)
        };
        var topCard = new Card(CardColor.Hearts, CardValue.Three);
        var turnManager = new TurnManager();
        
        var player = new Player("1", "Test", "Player");
        var context = CreateMockGameContext(new List<Player> { player });
        
        var chosenCard = strategy.ChooseCard(context, hand, topCard, turnManager.IsValidePlay);

        Assert.NotNull(chosenCard);
        Assert.Equal(CardValue.As, chosenCard.Value.Value);
    }
    
    [Fact]
    public void ActionCardPriorityStrategy_Returns_Null_When_No_Valid_Cards()
    {
        var strategy = new ActionCardPriorityStrategy();
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Two),
            new(CardColor.Hearts, CardValue.Three)
        };
        var topCard = new Card(CardColor.Spades, CardValue.King);
        var turnManager = new TurnManager();
        
        var player = new Player("1", "Test", "Player");
        var context = CreateMockGameContext(new List<Player> { player });
        
        var chosenCard = strategy.ChooseCard(context, hand, topCard, turnManager.IsValidePlay);

        Assert.Null(chosenCard);
    }
    
    #endregion

    #region MaxColorStrategy Tests

    [Fact]
    public void MaxColorStrategy_Chooses_Dominant_Color_Card()
    {
        var strategy = new MaxColorStrategy();
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Five),
            new(CardColor.Hearts, CardValue.As),
            new(CardColor.Hearts, CardValue.King),
            new(CardColor.Clubs, CardValue.King),
            new(CardColor.Spades, CardValue.Queen),
        };
        var topCard = new Card(CardColor.Spades, CardValue.King);
        var turnManager = new TurnManager();
        
        var player = new Player("1", "Test", "Player");
        var context = CreateMockGameContext(new List<Player> { player });
        
        var chosenCard = strategy.ChooseCard(context, hand, topCard, turnManager.IsValidePlay);
        
        Assert.NotNull(chosenCard);
        Assert.Equal(CardValue.King, chosenCard.Value.Value);
        Assert.Equal(CardColor.Hearts, chosenCard.Value.Color);
    }

    [Fact]
    public void MaxColorStrategy_Fallbacks_When_Dominant_Color_Not_Valid()
    {
        var strategy = new MaxColorStrategy();
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Two),
            new(CardColor.Hearts, CardValue.Three),
            new(CardColor.Spades, CardValue.King),
        };
        var topCard = new Card(CardColor.Spades, CardValue.Five);
        var turnManager = new TurnManager();
        
        var player = new Player("1", "Test", "Player");
        var context = CreateMockGameContext(new List<Player> { player });
        
        var chosenCard = strategy.ChooseCard(context, hand, topCard, turnManager.IsValidePlay);
        
        Assert.NotNull(chosenCard);
        Assert.Equal(CardColor.Spades, chosenCard.Value.Color);
    }

    #endregion

    #region MinimizingPointsStrategy Tests

    [Fact]
    public void MinimizingPointsStrategy_Chooses_High_Value_Cards_First()
    {
        var strategy = new MinimizingPointsStrategy();
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Five),
            new(CardColor.Hearts, CardValue.As),
            new(CardColor.Hearts, CardValue.Ten),
            new(CardColor.Clubs, CardValue.King),
            new(CardColor.Spades, CardValue.Queen),
        };
        var topCard = new Card(CardColor.Hearts, CardValue.Five);
        var turnManager = new TurnManager();
        
        var player = new Player("1", "Test", "Player");
        var context = CreateMockGameContext(new List<Player> { player });
        
        var chosenCard = strategy.ChooseCard(context, hand, topCard, turnManager.IsValidePlay);
        
        Assert.NotNull(chosenCard);
        Assert.Equal(CardValue.As, chosenCard.Value.Value);
        
        hand.Remove(chosenCard.Value);
        chosenCard = strategy.ChooseCard(context, hand, topCard, turnManager.IsValidePlay);
        
        Assert.NotNull(chosenCard);
        Assert.True(chosenCard.Value.Value == CardValue.Ten || chosenCard.Value.Value == CardValue.King);
    }
    [Fact]
    public void MinimizingPointsStrategy_Chooses_Lower_Points_Over_Higher()
    {
        var strategy = new MinimizingPointsStrategy();
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Three),
            new(CardColor.Hearts, CardValue.King),
        };
        var topCard = new Card(CardColor.Hearts, CardValue.Five);
        var turnManager = new TurnManager();
        
        var player = new Player("1", "Test", "Player");
        var context = CreateMockGameContext(new List<Player> { player });
        
        var chosenCard = strategy.ChooseCard(context, hand, topCard, turnManager.IsValidePlay);
        
        Assert.NotNull(chosenCard);
        Assert.Equal(CardValue.Three, chosenCard.Value.Value);
    }

    #endregion
    
    #region AwarenessBlockingStrategy Tests

    [Fact]
    public void AwarenessBlockingStrategy_Uses_Fallback_When_No_Danger()
    {
        var fallbackStrategy = new MinimizingPointsStrategy();
        var strategy = new AwarenessBlockingStrategy(fallbackStrategy);
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Five),
            new(CardColor.Hearts, CardValue.King),
        };
        var topCard = new Card(CardColor.Hearts, CardValue.Two);
        var turnManager = new TurnManager();
        
        var player1 = new Player("1", "Player", "One");
        var player2 = new Player("2", "Player", "Two");
        var context = CreateMockGameContext(new List<Player> { player1, player2 });
        
        var chosenCard = strategy.ChooseCard(context, hand, topCard, turnManager.IsValidePlay);
        
        Assert.NotNull(chosenCard);
        Assert.Equal(CardValue.Five, chosenCard.Value.Value);
    }
    [Fact]
    public void AwarenessBlockingStrategy_Prioritizes_Attack_Cards_In_Danger()
    {
        var fallbackStrategy = new MinimizingPointsStrategy();
        var strategy = new AwarenessBlockingStrategy(fallbackStrategy);
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Five),
            new(CardColor.Hearts, CardValue.Two),
            new(CardColor.Hearts, CardValue.King),
        };
        var topCard = new Card(CardColor.Hearts, CardValue.Eight);
        var turnManager = new TurnManager();
        
        var player1 = new Player("1", "Player", "One");
        var player2 = new Player("2", "Player", "Two");
        player2.AddCard(new Card(CardColor.Spades, CardValue.King)); // Only 1 card
        
        var awareness = new GameAwarenessTracker(new List<Player> { player1, player2 });
        var game = new MockFishingGame(new List<Player> { player1, player2 });
        var context = new GameContext(new List<Player> { player1, player2 }.AsReadOnly(), awareness, game);
        context.SetCurrentPlayer(player1);
        
        var chosenCard = strategy.ChooseCard(context, hand, topCard, turnManager.IsValidePlay);
        Assert.Equal(CardValue.Two, chosenCard.Value.Value);
    }
    [Fact]
    public void AwarenessBlockingStrategy_Chooses_Least_Likely_Color_In_Danger()
    {
        var fallbackStrategy = new MinimizingPointsStrategy();
        var strategy = new AwarenessBlockingStrategy(fallbackStrategy);
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Five),
            new(CardColor.Spades, CardValue.Five),
            new(CardColor.Clubs, CardValue.Five),
        };
        var topCard = new Card(CardColor.Diamonds, CardValue.Five);
        var turnManager = new TurnManager();
        
        var player1 = new Player("1", "Player", "One");
        var player2 = new Player("2", "Player", "Two");
        player2.AddCard(new Card(CardColor.Hearts, CardValue.King));
        
        var awareness = new GameAwarenessTracker(new List<Player> { player1, player2 });
        awareness.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.Two), player2.Id);
        awareness.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.Three), player2.Id);
        awareness.RecordPlayedCard(new Card(CardColor.Hearts, CardValue.Four), player2.Id);
        
        var game = new MockFishingGame(new List<Player> { player1, player2 });
        var context = new GameContext(new List<Player> { player1, player2 }.AsReadOnly(), awareness, game);
        context.SetCurrentPlayer(player1);
        
        var chosenCard = strategy.ChooseCard(context, hand, topCard, turnManager.IsValidePlay);
        
        Assert.NotNull(chosenCard);
        Assert.NotEqual(CardColor.Hearts, chosenCard.Value.Color);
    }

    #endregion
    #region ChooseColor Tests

    [Fact]
    public void Strategy_ChooseColor_Returns_Chosen_Card_Color_By_Default()
    {
        var strategy = new RandomStrategy();
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Five),
            new(CardColor.Spades, CardValue.Jack),
        };
        var topCard = new Card(CardColor.Clubs, CardValue.Three);
        var turnManager = new TurnManager();
        
        var player = new Player("1", "Test", "Player");
        var context = CreateMockGameContext(new List<Player> { player });
        
        var chosenColor = strategy.ChooseColor(context, hand, topCard, turnManager.IsValidePlay);
        
        Assert.NotNull(chosenColor);
        Assert.True(chosenColor == CardColor.Hearts || chosenColor == CardColor.Spades);
    }

    [Fact]
    public void AwarenessBlockingStrategy_ChooseColor_Blocks_Danger_Player_Favorite()
    {
        var strategy = new AwarenessBlockingStrategy(new RandomStrategy());
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Jack),
        };
        var topCard = new Card(CardColor.Clubs, CardValue.Three);
        var turnManager = new TurnManager();
        
        var player1 = new Player("1", "Player", "One");
        var player2 = new Player("2", "Player", "Two");
        player2.AddCard(new Card(CardColor.Hearts, CardValue.King));
        
        var awareness = new GameAwarenessTracker(new List<Player> { player1, player2 });
        for (int i = 0; i < 5; i++)
        {
            awareness.RecordPlayedCard(new Card(CardColor.Hearts, (CardValue)i), player2.Id);
        }
        
        var game = new MockFishingGame(new List<Player> { player1, player2 });
        var context = new GameContext(new List<Player> { player1, player2 }.AsReadOnly(), awareness, game);
        context.SetCurrentPlayer(player1);
        
        var chosenColor = strategy.ChooseColor(context, hand, topCard, turnManager.IsValidePlay);
        
        Assert.NotNull(chosenColor);
        Assert.NotEqual(CardColor.Hearts, chosenColor);
    }

    #endregion
}