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
| 0 | Base du projet, build hors ligne, CI | en cours |
| 1 | Banc d'essai : corpus d'applis avec sources connues et score automatique | à faire |
| 2 | Récupération : sources embarquées, PDB, SourceLink, chemins d'origine, `deps.json` → NuGet, fichiers de publication, `Nullable`/`ImplicitUsings`, constantes | à faire |
| 3 | Moteur de nommage déterministe (règles classées par niveau de confiance) | à faire |
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

Pour chaque variable locale, on applique la règle de plus haute confiance qui s'applique :

1. nom d'origine (PDB, variable capturée par une méthode async ou une lambda) ;
2. variable passée en argument → nom du paramètre qui la reçoit ;
3. résultat d'un appel de méthode → nom dérivé de la méthode (`CalculerTotalTtc()` → `totalTtc`) ;
4. assignée à un champ ou une propriété → nom de ce membre (`_remise` → `remise`) ;
5. variable de boucle sur une collection → singulier de la collection (`Lignes` → `ligne`) ;
6. à défaut, nom dérivé du type.

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
- `Redecomp.Tests/` : les tests, exécutables sans package NuGet
- `samples/` : les applis témoins du banc d'essai, avec leurs sources d'origine
