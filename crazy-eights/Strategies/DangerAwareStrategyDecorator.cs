using crazy_eights.Models;

namespace crazy_eights.Strategies;

/// <summary>
/// Décorateur de stratégie permettant d'adapter dynamiquement le comportement d'un joueur lorsqu'un adversaire est en danger.
/// </summary>
public class DangerAwareStrategyDecorator : IPlayerStrategy
{
    private readonly IPlayerStrategy _innerStrategy;
    private readonly Func<bool> _isAnyOpponentInDanger;
    private readonly IPlayerStrategy _dangerStrategy;

    /// <inheritdoc/>
    public string Name => _innerStrategy.Name;

    /// <summary>
    /// Initialise une nouvelle instance du décorateur de stratégie sensible au danger.
    /// </summary>
    /// <param name="innerStrategy">La stratégie de base à envelopper.</param>
    /// <param name="isAnyOpponentInDanger">Fonction retournant vrai si un adversaire est en situation critique.</param>
    public DangerAwareStrategyDecorator(
        IPlayerStrategy innerStrategy, 
        Func<bool> isAnyOpponentInDanger, 
        IPlayerStrategy? dangerStrategy = null)
    {
        _innerStrategy = innerStrategy;
        _isAnyOpponentInDanger = isAnyOpponentInDanger;
        // Si aucune stratégie de danger n'est fournie, on utilise par défaut la priorité des cartes action
        _dangerStrategy = dangerStrategy ?? new ActionCardPriorityStrategy();
    }

    /// <inheritdoc/>
    public Card? ChooseCard(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay,
        bool isOpponentInDanger = false)
    {
        if (_isAnyOpponentInDanger() || isOpponentInDanger)
        {
            var dangerCard = _dangerStrategy.ChooseCard(hand, topCard, isValidPlay, isOpponentInDanger);
            if (dangerCard.HasValue)
            {
                return dangerCard;
            }
        }

        return _innerStrategy.ChooseCard(hand, topCard, isValidPlay, isOpponentInDanger);
    }

    /// <inheritdoc/>
    public CardColor ChooseColor(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay,
        bool isOpponentInDanger = false)
    {
        if (_isAnyOpponentInDanger() || isOpponentInDanger)
        {
            return _dangerStrategy.ChooseColor(hand, topCard, isValidPlay, isOpponentInDanger);
        }

        return _innerStrategy.ChooseColor(hand, topCard, isValidPlay, isOpponentInDanger);
    }
}