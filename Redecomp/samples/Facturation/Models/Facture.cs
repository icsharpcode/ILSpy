namespace Agl.Facturation.Models;

/// <summary>
/// Représente une facture émise à un client.
/// </summary>
/// <param name="Numero">Numéro unique, format FAC-AAAA-NNNN.</param>
public record Facture(string Numero, string Client, List<LigneFacture> Lignes)
{
    /// <summary>Statut courant dans le workflow de validation.</summary>
    public StatutFacture Statut { get; set; } = StatutFacture.Brouillon;
}

/// <summary>Une ligne de facture (prestation ou marchandise).</summary>
public record LigneFacture(string Libelle, int Quantite, decimal PrixUnitaireHt);

public enum StatutFacture { Brouillon, Validee, Envoyee, Payee }
