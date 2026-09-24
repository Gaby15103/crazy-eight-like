using crazy_eights.Models;

namespace crazy_eights.Strategies;

/// <summary>
/// Définit le contrat pour les différentes stratégies de jeu des joueurs automatisés.
/// </summary>
public interface IPlayerStrategy
{
    /// <summary>
    /// Le nom descriptif de la stratégie.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Sélectionne la carte la plus appropriée à jouer selon la logique de la stratégie.
    /// </summary>
    /// <param name="hand">La liste des cartes actuellement dans la main du joueur.</param>
    /// <param name="topCard">La carte située sur le dessus de la pile de dépôt.</param>
    /// <param name="isValidPlay">Fonction de validation permettant de vérifier si un coup est autorisé.</param>
    /// <param name="isOpponentInDanger">Indique si un adversaire est en situation critique (possède une seule carte).</param>
    /// <returns>La carte sélectionnée, ou null si aucune carte n'est jouable.</returns>
    Card? ChooseCard(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay, bool isOpponentInDanger = false);

    /// <summary>
    /// Détermine la meilleure couleur à appliquer lors de l'utilisation d'un Valet.
    /// </summary>
    /// <param name="hand">La liste des cartes actuellement dans la main du joueur.</param>
    /// <param name="topCard">La carte située sur le dessus de la pile de dépôt.</param>
    /// <param name="isValidPlay">Fonction de validation permettant de vérifier si un coup est autorisé.</param>
    /// <param name="isOpponentInDanger">Indique si un adversaire est en situation critique (possède une seule carte).</param>
    /// <returns>La couleur choisie pour la suite de la partie.</returns>
    CardColor ChooseColor(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay, bool isOpponentInDanger = false);
}