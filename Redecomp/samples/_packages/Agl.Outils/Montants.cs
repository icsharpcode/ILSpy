using System.Globalization;

namespace Agl.Outils;

/// <summary>Formatage des montants en francs CFA.</summary>
public static class Montants
{
    public static string EnXaf(decimal montant)
        => montant.ToString("N0", CultureInfo.InvariantCulture) + " XAF";
}
