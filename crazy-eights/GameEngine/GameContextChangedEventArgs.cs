namespace crazy_eights.Strategies;

/// <summary>
/// Fournit les données d'événement pour les modifications apportées au contexte du jeu.
/// </summary>
public class GameContextChangedEventArgs : EventArgs
{
    /// <summary>
    /// Obtient le type de modification qui a eu lieu.
    /// </summary>
    public string ChangeType { get; }

    /// <summary>
    /// Obtient les données optionnelles associées à la modification.
    /// </summary>
    public object? Data { get; }

    public GameContextChangedEventArgs(string changeType, object? data)
    {
        ChangeType = changeType;
        Data = data;
    }
}