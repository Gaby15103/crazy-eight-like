using crazy_eights.Strategies;

namespace crazy_eights.Models;
/// <summary>
/// Représente un joueur dans le jeu, héritant des propriétés d'une personne.
/// Gère la main de cartes du joueur, son score et sa stratégie de jeu.
/// </summary>
public class Player : Person
{
    /// <summary>
    /// La liste des cartes actuellement dans la main du joueur
    /// </summary>
    private readonly List<Card> _hand;
    /// <summary>
    /// La liste en lecture seule des cartes actuellement dans la main du joueur
    /// </summary>
    public IReadOnlyList<Card> Hand => _hand.AsReadOnly();
    /// <summary>
    /// Retourn le score cumulé du joueur
    /// </summary>
    public int Score { get; set; }
    /// <summary>
    /// La stratégie de jeu utilisée par le joueur
    /// </summary>
    public IPlayerStrategy Strategy { get; set; }
    
    public Player(string id, string firstName, string lastName) 
        : base(id, firstName, lastName)
    {
        _hand = new List<Card>();
        Score = 0;
        Strategy = new RandomStrategy();
    }

    public bool CanPlayTwo()
    {
        return _hand.Any(c => c.Value == CardValue.Two);
    }

    /// <summary>
    /// Ajoute une carte à la main du joueur.
    /// </summary>
    /// <param name="card">La carte à ajouter à la main.</param>
    public void AddCard(Card card)
    {
        _hand.Add(card);
    }
    /// <summary>
    /// Retire une carte spécifique de la main du joueur.
    /// </summary>
    /// <param name="card">La carte à retirer de la main.</param>
    public void RemoveCard(Card card)
    {
        _hand.Remove(card);
    }
}