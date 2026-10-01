using crazy_eights.Models;

namespace crazy_eights.Strategies;

/// <summary>
/// Stratégie privilégiant les cartes de la couleur la plus représentée dans la main du joueur.
/// </summary>
public class MaxColorStrategy : IPlayerStrategy
{
    /// <inheritdoc/>
    public string Name { get; } = "Priorité des couleurs majoritaires";
    

    /// <inheritdoc/>
    public Card? ChooseCard(GameContext context, IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay)
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
    
    /// <inheritdoc/>
    public CardColor ChooseColor(GameContext context,IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay)
    {
        var bestCard = this.ChooseCard(context, hand, topCard, isValidPlay);
        if (bestCard.HasValue)
        {
            return bestCard.Value.Color;
        }
        return topCard.Color;
    }
}