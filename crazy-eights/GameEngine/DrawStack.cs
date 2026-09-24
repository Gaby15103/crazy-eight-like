using crazy_eights.Models;

namespace crazy_eights.GameEngine;
/// <summary>
/// La pile de pioche du jeu qui peut ce mélanger
/// </summary>
public class DrawStack
{
    /// <summary>
    /// La liste de carte de la pile de pioche
    /// </summary>
    private List<Card> _cards;
    /// <summary>
    /// Le nombre de carte dans la pile de pioche
    /// </summary>
    public int Count => _cards.Count;
    
    public DrawStack(List<Card> cards)
    {
        _cards = cards;
    }
    /// <summary>
    /// Fonction pour pioché la carte sur le dessus de la pile de pioche
    /// </summary>
    /// <returns>La carte pioché</returns>
    /// <exception cref="InvalidOperationException">Si le nombre de carte dans la pile de pioche est 0 et qu'il est impossible de piocher</exception>
    public Card DrawCard()
    {
        if (_cards.Count == 0)
        {
            throw new InvalidOperationException("La pioche est vide ! Impossible de piocher.");
        }

        Card drawnCard = _cards[^1];
        _cards.RemoveAt(_cards.Count - 1);
        return drawnCard;
    }
    /// <summary>
    /// Fonction pour remettre les cartes du dessous de la pile de dépôt
    /// </summary>
    /// <param name="recycledCards">La liste des cartes de la pile de dépôt</param>
    public void Refill(List<Card> recycledCards)
    {
        if (recycledCards is { Count: > 0 })
        {
            _cards.AddRange(recycledCards);
            Shuffle();
        }
    }
    /// <summary>
    /// Function pour mélanger la pile de pioche utilisant l'algorithme de mélange de Fisher-Yates
    /// </summary>
    public void Shuffle()
    {
        Random rng = new Random();
        int n = _cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (_cards[k], _cards[n]) = (_cards[n], _cards[k]);
        }
    }
}