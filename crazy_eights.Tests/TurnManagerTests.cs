using crazy_eights.GameEngine;
using crazy_eights.Models;
using Xunit;

public class TurnManagerTests
{
    [Theory]
    [InlineData("Cœur", CardValue.Five, "Cœur", CardValue.Eight, true)]  // Même couleur
    [InlineData("Pique", CardValue.Eight, "Cœur", CardValue.Eight, true)] // Même valeur
    [InlineData("Trèfle", CardValue.Jack, "Cœur", CardValue.Eight, true)] // C'est un Valet (toujours valide)
    [InlineData("Carreau", CardValue.Three, "Cœur", CardValue.Eight, false)] // Ni couleur ni valeur
    public void IsValidePlay_Returns_Expected_Result(string playColor, CardValue playVal, string topColor, CardValue topVal, bool expected)
    {
        var turnManager = new TurnManager();
        var cardToPlay = new Card(new CardColor(playColor), playVal);
        var topCard = new Card(new CardColor(topColor), topVal);
        
        bool isValid = turnManager.IsValidePlay(cardToPlay, topCard);
        
        Assert.Equal(expected, isValid);
    }
}