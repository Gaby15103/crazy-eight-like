using crazy_eights.Models;

namespace crazy_eights.Strategies;

/// <summary>
/// Stratégie visant à se débarrasser en premier des cartes ayant la plus forte valeur en points.
/// </summary>
public class MinimizingPointsStrategy : IPlayerStrategy
{
    /// <inheritdoc/>
    public string Name { get; } = "Priorité du moins de points possible";

    /// <inheritdoc/>
    public Card? ChooseCard(GameContext context, IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay)
    {
        
        var validCards = hand.Where(c => isValidPlay(c, topCard)).ToList();
        if (!validCards.Any()) return null;
        
        return validCards.OrderByDescending(c => c.Value.GetPoints()).FirstOrDefault();
    }
    
    /// <inheritdoc/>
    public CardColor ChooseColor(GameContext context, IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay)
    {
        var bestCard = this.ChooseCard(context, hand, topCard, isValidPlay);
        if (bestCard.HasValue)
        {
            return bestCard.Value.Color;
        }
        return topCard.Color;
    }
}