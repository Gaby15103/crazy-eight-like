namespace crazy_eights.GameEngine.Observers;

using crazy_eights.Models;

/// <summary>
/// Interface définissant le contrat pour les observateurs du jeu.
/// Implémente le pattern Observateur standard de C#.
/// 
/// Un observateur reçoit les notifications de:
/// - Messages de jeu (actions, effets spéciaux)
/// - Changements d'état du jeu
/// - Alertes critiques (une seule carte, fin de partie)
/// </summary>
public interface IGameObserver
{
    /// <summary>
    /// Appelé quand un message de jeu est publié
    /// Exemples: "Alice a joué un Roi de Pique", "Pioche recyclée"
    /// </summary>
    /// <param name="args">Arguments contenant le message et son type</param>
    void OnGameMessage(GameMessageEventArgs args);

    /// <summary>
    /// Appelé quand l'état du jeu change
    /// Exemples: CardPlayed, CardDrawn, SpecialEffectApplied, OneCardLeft
    /// </summary>
    /// <param name="args">Arguments contenant le nouvel état et le joueur affecté</param>
    void OnGameStateChanged(GameStateChangedEventArgs args);

    /// <summary>
    /// Appelé quand un joueur n'a plus qu'une seule carte
    /// Permet aux observateurs (notamment les stratégies) d'adapter leur comportement
    /// </summary>
    /// <param name="player">Le joueur en danger</param>
    void OnPlayerInDanger(Player player);

    /// <summary>
    /// Appelé quand la partie se termine
    /// </summary>
    /// <param name="winner">Le joueur gagnant</param>
    void OnGameEnded(Player winner);
}