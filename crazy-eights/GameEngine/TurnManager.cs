using crazy_eights.Models;

namespace crazy_eights.GameEngine;
/// <summary>
/// Gère la logique des tours de jeu, la validation des coups et l'application des effets des cartes à effet.
/// </summary>
public class TurnManager
{
    /// <summary>
    /// Vérifie si une carte peut être jouée par rapport à la carte au sommet de la pile de dépôt.
    /// </summary>
    /// <param name="cardToPlay">La carte que le joueur souhaite jouer.</param>
    /// <param name="topDepositCard">La carte actuelle au sommet de la pile de dépôt.</param>
    /// <returns>Si le coup est valide.</returns>
    public bool IsValidePlay(Card cardToPlay, Card topDepositCard)
    {
        return cardToPlay.Color.Name == topDepositCard.Color.Name ||
               cardToPlay.Value == topDepositCard.Value ||
               cardToPlay.Value == CardValue.Jack;
    }
    /// <summary>
    /// Applique l'effet associé à une carte jouée.
    /// </summary>
    /// <param name="playedCard">La carte qui vient d'être jouée.</param>
    /// <param name="board">Le plateau de jeu contenant les joueurs et les piles.</param>
    /// <param name="currentPlayerIndex">L'index du joueur actuel.</param>
    /// <param name="notifyMessage">Action de notification pour informer les joueurs d'un événement ou d'un effet.</param>
    /// <param name="newCardColor">La nouvelle couleur choisie, si changé.</param>
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