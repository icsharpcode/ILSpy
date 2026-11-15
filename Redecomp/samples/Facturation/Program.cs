using System.Reflection;
using System.Text.Json;
using Agl.Facturation.Models;
using Agl.Facturation.Services;

// Point d'entrée : charge la config, calcule et affiche une facture de test
var config = JsonDocument.Parse(File.ReadAllText("appsettings.json")).RootElement;
decimal tauxTva = config.GetProperty("TauxTva").GetDecimal();
var calculateur = new CalculateurFacture(tauxTva);

var factureTest = new Facture("FAC-2026-0001", "Port Autonome de Douala", new()
{
    new LigneFacture("Manutention conteneur 40'", 12, 85_000m),
    new LigneFacture("Stockage (jours)", 30, 4_500m),
});

decimal totalTtc = calculateur.CalculerTotalTtc(factureTest);
using var flux = Assembly.GetExecutingAssembly().GetManifestResourceStream("Agl.Facturation.Templates.facture.txt")!;
string modele = new StreamReader(flux).ReadToEnd();
Console.WriteLine(modele.Replace("{numero}", factureTest.Numero).Replace("{client}", factureTest.Client).Replace("{total}", totalTtc.ToString("N0")));
bool envoyee = await calculateur.EnvoyerAsync(factureTest);
Console.WriteLine($"Envoyée : {envoyee} — {calculateur.LibelleStatut(factureTest.Statut)}");
