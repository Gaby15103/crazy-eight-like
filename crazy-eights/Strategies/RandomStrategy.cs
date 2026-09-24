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
    public Card? ChooseCard(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay,
        bool isOpponentInDanger = false)
    {
        var validCards = hand.Where(c => isValidPlay(c, topCard)).ToList();
        if (!validCards.Any()) return null;
        
        int index = _random.Next(0, validCards.Count);
        return validCards[index];
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