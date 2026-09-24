using crazy_eights.Models;

namespace crazy_eights.GameEngine;

public class TurnManager
{
    public bool IsValidePlay(Card cardToPlay, Card topDepositCard)
    {
        return cardToPlay.Color.Name == topDepositCard.Color.Name ||
               cardToPlay.Value == topDepositCard.Value ||
               cardToPlay.Value == CardValue.Jack;
    }

    public void ApplyCardEffect(Card playedCard, GameBoard board, ref int currentPlayerIndex,
        Action<string, MessageType> notifyMessage, CardColor newCardColor)
    {
        Player currentPlayer = board.Players[currentPlayerIndex];
        switch (playedCard.Value)
        {
            case CardValue.Ten:
                board.ReverseTurnOrder();
                notifyMessage(
                    $"{currentPlayer.FirstName} a changé le sens du jeu à {(board.IsClockwise ? "Horaires ↻" : "Anti-horaires ↺")}",
                    MessageType.Effect
                );
                currentPlayerIndex = board.GetNextPlayerIndex(currentPlayerIndex);
                break;
            case CardValue.As:
                int skippedIndex = board.GetNextPlayerIndex(currentPlayerIndex);
                notifyMessage(
                    $"{currentPlayer.FirstName} fait sauter le tour de {board.Players[skippedIndex].FirstName}",
                    MessageType.Effect
                );
                currentPlayerIndex = board.GetNextPlayerIndex(skippedIndex);
                break;
            case CardValue.Two:
                int nbCardToDraw = 2;
                int targetIndex = board.GetNextPlayerIndex(currentPlayerIndex);

                notifyMessage(
                    $"{currentPlayer.FirstName} attaque {board.Players[targetIndex].FirstName} avec un 2 ({nbCardToDraw} cartes) !",
                    MessageType.Effect
                );

                currentPlayerIndex = targetIndex;

                while (board.Players[currentPlayerIndex].CanPlayTwo())
                {
                    Player defendingPlayer = board.Players[currentPlayerIndex];
                    Card counterCard = defendingPlayer.Hand.First(c => c.Value == CardValue.Two);

                    defendingPlayer.RemoveCard(counterCard);
                    board.DepositeStack.Push(counterCard);

                    notifyMessage(
                        $"{defendingPlayer.FirstName} contre avec un Deux de {counterCard.Color} ! La peine monte à {nbCardToDraw + 2} cartes.",
                        MessageType.Play
                    );

                    nbCardToDraw += 2;
                    currentPlayerIndex = board.GetNextPlayerIndex(currentPlayerIndex);
                    Thread.Sleep(500);
                }

                Player victim = board.Players[currentPlayerIndex];
                notifyMessage($"{victim.FirstName} doit piocher les {nbCardToDraw} cartes de pénalité.",
                    MessageType.Effect);

                for (int i = 0; i < nbCardToDraw; i++)
                {
                    if (board.DrawStack.Count > 0)
                    {
                        victim.AddCard(board.DrawStack.DrawCard());
                    }
                }

                currentPlayerIndex = board.GetNextPlayerIndex(currentPlayerIndex);
                break;
            case CardValue.Jack:
                notifyMessage(
                    $"{currentPlayer.FirstName} a changé la couleur pour {newCardColor}",
                    MessageType.Effect
                );
                board.DepositeStack.SetTopCardColor(newCardColor);
                currentPlayerIndex = board.GetNextPlayerIndex(currentPlayerIndex);
                break;
            default:
                currentPlayerIndex = board.GetNextPlayerIndex(currentPlayerIndex);
                break;
        }
    }
}