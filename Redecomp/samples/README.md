# Applis témoins

Chaque dossier est une appli dont on garde les sources d'origine : c'est la référence à laquelle le banc d'essai compare le projet reconstruit par Redecomp.

Les samples n'héritent pas des réglages du dépôt, pour ressembler à des applis tierces ordinaires. Leurs commentaires sont volontairement en français avec des accents : on vérifie ainsi que le texte non ASCII est récupéré intact.

- `Portail/` : solution de deux projets (`Portail.App` → `Portail.Metier`), dont l'un dépend du package NuGet `Agl.Outils`.
- `_packages/` : sources des packages NuGet utilisés par les samples, empaquetés dans un flux local au moment des tests.
- `Inventaire/` : appli console orientée nommage (variables nourries par des appels de méthodes, agrégats LINQ, valeurs retournées).
- `Facturation/` : petite appli console (record, enum, LINQ, async, constante, ressource embarquée, `appsettings.json`, commentaires `//` et doc `///`).
