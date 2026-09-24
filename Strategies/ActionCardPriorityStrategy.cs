using crazy_eights.Models;

namespace crazy_eights.Strategies;
/// <summary>
/// Strategy ou le but est d'utiliser les cartes a effet quand possible
/// </summary>
public class ActionCardPriorityStrategy : IPlayerStrategy
{
    public string Name { get; } = "Priorité des cartes action";
    
    public Card? ChooseCard(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay)
    {
        var validCards = hand.Where(c => isValidPlay(c, topCard)).ToList();
        if (!validCards.Any()) return null;

        var actionCard = validCards.FirstOrDefault(c => c.Value.IsActionCard());
        return actionCard;
    }
    
    public CardColor ChooseColor(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay)
    {
        var bestCard = this.ChooseCard(hand, topCard, isValidPlay);
        if (bestCard.HasValue)
        {
            return bestCard.Value.Color;
        }
        return topCard.Color;
    }
}