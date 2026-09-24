using crazy_eights.Models;

namespace crazy_eights.Strategies;

/// <summary>
/// Stratégie visant à se débarrasser en premier des cartes ayant la plus forte valeur en points.
/// </summary>
public class MinimizingPointsStrategy : IPlayerStrategy
{
    /// <inheritdoc/>
    public string Name { get; } = "Priorité du moins de points possible";
    
    private readonly ActionCardPriorityStrategy _emergencyStrategy = new();

    /// <inheritdoc/>
    public Card? ChooseCard(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay,
        bool isOpponentInDanger = false)
    {
        if (isOpponentInDanger)
        {
            var actionCard = _emergencyStrategy.ChooseCard(hand, topCard, isValidPlay);
            if (actionCard.HasValue) return actionCard;
        }
        
        var validCards = hand.Where(c => isValidPlay(c, topCard)).ToList();
        if (!validCards.Any()) return null;
        
        return validCards.OrderByDescending(c => c.Value.GetPoints()).FirstOrDefault();
    }
    
    /// <inheritdoc/>
    public CardColor ChooseColor(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay,
        bool isOpponentInDanger = false)
    {
        var bestCard = this.ChooseCard(hand, topCard, isValidPlay, isOpponentInDanger);
        if (bestCard.HasValue)
        {
            return bestCard.Value.Color;
        }
        return topCard.Color;
    }
}