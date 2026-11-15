# Redecomp

Décompilateur .NET qui vise à reconstruire **un projet complet et recompilable**, au plus près des sources d'origine, à partir d'un assembly compilé. Il s'appuie sur le moteur `ICSharpCode.Decompiler` d'ILSpy, présent dans ce même dépôt.

## Principe

Un compilateur jette de l'information : les commentaires, souvent les noms des variables locales, la structure des dossiers, une partie du `.csproj`. Redecomp travaille en trois couches, de la plus sûre à la moins sûre :

1. **Récupérer** tout ce qui existe encore : sources embarquées, PDB, SourceLink, doc XML, `deps.json`, fichiers de publication.
2. **Reconstruire de façon déterministe** ce qui est perdu, à partir de faits lus dans le code : noms de variables, constantes, structure du projet. Même entrée, même sortie, et aucun nom ne contredit ce que fait le code.
3. **Compléter par IA** ce qui reste générique : noms, commentaires, doc. L'IA ne fait que renommer et commenter, et chaque proposition est vérifiée par recompilation.

## Feuille de route

| Phase | Contenu | Statut |
|---|---|---|
| 0 | Base du projet, build hors ligne | fait |
| 1 | Banc d'essai : corpus d'applis avec sources connues et score automatique | fait (1 appli, à enrichir) |
| 2 | Récupération : sources embarquées ✅, PDB, chemins d'origine ✅, fichiers de publication ✅, SourceLink, `deps.json` → NuGet, `Nullable`/`ImplicitUsings`, constantes | en cours |
| 3 | Moteur de nommage déterministe (règles classées par niveau de confiance) | en cours |
| 4 | Couche IA : renommage et commentaires, IL vérifié identique après chaque changement | à faire |
| 5 | Cas spéciaux : Razor, Blazor, BAML, single-file, ReadyToRun, obfuscation | à faire |
| 6 | Diffusion : CLI, puis interface | à faire |

### Définition de « parfait »

Une version est meilleure qu'une autre si elle améliore le score du banc d'essai sans en dégrader aucun critère :

- le projet généré recompile ;
- l'appli recompilée se comporte comme l'originale ;
- pourcentage de fichiers, de noms de variables, de commentaires et de doc retrouvés par rapport aux sources d'origine ;
- écart entre l'arborescence générée et l'arborescence d'origine.

### Moteur de nommage déterministe (phase 3)

`Redecomp.Core/Naming/DeterministicVariableNames.cs` s'insère dans le pipeline d'ILSpy juste avant son propre nommage. Il ne touche qu'aux variables sans nom d'origine (pas de PDB, pas de champ async ou lambda) et applique la première règle qui s'applique :

1. variable retournée (ou son `ToString()`) → nom dérivé de la méthode (`CompterRuptures` → `ruptures`) ;
2. résultat d'une méthode de l'assembly → nom dérivé de cette méthode (`CalculerTotalTtc()` → `totalTtc`, `bool` de `SauvegarderAsync()` → `sauvegarde`) ;
3. agrégat LINQ → agrégat + membre sélectionné ou collection (`Max(a => a.Quantite)` → `maxQuantite`, `Lignes.Sum(...)` → `totalLignes`).

Les règles d'ILSpy prennent le relais ensuite (paramètre qui reçoit la variable, membre assigné, singulier de la collection pour un `foreach`, type). Le verbe de tête est retiré à partir d'une liste anglais/français (`MethodNameRules.cs`).

Les noms restants (`num2 = totalLignes > 1000000m ? ... : 0m`) décrivent un calcul, pas une provenance : aucun fait du code ne donne leur sens. Ils relèvent de la couche IA (phase 4).

## Banc d'essai

`Redecomp.Bench` compile chaque appli de `samples/` en trois variantes (sans PDB, avec PDB et doc XML, avec sources embarquées), la reconstruit, puis compare le résultat aux sources d'origine. Le dernier score est dans [`bench/results.md`](bench/results.md). Pour le mettre à jour :

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
