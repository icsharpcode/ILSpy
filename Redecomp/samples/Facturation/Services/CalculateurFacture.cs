using Agl.Facturation.Models;

namespace Agl.Facturation.Services;

/// <summary>
/// Calcule les montants HT/TTC d'une facture selon le taux de TVA camerounais.
/// </summary>
public class CalculateurFacture
{
    // Remise appliquée au-delà de ce seuil de HT (règle commerciale 2026)
    private const decimal SeuilRemise = 1_000_000m;
    private readonly decimal _tauxTva;

    public CalculateurFacture(decimal tauxTva = 0.1925m) => _tauxTva = tauxTva;

    /// <summary>Retourne le total TTC, remise incluse.</summary>
    /// <param name="facture">La facture à calculer.</param>
    public decimal CalculerTotalTtc(Facture facture)
    {
        // 1. Somme des lignes HT
        decimal totalHt = facture.Lignes.Sum(ligne => ligne.Quantite * ligne.PrixUnitaireHt);

        // 2. Remise de 5 % pour les gros montants
        decimal remise = totalHt > SeuilRemise ? totalHt * 0.05m : 0m;
        decimal netHt = totalHt - remise;

        // 3. TVA (19,25 % au Cameroun)
        decimal montantTva = Math.Round(netHt * _tauxTva, 0);
        return netHt + montantTva;
    }

    public string LibelleStatut(StatutFacture statut) => statut switch
    {
        StatutFacture.Brouillon => "En cours de saisie",
        StatutFacture.Validee => "Validée par le chef de service",
        StatutFacture.Payee => "Réglée",
        _ => "Envoyée au client"
    };

    /// <summary>Simule l'envoi de la facture à l'ERP (appel réseau).</summary>
    public async Task<bool> EnvoyerAsync(Facture facture, CancellationToken ct = default)
    {
        int tentative = 0;
        while (tentative < 3)
        {
            tentative++;
            try
            {
                await Task.Delay(10, ct);   // appel HTTP simulé
                facture.Statut = StatutFacture.Envoyee;
                return true;
            }
            catch (TaskCanceledException) when (tentative < 3)
            {
                // on réessaie silencieusement
            }
        }
        return false;
    }
}
