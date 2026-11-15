# Redecomp

Décompilateur .NET qui vise à reconstruire **un projet complet et recompilable**, au plus près des sources d'origine, à partir d'un assembly compilé. Il s'appuie sur le moteur `ICSharpCode.Decompiler` d'ILSpy, présent dans ce même dépôt.

## Ce que Redecomp ajoute à ILSpy

| Fonction | ILSpy | Redecomp |
|---|---|---|
| Entrée | une DLL à la fois | une DLL, ou un **dossier publié** entier |
| Projets | un `.csproj` par DLL | une **solution `.slnx`** : un projet par projet d'origine, reliés par `ProjectReference` |
| Dépendances | `<Reference>` vers les DLL de la machine qui a décompilé | **`PackageReference` NuGet avec version** (lues dans le `deps.json`), `FrameworkReference` pour ASP.NET Core et les autres frameworks partagés |
| Sources embarquées dans le PDB | ignorées, le code est décompilé | **fichiers d'origine** à l'octet près |
| SourceLink | ignoré | **fichiers d'origine téléchargés**, gardés seulement si leur checksum correspond au PDB |
| Arborescence | un dossier par namespace | **chemins d'origine** lus dans le PDB, types regroupés dans leur fichier d'origine |
| Fichiers à côté de la DLL (`appsettings.json`...) | absents | **copiés dans le projet** |
| Noms des variables sans PDB | `num`, `result`, `value` | **noms déduits du code** (voir plus bas), jamais ceux du PDB écrasés |
| Preuve de fidélité | aucune | **`--verify`** : recompile le projet et compare l'IL méthode par méthode avec l'original |

## Utilisation

```
redecomp <app.dll | dossier-publié> -o <dossier-de-sortie> [--offline] [--verify]
```

- Un dossier publié (avec son `.deps.json`) donne une solution complète ; une DLL seule donne un projet.
- `--offline` désactive les téléchargements SourceLink.
- `--verify` compile le résultat et indique, pour chaque projet, le pourcentage de méthodes dont le code IL est identique à l'original, avec la liste de celles qui diffèrent. Le code de sortie vaut 1 si une méthode diffère.

La comparaison IL porte sur le code exécuté (signature, variables locales, instructions). Les blocs d'attributs ne sont pas comparés : le compilateur déplace les annotations `Nullable` entre un type et ses membres sans changer le code.

## Principe

Un compilateur jette de l'information : les commentaires, souvent les noms des variables locales, la structure des dossiers, une partie du `.csproj`. Redecomp travaille en trois couches, de la plus sûre à la moins sûre :

1. **Récupérer** tout ce qui existe encore : sources embarquées, PDB, SourceLink, doc XML, `deps.json`, fichiers de publication.
2. **Reconstruire de façon déterministe** ce qui est perdu, à partir de faits lus dans le code : noms de variables, constantes, structure du projet. Même entrée, même sortie, et aucun nom ne contredit ce que fait le code.
3. **Compléter par IA** ce qui reste générique : noms, commentaires, doc. L'IA ne fait que renommer et commenter, et chaque proposition est vérifiée par recompilation.

## Feuille de route

| Phase | Contenu | Statut |
|---|---|---|
| 0 | Base du projet, build hors ligne | fait |
| 1 | Banc d'essai : corpus d'applis avec sources connues, score automatique, preuve IL | fait (3 applis, dont une multi-projets) |
| 2 | Récupération : sources embarquées ✅, chemins d'origine ✅, fichiers de publication ✅, SourceLink ✅, `deps.json` → NuGet ✅, solution multi-projets ✅, `FrameworkReference` ✅, `ImplicitUsings` ✅, `Nullable`, constantes | en cours |
| 3 | Moteur de nommage déterministe (règles classées par niveau de confiance) | en cours |
| 4 | Couche IA : renommage et commentaires, IL vérifié identique après chaque changement | à faire |
| 5 | Cas spéciaux : Razor, Blazor, BAML, single-file, ReadyToRun, obfuscation | à faire |
| 6 | Diffusion : CLI, puis interface | à faire |

### Définition de « parfait »

Une version est meilleure qu'une autre si elle améliore le score du banc d'essai sans en dégrader aucun critère :

- le projet généré recompile ;
- l'appli recompilée se comporte comme l'originale ;
- pourcentage de méthodes dont le code IL recompilé est identique à l'original ;
- pourcentage de fichiers, de noms de variables, de commentaires et de doc retrouvés par rapport aux sources d'origine ;
- écart entre l'arborescence générée et l'arborescence d'origine.

### Moteur de nommage déterministe (phase 3)

`Redecomp.Core/Naming/DeterministicVariableNames.cs` s'insère dans le pipeline d'ILSpy juste avant son propre nommage. Il ne touche qu'aux variables sans nom d'origine (pas de PDB, pas de champ async ou lambda) et applique la première règle qui s'applique :

1. variable retournée (ou son `ToString()`) → nom dérivé de la méthode (`CompterRuptures` → `ruptures`) ;
2. résultat d'une méthode de l'assembly → nom dérivé de cette méthode (`CalculerTotalTtc()` → `totalTtc`, `bool` de `SauvegarderAsync()` → `sauvegarde`) ;
3. agrégat LINQ → agrégat + membre sélectionné ou collection (`Max(a => a.Quantite)` → `maxQuantite`, `Lignes.Sum(...)` → `totalLignes`).

Les règles d'ILSpy prennent le relais ensuite (paramètre qui reçoit la variable, membre assigné, singulier de la collection pour un `foreach`, type). Le verbe de tête est retiré à partir d'une liste anglais/français (`MethodNameRules.cs`).

Les noms restants (`num2 = totalLignes > 1000000m ? ... : 0m`) décrivent un calcul, pas une provenance : aucun fait du code ne donne leur sens. Ils relèvent de la couche IA (phase 4).

### Écarts connus révélés par la preuve IL

- Sans PDB, un `Main` en instructions de haut niveau est décompilé en classe `Program` classique : les méthodes générées autour (`<Main>$`, sa machine à états) ne recompilent pas à l'identique. Le comportement reste le même.
- Le réglage `Nullable` du projet d'origine n'est pas encore reconstruit.

## Banc d'essai

`Redecomp.Bench` compile chaque appli de `samples/` en quatre variantes (sans PDB, avec PDB et doc XML, avec sources embarquées, avec SourceLink), la reconstruit, puis compare le résultat aux sources d'origine. Les applis multi-projets sont publiées puis reconstruites en solution. La variante SourceLink pointe vers les samples de ce fork sur GitHub, à un commit figé : elle télécharge réellement les sources. Le dernier score est dans [`bench/results.md`](bench/results.md). Pour le mettre à jour :

```
dotnet run --project Redecomp.Bench -- --out bench/results.md
```

Les tests se lancent avec `dotnet run --project Redecomp.Tests` (un filtre optionnel en argument).

## Build

Avec accès à NuGet, depuis ce dossier :

```
dotnet build Redecomp.slnx
```

Sans accès à NuGet, compiler d'abord le moteur à part, puis pointer dessus :

```
dotnet build Redecomp.slnx -p:RedecompDecompilerAssembly=/chemin/vers/ICSharpCode.Decompiler.dll
```

## Organisation

- `Redecomp.Core/` : la bibliothèque (récupération, reconstruction)
- `Redecomp.Cli/` : l'outil en ligne de commande `redecomp`
- `Redecomp.Bench/` : le banc d'essai
- `Redecomp.Tests/` : les tests, exécutables sans package NuGet
- `bench/` : les derniers scores du banc d'essai
- `samples/` : les applis témoins du banc d'essai, avec leurs sources d'origine
