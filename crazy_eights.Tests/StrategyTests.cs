using crazy_eights.Models;
using crazy_eights.Strategies;
using Xunit;

public class StrategyTests
{
    [Fact]
    public void ActionCardPriorityStrategy_Chooses_Action_Card_First()
    {
        var strategy = new ActionCardPriorityStrategy();
        var hand = new List<Card>
        {
            new Card(new CardColor("Cœur"), CardValue.Five),
            new Card(new CardColor("Cœur"), CardValue.As)
        };
        var topCard = new Card(new CardColor("Cœur"), CardValue.Three);
        
        Func<Card, Card, bool> isValidPlay = (c, t) => c.Color.Name == t.Color.Name;

        var chosenCard = strategy.ChooseCard(hand, topCard, isValidPlay);

        Assert.NotNull(chosenCard);
        Assert.Equal(CardValue.As, chosenCard.Value.Value); 
    }
}