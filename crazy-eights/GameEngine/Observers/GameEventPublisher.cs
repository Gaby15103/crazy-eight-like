namespace crazy_eights.GameEngine.Observers;

using crazy_eights.Models;

/// <summary>
/// Implémentation du pattern Observateur - Rôle du Sujet (Subject).
/// 
/// Cette classe gère la publication d'événements à tous les observateurs enregistrés.
/// Elle centralise toutes les notifications du jeu en un seul point.
/// 
/// Architecture avantages:
/// - Découplage entre le jeu (FishingGame) et les observateurs
/// - Facile d'ajouter de nouveaux observateurs sans modifier le jeu
/// - Permet le test unitaire des observateurs indépendamment
/// </summary>
public class GameEventPublisher : IGameEventPublisher
{
    /// <summary>
    /// Liste des observateurs enregistrés
    /// </summary>
    private readonly List<IGameObserver> _observers = new();

    /// <summary>
    /// Enregistre un observateur pour recevoir les notifications du jeu
    /// </summary>
    /// <param name="observer">L'observateur à enregistrer</param>
    public void Subscribe(IGameObserver observer)
    {
        if (observer != null && !_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    /// <summary>
    /// Désenregistre un observateur
    /// </summary>
    /// <param name="observer">L'observateur à retirer</param>
    public void Unsubscribe(IGameObserver observer)
    {
        _observers.Remove(observer);
    }

    /// <summary>
    /// Publie un message de jeu à tous les observateurs enregistrés
    /// </summary>
    /// <param name="message">Le message texte à publier</param>
    /// <param name="type">Le type de message (Play, Effect, System)</param>
    public void PublishMessage(string message, MessageType type)
    {
        var args = new GameMessageEventArgs(message, type);
        NotifyAllObservers(observer => observer.OnGameMessage(args));
    }

    /// <summary>
    /// Publie un changement d'état à tous les observateurs enregistrés
    /// </summary>
    /// <param name="state">Le nouvel état du jeu</param>
    /// <param name="affectedPlayer">Le joueur affecté (optionnel)</param>
    public void PublishStateChange(GameState state, Player? affectedPlayer = null)
    {
        var args = new GameStateChangedEventArgs(state, affectedPlayer);
        NotifyAllObservers(observer => observer.OnGameStateChanged(args));
    }

    /// <summary>
    /// Publie une alerte de joueur en danger à tous les observateurs enregistrés
    /// </summary>
    /// <param name="player">Le joueur n'ayant plus qu'une carte</param>
    public void PublishPlayerInDanger(Player player)
    {
        NotifyAllObservers(observer => observer.OnPlayerInDanger(player));
    }

    /// <summary>
    /// Publie la fin de partie à tous les observateurs enregistrés
    /// </summary>
    /// <param name="winner">Le joueur gagnant</param>
    public void PublishGameEnded(Player winner)
    {
        NotifyAllObservers(observer => observer.OnGameEnded(winner));
    }

    /// <summary>
    /// Notifie tous les observateurs en exécutant une action donnée
    /// Méthode utilitaire interne pour éviter la duplication de code
    /// </summary>
    /// <param name="notifyAction">L'action à exécuter pour chaque observateur</param>
    private void NotifyAllObservers(Action<IGameObserver> notifyAction)
    {
        foreach (var observer in _observers)
        {
            try
            {
                notifyAction(observer);
            }
            catch (Exception ex)
            {
                // Évite qu'une exception d'un observateur bloque les autres
                Console.WriteLine($"Erreur lors de la notification d'un observateur: {ex.Message}");
            }
        }
    }
}