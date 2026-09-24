using crazy_eights.Models;

namespace crazy_eights.Strategies;
/// <summary>
/// Strategy ou le but est d'utiliser les cartes avec le plus grand nombre
/// de point en premier
/// </summary>
public class MinimizingPointsStrategy : IPlayerStrategy
{
    public string Name { get; } = "Priorité du moins de point possible";
    public Card? ChooseCard(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay)
    {
        var validCards = hand.Where(c => isValidPlay(c, topCard)).ToList();
        if (!validCards.Any()) return null;
        
        return validCards.OrderByDescending(c => c.Value.GetPoints()).FirstOrDefault();
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