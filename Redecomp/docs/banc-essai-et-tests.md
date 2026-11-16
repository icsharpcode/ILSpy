# Banc d'essai et tests

## Tests

```
dotnet run --project Redecomp.Tests            # toute la suite
dotnet run --project Redecomp.Tests -- SourceLink   # filtre sur le nom des tests
```

Le lanceur est une simple appli console (`TestRunner.cs`) : chaque méthode statique publique marquée `[Test]` est exécutée. Pas de package NuGet nécessaire, la suite tourne aussi hors ligne.

Les tests compilent les applis témoins dans un dossier temporaire, les reconstruisent, puis vérifient le résultat : fichiers, noms, références, recompilation, exécution et IL. Ils ne touchent jamais au réseau, sauf ceux qui passent par la reconstruction par défaut sur la variante SourceLink ; les tests propres à SourceLink utilisent un faux téléchargeur qui sert les fichiers locaux.

## Banc d'essai

```
dotnet run --project Redecomp.Bench -- --out bench/results.md
```

Pour chaque appli témoin et chaque variante de compilation, le banc compile l'appli, la reconstruit, la recompile, l'exécute et compare le tout aux sources d'origine. Le résultat est un tableau Markdown ; la dernière version est dans [`bench/results.md`](../bench/results.md).

### Variantes de compilation

| Variante | Ce qui est livré avec la DLL |
|---|---|
| `NoPdb` | Rien : cas le plus courant en production. |
| `PortablePdbWithDocs` | Un PDB portable et le fichier de doc XML. |
| `EmbeddedSources` | Un PDB embarqué dans la DLL, avec toutes les sources dedans. |
| `SourceLink` | Un PDB portable dont la carte SourceLink pointe vers les samples de ce fork sur GitHub, à un commit figé. Cette variante télécharge réellement les sources. |

Le commit figé est la constante `SampleBuilder.SourceLinkBaseUrl`. Les checksums du PDB garantissent qu'un commit dont les samples ont changé fait seulement échouer la récupération, sans jamais renvoyer d'autres sources. Si tu modifies un sample, pousse le changement et mets cette constante à jour avec le nouveau commit.

### Critères mesurés

| Colonne | Mesure |
|---|---|
| Sources | D'où viennent les fichiers C# : `EmbeddedSources`, `SourceLink` ou `Decompiled`. |
| Rebuilds | Le projet reconstruit compile. |
| Same behavior | L'appli recompilée affiche exactement la même sortie que l'originale. |
| IL code identical | Méthodes dont le code IL recompilé est identique à l'original. |
| Files at original path | Fichiers d'origine retrouvés au même chemin relatif. |
| Original local names | Noms de variables locales d'origine retrouvés. |
| Generic local names | Noms génériques (`num`, `result`, `value`...) restants, avec ILSpy seul puis avec Redecomp. |
| Comments | Lignes de commentaires `//` et `///` d'origine retrouvées. |

Les mesures sur les noms et les commentaires sont lexicales (`SourceMetrics.cs`) : elles repèrent les déclarations et les commentaires sans compilateur, pour que le banc reste sans dépendance.

Une modification améliore Redecomp si elle fait monter au moins un critère sans en faire baisser aucun.

## Applis témoins

Dans `samples/`, chaque dossier garde les sources d'origine, qui servent de référence.

| Sample | Ce qu'il met à l'épreuve |
|---|---|
| `Facturation` | Record, enum, LINQ, async, constante, ressource embarquée, `appsettings.json`, commentaires et doc. |
| `Inventaire` | Variables nourries par des appels de méthodes, agrégats LINQ et valeurs retournées : le nommage déterministe. |
| `Portail` | Solution de deux projets, dont l'un dépend d'un package NuGet. |
| `_packages` | Sources des packages NuGet des samples, empaquetés dans un flux local au moment des tests. |

Les samples n'héritent pas des réglages du dépôt, pour ressembler à des applis tierces ordinaires. Leurs commentaires sont en français avec des accents, pour vérifier que le texte non ASCII est récupéré intact.

### Ajouter une appli témoin

- **Un seul projet** : un dossier dans `samples/` avec son `.csproj` à la racine. Le banc le prend automatiquement.
- **Plusieurs projets** : un dossier avec un `.slnx` à la racine et un seul projet exécutable (`<OutputType>Exe</OutputType>`). Le banc le publie puis reconstruit la solution.
- **Un package NuGet** : ses sources dans `samples/_packages/<Nom>/`, avec `PackageId` et `Version` dans le `.csproj`.

L'appli doit écrire sur la sortie standard un résultat qui ne dépend ni de l'heure ni de la machine : c'est ce que compare le critère « Same behavior ».
