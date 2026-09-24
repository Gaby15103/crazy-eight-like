namespace crazy_eights.Models;
/// <summary>
/// Représente la couleur (l'enseigne) d'une carte à jouer dans le jeu de Crazy Eights.
/// </summary>
public struct CardColor : IEquatable<CardColor>
{
    /// <summary>
    /// Le nom de la couleur d'une carte
    /// </summary>
    public string Name { get; set; }
    
    public CardColor(string name)
    {
        Name = name;
    }
    /// <summary>
    /// Obtient la couleur Trèfle (♣).
    /// </summary>
    public static CardColor Clubs { get; } = new CardColor("Trèfle");
    /// <summary>
    /// Obtient la couleur Carreau (♦).
    /// </summary>
    public static CardColor Diamonds { get; } = new CardColor("Carreau");
    /// <summary>
    /// Obtient la couleur Cœur (♥).
    /// </summary>
    public static CardColor Hearts { get; } = new CardColor("Coeur");
    /// <summary>
    /// Obtient la couleur Pique (♠).
    /// </summary>
    public static CardColor Spades { get; } = new CardColor("Pique");
    /// <summary>
    /// Retourne le nom de la couleur sous forme de chaîne de caractères.
    /// </summary>
    /// <returns>Le nom de la couleur.</returns>
    public override string ToString() => Name;
    /// <summary>
    /// Obtient le symbole Unicode correspondant à la couleur de la carte (♣, ♦, ♥, ♠).
    /// </summary>
    /// <returns>Une chaîne contenant le symbole de la couleur, ou "?" si elle est inconnue.</returns>
    public string GetSymbol()
    {
        if (Name == Spades.Name) return "\u2660";
        if (Name == Hearts.Name) return "\u2665";
        if (Name == Diamonds.Name) return "\u2666";
        if (Name == Clubs.Name) return "\u2663";
        return "?";
    }

    /// <summary>
    /// Retourne si la couleur actuelle est égale à une autre couleur spécifiée.
    /// </summary>
    /// <param name="other">La couleur à comparer avec la couleur actuelle.</param>
    /// <returns> si les deux couleurs ont le même nom</returns>
    public bool Equals(CardColor other)
    {
        return Name == other.Name;
    }

    /// <summary>
    /// Retourne si l'objet spécifié est égal à la couleur actuelle.
    /// </summary>
    /// <param name="obj">L'objet à comparer avec la couleur actuelle.</param>
    /// <returns>si l'objet est un carte et possède le même nom </returns>
    public override bool Equals(object? obj)
    {
        return obj is CardColor other && Equals(other);
    }
    
    /// <summary>
    /// Compare deux instances de carte pour déterminer si elles sont égales.
    /// </summary>
    /// <param name="left">La première couleur à comparer.</param>
    /// <param name="right">La seconde couleur à comparer.</param>
    /// <returns>si les deux couleurs sont égales</returns>
    public static bool operator ==(CardColor left, CardColor right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compare deux instances de <see cref="CardColor"/> pour déterminer si elles sont différentes.
    /// </summary>
    /// <param name="left">La première couleur à comparer.</param>
    /// <param name="right">La seconde couleur à comparer.</param>
    /// <returns><c>true</c> si les deux couleurs sont différentes ; sinon, <c>false</c>.</returns>
    public static bool operator !=(CardColor left, CardColor right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Retourne le code de hachage de cette instance.
    /// </summary>
    /// <returns>Un entier 32 bits signé représentant le code de hachage du nom de la couleur.</returns>
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}