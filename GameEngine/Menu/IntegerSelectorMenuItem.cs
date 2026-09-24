namespace crazy_eights.GameEngine.Menu;

/// <summary>
/// Élément de menu interactif permettant de sélectionner un integer.
/// </summary>
public class IntegerSelectorMenuItem : MenuItem
{
    /// <summary>
    /// Valeur du selecteur
    /// </summary>
    private int _value;
    /// <summary>
    /// Le minimum accepté comme valeur
    /// </summary>
    private readonly int _min;
    /// <summary>
    /// Le maximum accepté comme valeur 
    /// </summary>
    private readonly int _max;
    /// <summary>
    /// Obtient la valeur actuelle de l'élément.
    /// </summary>
    public int Value => _value;

    public IntegerSelectorMenuItem(string label, int initialValue, int min, int max) : base(label)
    {
        _value = initialValue;
        _min = min;
        _max = max;
    }
    /// <summary>
    /// Retourne Le visuel pour le selecteur avec le nom et la valeur
    /// </summary>
    /// <returns>La string représentant le nom et la valeur du selecteur</returns>
    public override string GetDisplayString()
    {
        return $"{Name} : [ {_value} ]  (◄ / ►)";
    }
    /// <summary>
    /// Fonction appeler quand l'item est sélectionné
    /// </summary>
    public override void OnSelected()
    {
        // Optionnel 
    }
    /// <summary>
    /// Gestion des touches horizontales Gauche ou Droite pour les sélecteurs.
    /// </summary>
    /// <param name="isRight">True si la flèche droite est pressée, false pour la flèche gauche.</param>
    public override void HandleLeftRight(bool isRight)
    {
        if (isRight && _value < _max)
        {
            _value++;
        }
        else if (!isRight && _value > _min)
        {
            _value--;
        }
    }
}