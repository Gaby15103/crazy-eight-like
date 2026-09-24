# Paire de cartes — Fishing Game

Application console en C# qui simule automatiquement une partie de **Pêche / Pioche**, une variante de jeu de cartes inspirée de Crazy Eights.

## Objectifs du projet

- Modéliser un jeu standard de 52 cartes.
- Utiliser des `struct`, des `enum`, des interfaces et des classes avec des responsabilités distinctes.
- Implémenter une partie automatique avec 2 à 4 joueurs.
- Appliquer le pattern **Observateur** pour notifier l’interface des événements de la partie.
- Utiliser `async` / `await` et des délais pour rendre les tours visibles.
- Implémenter différentes stratégies de jeu, dont une stratégie qui cherche à minimiser le nombre de points restants.

## Règles du jeu

### Cartes

Le jeu contient 52 cartes, soit toutes les combinaisons possibles entre :

- **Valeurs** : As, 2, 3, 4, 5, 6, 7, 8, 9, 10, Valet, Dame et Roi.
- **Couleurs** : Trèfle, Carreau, Cœur et Pique.

Une carte est définie par une valeur et une couleur. Par exemple : `As de Trèfle` ou `Roi de Pique`.

### Mise en place

- Une partie se joue avec **2, 3 ou 4 joueurs**.
- Chaque joueur reçoit aléatoirement le même nombre de cartes : entre **5 et 8 cartes**.
- Les cartes restantes sont placées dans la **pile de pioche**.
- Un joueur est choisi aléatoirement pour commencer.
- Le premier joueur pose une carte ouverte dans la **pile de dépôt**.

### Déroulement d’un tour

Les joueurs jouent dans le sens horaire, sauf si la direction est inversée par un 10. Une carte jouée doit avoir la même couleur ou la même valeur que la carte visible au sommet de la pile de dépôt.

Si un joueur ne peut pas jouer, il pioche une carte et son tour passe au joueur suivant.

Lorsque la pile de pioche est vide, la pile de dépôt est mélangée, à l’exception de sa carte visible, puis réutilisée comme nouvelle pile de pioche.

### Cartes spéciales

| Carte | Effet |
| --- | --- |
| **Valet (J)** | Change la couleur demandée. Il peut être joué sur n’importe quelle couleur ou valeur, sauf sur un 2. |
| **As** | Le joueur suivant est sauté. |
| **10** | Inverse le sens du jeu : horaire ↔ anti-horaire. |
| **2** | Le joueur suivant doit piocher 2 cartes et son tour est sauté. Il peut contrer avec un autre 2. |

Les attaques de 2 peuvent se cumuler :

- Un premier 2 impose 2 cartes au joueur suivant.
- Un contre avec un 2 impose 4 cartes au joueur suivant.
- Un nouveau contre avec un 2 impose 8 cartes au joueur suivant.

Le joueur qui n’a plus de cartes gagne immédiatement la partie.

### Décompte des points

À la fin de la partie, les cartes encore présentes dans la main de chaque joueur sont additionnées :

- As : **11 points**
- Valet, Dame et Roi : **2 points** chacun
- Cartes numérotées : leur valeur faciale, par exemple 3 = **3 points** et 7 = **7 points**

Le programme affiche le vainqueur ainsi que le classement final des joueurs selon leur nombre de points restants.

## Fonctionnalités implémentées

- Création et mélange d’un jeu complet de 52 cartes.
- Configuration de 2 à 4 joueurs et de 5 à 8 cartes initiales par joueur.
- Sélection aléatoire du joueur qui commence.
- Jeu automatique, sans saisie nécessaire à chaque tour.
- Gestion des cartes spéciales et des attaques de 2.
- Inversion du sens de jeu avec le 10.
- Recyclage de la pile de dépôt lorsque la pioche est vide.
- Notification lorsqu’un joueur ne possède plus qu’une carte.
- Stratégies de jeu interchangeables, dont une stratégie de minimisation des points.
- Affichage des actions, de la carte jouée et de l’état de la partie dans l’interface terminal.
- Délais asynchrones entre les tours afin de suivre le déroulement de la partie.

## Concepts du domaine et architecture

Les principaux concepts demandés sont représentés dans le projet :

- `Card` : `struct` représentant une carte.
- `CardColor` : `struct` représentant une couleur.
- `CardValue` : type représentant la valeur d’une carte.
- `Person` : informations personnelles d’un joueur.
- `Player` : joueur, main de cartes, stratégie et score.
- `CardPair` : jeu complet contenant les 52 cartes.
- `GameBoard` : état de la table et des joueurs.
- `FishingGame` : orchestration du déroulement de la partie.
- `DrawStack` : pile de pioche.
- `DepositStack` : pile de dépôt.
- `IPlayerStrategy` : contrat des stratégies de jeu.
- Événements C# : observateurs des messages, des alertes et de la fin de partie.

Le diagramme de classes est disponible dans [`diagram_de_class.uml`](diagram_de_class.uml).

## Prérequis

Pour exécuter le projet localement, il faut installer :

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

Le projet cible `net10.0` et utilise le paquet [`Terminal.Gui`](https://github.com/gui-cs/Terminal.Gui).

## Exécution avec .NET

Depuis la racine du dépôt :

```bash
dotnet restore crazy-eights/crazy-eights.csproj
dotnet run --project crazy-eights/crazy-eights.csproj
```

Le menu permet de lancer une partie par défaut ou de configurer le nombre de joueurs et le nombre de cartes distribuées.

Pour compiler en mode Release :

```bash
dotnet build crazy-eights/crazy-eights.csproj --configuration Release
```
## Tests

Les tests automatisés se trouvent dans le projet `crazy_eights.Tests`. Pour exécuter tous les tests :

```bash
dotnet test
```

## Organisation du dépôt

```text
.
├── crazy-eights/          # Application C#
├── crazy_eights.Tests/    # Tests automatisés
├── diagram_de_class.uml   # Diagramme de classes
├── Dockerfile             # Image Docker
├── compose.yaml           # Exécution avec Docker Compose
└── crazy-eights.sln       # Solution .NET
```