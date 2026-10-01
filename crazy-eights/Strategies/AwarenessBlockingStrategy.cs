using crazy_eights.Models;

namespace crazy_eights.Strategies;
/// <summary>
/// Stratégie de jeu axée sur le blocage des adversaires en situation de danger.
/// </summary>
public class AwarenessBlockingStrategy : IPlayerStrategy
{
    private readonly IPlayerStrategy _fallbackStrategy;
    /// <inheritdoc />
    public string Name => "Blocage intelligent";

    public AwarenessBlockingStrategy(IPlayerStrategy? fallbackStrategy = null)
    {
        _fallbackStrategy = fallbackStrategy ?? new MinimizingPointsStrategy();
    }
    /// <inheritdoc />
    public Card? ChooseCard(
        GameContext context,
        IEnumerable<Card> hand,
        Card topCard,
        Func<Card, Card, bool> isValidPlay)
    {
        var validCards = hand.Where(c => isValidPlay(c, topCard)).ToList();
        if (!validCards.Any()) return null;
        
        if (!context.IsAnyOpponentInDanger || context.DangerPlayer is null)
            return _fallbackStrategy.ChooseCard(context, hand, topCard, isValidPlay);

        var dangerPlayer = context.DangerPlayer;
        
        var actionCards = validCards
            .Where(c => c.Value.IsActionCard())
            .OrderBy(c => c.Value == CardValue.Two ? 0 : 
                         c.Value == CardValue.Jack ? 1 : 
                         c.Value == CardValue.As ? 2 : 3)
            .ToList();

        if (actionCards.Any())
            return actionCards.First();
        
        return validCards
            .OrderBy(c => context.Awareness.GetColorProbabilityInHand(
                dangerPlayer.Id, c.Color, dangerPlayer.Hand.Count))
            .First();
    }
    /// <inheritdoc />
    public CardColor ChooseColor(
        GameContext context,
        IEnumerable<Card> hand,
        Card topCard,
        Func<Card, Card, bool> isValidPlay)
    {
        if (!context.IsAnyOpponentInDanger || context.DangerPlayer is null)
            return _fallbackStrategy.ChooseColor(context, hand, topCard, isValidPlay);

        return context.Awareness.PredictBestBlockingColor(
            context.DangerPlayer.Id,
            context.DangerPlayer.Hand.Count);
    }
}