namespace crazy_eights.GameEngine;
/// <summary>
/// Représente les différents types de messages de notification transmis au cours de la partie.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Envoyé lorsqu'un joueur joue une carte valide de sa main.
    /// </summary>
    Play,

    /// <summary>
    /// Déclenché par une carte avec un effet.
    /// </summary>
    Effect,

    /// <summary>
    /// Message automatique du système.
    /// </summary>
    System
}