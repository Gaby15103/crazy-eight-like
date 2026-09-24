namespace crazy_eights.Models;
/// <summary>
/// 
/// </summary>
public struct Card
{
    /// <summary>
    /// La couleur de la carte soit Carreau, Pique, Coeur ou Trèfle
    /// </summary>
    public CardColor Color;
    /// <summary>
    /// La valeur de la carte
    /// </summary>
    public CardValue  Value;

    private readonly CardColor _originalColor; 
    /// <summary>
    /// Nom complet de la carte
    /// </summary>
    public string Name => $"{Value.GetName()} de {Color.Name}";
    /// <summary>
    /// Le nombre de point que vaut cette carte
    /// </summary>
    public int Points => Value.GetPoints();
    
    public Card(CardColor color, CardValue cardValue)
    {
        Color = color;
        Value = cardValue;
        _originalColor = Color;
    }
    /// <summary>
    /// Retourne une carte a sa couleur original
    /// </summary>
    public void Reset()
    {
        Color = _originalColor;
    }
}