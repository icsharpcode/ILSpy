# Architecture

Redecomp est une couche au-dessus du moteur `ICSharpCode.Decompiler` d'ILSpy. Il ne modifie pas le moteur : il passe par ses points d'extension publics, puis retravaille le projet que le moteur produit.

## Pipeline d'un projet

`ProjectReconstructor.Reconstruct` traite une DLL dans cet ordre :

```
DLL ──► PDB trouvé ? (embarqué, à côté, chemin enregistré)        PortablePdbLocator
   │
   ├─► Décompilation du projet complet par ILSpy                  RedecompProjectDecompiler
   │     + nommage déterministe des variables (transform ILAst)   DeterministicVariableNames
   │
   ├─► Sources embarquées dans le PDB ?                           EmbeddedSourceReader
   │     oui → remplacent les fichiers décompilés                 EmbeddedSourceLayout
   │     non, mais PDB → fichiers déplacés aux chemins d'origine  SourceRelocator, TypeDocumentMap
   │                     → originaux via SourceLink si possible   SourceLinkRecovery
   │
   ├─► .csproj : références réécrites                             ReferenceRewriter, DepsFile
   │
   └─► Fichiers livrés à côté de la DLL copiés                    ContentFileCollector
```

La décompilation tourne toujours, même quand les sources d'origine existent : c'est elle qui produit le `.csproj`, les ressources et les attributs d'assembly, qui ne sont pas dans les sources.

`SolutionReconstructor.Reconstruct` lit le `.deps.json` d'un dossier publié, appelle ce pipeline pour chaque projet avec un `DependencyContext` (qui connaît les autres projets et les packages), puis écrit le `.slnx`.

## Modules

| Dossier | Rôle |
|---|---|
| `Recovery/` | Lire ce qui existe encore : PDB, sources embarquées, SourceLink, `deps.json`, fichiers de contenu, chemins d'origine. Aucune décompilation ici. |
| `Reconstruction/` | Orchestrer ILSpy et réécrire sa sortie : projet, solution, références, emplacement des fichiers. |
| `Naming/` | Nommage déterministe des variables locales. |
| `Verification/` | Comparaison IL entre l'original et le projet recompilé. |
| `Vendor/` | Fichiers copiés d'autres projets du dépôt (voir son README), pour que `Redecomp.Core` ne dépende que du moteur. |

## Points d'extension d'ILSpy utilisés

- **`WholeProjectDecompiler.CreateDecompiler`** (méthode virtuelle) : `RedecompProjectDecompiler` la surcharge pour insérer `DeterministicVariableNames` dans la liste `ILTransforms`, juste avant `AssignVariableNames`.
- **`IDebugInfoProvider`** : le fournisseur de PDB portable est passé au décompilateur pour les noms des variables et les séquences.
- **`WholeProjectDecompiler.GetFileNameForType`** : permet de savoir où ILSpy a écrit chaque type, pour le déplacer ensuite à son chemin d'origine.
- **`ReflectionDisassembler`** : sert à la comparaison IL.

## Choix de conception

**Les originaux d'abord, la décompilation en dernier recours.** Une source embarquée ou téléchargée et vérifiée est toujours meilleure que du code décompilé.

**Rien n'est gardé sans preuve.** Un fichier SourceLink n'est utilisé que si son SHA-256 (ou SHA-1) est celui que le compilateur a enregistré dans le PDB. Un fichier modifié dans le dépôt depuis la compilation est donc ignoré, jamais pris pour l'original.

**Nommage déterministe.** Un nom n'est donné que s'il découle d'un fait lu dans le code : la méthode qui retourne la variable, la méthode dont elle reçoit le résultat, l'agrégat LINQ qu'elle stocke. Même entrée, même sortie. Les variables qui ont un nom d'origine (PDB, champs de machine à états async ou de lambda) ne sont jamais renommées : la transformation ne touche qu'aux variables marquées `HasGeneratedName`.

**Le `.csproj` ne doit dépendre d'aucune machine.** Les références vers le framework de base disparaissent (le SDK les fournit), les autres frameworks partagés deviennent des `FrameworkReference`, les DLL de packages des `PackageReference` avec la version du `deps.json`, les DLL de projets des `ProjectReference`.

**La fidélité se mesure sur le code exécuté.** La comparaison IL porte sur la signature, les variables locales et les instructions de chaque méthode, avec les références aux membres résolues en noms (la numérotation des tokens ne compte pas). Les blocs d'attributs sont exclus : le compilateur déplace les annotations `Nullable` entre un type et ses membres selon les autres membres du type, sans changer le code.

## Nommage déterministe en détail

Pour chaque variable sans nom d'origine, la première règle qui s'applique gagne :

1. **Variable retournée** (directement ou via `ToString()`) par une méthode : nom dérivé de la méthode.
2. **Résultat d'une méthode de l'assembly décompilé** : nom dérivé de cette méthode. Les méthodes du framework sont ignorées, leurs noms décrivent mal la valeur (`File.ReadAllText` donnerait `allText`).
3. **Agrégat LINQ** (`Sum`, `Count`, `Max`, `Min`, `Average`) : nom de l'agrégat (`Sum` donne `total`) suivi du membre sélectionné par la lambda, ou à défaut de la collection.

Dérivation d'un nom depuis une méthode (`MethodNameRules`) :

| Méthode | Résultat | Nom |
|---|---|---|
| `CalculerTotalTtc` | `decimal` | `totalTtc` (verbe retiré) |
| `FetchOrdersAsync` | `List<Order>` | `orders` (`Async` retiré) |
| `SauvegarderAsync` | `bool` | `sauvegarde` (participe passé) |
| `RemoveItem` | `bool` | `itemRemoved` |
| `Process` | `string` | `processResult` |
| `ToString`, `Invoke` | | aucun nom, les règles d'ILSpy s'appliquent |

Les verbes reconnus sont listés en anglais et en français dans `MethodNameRules.cs`. Pour en ajouter un, ajoute-le avec son participe passé et un cas dans `MethodNameRulesTests.cs`.

## Ajouter une étape de récupération

1. Écrire le test d'abord, dans `Redecomp.Tests`, et le voir échouer.
2. Mettre la lecture des données dans `Recovery/` (sans dépendance au décompilateur si possible).
3. Brancher l'étape dans `ProjectReconstructor.Reconstruct`, après la décompilation.
4. Si l'étape change ce que mesure le banc d'essai, régénérer `bench/results.md` et vérifier qu'aucun critère ne baisse.
