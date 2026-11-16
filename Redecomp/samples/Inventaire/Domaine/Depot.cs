using System.Text;

namespace Agl.Inventaire.Domaine;

/// <summary>Dépôt de stockage : gère les articles et produit les indicateurs.</summary>
public class Depot
{
    private readonly List<Article> _articles = new();

    public void Ajouter(Article article) => _articles.Add(article);

    /// <summary>Nombre d'articles dont le stock est épuisé.</summary>
    public int CompterRuptures()
    {
        int ruptures = _articles.Count(a => a.Quantite == 0);
        Console.WriteLine($"[log] ruptures calculées : {ruptures}");
        return ruptures;
    }

    /// <summary>Valeur totale du stock (quantité × prix unitaire).</summary>
    public decimal CalculerValeurStock()
    {
        decimal valeur = 0m;
        foreach (var article in _articles)
        {
            // les articles en rupture ne comptent pas
            if (article.Quantite > 0)
                valeur += article.Quantite * article.PrixUnitaire;
        }
        return valeur;
    }

    public int CalculerQuantiteTotale()
    {
        int quantiteTotale = _articles.Sum(a => a.Quantite);
        int plusGrosStock = _articles.Max(a => a.Quantite);
        Console.WriteLine($"[log] plus gros stock : {plusGrosStock}");
        return quantiteTotale;
    }

    public string GenererRapport()
    {
        var rapport = new StringBuilder();
        int numeroLigne = 1;
        foreach (var article in _articles.OrderBy(a => a.Code))
        {
            rapport.AppendLine($"{numeroLigne}. {article.Code} - {article.Libelle} : {article.Quantite}");
            numeroLigne++;
        }
        return rapport.ToString();
    }

    /// <summary>Sauvegarde le stock dans un fichier texte.</summary>
    public async Task<bool> SauvegarderAsync(string chemin)
    {
        string contenu = GenererRapport();
        await File.WriteAllTextAsync(chemin, contenu);
        return File.Exists(chemin);
    }
}
