using crazy_eights.Strategies;

namespace crazy_eights.GameEngine;
/// <summary>
/// Représente la configuration initiale et les paramètres d'une partie
/// </summary>
public class GameConfig
{
    /// <summary>
    /// Nombre de joueurs participant à la partie
    /// </summary>
    public int PlayerCount { get; set; } = 3;
    /// <summary>
    /// Nombre initial de cartes distribuées à chaque joueur en début de partie
    /// </summary>
    public int InitialHandSize { get; set; } = 5;
    /// <summary>
    /// Liste des stratégies attribuées aux joueurs.
    /// </summary>
    public List<IPlayerStrategy> PlayerStrategies { get; set; } = new();
}