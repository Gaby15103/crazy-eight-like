namespace crazy_eights.GameEngine;

/// <summary>
/// Événements liés au déroulement de la partie de Paire de Cartes.
/// Contient les informations textuelles et le type de message pour l'historique.
/// </summary>
public class GameEventArgs : EventArgs
{
    /// <summary>
    /// Message descriptif associé à l'événement de jeu.
    /// </summary>
    public string Message { get; }
    /// <summary>
    /// Type de message catégorisant l'événement (Play, Effect, System).
    /// </summary>
    public MessageType Type { get; }

    public GameEventArgs(string message, MessageType type)
    {
        Message = $"[{DateTime.Now:HH:mm:ss}] [{type}] {message}" ;
        Type = type;
    }
    /// <summary>
    /// Retourne la représentation textuelle formatée de l'événement.
    /// </summary>
    /// <returns>Le message formaté avec l'horodatage et le type.</returns>
    public override string ToString()
    {
        return Message;
    }
}