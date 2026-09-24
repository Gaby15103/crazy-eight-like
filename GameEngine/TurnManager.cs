using crazy_eights.Models;

namespace crazy_eights.GameEngine;

public class TurnManager
{
    public bool IsValidePlay(Card cardToPlay, Card topDepositCard)
    {
        return cardToPlay.Color.Name == topDepositCard.Color.Name || cardToPlay.Value == topDepositCard.Value ||
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
                    $"{currentPlayer.FirstName} a changé le sense du jeu à {(board.IsClockwise ? "Horaires ↻" : "Anti-horaires ↺")}",
                    MessageType.Effect
                    );
                break;
            case CardValue.As:
                notifyMessage(
                    $"{currentPlayer.FirstName} fait sauté son tour à {board.Players[board.GetNextPlayerIndex(currentPlayerIndex)].FirstName}",
                    MessageType.Effect
                    );
                currentPlayerIndex = board.GetNextPlayerIndex(currentPlayerIndex);
                break;
            case CardValue.Two:
                int nbCardToDraw = 2;
                notifyMessage(
                    $"{currentPlayer.FirstName} fait pigé {nbCardToDraw} cart à {board.Players[board.GetNextPlayerIndex(currentPlayerIndex)].FirstName}",
                    MessageType.Effect
                );
                currentPlayerIndex = board.GetNextPlayerIndex(currentPlayerIndex);
                while (board.Players[currentPlayerIndex].CanPlayTwo())
                {
                    Card cardToPlay = board.Players[currentPlayerIndex].Hand.FirstOrDefault(c => c.Value == CardValue.Two);
                    board.Players[currentPlayerIndex].RemoveCard(cardToPlay);
                    board.DepositeStack.Push(cardToPlay);
                    notifyMessage(
                        $"{currentPlayer.FirstName} a joué {cardToPlay.Value.GetName()} de {cardToPlay.Color}",
                        MessageType.Play);
                    nbCardToDraw += 2;
                    notifyMessage(
                        $"{currentPlayer.FirstName} fait pigé {nbCardToDraw} cart à {board.Players[board.GetNextPlayerIndex(currentPlayerIndex)].FirstName}",
                        MessageType.Effect
                    );
                    currentPlayerIndex = board.GetNextPlayerIndex(currentPlayerIndex);
                    Task.Delay(1000);
                }
                for (int i = 0; i < nbCardToDraw; i++)
                {
                    Card drawnCard = board.DrawStack.DrawCard();
                    board.Players[currentPlayerIndex].AddCard(drawnCard);
                }
                break;
            case CardValue.Jack:
                notifyMessage(
                    $"{currentPlayer.FirstName} à changer la couleur à {newCardColor.ToString()}",
                    MessageType.Effect
                );
                board.DepositeStack.TopCard.Color = newCardColor;
                break;
            default:
                break;
        }

        currentPlayerIndex = board.GetNextPlayerIndex(currentPlayerIndex);
    }
}