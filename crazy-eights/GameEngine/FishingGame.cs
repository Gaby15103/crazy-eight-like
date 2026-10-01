using crazy_eights.Models;
using crazy_eights.Strategies;

namespace crazy_eights.GameEngine;

/// <summary>
/// Gère la logique principale du jeu de cartes Paire de Cartes.
/// Contrôle le déroulement des tours, la distribution initiale et les états du jeu.
/// </summary>
public class FishingGame
{
    /// <summary>
    /// Le plateau de jeu contenant les joueurs et les piles pioche et défausse.
    /// </summary>
    public readonly GameBoard Board;

    /// <summary>
    /// Le gestionnaire des tours chargé de valider les coups et d'appliquer les effets des cartes.
    /// </summary>
    private readonly TurnManager _turnManager;

    /// <summary>
    /// Indique si la partie est actuellement en cours.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Le joueur dont c'est le tour actuel.
    /// </summary>
    public Player CurrentPlayer { get; private set; }

    /// <summary>
    /// Le Context de la partie en cours
    /// </summary>
    private readonly GameContext _context;

    /// <summary>
    /// Fired when the current player changes.
    /// </summary>
    public event EventHandler<Player>? OnCurrentPlayerChanged;

    /// <summary>
    /// Fired when a card is played.
    /// </summary>
    public event EventHandler<(Card card, string playerId)>? OnCardPlayed;

    /// <summary>
    /// Fired when a card is drawn.
    /// </summary>
    public event EventHandler<Card>? OnCardDrawn;

    /// <summary>
    /// Fired when game direction changes.
    /// </summary>
    public event EventHandler<bool>? OnDirectionChanged;

    /// <summary>
    /// Fired when draw pile size changes.
    /// </summary>
    public event EventHandler<int>? OnDrawPileSizeChanged;

    /// <summary>
    /// Déclenché lorsqu'un nouveau message d'action ou d'état doit être journalisé.
    /// </summary>
    public event EventHandler<GameEventArgs>? OnMessageLogged;

    /// <summary>
    /// Déclenché lorsqu'un joueur n'a plus qu'une seule carte en main.
    /// </summary>
    public event EventHandler<Player>? OnOneCardLeft;

    /// <summary>
    /// Déclenché lorsque la partie se termine (victoire d'un joueur ou match nul).
    /// </summary>
    public event EventHandler<Player>? OnGameEnded;

    /// <summary>
    /// Initialise une nouvelle instance du jeu
    /// Génère le paquet, mélange les cartes, distribue les mains initiales et prépare le plateau.
    /// </summary>
    /// <param name="players">La liste des joueurs participant à la partie.</param>
    /// <param name="config">La configuration du jeu, incluant la taille de la main initiale.</param>
    public FishingGame(List<Player> players, GameConfig config)
    {
        CardPair cardPair = new CardPair();
        List<Card> allCards = cardPair.Generate52Cards();

        DrawStack drawStack = new DrawStack(allCards);
        drawStack.Shuffle();


        foreach (Player player in players)
        {
            for (int i = 0; i < config.InitialHandSize; i++)
            {
                if (drawStack.Count > 0)
                {
                    player.AddCard(drawStack.DrawCard());
                }
            }
        }

        List<Card> initialDeposite = new List<Card>();
        if (drawStack.Count > 0)
        {
            initialDeposite.Add(drawStack.DrawCard());
        }

        DepositStack depositStack = new DepositStack(initialDeposite);

        Board = new GameBoard(players, drawStack, depositStack);
        var awareness = new GameAwarenessTracker(players);
        _context = new GameContext(players, awareness, this);
        _turnManager = new TurnManager();
        Random rnd = new Random();
        CurrentPlayer = Board.Players[rnd.Next(0, Board.Players.Count)];
        IsRunning = true;
    }

    /// <summary>
    /// Initialise une nouvelle instance du jeu pour les tests.
    /// Génère le paquet, mélange les cartes, distribue les mains initiales et prépare le plateau.
    /// </summary>
    /// <param name="players">La liste des joueurs participant à la partie.</param>
    /// <param name="config">La configuration du jeu, incluant la taille de la main initiale.</param>
    /// <param name="shuffleDeck">Si le packet de carte doit être mélangé</param>
    public FishingGame(List<Player> players, GameConfig config, bool shuffleDeck = true)
    {
        CardPair cardPair = new CardPair();
        List<Card> allCards = cardPair.Generate52Cards();

        DrawStack drawStack = new DrawStack(allCards);
        if (shuffleDeck)
        {
            drawStack.Shuffle();
        }

        foreach (Player player in players)
        {
            if (player.Hand.Count == 0)
            {
                for (int i = 0; i < config.InitialHandSize; i++)
                {
                    if (drawStack.Count > 0)
                    {
                        player.AddCard(drawStack.DrawCard());
                    }
                }
            }
        }

        List<Card> initialDeposite = new List<Card>();
        if (drawStack.Count > 0)
        {
            initialDeposite.Add(drawStack.DrawCard());
        }

        DepositStack depositStack = new DepositStack(initialDeposite);

        Board = new GameBoard(players, drawStack, depositStack);
        var awareness = new GameAwarenessTracker(players);
        _context = new GameContext(players, awareness, this);
        _turnManager = new TurnManager();

        CurrentPlayer = Board.Players[0];
        IsRunning = true;
    }

    /// <summary>
    /// Lance la boucle de jeu principale de manière asynchrone.
    /// Gère l'alternance des tours, l'application des stratégies, la pioche automatique
    /// et le recyclage de la défausse jusqu'à la fin de la partie.
    /// </summary>
    public async Task StartGameAsync()
    {
        IsRunning = true;
        NotifyMessage("La partie commence !");

        int currentPlayerIndex = 0;

        while (IsRunning)
        {
            CurrentPlayer = Board.Players[currentPlayerIndex];

            RaiseCurrentPlayerChanged(CurrentPlayer);
            RaiseDrawPileSizeChanged(Board.DrawStack.Count);

            await Task.Delay(2000);

            Card? cardToPlay = CurrentPlayer.Strategy.ChooseCard(
                _context,
                CurrentPlayer.Hand,
                Board.DepositStack.TopCard,
                (card, top) => _turnManager.IsValidePlay(card, top)
            );

            if (cardToPlay.HasValue)
            {
                CurrentPlayer.RemoveCard(cardToPlay.Value);
                Board.DepositStack.Push(cardToPlay.Value);
                
                RaiseCardPlayed(cardToPlay.Value, CurrentPlayer.Id);
                
                NotifyMessage($"{CurrentPlayer.FirstName} a joué {cardToPlay.Value.Value.GetName()} de {cardToPlay.Value.Color}");
                
                if (cardToPlay.Value.Value == CardValue.Jack)
                {
                    CardColor color = CurrentPlayer.Strategy.ChooseColor(
                        _context,
                        CurrentPlayer.Hand,
                        Board.DepositStack.TopCard,
                        (card, top) => _turnManager.IsValidePlay(card, top)
                    );
                    await Task.Delay(500);
                    _turnManager.ApplyCardEffect(cardToPlay.Value, Board, ref currentPlayerIndex, NotifyMessage, color, RaiseDirectionChanged);
                }
                else
                {
                    await Task.Delay(500);
                    _turnManager.ApplyCardEffect(cardToPlay.Value, Board, ref currentPlayerIndex, NotifyMessage,
                        Board.DepositStack.TopCard.Color, RaiseDirectionChanged);
                }

                if (CurrentPlayer.Hand.Count == 1)
                {
                    OnOneCardLeft?.Invoke(this, CurrentPlayer);
                }

                if (CurrentPlayer.Hand.Count == 0)
                {
                    IsRunning = false;
                    OnGameEnded?.Invoke(this, CurrentPlayer);
                    break;
                }
            }
            else
            {
                if (Board.DrawStack.Count == 0)
                {
                    List<Card> recycledCards = Board.DepositStack.TakeAllExcepTop();
                    Board.DrawStack.Refill(recycledCards);
                    NotifyMessage("Pioche vide : recyclage de la pile de dépôt.");
                }

                if (Board.DrawStack.Count > 0)
                {
                    Card drawnCard = Board.DrawStack.DrawCard();
                    CurrentPlayer.AddCard(drawnCard);
                    
                    RaiseCardDrawn(drawnCard);
                    RaiseDrawPileSizeChanged(Board.DrawStack.Count);

                    
                    NotifyMessage($"{CurrentPlayer.FirstName} a pioché une carte.");
                }
                else
                {
                    NotifyMessage("Match nul : plus aucune carte disponible.");
                    IsRunning = false;
                    break;
                }
                
                currentPlayerIndex = Board.GetNextPlayerIndex(currentPlayerIndex);
            }
        }
    }
    
    /// <summary>
    /// Raises the OnCardPlayed event. Protected so subclasses can use it.
    /// </summary>
    protected void RaiseCardPlayed(Card card, string playerId)
    {
        OnCardPlayed?.Invoke(this, (card, playerId));
    }

    /// <summary>
    /// Raises the OnCardDrawn event. Protected so subclasses can use it.
    /// </summary>
    protected void RaiseCardDrawn(Card card)
    {
        OnCardDrawn?.Invoke(this, card);
    }

    /// <summary>
    /// Raises the OnDirectionChanged event. Protected so subclasses can use it.
    /// </summary>
    protected void RaiseDirectionChanged(bool isClockwise)
    {
        OnDirectionChanged?.Invoke(this, isClockwise);
    }

    /// <summary>
    /// Raises the OnDrawPileSizeChanged event. Protected so subclasses can use it.
    /// </summary>
    protected void RaiseDrawPileSizeChanged(int size)
    {
        OnDrawPileSizeChanged?.Invoke(this, size);
    }

    /// <summary>
    /// Raises the OnCurrentPlayerChanged event. Protected so subclasses can use it.
    /// </summary>
    protected void RaiseCurrentPlayerChanged(Player player)
    {
        OnCurrentPlayerChanged?.Invoke(this, player);
    }

    /// <summary>
    /// Fonction qui déclenche l'événement de notification pour envoyer un message de log du jeu
    /// </summary>
    /// <param name="message">Le message texte décrivant l'action survenue dans le jeu.</param>
    /// <param name="type">Le type du message</param>
    protected void NotifyMessage(string message, MessageType type = MessageType.Play)
    {
        OnMessageLogged?.Invoke(this, new GameEventArgs(message, type));
    }
}