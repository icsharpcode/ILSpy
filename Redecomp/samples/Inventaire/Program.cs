using Agl.Inventaire.Domaine;

// Inventaire du dépôt de Bonabéri
var depot = new Depot();
depot.Ajouter(new Article("CONT-40", "Conteneur 40 pieds", 12, 2_500_000m));
depot.Ajouter(new Article("CONT-20", "Conteneur 20 pieds", 0, 1_400_000m));
depot.Ajouter(new Article("PAL-EU", "Palette europe", 340, 9_500m));

int ruptures = depot.CompterRuptures();
decimal valeurStock = depot.CalculerValeurStock();
int quantiteTotale = depot.CalculerQuantiteTotale();
string rapport = depot.GenererRapport();
bool sauvegarde = await depot.SauvegarderAsync(Path.Combine(Path.GetTempPath(), "inventaire.txt"));

Console.Write(rapport);
Console.WriteLine($"Ruptures : {ruptures} | Valeur : {valeurStock:N0} | Quantité : {quantiteTotale} | Sauvegardé : {sauvegarde}");
