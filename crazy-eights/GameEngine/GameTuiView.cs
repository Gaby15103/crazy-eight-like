using System.Collections.ObjectModel;
using crazy_eights.Models;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace crazy_eights.GameEngine;

/// <summary>
/// L'interface utilisateur textuelle du jeu Paire de Cartes basée sur la bibliothèque Terminal.Gui.
/// Gère l'affichage du plateau de jeu, des mains des joueurs, des piles et de l'historique des actions.
/// </summary>
public class GameTuiView
{
    /// <summary>
    /// L'application pour render un tui Terminal.Gui
    /// </summary>
    private IApplication _app = null!;
    /// <summary>
    /// Conteneur de base de Terminal.Gui
    /// </summary>
    private Window _win = null!;
    /// <summary>
    /// Label pour représenter le sense du jeu
    /// </summary>
    private Label _lblSens = null!;
    /// <summary>
    /// Label représentant la pile de pioche
    /// </summary>
    private Label _lblDraw = null!;
    /// <summary>
    /// Label représentant la pile de dépôt
    /// </summary>
    private Label _lblDiscard = null!;
    /// <summary>
    /// Label représentant la couleur de la carte sur le dessus de la pile de dépôt
    /// </summary>
    private Label _lblDiscardSymbol = null!;
    /// <summary>
    /// Label affichant les joueurs de la partie
    /// </summary>
    private Label _lblPlayers = null!;
    /// <summary>
    /// Label affichant le nom du joueur actif
    /// </summary>
    private Label _lblHandTitle = null!;
    /// <summary>
    /// Label affichant la main du joueur actif
    /// </summary>
    private Label _lblHand = null!;
    /// <summary>
    /// Label affichant la stratégie utilisé par le joueur actif
    /// </summary>
    private Label _lblStrategy = null!;
    /// <summary>
    /// Label représentant la liste des logs
    /// </summary>
    private ListView _logListView = null!;
    /// <summary>
    /// La list des logs
    /// </summary>
    private readonly ObservableCollection<GameEventArgs> _logMessages = new();

    /// <summary>
    /// Initialise l'application Terminal.Gui, configure la fenêtre principale, 
    /// les panneaux de la table de jeu, la liste des joueurs et l'historique des coups.
    /// </summary>
    public void Initialize()
    {
        _app = Application.Create();
        _app.Init();

        _win = new Window()
        {
            Title = " CRAZY EIGHTS - TABLE DE JEU ",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        var leftPane = new FrameView()
        {
            Title = "Plateau & Joueurs",
            X = 0,
            Y = 0,
            Width = Dim.Percent(60),
            Height = Dim.Fill()
        };

        _lblSens = new Label() { Title = "Sens du tour : - | Pioche : 0", X = 1, Y = 1 };
        
        _lblDraw = new Label() { Text = "Pioche\n┌──────┐\n│      │\n│   🂠  │\n│      │\n│      │\n└──────┘", X = 3, Y = 3 };
        _lblDiscard = new Label() { Text = "Dépôt\n┌──────┐\n│      │\n│      │\n│      │\n│      │\n└──────┘", X = 20, Y = 3 };
        _lblDiscardSymbol = new Label() { Text = "?", X = 23, Y = 6 };

        var lblPlayersTitle = new Label() { Text = "--- JOUEURS ---", X = 38, Y = 3 };
        _lblPlayers = new Label() { Text = "", X = 38, Y = 5, Height = 6 };
        
        _lblHandTitle = new Label() { Text = "--- MAIN DE {} ---", X = 1, Y = 13 };
        _lblStrategy = new Label() {Text = "Strategy : ", X = 1, Y = 14 };
        _lblHand = new Label() { Text = "", X = 1, Y = 15, Height = 10 };

        leftPane.Add(_lblSens, _lblDraw, _lblDiscard, _lblDiscardSymbol, lblPlayersTitle, _lblPlayers, _lblHandTitle, _lblStrategy, _lblHand);

        var rightPane = new FrameView()
        {
            Title = "Historique des coups",
            X = Pos.Right(leftPane),
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        _logListView = new ListView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        _logListView.SetSource(_logMessages);

        rightPane.Add(_logListView);

        _win.Add(leftPane, rightPane);
    }

    /// <summary>
    /// Met à jour l'affichage de la table en temps réel à chaque tour.
    /// </summary>
    public void UpdateBoard(GameBoard board, string currentPlayerName, GameEventArgs actionMessage,
        IReadOnlyList<Card> activePlayerHand, string activePlayerStrategyName)
    {
        _logMessages.Add(actionMessage);

        if (_logMessages.Count > 0)
        {
            _logListView.SelectedItem = _logMessages.Count - 1;
        }

        _lblSens.Text =
            $"Sens : {(board.IsClockwise ? "Horaires ↻" : "Anti-horaires ↺")} | Pioche : {board.DrawStack.Count}";

        _lblDraw.Text = $"Pioche\n┌──────┐\n│      │\n│  🂠  │\n│ {board.DrawStack.Count,3}  │\n│      │\n└──────┘";
        
        var topCard = board.DepositStack.TopCard;
        string shortVal = topCard.Value.GetShortName().PadRight(2);
                           
        _lblDiscard.Text = $"Dépôt\n┌──────┐\n│      │\n│     │\n│ {shortVal,3}  │\n│      │\n└──────┘";
        
        _lblDiscardSymbol.Text = topCard.Color.GetSymbol();


        string playersText = string.Empty;
        foreach (var player in board.Players)
        {
            string indicator = (player.FirstName == currentPlayerName) ? "▶ [ACTIF]" : "       ";
            playersText += $"{indicator} {player.FirstName,-10} : {player.Hand.Count,2} cartes\n";
        }
        _lblPlayers.Text = playersText;
        
        string line1 = "";
        string line2 = "";
        string line3 = "";
        string line4 = "";

        for (int i = 0; i < activePlayerHand.Count; i++)
        {
            var card = activePlayerHand[i];
            string val = card.Value.GetShortName().PadRight(2);
            string sym = card.Color.GetSymbol();

            line1 += "┌─────┐ ";
            line2 += $"│{val} {sym} │ ";
            line3 += "└─────┘ ";
            line4 += $"[{i + 1}]".PadRight(8);
            
            if ((i + 1) % 8 == 0 && i < activePlayerHand.Count - 1)
            {
                line1 += "\n";
                line2 += "\n";
                line3 += "\n";
                line4 += "\n";
            }
        }
        _lblHand.Text = $"{line1}\n{line2}\n{line3}\n{line4}";
        _lblHandTitle.Text = $"--- MAIN DE {currentPlayerName} ---";
        _lblStrategy.Text = $"Strategy :  {activePlayerStrategyName}";
    }

    /// <summary>
    /// Lance l'exécution de l'application et affiche la fenêtre principale à l'écran.
    /// </summary>
    public void Run()
    {
        _app.Run(_win);
    }
    /// <summary>
    /// Arrête proprement l'application Terminal.Gui et libère les ressources graphiques associées.
    /// </summary>
    public void Shutdown()
    {
        _app.Dispose();
    }
}