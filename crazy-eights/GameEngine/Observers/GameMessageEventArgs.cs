namespace crazy_eights.GameEngine.Observers;

/// <summary>
/// Arguments d'événement pour les messages du jeu
/// Contient le message, son type et l'horodatage
/// Utilisé pour notifier les observateurs des actions de jeu
/// </summary>
public class GameMessageEventArgs : EventArgs
{
    /// <summary>
    /// Le message texte décrivant l'action du jeu
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Le type de message (Play, Effect, System)
    /// </summary>
    public MessageType Type { get; }

    /// <summary>
    /// L'horodatage du moment où l'événement s'est produit
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Initialise une nouvelle instance de GameMessageEventArgs
    /// </summary>
    /// <param name="message">Le message texte de l'événement</param>
    /// <param name="type">Le type de message</param>
    public GameMessageEventArgs(string message, MessageType type)
    {
        Message = message;
        Type = type;
        Timestamp = DateTime.Now;
    }

    /// <summary>
    /// Retourne la représentation textuelle formatée de l'événement avec horodatage
    /// </summary>
    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] [{Type}] {Message}";
    }
}