namespace crazy_eights.GameEngine.Observers;

using crazy_eights.Models;

/// <summary>
/// Implémentation de IGameObserver pour l'interface utilisateur Terminal.
/// 
/// Cette classe représente l'observateur qui met à jour l'affichage TUI
/// en réponse aux événements du jeu.
/// 
/// Responsabilités:
/// - Afficher les messages de jeu
/// - Mettre à jour l'affichage lors des changements d'état
/// - Signaler les alertes (joueur en danger, fin de partie)
/// </summary>
public class GameTuiObserver : IGameObserver
{
    /// <summary>
    /// Référence à la vue TUI à mettre à jour
    /// </summary>
    private readonly GameTuiView _tuiView;

    /// <summary>
    /// Référence au plateau de jeu pour accéder aux données
    /// </summary>
    private readonly GameBoard _gameBoard;

    /// <summary>
    /// Initialise une nouvelle instance du GameTuiObserver
    /// </summary>
    /// <param name="tuiView">La vue TUI à mettre à jour</param>
    /// <param name="gameBoard">Le plateau de jeu</param>
    public GameTuiObserver(GameTuiView tuiView, GameBoard gameBoard)
    {
        _tuiView = tuiView ?? throw new ArgumentNullException(nameof(tuiView));
        _gameBoard = gameBoard ?? throw new ArgumentNullException(nameof(gameBoard));
    }

    /// <summary>
    /// Appelé quand un message de jeu est publié
    /// </summary>
    public void OnGameMessage(GameMessageEventArgs args)
    {
        // Convertir en GameEventArgs pour la compatibilité avec GameTuiView
        var legacyArgs = new GameEventArgs(args.Message, args.Type);
        _tuiView.UpdateBoard(_gameBoard, "System", legacyArgs, new List<Card>(), "N/A");
    }

    /// <summary>
    /// Appelé quand l'état du jeu change
    /// </summary>
    public void OnGameStateChanged(GameStateChangedEventArgs args)
    {
        string stateMessage = args.State switch
        {
            GameState.Started => "La partie commence!",
            GameState.PlayerTurn => $"Au tour de {args.AffectedPlayer?.FirstName}",
            GameState.CardPlayed => "Carte jouée",
            GameState.CardDrawn => "Carte pioché",
            GameState.SpecialEffectApplied => "Effet spécial appliqué",
            GameState.OneCardLeft => $"⚠️ {args.AffectedPlayer?.FirstName} n'a plus qu'une carte!",
            GameState.Completed => "La partie est terminée",
            _ => "État inconnu"
        };

        var args_event = new GameEventArgs(stateMessage, MessageType.System);
        _tuiView.UpdateBoard(_gameBoard, args.AffectedPlayer?.FirstName ?? "System", args_event, new List<Card>(), "N/A");
    }

    /// <summary>
    /// Appelé quand un joueur n'a plus qu'une seule carte
    /// </summary>
    public void OnPlayerInDanger(Player player)
    {
        var args = new GameEventArgs($"ALERTE UNO : {player.FirstName} n'a plus qu'une carte!", MessageType.System);
        _tuiView.UpdateBoard(_gameBoard, player.FirstName, args, player.Hand, player.Strategy?.Name ?? "N/A");
    }

    /// <summary>
    /// Appelé quand la partie se termine
    /// </summary>
    public void OnGameEnded(Player winner)
    {
        var args = new GameEventArgs($"FIN DE PARTIE ! Gagnant : {winner.FirstName} {winner.LastName}", MessageType.System);
        _tuiView.UpdateBoard(_gameBoard, winner.FirstName, args, winner.Hand, winner.Strategy?.Name ?? "N/A");
    }
}