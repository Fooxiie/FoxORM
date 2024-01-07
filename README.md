# FoxORM

>Ce plugin n'est pas un plugin apportant des fonctionnalités pour les admins. Il est réserver au développeur. Son fichier .dll est donc obligatoire pour les plugins qui l'utilisent mais lui seul est inutile.

FoxORM est un ORM spécialement conçu pour les plugins Nova-Life. En effet il va vous permettre comme tous les ORMs à facilité votre vie de développeur de plugins Nova-Life. Au vu de sa licence, je vous le rappelle ici mais il est obligatoire de citer ce dépôt GitHub sur votre ReadMe.

# Fonctionnalités disponibles

> FoxORM est un ORM voué à évoluer. Il sera fait en sorte d'éviter les régression pour permettre la rétro-compatibilité. Cependant son nombre de fonctionnalité évoluera surement avec les besoins qui naitrons après les premiers plugins équiper.

-  Création du fichier de base de donné (ou utilisation d'un existant).
- Création de table représentant une classe
- Sauvegarde/Créations des instances de classes
- Récupération de votre object avec un ID (FindOne)
- Récupération d'une liste d'object de votre table (FetchAll)
- Récupération d'une liste d'object avec un prédicat (FetchWhere)
- Suppression de votre objet en base de données
- Jointure de table (A venir) avec prédicat

## Structure des classes

Vous imaginez bien que vos classes vont devoir respecter plusieurs critère afin d'être utilisable avec l'ORM. Ces critères sont les suivants.
- Toutes les classes utilisé dans l'ORM doivent avoir un id nommer "Id" avec l'annotation "PrimaryKey". "AutoIncrement" est optionnel mais je vois pas trop le cas ou ne pas l'utiliser. Exemple : <br> `[AutoIncrement] [PrimaryKey] public int Id {get; set;}`
- Pour avoir des champs à ne pas enregistrer en base utiliser l'annotation "NonSerialized". Exemple : <br> `[NonSerialized] public int ValeurCalculé`

## Getting started

Pour l'exemple nous allons prendre la structure de FoxLottery à savoir.

```c#
public class LotteryModel
{
     [AutoIncrement] [PrimaryKey] public int Id { get; set; }

     public int montant { get; set; }

     public string enterpriseName { get; set; }

     public int bizID { get; set; }

     public float price { get; set; }

     public int status { get; set; }

     public int numSortie { get; set; }

     [NonSerialized] public int NombreParticipant;
}
```

Des loteries ou des tickets sont relier dessus. Situation très simple. Nous allons donc voir comment exploiter ça facilement en quelque ligne avec FoxORM.

Pensez à ajouter le fichier .dll en référence à votre project.
```c#
// Ici je définis facilement en créant une instance de ma classe
// un fichier database.sqlite dans le dossier NomPlugin qui sera créer automatiquement
_foxOrm = new FoxOrm("./NomPlugin/database.sqlite");
// La fonction registerTable permet de créer ma table en fonction de ma classe c#
_foxOrm.RegisterTable<LotteryModel>();
_foxOrm.RegisterTable<TicketModel>();

// Pour créer une instance et l'enregistrer dans la base
// Je creer une première lottery. Je lui donne des valeurs histoire de
var lottery = new Lottery { montant = 10, enterpriseName = "ABC Corp", bizID = 1, price = 10.0f };

// Avec un ligne je l'enregistre en base de donnée
// Dans ma variable result je vais récupérer un bool false si l'ajout ne s'est pas fait sinon true
bool result = await _foxOrm.Save<Lottery>(lottery);

// Si je veux récupérer ma lottery avec son ID
Lottery queriedLottery = await _foxOrm.Query<Lottery>(2);

// Imaginons je veux supprimer mon résultat de la base de données
bool resultSupression = await _foxOrm.Delete<Lottery>(queriedLottery);

// Si je veux toutes les lottery je fais comme ça
var allLottery = await _foxOrm.QueryAll<Lottery>();

// Mais si je veux toutes les lottery d'un montant plus que 1000
var queriedLottery = await _foxOrm.Query<Lottery>(lottery => lottery.montant > 1000);
```
> N'oubliez pas de livrer FoxORM.dll avec votre plugin !

# Pour toute question ou bug
Rejoindre le discord : https://discord.gg/aztFmNxEqp

