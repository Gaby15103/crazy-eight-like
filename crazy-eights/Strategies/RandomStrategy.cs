using crazy_eights.Models;

namespace crazy_eights.Strategies;

/// <summary>
/// Stratégie sélectionnant de manière aléatoire une carte valide parmi les choix disponibles.
/// </summary>
public class RandomStrategy : IPlayerStrategy
{
    /// <inheritdoc/>
    public string Name { get; } = "Priorité random";
    
    /// <summary>
    /// Instance de générateur pseudo-aléatoire.
    /// </summary>
    private readonly Random _random = new();

    /// <inheritdoc/>
    public Card? ChooseCard(GameContext context, IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay)
    {
        var validCards = hand.Where(c => isValidPlay(c, topCard)).ToList();
        if (!validCards.Any()) return null;
        
        int index = _random.Next(0, validCards.Count);
        return validCards[index];
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