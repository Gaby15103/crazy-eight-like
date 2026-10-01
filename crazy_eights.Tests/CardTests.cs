using crazy_eights.Models;
using Xunit;

namespace crazy_eights.Tests;
public class CardTests
{
    [Fact]
    public void Card_Reset_Restores_Original_Color()
    {
        var card = new Card(CardColor.Hearts, CardValue.Jack);
        
        var modifiedCard = card.WithColor(CardColor.Spades);
        modifiedCard.Reset();
        
        Assert.Equal(CardColor.Hearts.Name, modifiedCard.Color.Name);
    }
}