namespace crazy_eights.GameEngine.Observers;

using crazy_eights.Models;

/// <summary>
/// Interface définissant le contrat pour la publication d'événements du jeu.
/// Implémente le pattern Observateur - Rôle du Sujet (Subject/Publisher).
/// 
/// La classe qui implémente cette interface est responsable de:
/// - Enregistrer les observateurs
/// - Notifier les observateurs des changements
/// </summary>
public interface IGameEventPublisher
{
    /// <summary>
    /// Enregistre un observateur pour recevoir les notifications du jeu
    /// </summary>
    /// <param name="observer">L'observateur à enregistrer</param>
    void Subscribe(IGameObserver observer);

    /// <summary>
    /// Désenregistre un observateur
    /// </summary>
    /// <param name="observer">L'observateur à retirer</param>
    void Unsubscribe(IGameObserver observer);

    /// <summary>
    /// Publie un message de jeu à tous les observateurs
    /// </summary>
    /// <param name="message">Le message texte</param>
    /// <param name="type">Le type de message</param>
    void PublishMessage(string message, MessageType type);

    /// <summary>
    /// Publie un changement d'état à tous les observateurs
    /// </summary>
    /// <param name="state">Le nouvel état du jeu</param>
    /// <param name="affectedPlayer">Le joueur affecté (optionnel)</param>
    void PublishStateChange(GameState state, Player? affectedPlayer = null);

    /// <summary>
    /// Publie une alerte de joueur en danger à tous les observateurs
    /// </summary>
    /// <param name="player">Le joueur n'ayant plus qu'une carte</param>
    void PublishPlayerInDanger(Player player);

    /// <summary>
    /// Publie la fin de partie à tous les observateurs
    /// </summary>
    /// <param name="winner">Le joueur gagnant</param>
    void PublishGameEnded(Player winner);
}