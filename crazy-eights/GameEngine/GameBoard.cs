using crazy_eights.Models;

namespace crazy_eights.GameEngine;
/// <summary>
/// Représente le plateau de jeu qui gère les joueurs, les piles de cartes et l'ordre des tours pour une partie.
/// </summary>
public class GameBoard
{
    /// <summary>
    /// Liste des joueurs qui participe à la partie
    /// </summary>
    public List<Player> Players { get;}
    /// <summary>
    /// La pile de pioche dont les joueur piochsnt
    /// </summary>
    public DrawStack DrawStack { get; }
    /// <summary>
    /// La pile de dépôt où les joueurs placent leur carte joué
    /// </summary>
    public DepositeStack DepositeStack { get; }
    /// <summary>
    /// Si le jeux est en sens horaire
    /// </summary>
    public bool IsClockwise { get; private set; }


    public GameBoard(List<Player> players, DrawStack drawStack, DepositeStack depositeStack)
    {
        Players = players ?? new List<Player>();
        DrawStack = drawStack;
        DepositeStack = depositeStack;
        IsClockwise = true;
    }
    /// <summary>
    /// Fonction pour changer le sens de rotation du jeu
    /// </summary>
    public void ReverseTurnOrder()
    {
        IsClockwise = !IsClockwise;
    }
    /// <summary>
    /// Fonction pour obtenir le prochain joueur à jouer
    /// </summary>
    /// <param name="currentIndex">L'index du joueur dont c'est le tour actuel</param>
    /// <returns>L'index do prochain joueur suivant</returns>
    public int GetNextPlayerIndex(int currentIndex)
    {
        int count = Players.Count;
        if (count == 0) return 0;

        if (IsClockwise)
        {
            return (currentIndex + 1) % count;
        }

        return (currentIndex - 1 + count) % count;
    }
}