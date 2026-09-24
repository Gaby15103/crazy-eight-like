using crazy_eights.Models;

namespace crazy_eights.Strategies;

/// <summary>
/// Stratégie privilégiant l'utilisation des cartes à effet (action) en priorité.
/// </summary>
public class ActionCardPriorityStrategy : IPlayerStrategy
{
    /// <inheritdoc/>
    public string Name { get; } = "Priorité des cartes action";
    
    private readonly ActionCardPriorityStrategy _emergencyStrategy = new();
    
    /// <inheritdoc/>
    public Card? ChooseCard(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay,
        bool isOpponentInDanger = false)
    {
        Card? actionCard;
        if (isOpponentInDanger)
        {
            actionCard = _emergencyStrategy.ChooseCard(hand, topCard, isValidPlay);
            if (actionCard.HasValue) return actionCard;
        }
        
        var validCards = hand.Where(c => isValidPlay(c, topCard)).ToList();
        if (!validCards.Any()) return null;

        actionCard = validCards.FirstOrDefault(c => c.Value.IsActionCard());
        return actionCard;
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