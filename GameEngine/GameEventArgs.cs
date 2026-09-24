namespace crazy_eights.GameEngine;

/// <summary>
/// Événements liés au déroulement de la partie de Crazy Eights.
/// </summary>
public class GameEventArgs : EventArgs
{
    /// <summary>
    /// Message descriptif associé à l'événement de jeu.
    /// </summary>
    public string Message { get; }
    
    public MessageType Type { get; }
    
    public GameEventArgs(string message, MessageType type)
    {
        Message = $"[{DateTime.Now:HH:mm:ss}] [{type}] {message}" ;
        Type = type;
    }

    public override string ToString()
    {
        return Message;
    }
}