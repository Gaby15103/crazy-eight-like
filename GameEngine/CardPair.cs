using crazy_eights.Models;

namespace crazy_eights.GameEngine;

/// <summary>
/// Class utilitaire pour générer un jeu standard de 52 cartes à jouer
/// </summary>
public class CardPair
{
    /// <summary>
    /// Génère un jeu standard de 52 cartes à jouer
    /// </summary>
    /// <returns>Une list de Card représentant un nouveau jeu de 52 cartes ordonné</returns>
    public List<Card> Generate52Cards()
    {
        List<CardColor> colors =
            [CardColor.Clubs, CardColor.Diamonds, CardColor.Hearts, CardColor.Spades];

        return [.. from color in colors from value in Enum.GetValues<CardValue>() select new Card(color, value)];
    }
}