using crazy_eights.Models;

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
        _turnManager = new TurnManager();
    
        // Possibilité de fixer le joueur de départ pour les tests
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
        string lastMessage = "La partie commence !";
        NotifyMessage(lastMessage);

        int currentPlayerIndex = 0;

        while (IsRunning)
        {
            CurrentPlayer = Board.Players[currentPlayerIndex];

            await Task.Delay(2000);

            Card? cardToPlay = null;
            
            bool isOpponentInDanger = Board.Players
                .Where(p => p != CurrentPlayer)
                .Any(p => p.Hand.Count == 1);

            if (CurrentPlayer.Strategy != null)
            {
                cardToPlay = CurrentPlayer.Strategy.ChooseCard(
                    CurrentPlayer.Hand,
                    Board.DepositStack.TopCard,
                    (card, top) => _turnManager.IsValidePlay(card, top),
                    isOpponentInDanger
                );
            }
            else
            {
                cardToPlay =
                    CurrentPlayer.Hand.FirstOrDefault(c => _turnManager.IsValidePlay(c, Board.DepositStack.TopCard));
            }

            if (cardToPlay.HasValue)
            {
                CurrentPlayer.RemoveCard(cardToPlay.Value);
                Board.DepositStack.Push(cardToPlay.Value);
                lastMessage =
                    $"{CurrentPlayer.FirstName} a joué {cardToPlay.Value.Value.GetName()} de {cardToPlay.Value.Color}";
                NotifyMessage(lastMessage);
                if (cardToPlay.Value.Value == CardValue.Jack && CurrentPlayer.Strategy != null)
                {
                    CardColor color = CurrentPlayer.Strategy.ChooseColor(
                        CurrentPlayer.Hand,
                        Board.DepositStack.TopCard,
                        (card, top) => _turnManager.IsValidePlay(card, top),
                        isOpponentInDanger
                    );
                    await Task.Delay(500);
                    _turnManager.ApplyCardEffect(cardToPlay.Value, Board, ref currentPlayerIndex, NotifyMessage,
                        color);
                }
                else
                {
                    await Task.Delay(500);
                    _turnManager.ApplyCardEffect(cardToPlay.Value, Board, ref currentPlayerIndex, NotifyMessage,
                        Board.DepositStack.TopCard.Color);
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
                    lastMessage = "Pioche vide : recyclage de la pile de dépôt.";
                }

                if (Board.DrawStack.Count > 0)
                {
                    Card drawnCard = Board.DrawStack.DrawCard();
                    CurrentPlayer.AddCard(drawnCard);
                    lastMessage = $"{CurrentPlayer.FirstName} a pioché une carte.";
                }
                else
                {
                    lastMessage = "Match nul : plus aucune carte disponible.";
                    IsRunning = false;
                    break;
                }

                NotifyMessage(lastMessage);
                currentPlayerIndex = Board.GetNextPlayerIndex(currentPlayerIndex);
            }
        }
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