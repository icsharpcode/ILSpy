using Agl.Outils;

namespace Portail.Metier;

/// <summary>Escale d'un navire au port, facturée à la durée.</summary>
public record Escale(string Navire, int DureeHeures)
{
    // Tarif horaire de stationnement à quai
    public const decimal TarifHoraire = 150_000m;

    public decimal Cout() => DureeHeures * TarifHoraire;

    public string Resume() => $"{Navire} : {DureeHeures} h, {Montants.EnXaf(Cout())}";
}
