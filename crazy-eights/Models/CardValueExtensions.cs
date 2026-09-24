namespace crazy_eights.Models;
/// <summary>
/// Extensions d'une Valeur de carte
/// </summary>
public static class CardValueExtensions
{
    /// <summary>
    /// Retourne la nombre de points associé a une valeur
    /// </summary>
    /// <param name="value">La valeur a trouver le nombre de point</param>
    /// <returns>Le nombre de point qu'a la valeur</returns>
    public static int GetPoints(this CardValue value)
    {
        return value switch
        {
            CardValue.As => 11,
            CardValue.Jack => 2,
            CardValue.Queen => 2,
            CardValue.King => 2,
            CardValue.Two => 2,
            CardValue.Three => 3,
            CardValue.Four => 4,
            CardValue.Five => 5,
            CardValue.Six => 6,
            CardValue.Seven => 7,
            CardValue.Eight => 8,
            CardValue.Nine => 9,
            CardValue.Ten => 10,
            _ => 0
        };
    }
    /// <summary>
    /// Retourne le nom de la valeur en string
    /// </summary>
    /// <param name="value">La valuer a trouver le nom</param>
    /// <returns>Le nom de la valeur</returns>
    public static string GetName(this CardValue value)
    {
        return value switch
        {
            CardValue.As => "As",
            CardValue.Two => "Deux",
            CardValue.Three => "Trois",
            CardValue.Four => "Quatre",
            CardValue.Five => "Cinq",
            CardValue.Six => "Six",
            CardValue.Seven => "Sept",
            CardValue.Eight => "Huit",
            CardValue.Nine => "Neuf",
            CardValue.Ten => "Dix",
            CardValue.Jack => "Valet",
            CardValue.Queen => "Dame",
            CardValue.King => "Roi",
            _ => value.ToString()
        };
    }
    /// <summary>
    /// Retourne le nom raccourcie de la valeur
    /// </summary>
    /// <param name="value">La valuer a trouver le nom raccourcie</param>
    /// <returns>Le nom raccourcie de la valeur</returns>
    public static string GetShortName(this CardValue value)
    {
        return value switch
        {
            CardValue.As => "A",
            CardValue.Two => "2",
            CardValue.Three => "3",
            CardValue.Four => "4",
            CardValue.Five => "5",
            CardValue.Six => "6",
            CardValue.Seven => "7",
            CardValue.Eight => "8",
            CardValue.Nine => "9",
            CardValue.Ten => "10",
            CardValue.Jack => "V",
            CardValue.Queen => "D",
            CardValue.King => "R",
            _ => "?"
        };
    }
    /// <summary>
    /// Retourne si la valeur est a effet quand joué
    /// </summary>
    /// <param name="value">La valeur a trouver si elle à un effet quand joué</param>
    /// <returns>Si la valeur a un effet quand joué</returns>
    public static bool IsActionCard(this CardValue value)
    {
        return value is CardValue.As 
            or CardValue.Two 
            or CardValue.Ten 
            or CardValue.Jack;
    }
}