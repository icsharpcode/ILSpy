namespace Agl.Inventaire.Domaine;

/// <summary>Un article stocké au dépôt.</summary>
public record Article(string Code, string Libelle, int Quantite, decimal PrixUnitaire);
