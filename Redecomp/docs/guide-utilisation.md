# Guide d'utilisation

## Installation

Redecomp se compile depuis ce dépôt avec le SDK .NET 10 ou plus récent.

```
cd Redecomp
dotnet build Redecomp.slnx -c Release
```

L'outil est ensuite dans `Redecomp.Cli/bin/Release/net10.0/`. Lancé sans argument, il affiche son usage :

```
dotnet Redecomp.Cli/bin/Release/net10.0/redecomp.dll
```

Sur une machine sans accès à NuGet, compile d'abord `ICSharpCode.Decompiler` à part, puis passe sa DLL au build :

```
dotnet build Redecomp.slnx -p:RedecompDecompilerAssembly=/chemin/vers/ICSharpCode.Decompiler.dll
```

## Commande

```
redecomp <app.dll | dossier-publié> -o <dossier-de-sortie> [--offline] [--verify]
```

| Option | Effet |
|---|---|
| `-o`, `--output` | Dossier où écrire le projet ou la solution. Obligatoire. |
| `--offline` | N'essaie pas de télécharger les sources via SourceLink. |
| `--verify` | Recompile le résultat et compare son IL, méthode par méthode, avec l'original. |

### Une DLL seule

```
redecomp bin/Release/net10.0/MonAppli.dll -o reconstruit
```

Produit un projet : `MonAppli.csproj`, les fichiers C#, les ressources et les fichiers livrés à côté de la DLL (`appsettings.json`, etc.).

Si un `MonAppli.deps.json` se trouve à côté de la DLL, les packages NuGet sont déclarés en `PackageReference` avec leur version. Les DLL des autres projets de la solution restent des références vers des DLL ; pour obtenir toute la solution, passe le dossier publié.

### Un dossier publié

```
dotnet publish MonAppli -c Release -o publication
redecomp publication -o reconstruit
```

Produit une solution complète :

```
reconstruit/
├── MonAppli.slnx
├── MonAppli/            ← le projet de l'application, avec ses fichiers de contenu
│   └── MonAppli.csproj  ← ProjectReference vers MonAppli.Metier
└── MonAppli.Metier/
    └── MonAppli.Metier.csproj  ← PackageReference vers les packages NuGet
```

Le dossier doit contenir un seul `.deps.json` : c'est lui qui dit quelles DLL étaient des projets de la solution et lesquelles venaient de packages.

### Lire le résultat

Pour chaque projet, la CLI indique d'où viennent les fichiers C# :

| Message | Signification |
|---|---|
| `original files embedded in the PDB` | Fichiers d'origine, à l'octet près, lus dans le PDB (`EmbedAllSources`). |
| `original files downloaded through SourceLink, checksums verified` | Fichiers d'origine téléchargés depuis le dépôt indiqué par le PDB, chacun vérifié par son checksum. |
| `decompiled from IL` | Code décompilé. Avec un PDB, les noms des variables et l'arborescence d'origine sont repris ; sans PDB, les noms sont déduits du code. |

Avec `--verify`, une ligne de plus :

```
  IL code:     92% (49/53) of the methods identical to the original
    differs: Program .method private hidebysig static class ... '<Main>$' (...)
```

Le code de sortie vaut `0` si toutes les méthodes sont identiques, `1` sinon (ou si le projet ne compile pas), `2` en cas d'erreur d'utilisation. On peut donc s'en servir dans un script ou une CI.

## Ce qui est récupéré, dans l'ordre

1. **Sources embarquées** dans le PDB : fichiers d'origine, commentaires compris.
2. **SourceLink** : fichiers d'origine téléchargés, gardés seulement si leur checksum correspond au PDB. Un fichier modifié depuis la compilation est ignoré.
3. **Décompilation** pour tout le reste, avec :
   - les noms de variables du PDB s'il existe, sinon des noms déduits du code ;
   - les fichiers placés aux chemins d'origine lus dans le PDB ;
   - la doc XML (`///`) si le fichier `.xml` est livré à côté de la DLL.

Dans tous les cas, le `.csproj` est nettoyé : plus de chemins vers les DLL de la machine qui a décompilé, des `PackageReference` NuGet, des `FrameworkReference` pour ASP.NET Core et les autres frameworks partagés, `ImplicitUsings` quand le projet d'origine l'utilisait.

## Limites actuelles

- Le réglage `Nullable` du projet d'origine n'est pas reconstruit.
- Sans PDB, un `Main` en instructions de haut niveau est décompilé en classe `Program` classique : le comportement est le même, mais quelques méthodes générées ne recompilent pas à l'identique.
- Les commentaires `//` ne sont récupérables que via les sources embarquées ou SourceLink.
- Les applis ASP.NET avec vues Razor compilées, WPF (BAML), single-file et ReadyToRun ne sont pas encore traitées spécifiquement.
