namespace crazy_eights.Models;

/// <summary>
/// 
/// </summary>
public struct Card : IEquatable<Card>
{
    /// <summary>
    /// La couleur de la carte soit Carreau, Pique, Coeur ou Trèfle
    /// </summary>
    public CardColor Color;

    /// <summary>
    /// La valeur de la carte
    /// </summary>
    public CardValue Value;

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

    /// <summary>
    /// Détermine si la carte actuelle est égale à une autre carte.
    /// </summary>
    /// <param name="other">La carte à comparer avec la carte actuelle.</param>
    /// <returns>si les deux cartes sont égales.</returns>
    public bool Equals(Card other)
    {
        return Color == other.Color && Value == other.Value && _baseColor == other._baseColor;
    }

    /// <summary>
    /// Détermine si l'objet spécifié est une instance de type Card et s'il est égal à la carte actuelle.
    /// </summary>
    /// <param name="obj">L'objet à comparer avec la carte actuelle.</param>
    /// <returns>si l'objet est une carte identique</returns>
    public override bool Equals(object? obj)
    {
        return obj is Card other && Equals(other);
    }

    /// <summary>
    /// Retourne le code de hachage de cette instance de carte, calculé à partir de ses composants uniques.
    /// </summary>
    /// <returns>Un entier 32 bits signé représentant le code de hachage.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Color, Value, _baseColor);
    }
    
    public static bool operator ==(Card left, Card right) => left.Equals(right);
    public static bool operator !=(Card left, Card right) => !left.Equals(right);
}