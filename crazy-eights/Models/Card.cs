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
    /// <summary>
    /// Couleur de base de la carte
    /// </summary>
    private readonly CardColor _baseColor;
    /// <summary>
    /// Nom complet de la carte
    /// </summary>
    public string Name => $"{Value.GetName()} de {Color.Name}";
    /// <summary>
    /// Le nombre de points que vaut cette carte
    /// </summary>
    public int Points => Value.GetPoints();
    
    public Card(CardColor color, CardValue cardValue)
    {
        Color = color;
        Value = cardValue;
        _baseColor = Color;
    }
    /// <summary>
    /// Constructeur intern
    /// </summary>
    /// <param name="color">Couleur active de la carte</param>
    /// <param name="cardValue">La valeur de la carte</param>
    /// <param name="baseColor">la couleur de base de la carte</param>
    private Card(CardColor color, CardValue cardValue, CardColor baseColor)
    {
        Color = color;
        Value = cardValue;
        _baseColor = baseColor;
    }
    /// <summary>
    /// Retourne une carte a sa couleur original
    /// </summary>
    public void Reset()
    {
        Color = _baseColor;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="newColor"></param>
    /// <returns></returns>
    public Card WithColor(CardColor newColor)
    {
        return new Card(newColor, Value, _baseColor);
    }
}