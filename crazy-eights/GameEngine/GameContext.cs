using crazy_eights.GameEngine;
using crazy_eights.Models;

namespace crazy_eights.Strategies;

/// <summary>
/// Fournit un contexte de jeu riche aux stratégies et se met à jour automatiquement via les événements du jeu.
/// Implémente le patron Observateur pour rester synchronisé avec l'état de la partie.
/// </summary>
public class GameContext
{
    /// <summary>
    /// Tous les joueurs de la partie.
    /// </summary>
    public IReadOnlyList<Player> AllPlayers { get; }

    /// <summary>
    /// Le suivi de conscience doté de l'historique du jeu et de données probabilistes.
    /// </summary>
    public GameAwarenessTracker Awareness { get; }

    /// <summary>
    /// Le joueur dont c'est actuellement le tour.
    /// </summary>
    public Player CurrentPlayer { get; private set; }

    /// <summary>
    /// La liste des adversaires (tous les joueurs sauf le joueur actuel).
    /// </summary>
    public IReadOnlyList<Player> Opponents =>
        AllPlayers.Where(p => p != CurrentPlayer).ToList().AsReadOnly();

    /// <summary>
    /// L'adversaire ayant le moins de cartes (si l'un d'eux possède exactement 1 carte).
    /// </summary>
    public Player? DangerPlayer =>
        Opponents.FirstOrDefault(p => p.Hand.Count == 1);

    /// <summary>
    /// Vrai si un adversaire possède exactement 1 carte.
    /// </summary>
    public bool IsAnyOpponentInDanger => DangerPlayer != null;

    /// <summary>
    /// La carte du dessus actuellement sur la pile de dépôt.
    /// </summary>
    public Card TopCard { get; private set; }

    /// <summary>
    /// Vrai si le sens du jeu est horaire.
    /// </summary>
    public bool IsClockwise { get; private set; }

    /// <summary>
    /// Le nombre de cartes restantes dans la pile de pioche.
    /// </summary>
    public int DrawPileSize { get; private set; }

    /// <summary>
    /// Événement déclenché lorsque l'état du contexte du jeu change.
    /// </summary>
    public event EventHandler<GameContextChangedEventArgs>? OnContextChanged;

    public GameContext(
        IReadOnlyList<Player> allPlayers,
        GameAwarenessTracker awareness,
        FishingGame game)
    {
        AllPlayers = allPlayers;
        Awareness = awareness;
        CurrentPlayer = allPlayers[0];
        TopCard = default;
        IsClockwise = true;
        DrawPileSize = 0;

        game.OnCurrentPlayerChanged += HandleCurrentPlayerChanged;
        game.OnCardPlayed += HandleCardPlayed;
        game.OnCardDrawn += HandleCardDrawn;
        game.OnDirectionChanged += HandleDirectionChanged;
        game.OnDrawPileSizeChanged += HandleDrawPileSizeChanged;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    public void SetCurrentPlayer(Player player)
    {
        CurrentPlayer = player;
    }

    /// <summary>
    /// Gère le changement de joueur actif et déclenche l'événement de modification du contexte.
    /// </summary>
    /// <param name="sender">L'objet source de l'événement.</param>
    /// <param name="player">Le nouveau joueur dont c'est le tour.</param>
    private void HandleCurrentPlayerChanged(object? sender, Player player)
    {
        CurrentPlayer = player;
        OnContextChanged?.Invoke(this, new GameContextChangedEventArgs(
            "CurrentPlayer", player));
    }

    /// <summary>
    /// Gère la carte jouée, met à jour la carte du dessus, enregistre l'action dans le suivi et déclenche l'événement.
    /// </summary>
    /// <param name="sender">L'objet source de l'événement.</param>
    /// <param name="data">Un tuple contenant la carte jouée et l'identifiant du joueur.</param>
    private void HandleCardPlayed(object? sender, (Card card, string playerId) data)
    {
        TopCard = data.card;
        Awareness.RecordPlayedCard(data.card, data.playerId);
        OnContextChanged?.Invoke(this, new GameContextChangedEventArgs(
            "CardPlayed", data));
    }

    /// <summary>
    /// Gère la carte piochée, l'enregistre dans le suivi et déclenche l'événement de modification du contexte.
    /// </summary>
    /// <param name="sender">L'objet source de l'événement.</param>
    /// <param name="card">La carte qui a été piochée.</param>
    private void HandleCardDrawn(object? sender, Card card)
    {
        Awareness.RecordDrawnCard(card);
        OnContextChanged?.Invoke(this, new GameContextChangedEventArgs(
            "CardDrawn", card));
    }

    /// <summary>
    /// Gère le changement de direction du jeu et déclenche l'événement de modification du contexte.
    /// </summary>
    /// <param name="sender">L'objet source de l'événement.</param>
    /// <param name="isClockwise">si le nouveau sens est horaire.</param>
    private void HandleDirectionChanged(object? sender, bool isClockwise)
    {
        IsClockwise = isClockwise;
        OnContextChanged?.Invoke(this, new GameContextChangedEventArgs(
            "DirectionChanged", isClockwise));
    }

    /// <summary>
    /// Gère le changement de taille de la pile de pioche et déclenche l'événement de modification du contexte.
    /// </summary>
    /// <param name="sender">L'objet source de l'événement.</param>
    /// <param name="size">La nouvelle taille de la pile de pioche.</param>
    private void HandleDrawPileSizeChanged(object? sender, int size)
    {
        DrawPileSize = size;
        OnContextChanged?.Invoke(this, new GameContextChangedEventArgs(
            "DrawPileSizeChanged", size));
    }
}