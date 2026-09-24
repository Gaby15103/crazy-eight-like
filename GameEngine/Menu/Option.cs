namespace crazy_eights.GameEngine.Menu;
/// <summary>
/// class de l'objet option
/// </summary>
public class Option
{
    /// <summary>
    /// Le nom de l'option à afficher à l'utilisateur
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// La fonction lambda de l'option
    /// </summary>
    public Action Selected { get; }
    /// <summary>
    /// Si l'option sert à sortir du programme
    /// </summary>
    public bool Exit { get; set; }
    /// <summary>
    /// Initialise l'objet option
    /// </summary>
    /// <param name="name">Le nom de l'option à afficher à l'utilisateur</param>
    /// <param name="selected">La fonction lambda de l'option</param>
    /// <param name="exit">Si l'option sert à sortir du programme</param>
    public Option(string name, Action selected, bool exit = false)
    {
        Name = name;
        Selected = selected;
        Exit = exit;
    }
}