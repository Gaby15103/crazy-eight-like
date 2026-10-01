using crazy_eights.GameEngine;
using crazy_eights.Models;
using Xunit;
namespace crazy_eights.Tests;
public class TurnManagerTests
{
    public GameBoard CreateBoard(List<Player> players)
    {
        DrawStack drawStack = new DrawStack(new CardPair().Generate52Cards());
        List<Card> initialDeposite = new List<Card>();
        if (drawStack.Count > 0)
        {
            initialDeposite.Add(drawStack.DrawCard());
        }
        return new GameBoard(players,drawStack, new DepositStack(initialDeposite));
    }
    private GameBoard CreateBoard(List<Player> players, DrawStack drawStack)
    {
        List<Card> initialDeposite = new List<Card>();
        if (drawStack.Count > 0)
        {
            initialDeposite.Add(drawStack.DrawCard());
        }
        return new GameBoard(players,drawStack, new DepositStack(initialDeposite));
    }
    [Theory]
    [InlineData("Coeur", CardValue.Five, "Coeur", CardValue.Eight, true)]
    [InlineData("Pique", CardValue.Eight, "Coeur", CardValue.Eight, true)]
    [InlineData("Trèfle", CardValue.Jack, "Coeur", CardValue.Eight, true)]
    [InlineData("Carreau", CardValue.Three, "Coeur", CardValue.Eight, false)]
    public void IsValidePlay_Returns_Expected_Result(string playColorName, CardValue playVal, string topColorName, CardValue topVal, bool expected)
    {
        var turnManager = new TurnManager();
        var cardToPlay = new Card(GetCardColorByName(playColorName), playVal);
        var topCard = new Card(GetCardColorByName(topColorName), topVal);
        
        bool isValid = turnManager.IsValidePlay(cardToPlay, topCard);

        Assert.Equal(expected, isValid);
    }

    [Fact]
    public void IsValidePlay_Jack_Always_Valid()
    {
        var turnManager = new TurnManager();
        var jackCard = new Card(CardColor.Hearts, CardValue.Jack);
        var topCard = new Card(CardColor.Spades, CardValue.Two);

        bool isValid = turnManager.IsValidePlay(jackCard, topCard);

        Assert.True(isValid);
    }

    [Fact]
    public void IsValidePlay_Same_Value_Always_Valid()
    {
        var turnManager = new TurnManager();
        var card = new Card(CardColor.Hearts, CardValue.Five);
        var topCard = new Card(CardColor.Spades, CardValue.Five);

        bool isValid = turnManager.IsValidePlay(card, topCard);

        Assert.True(isValid);
    }

    [Fact]
    public void IsValidePlay_Same_Color_Always_Valid()
    {
        var turnManager = new TurnManager();
        var card = new Card(CardColor.Hearts, CardValue.Two);
        var topCard = new Card(CardColor.Hearts, CardValue.King);

        bool isValid = turnManager.IsValidePlay(card, topCard);

        Assert.True(isValid);
    }

    [Fact]
    public void ApplyCardEffect_Ten_Reverses_Direction()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones")
        };

        var board = CreateBoard(players);
        var turnManager = new TurnManager();
        int currentPlayerIndex = 0;
        bool directionEventRaised = false;
        bool raisedClockwise = true;
        
        Assert.True(board.IsClockwise);

        turnManager.ApplyCardEffect(
            new Card(CardColor.Hearts, CardValue.Ten),
            board,
            ref currentPlayerIndex,
            (msg, type) => { },
            CardColor.Hearts,
            (isClockwise) => 
            { 
                directionEventRaised = true;
                raisedClockwise = isClockwise;
            }
        );

        Assert.False(board.IsClockwise);
        Assert.True(directionEventRaised);
        Assert.False(raisedClockwise);
    }

    [Fact]
    public void ApplyCardEffect_As_Skips_Next_Player()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones"),
            new("3", "Charlie", "Brown")
        };

        var board =  CreateBoard(players);
        var turnManager = new TurnManager();
        int currentIndex = 0;

        turnManager.ApplyCardEffect(
            new Card(CardColor.Hearts, CardValue.As),
            board,
            ref currentIndex,
            (msg, type) => { },
            CardColor.Hearts,
            (isClockwise) => { }
        );
        
        Assert.Equal(2, currentIndex);
    }

    [Fact]
    public void ApplyCardEffect_Two_Applies_Penalty()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones")
        };

        var drawStack = new DrawStack(new CardPair().Generate52Cards());
        drawStack.Shuffle();
        var board =  CreateBoard(players, drawStack);
        var turnManager = new TurnManager();
        int currentIndex = 0;
        int initialHandSize = board.Players[1].Hand.Count;

        turnManager.ApplyCardEffect(
            new Card(CardColor.Hearts, CardValue.Two),
            board,
            ref currentIndex,
            (msg, type) => { },
            CardColor.Hearts,
            (isClockwise) => { }
        );
        
        Assert.Equal(initialHandSize + 2, board.Players[1].Hand.Count);
    }

    [Fact]
    public void ApplyCardEffect_Two_Moves_To_Victim()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones")
        };

        var drawStack = new DrawStack(new CardPair().Generate52Cards());
        drawStack.Shuffle();
        var board =  CreateBoard(players, drawStack);
        var turnManager = new TurnManager();
        int currentIndex = 0;

        turnManager.ApplyCardEffect(
            new Card(CardColor.Hearts, CardValue.Two),
            board,
            ref currentIndex,
            (msg, type) => { },
            CardColor.Hearts,
            (isClockwise) => { }
        );
        
        Assert.Equal(0, currentIndex);
    }

    [Fact]
    public void ApplyCardEffect_Two_Counter_Increases_Penalty()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones"),
            new("3", "Charlie", "Brown")
        };

        var drawStack = new DrawStack(new CardPair().Generate52Cards());
        drawStack.Shuffle();
        
        var bobTwo = drawStack.DrawCard();
        while (bobTwo.Value != CardValue.Two)
        {
            bobTwo = drawStack.DrawCard();
        }
        
        var board =  CreateBoard(players, drawStack);
        board.Players[1].AddCard(bobTwo);
        
        var turnManager = new TurnManager();
        int currentIndex = 0;
        int charlieInitialHandSize = board.Players[2].Hand.Count;

        turnManager.ApplyCardEffect(
            new Card(CardColor.Hearts, CardValue.Two),
            board,
            ref currentIndex,
            (msg, type) => { },
            CardColor.Hearts,
            (isClockwise) => { }
        );
        
        Assert.True(board.Players[2].Hand.Count >= charlieInitialHandSize + 4);
    }

    [Fact]
    public void ApplyCardEffect_Jack_Changes_Color()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones")
        };

        var board =  CreateBoard(players);
        var turnManager = new TurnManager();
        int currentIndex = 0;

        turnManager.ApplyCardEffect(
            new Card(CardColor.Hearts, CardValue.Jack),
            board,
            ref currentIndex,
            (msg, type) => { },
            CardColor.Spades,  // Change to Spades
            (isClockwise) => { }
        );

        Assert.Equal(CardColor.Spades, board.DepositStack.TopCard.Color);
    }

    [Fact]
    public void ApplyCardEffect_Jack_Advances_Turn()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones")
        };

        var board =  CreateBoard(players);
        var turnManager = new TurnManager();
        int currentIndex = 0;

        turnManager.ApplyCardEffect(
            new Card(CardColor.Hearts, CardValue.Jack),
            board,
            ref currentIndex,
            (msg, type) => { },
            CardColor.Clubs,
            (isClockwise) => { }
        );

        Assert.Equal(1, currentIndex);
    }

    [Fact]
    public void ApplyCardEffect_Regular_Card_Advances_Turn()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones")
        };

        var board =  CreateBoard(players);
        var turnManager = new TurnManager();
        int currentIndex = 0;

        turnManager.ApplyCardEffect(
            new Card(CardColor.Hearts, CardValue.Five),
            board,
            ref currentIndex,
            (msg, type) => { },
            CardColor.Hearts,
            (isClockwise) => { }
        );

        Assert.Equal(1, currentIndex);
    }

    [Fact]
    public void ApplyCardEffect_Ace_Skips_Single_Player()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones"),
            new("3", "Charlie", "Brown"),
            new("4", "Diana", "Prince")
        };
        DrawStack drawStack = new DrawStack(new CardPair().Generate52Cards());
        List<Card> initialDeposite = new List<Card>();
        if (drawStack.Count > 0)
        {
            initialDeposite.Add(drawStack.DrawCard());
        }
        var board = new GameBoard(players,drawStack, new DepositStack(initialDeposite));
        var turnManager = new TurnManager();
        int currentIndex = 0;

        // Alice plays an Ace
        turnManager.ApplyCardEffect(
            new Card(CardColor.Hearts, CardValue.As),
            board,
            ref currentIndex,
            (msg, type) => { },
            CardColor.Hearts,
            (isClockwise) => { }
        );
        
        Assert.Equal(2, currentIndex);
    }

    [Fact]
    public void ApplyCardEffect_Direction_Changed_Notification_Called()
    {
        var players = new List<Player>
        {
            new("1", "Alice", "Smith"),
            new("2", "Bob", "Jones")
        };

        var board =  CreateBoard(players);
        var turnManager = new TurnManager();
        int currentIndex = 0;
        var callbackInvoked = false;

        turnManager.ApplyCardEffect(
            new Card(CardColor.Hearts, CardValue.Ten),
            board,
            ref currentIndex,
            (msg, type) => { },
            CardColor.Hearts,
            (isClockwise) => { callbackInvoked = true; }
        );

        Assert.True(callbackInvoked);
    }

    /// <summary>
    /// Helper to get CardColor by name string for testing.
    /// </summary>
    private CardColor GetCardColorByName(string name)
    {
        return name switch
        {
            "Coeur" => CardColor.Hearts,
            "Pique" => CardColor.Spades,
            "Trèfle" => CardColor.Clubs,
            "Carreau" => CardColor.Diamonds,
            _ => CardColor.Hearts
        };
    }
}