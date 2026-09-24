using crazy_eights.Models;
using Xunit;

public class CardTests
{
    [Fact]
    public void Card_Reset_Restores_Original_Color()
    {
        var card = new Card(new CardColor("Cœur"), CardValue.Jack);
        
        var modifiedCard = card.WithColor(new CardColor("Pique"));
        modifiedCard.Reset();
        
        Assert.Equal("Cœur", modifiedCard.Color.Name);
    }
}