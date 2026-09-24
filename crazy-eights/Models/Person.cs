namespace crazy_eights.Models;
/// <summary>
/// Représente une personne avec un identifiant, un prénom et un nom de famille.
/// </summary>
public class Person
{
    /// <summary>
    /// L'identifiant unique de la personne.
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Le prénom de la personne.
    /// </summary>
    public string FirstName { get; set; }
    /// <summary>
    /// Le nom de famille de la personne.
    /// </summary>
    public string LastName { get; set; }
    public Person(string id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }
    /// <summary>
    /// Retourne le nom complet de la personne
    /// </summary>
    /// <returns>Une chaîne de caractères contenant le prénom et le nom</returns>
    public string GetFullName() => $"{FirstName} {LastName}";
}