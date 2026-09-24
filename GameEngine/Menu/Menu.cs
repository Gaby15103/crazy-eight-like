namespace crazy_eights.GameEngine.Menu;

/// <summary>
/// Représente un menu interactif instanciable piloté par objets.
/// </summary>
public class Menu
{
    /// <summary>
    /// Le titre du menu
    /// </summary>
    private readonly string? _title;
    /// <summary>
    /// Les items du menu
    /// </summary>
    private readonly List<MenuItem> _items;
    /// <summary>
    /// Action lambda qui peut être appeler avant de générer le menu
    /// </summary>
    private readonly Action? _headerAction;
    /// <summary>
    /// L'item du menu sélectionné
    /// </summary>
    private int _selectedIndex;

    public Menu(List<MenuItem> items, string? title = null, Action? headerAction = null)
    {
        _items = items ?? new List<MenuItem>();
        _title = title;
        _headerAction = headerAction;
        _selectedIndex = 0;
    }

    /// <summary>
    /// Lance la boucle interactive du menu
    /// </summary>
    public void Run()
    {
        ConsoleKeyInfo keyInfo;
        do
        {
            Render();

            keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                if (_selectedIndex + 1 < _items.Count)
                {
                    _selectedIndex++;
                }
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                if (_selectedIndex - 1 >= 0)
                {
                    _selectedIndex--;
                }
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                _items[_selectedIndex].HandleLeftRight(false);
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                _items[_selectedIndex].HandleLeftRight(true);
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                _items[_selectedIndex].OnSelected();
                break;
            }
        } while (keyInfo.Key != ConsoleKey.Escape);
    }

    /// <summary>
    /// Affiche le menu complet et l'élément actuellement sélectionné
    /// </summary>
    private void Render()
    {
        Console.Clear();

        if (!string.IsNullOrEmpty(_title))
        {
            Console.WriteLine($"=== {_title} ===");
            Console.WriteLine("Instructions : [Haut/Bas] Naviguer | [Gauche/Droite] Modifier les valeurs | [Entrée] Valider\n");
        }

        _headerAction?.Invoke();

        for (int i = 0; i < _items.Count; i++)
        {
            MenuItem item = _items[i];
            bool isSelected = (i == _selectedIndex);

            string prefix = isSelected ? "> " : "  ";

            if (item.IsExit)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                
            }

            if (isSelected)
            {
                Console.Write(prefix);
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($" {item.GetDisplayString()} ");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"{prefix}{item.GetDisplayString()}");
                Console.ResetColor();
            }
        }
    }

    /// <summary>
    /// Affiche un message temporaire à l'écran avant de traffic her le menu.
    /// </summary>
    public void WriteTemporaryMessage(string message, int milliseconds = 3000)
    {
        Console.Clear();
        Console.WriteLine(message);
        Thread.Sleep(milliseconds);
        Render();
    }
}