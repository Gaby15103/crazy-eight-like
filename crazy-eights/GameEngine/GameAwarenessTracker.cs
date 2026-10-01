using crazy_eights.GameEngine;
using crazy_eights.Models;

namespace crazy_eights.Strategies;

/// <summary>
/// Suit l'état de la partie et gère des statistiques sur la composition probable de la main de chaque joueur.
/// </summary>
public class GameAwarenessTracker
{
    /// <summary>
    /// Toutes les cartes jouées et visibles dans l'historique de la pile de dépôt.
    /// </summary>
    private readonly HashSet<Card> _playedCards = new();

    /// <summary>
    /// Cartes piochées dans la pile de pioche.
    /// </summary>
    private readonly HashSet<Card> _drawnCards = new();

    /// <summary>
    /// Pour chaque joueur, suit des statistiques sur ses cartes jouées.
    /// </summary>
    private readonly Dictionary<string, PlayerProfile> _playerProfiles = new();

    /// <summary>
    /// Paquet complet pour les calculs de probabilité.
    /// </summary>
    private readonly List<Card> _completeDeck;
    
    public GameAwarenessTracker(List<Player> players)
    {
        _completeDeck = new CardPair().Generate52Cards();
        foreach (var player in players)
        {
            _playerProfiles[player.Id] = new PlayerProfile(player.FirstName);
        }
    }

    /// <summary>
    /// Enregistre une carte qui a été jouée par un joueur.
    /// </summary>
    /// <param name="card">La carte jouée.</param>
    /// <param name="playerId">L'identifiant du joueur.</param>
    public void RecordPlayedCard(Card card, string playerId)
    {
        _playedCards.Add(card);
        if (_playerProfiles.TryGetValue(playerId, out var profile))
        {
            profile.RecordPlayedCard(card);
        }
    }

    /// <summary>
    /// Enregistre une carte qui a été piochée depuis la pile de pioche.
    /// </summary>
    /// <param name="card">La carte piochée.</param>
    public void RecordDrawnCard(Card card)
    {
        _drawnCards.Add(card);
    }

    /// <summary>
    /// Obtient la probabilité qu'une couleur spécifique se trouve dans la main d'un adversaire.
    /// </summary>
    /// <param name="playerId">L'identifiant du joueur cible.</param>
    /// <param name="color">La couleur recherchée.</param>
    /// <param name="playerHandSize">La taille de la main du joueur.</param>
    /// <returns>La probabilité estimée.</returns>
    public double GetColorProbabilityInHand(string playerId, CardColor color, int playerHandSize)
    {
        if (!_playerProfiles.TryGetValue(playerId, out var profile))
            return 0.5;
        
        var unknownCardsOfColor = _completeDeck
            .Count(c => c.Color == color && !_playedCards.Contains(c) && !_drawnCards.Contains(c));
        
        var totalUnknownCards = _completeDeck
            .Count(c => !_playedCards.Contains(c) && !_drawnCards.Contains(c));

        if (totalUnknownCards == 0) return 0;
        
        double baseProbability = (double)unknownCardsOfColor / totalUnknownCards;
        
        double playerBias = profile.GetColorBias(color);
        
        return (0.4 * playerBias) + (0.6 * baseProbability);
    }

    /// <summary>
    /// Obtient la probabilité qu'une VALEUR de carte spécifique se trouve dans la main d'un adversaire.
    /// </summary>
    /// <param name="playerId">L'identifiant du joueur cible.</param>
    /// <param name="value">La valeur recherchée.</param>
    /// <param name="playerHandSize">La taille de la main du joueur.</param>
    /// <returns>La probabilité estimée.</returns>
    public double GetValueProbabilityInHand(string playerId, CardValue value, int playerHandSize)
    {
        var unknownCardsOfValue = _completeDeck
            .Count(c => c.Value == value && !_playedCards.Contains(c) && !_drawnCards.Contains(c));

        var totalUnknownCards = _completeDeck
            .Count(c => !_playedCards.Contains(c) && !_drawnCards.Contains(c));

        if (totalUnknownCards == 0) return 0;

        return (double)unknownCardsOfValue / totalUnknownCards;
    }

    /// <summary>
    /// Prédit la meilleure couleur à jouer pour bloquer un adversaire spécifique.
    /// </summary>
    /// <param name="targetPlayerId">L'identifiant du joueur cible.</param>
    /// <param name="targetPlayerHandSize">La taille de la main du joueur cible.</param>
    /// <returns>La couleur choisie pour le blocage.</returns>
    public CardColor PredictBestBlockingColor(string targetPlayerId, int targetPlayerHandSize)
    {
        var colorProbabilities = new Dictionary<CardColor, double>
        {
            { CardColor.Hearts, GetColorProbabilityInHand(targetPlayerId, CardColor.Hearts, targetPlayerHandSize) },
            { CardColor.Diamonds, GetColorProbabilityInHand(targetPlayerId, CardColor.Diamonds, targetPlayerHandSize) },
            { CardColor.Clubs, GetColorProbabilityInHand(targetPlayerId, CardColor.Clubs, targetPlayerHandSize) },
            { CardColor.Spades, GetColorProbabilityInHand(targetPlayerId, CardColor.Spades, targetPlayerHandSize) }
        };
        
        return colorProbabilities
            .OrderBy(kvp => kvp.Value)
            .First()
            .Key;
    }

    /// <summary>
    /// Estime la probabilité qu'un joueur puisse jouer une combinaison donnée de carte ou de couleur.
    /// </summary>
    /// <param name="playerId">L'identifiant du joueur.</param>
    /// <param name="nextPlayCard">La carte à jouer.</param>
    /// <param name="forcedColor">La couleur forcée (optionnel).</param>
    /// <returns>La probabilité estimée.</returns>
    public double EstimateCanPlay(string playerId, Card nextPlayCard, CardColor? forcedColor = null)
    {
        if (forcedColor.HasValue)
        {
            return GetColorProbabilityInHand(playerId, forcedColor.Value, 1);
        }
        
        double colorProb = GetColorProbabilityInHand(playerId, nextPlayCard.Color, 1);
        double valueProb = GetValueProbabilityInHand(playerId, nextPlayCard.Value, 1);
        
        return 1 - ((1 - colorProb) * (1 - valueProb));
    }

    /// <summary>
    /// Obtient le profil du joueur pour inspection ou test.
    /// </summary>
    /// <param name="playerId">L'identifiant du joueur.</param>
    /// <returns>Le profil du joueur, ou null s'il n'est pas trouvé.</returns>
    public PlayerProfile? GetPlayerProfile(string playerId)
    {
        return _playerProfiles.TryGetValue(playerId, out var profile) ? profile : null;
    }

    /// <summary>
    /// Représente les statistiques sur l'historique des cartes d'un seul joueur.
    /// </summary>
    public class PlayerProfile
    {
        public string PlayerName { get; }
        private readonly Dictionary<CardColor, int> _colorPlayCounts = new();
        private readonly Dictionary<CardValue, int> _valuePlayCounts = new();
        private int _totalCardsPlayed = 0;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PlayerProfile"/>.
        /// </summary>
        /// <param name="playerName">Le nom du joueur.</param>
        public PlayerProfile(string playerName)
        {
            PlayerName = playerName;
            foreach (CardColor color in CardColor.All)
            {
                _colorPlayCounts[color] = 0;
            }
            foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
            {
                _valuePlayCounts[value] = 0;
            }
        }

        /// <summary>
        /// Enregistre une carte jouée dans le profil du joueur.
        /// </summary>
        /// <param name="card">La carte jouée.</param>
        public void RecordPlayedCard(Card card)
        {
            _colorPlayCounts[card.Color]++;
            _valuePlayCounts[card.Value]++;
            _totalCardsPlayed++;
        }

        /// <summary>
        /// Retourne la proportion des cartes jouées par ce joueur qui étaient d'une couleur spécifique.
        /// </summary>
        /// <param name="color">La couleur évaluée.</param>
        /// <returns>Le ratio de la couleur (entre 0 et 1).</returns>
        public double GetColorBias(CardColor color)
        {
            if (_totalCardsPlayed == 0) return 0.25;
            return (double)_colorPlayCounts[color] / _totalCardsPlayed;
        }

        /// <summary>
        /// Retourne la proportion des cartes jouées par ce joueur qui avaient une valeur spécifique.
        /// </summary>
        /// <param name="value">La valeur évaluée.</param>
        /// <returns>Le ratio de la valeur (entre 0 et 1).</returns>
        public double GetValueBias(CardValue value)
        {
            if (_totalCardsPlayed == 0) return 1.0 / 13;
            return (double)_valuePlayCounts[value] / _totalCardsPlayed;
        }

        /// <summary>
        /// Obtient le nombre total de cartes jouées par le joueur.
        /// </summary>
        /// <returns>Le nombre total de cartes.</returns>
        public int GetTotalCardsPlayed() => _totalCardsPlayed;
    }
}