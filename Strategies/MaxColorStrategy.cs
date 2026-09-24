using crazy_eights.Models;

namespace crazy_eights.Strategies;
/// <summary>
/// Strategy ou le but est d'utiliser les cartes dont le joueur a le plus de cette couleur
/// </summary>
public class MaxColorStrategy : IPlayerStrategy
{
    public string Name { get; } = "Priorité des couleurs majoritaire";
    public Card? ChooseCard(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay)
    {
        var handList = hand.ToList();
        var validCards = handList.Where(c => isValidPlay(c, topCard)).ToList();
        if (!validCards.Any()) return null;

        var dominantColor = handList
            .GroupBy(c => c.Color)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        return validCards
            .Where(c => c.Color == dominantColor)
            .DefaultIfEmpty(validCards.FirstOrDefault())
            .First();
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