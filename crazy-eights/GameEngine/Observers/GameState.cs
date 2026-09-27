namespace crazy_eights.GameEngine.Observers;

/// <summary>
/// Énumération des états possibles du jeu
/// Utilisée par le pattern observateur pour notifier les changements d'état
/// </summary>
public enum GameState
{
    /// <summary>La partie a commencé</summary>
    Started,

    /// <summary>C'est le tour d'un joueur</summary>
    PlayerTurn,

    /// <summary>Une carte a été jouée</summary>
    CardPlayed,

    /// <summary>Une carte a été pioché</summary>
    CardDrawn,

    /// <summary>Un effet spécial de carte a été appliqué</summary>
    SpecialEffectApplied,

    /// <summary>Un joueur n'a plus qu'une carte</summary>
    OneCardLeft,

    /// <summary>La partie est terminée</summary>
    Completed
}