using crazy_eights.Models;

namespace crazy_eights.GameEngine;
/// <summary>
/// La pile de dépôt du jeu de carte
/// </summary>
public class DepositeStack
{
    /// <summary>
    /// La liste des cartes dans la pile de dépôt
    /// </summary>
    private List<Card> _cards;

    /// <summary>
    /// Retourne La carte sur le top de la pile de dépôt si la pile ne contient pas de carte retourne un exception
    /// </summary>
    /// <exception cref="InvalidOperationException">Si la pile de dépôt est vide</exception>
    public Card TopCard;

    public DepositeStack(List<Card> initialCards)
    {
        _cards = initialCards ?? new List<Card>();
        TopCard = _cards[^1];
    }
    /// <summary>
    /// Fonction pour rajouter une carte sur le dessus de la pile
    /// </summary>
    /// <param name="card">La carte a déposer sur le dessus de la pile</param>
    public void Push(Card card)
    {
        _cards.Add(card);
        TopCard = card;
    }
    /// <summary>
    /// Fonction pout retourner toutes les cartes de la pile sauf la carte sur le dessus et retourner tout les carte à leur couleur originel
    /// tel que les valets
    /// </summary>
    /// <returns></returns>
    public List<Card> TakeAllExcepTop()
    {
        if (_cards.Count <= 1)
        {
            return new List<Card>();
        }

        List<Card> recycledCards = _cards.GetRange(0, _cards.Count - 1);
        recycledCards.ForEach(rc => rc.Reset());

        Card topCard = _cards[^1];
        _cards = new List<Card> {topCard};
        
        return recycledCards;
    }
}