using crazy_eights.Models;

namespace crazy_eights.Strategies;

public interface IPlayerStrategy
{
    /// <summary>
    /// Le nom de la strategie
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Fonction pour choisir la carte le plus approprié à la strategie
    /// </summary>
    /// <param name="hand">les carte de la main du joueur</param>
    /// <param name="topCard">la carte sur le dessus de la pile de dépôt</param>
    /// <param name="isValidPlay">Fonction pour valider si une action est valide</param>
    /// <returns>La carte sélectionnée</returns>
    Card? ChooseCard(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay);
    /// <summary>
    /// Fonction pour trouver la meilleure couleur à appliqué a un valet selon la strategie
    /// </summary>
    /// <param name="hand">les carte de la main du joueur</param>
    /// <param name="topCard">la carte sur le dessus de la pile de dépôt</param>
    /// <param name="isValidPlay">Fonction pour valider si une action est valide</param>
    /// <returns></returns>
    CardColor ChooseColor(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay);
}