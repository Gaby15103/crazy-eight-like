using System;

namespace crazy_eights.GameEngine.Menu;

/// <summary>
/// Classe de base abstraite représentant un élément du menu (Principe ouvert/fermé de la POO).
/// </summary>
public abstract class MenuItem
{
    /// <summary>
    /// Le nom affiché à l'utilisateur.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Indique si cet élément sert à quitter le menu.
    /// </summary>
    public bool IsExit { get; set; }

    protected MenuItem(string name, bool isExit = false)
    {
        Name = name;
        IsExit = isExit;
    }

    /// <summary>
    /// Retourne la chaîne de caractères à afficher pour cet élément.
    /// </summary>
    public abstract string GetDisplayString();

    /// <summary>
    /// Action exécutée lorsque l'utilisateur valide avec la touche Entrée.
    /// </summary>
    public abstract void OnSelected();

    /// <summary>
    /// Gestion optionnelle des touches horizontales Gauche ou Droite pour les sélecteurs.
    /// </summary>
    /// <param name="isRight">True si la flèche droite est pressée, false pour la flèche gauche.</param>
    public virtual void HandleLeftRight(bool isRight)
    {
    }
}