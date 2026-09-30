using crazy_eights.GameEngine;
using crazy_eights.Models;
using crazy_eights.Strategies;
using Xunit;
namespace crazy_eights.Tests;
public class StrategyTests
{
    [Fact]
    public void ActionCardPriorityStrategy_Chooses_Action_Card_First()
    {
        var strategy = new ActionCardPriorityStrategy();
        var hand = new List<Card>
        {
            new (CardColor.Hearts, CardValue.Five),
            new (CardColor.Hearts, CardValue.As)
        };
        var topCard = new Card(CardColor.Hearts, CardValue.Three);
        var turnManager = new TurnManager();
        
        var chosenCard = strategy.ChooseCard(hand, topCard, turnManager.IsValidePlay);

        Assert.NotNull(chosenCard);
        Assert.Equal(CardValue.As, chosenCard.Value.Value); 
    }

    [Fact]
    public void MaxColorCardPriorityStrategy_Chooses_Max_Card_First()
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
        
        var chosenCard = strategy.ChooseCard(hand, topCard, turnManager.IsValidePlay);
        
        Assert.NotNull(chosenCard);
        Assert.Equal(CardValue.King, chosenCard.Value.Value);
        Assert.Equal(CardColor.Hearts, chosenCard.Value.Color);
    }

    [Fact]
    public void MinimizingPointsStrategy_Chooses_Minimizing_Points()
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
        
        var chosenCard = strategy.ChooseCard(hand, topCard, turnManager.IsValidePlay);
        
        Assert.NotNull(chosenCard);
        Assert.Equal(CardValue.As, chosenCard.Value.Value);
        
        hand.Remove(chosenCard.Value);
        chosenCard = strategy.ChooseCard(hand, topCard, turnManager.IsValidePlay);
        
        Assert.NotNull(chosenCard);
        Assert.Equal(CardValue.Ten, chosenCard.Value.Value);
    }

    [Fact]
    public void DangerAwareStrategy_Chooses_Action_Card_First()
    {
        var strategy = new DangerAwareStrategyDecorator(new MinimizingPointsStrategy(),() => false);
        var hand = new List<Card>
        {
            new(CardColor.Hearts, CardValue.Five),
            new(CardColor.Spades, CardValue.Three),
            new(CardColor.Clubs, CardValue.Two),
            new(CardColor.Spades, CardValue.King),
            new(CardColor.Spades, CardValue.Two),
        };
        var topCard = new Card(CardColor.Spades, CardValue.Five);
        var turnManager = new TurnManager();
        
        var chosenCard = strategy.ChooseCard(hand, topCard, turnManager.IsValidePlay, true);
        
        Assert.NotNull(chosenCard);
        Assert.Equal(CardValue.Two, chosenCard.Value.Value);
        Assert.Equal(CardColor.Spades, chosenCard.Value.Color);
    }
}