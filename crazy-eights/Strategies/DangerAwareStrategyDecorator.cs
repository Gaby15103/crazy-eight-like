using crazy_eights.Models;

namespace crazy_eights.Strategies;

/// <summary>
/// Décorateur de stratégie permettant d'adapter dynamiquement le comportement d'un joueur lorsqu'un adversaire est en danger.
/// </summary>
public class DangerAwareStrategyDecorator : IPlayerStrategy
{
    private readonly IPlayerStrategy _innerStrategy;
    private readonly Func<bool> _isAnyOpponentInDanger;
    private readonly ActionCardPriorityStrategy _actionStrategy = new();

    /// <inheritdoc/>
    public string Name => _innerStrategy.Name;

    /// <summary>
    /// Initialise une nouvelle instance du décorateur de stratégie sensible au danger.
    /// </summary>
    /// <param name="innerStrategy">La stratégie de base à envelopper.</param>
    /// <param name="isAnyOpponentInDanger">Fonction retournant vrai si un adversaire est en situation critique.</param>
    public DangerAwareStrategyDecorator(IPlayerStrategy innerStrategy, Func<bool> isAnyOpponentInDanger)
    {
        _innerStrategy = innerStrategy;
        _isAnyOpponentInDanger = isAnyOpponentInDanger;
    }

    /// <inheritdoc/>
    public Card? ChooseCard(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay,
        bool isOpponentInDanger = false)
    {
        if (!_isAnyOpponentInDanger() && !isOpponentInDanger)
            return _innerStrategy.ChooseCard(hand, topCard, isValidPlay);
        var actionCard = _actionStrategy.ChooseCard(hand, topCard, isValidPlay);
        if (actionCard.HasValue)
        {
            return actionCard;
        }

        return _innerStrategy.ChooseCard(hand, topCard, isValidPlay);
    }

    /// <inheritdoc/>
    public CardColor ChooseColor(IEnumerable<Card> hand, Card topCard, Func<Card, Card, bool> isValidPlay,
        bool isOpponentInDanger = false)
    {
        if (_isAnyOpponentInDanger() || isOpponentInDanger)
        {
            return _actionStrategy.ChooseColor(hand, topCard, isValidPlay);
        }

        return _innerStrategy.ChooseColor(hand, topCard, isValidPlay);
    }
}