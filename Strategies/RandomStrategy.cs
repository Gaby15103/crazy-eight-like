using crazy_eights.Models;

namespace crazy_eights.Strategies;
/// <summary>
/// Strategy ou le but est d'utiliser une carte au hasard dans les cartes jouable
/// </summary>
public class RandomStrategy : IPlayerStrategy
{
    public string Name { get; } = "Priorité random";
    /// <summary>
    /// Nouvelle instance de pseudo-random
    /// </summary>
    private Random _random = new();

    public Card? ChooseCard(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay)
    {
        var validCards = hand.Where(c => isValidPlay(c, topCard)).ToList();
        if (!validCards.Any()) return null;
        
        int index = _random.Next(0, validCards.Count);
        return validCards[index];
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