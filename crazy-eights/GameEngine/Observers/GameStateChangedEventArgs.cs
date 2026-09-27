namespace crazy_eights.GameEngine.Observers;

using crazy_eights.Models;

/// <summary>
/// Arguments d'événement pour les changements d'état du jeu
/// Contient le nouvel état et le joueur affecté (si applicable)
/// Utilisé pour notifier les observateurs des transitions d'état
/// </summary>
public class GameStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Le nouvel état du jeu
    /// </summary>
    public GameState State { get; }

    /// <summary>
    /// Le joueur affecté par le changement d'état (peut être null)
    /// </summary>
    public Player? AffectedPlayer { get; }

    /// <summary>
    /// L'horodatage du moment où le changement s'est produit
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Initialise une nouvelle instance de GameStateChangedEventArgs
    /// </summary>
    /// <param name="state">Le nouvel état du jeu</param>
    /// <param name="affectedPlayer">Le joueur affecté par ce changement (peut être null)</param>
    public GameStateChangedEventArgs(GameState state, Player? affectedPlayer = null)
    {
        State = state;
        AffectedPlayer = affectedPlayer;
        Timestamp = DateTime.Now;
    }

    /// <summary>
    /// Retourne la représentation textuelle du changement d'état
    /// </summary>
    public override string ToString()
    {
        string affectedInfo = AffectedPlayer != null
            ? $" - Joueur: {AffectedPlayer.FirstName} {AffectedPlayer.LastName}"
            : "";
        return $"[{Timestamp:HH:mm:ss}] État: {State}{affectedInfo}";
    }
}