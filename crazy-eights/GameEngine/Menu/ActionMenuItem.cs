namespace crazy_eights.GameEngine.Menu;

/// <summary>
/// Élément de menu classique déclenchant une action.
/// </summary>
public class ActionMenuItem : MenuItem
{
    /// <summary>
    /// Action faite par la selection de l'ActionMenuItem
    /// </summary>
    private readonly Action _action;

    public ActionMenuItem(string name, Action action, bool isExit = false) : base(name, isExit)
    {
        _action = action;
    }

    /// <summary>
    /// Retourne la chaîne de caractères à afficher pour cet élément.
    /// </summary>
    /// <returns>la chaîne de caractères à afficher pour cet élément</returns>
    public override string GetDisplayString() => Name;
    
    /// <summary>
    /// Fonction qui invoke _action quand l'ActionMenuItem est sélectionné
    /// </summary>
    public override void OnSelected()
    {
        _action?.Invoke();
    }
}