using Portail.Metier;

// Récapitulatif des escales de la semaine
var escales = new List<Escale>
{
    new("MSC Gülsün", 36),
    new("Maersk Douala", 18),
};
foreach (var escale in escales)
    Console.WriteLine(escale.Resume());
