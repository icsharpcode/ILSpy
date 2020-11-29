using System;
using System.Threading;

namespace Humanizer.Inflections
{
	/// <summary>
	/// Container for registered Vocabularies.  At present, only a single vocabulary is supported: Default.
	/// </summary>
	internal static class Vocabularies
	{
		private static readonly Lazy<Vocabulary> Instance;

		static Vocabularies()
		{
			Instance = new Lazy<Vocabulary>(BuildDefault, LazyThreadSafetyMode.PublicationOnly);
		}

		/// <summary>
		/// The default vocabulary used for singular/plural irregularities.
		/// Rules can be added to this vocabulary and will be picked up by called to Singularize() and Pluralize().
		/// At this time, multiple vocabularies and removing existing rules are not supported.
		/// </summary>
		public static Vocabulary Default => Instance.Value;

		private static Vocabulary BuildDefault()
		{
			var _default = new Vocabulary();

			_default.AddPlural("$", "s");
			_default.AddPlural("s$", "s");
			_default.AddPlural("(ax|test)is$", "$1es");
			_default.AddPlural("(octop|vir|alumn|fung|cact|foc|hippopotam|radi|stimul|syllab|nucle)us$", "$1i");
			_default.AddPlural("(alias|bias|iris|status|campus|apparatus|virus|walrus|trellis)$", "$1es");
			_default.AddPlural("(buffal|tomat|volcan|ech|embarg|her|mosquit|potat|torped|vet)o$", "$1oes");
			_default.AddPlural("([dti])um$", "$1a");
			_default.AddPlural("sis$", "ses");
			_default.AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
			_default.AddPlural("(hive)$", "$1s");
			_default.AddPlural("([^aeiouy]|qu)y$", "$1ies");
			_default.AddPlural("(x|ch|ss|sh)$", "$1es");
			_default.AddPlural("(matr|vert|ind|d)ix|ex$", "$1ices");
			_default.AddPlural("(^[m|l])ouse$", "$1ice");
			_default.AddPlural("^(ox)$", "$1en");
			_default.AddPlural("(quiz)$", "$1zes");
			_default.AddPlural("(buz|blit|walt)z$", "$1zes");
			_default.AddPlural("(hoo|lea|loa|thie)f$", "$1ves");
			_default.AddPlural("(alumn|alg|larv|vertebr)a$", "$1ae");
			_default.AddPlural("(criteri|phenomen)on$", "$1a");

			_default.AddSingular("s$", "");
			_default.AddSingular("(n)ews$", "$1ews");
			_default.AddSingular("([dti])a$", "$1um");
			_default.AddSingular("(analy|ba|diagno|parenthe|progno|synop|the|ellip|empha|neuro|oa|paraly)ses$", "$1sis");
			_default.AddSingular("([^f])ves$", "$1fe");
			_default.AddSingular("(hive)s$", "$1");
			_default.AddSingular("(tive)s$", "$1");
			_default.AddSingular("([lr]|hoo|lea|loa|thie)ves$", "$1f");
			_default.AddSingular("(^zomb)?([^aeiouy]|qu)ies$", "$2y");
			_default.AddSingular("(s)eries$", "$1eries");
			_default.AddSingular("(m)ovies$", "$1ovie");
			_default.AddSingular("(x|ch|ss|sh)es$", "$1");
			_default.AddSingular("(^[m|l])ice$", "$1ouse");
			_default.AddSingular("(o)es$", "$1");
			_default.AddSingular("(shoe)s$", "$1");
			_default.AddSingular("(cris|ax|test)es$", "$1is");
			_default.AddSingular("(octop|vir|alumn|fung|cact|foc|hippopotam|radi|stimul|syllab|nucle)i$", "$1us");
			_default.AddSingular("(alias|bias|iris|status|campus|apparatus|virus|walrus|trellis)es$", "$1");
			_default.AddSingular("^(ox)en", "$1");
			_default.AddSingular("(matr|d)ices$", "$1ix");
			_default.AddSingular("(vert|ind)ices$", "$1ex");
			_default.AddSingular("(quiz)zes$", "$1");
			_default.AddSingular("(buz|blit|walt)zes$", "$1z");
			_default.AddSingular("(alumn|alg|larv|vertebr)ae$", "$1a");
			_default.AddSingular("(criteri|phenomen)a$", "$1on");
			_default.AddSingular("([b|r|c]ook|room|smooth)ies$", "$1ie");

			_default.AddIrregular("person", "people");
			_default.AddIrregular("man", "men");
			_default.AddIrregular("human", "humans");
			_default.AddIrregular("child", "children");
			_default.AddIrregular("sex", "sexes");
			_default.AddIrregular("glove", "gloves");
			_default.AddIrregular("move", "moves");
			_default.AddIrregular("goose", "geese");
			_default.AddIrregular("wave", "waves");
			_default.AddIrregular("die", "dice");
			_default.AddIrregular("foot", "feet");
			_default.AddIrregular("tooth", "teeth");
			_default.AddIrregular("curriculum", "curricula");
			_default.AddIrregular("database", "databases");
			_default.AddIrregular("zombie", "zombies");
			_default.AddIrregular("personnel", "personnel");
			//Fix #789
			_default.AddIrregular("cache", "caches");

			//Fix 975
			_default.AddIrregular("ex", "exes", matchEnding: false);

			_default.AddIrregular("is", "are", matchEnding: false);
			_default.AddIrregular("that", "those", matchEnding: false);
			_default.AddIrregular("this", "these", matchEnding: false);
			_default.AddIrregular("bus", "buses", matchEnding: false);
			_default.AddIrregular("staff", "staff", matchEnding: false);
			_default.AddIrregular("training", "training", matchEnding: false);

			_default.AddUncountable("equipment");
			_default.AddUncountable("information");
			_default.AddUncountable("corn");
			_default.AddUncountable("milk");
			_default.AddUncountable("rice");
			_default.AddUncountable("money");
			_default.AddUncountable("species");
			_default.AddUncountable("series");
			_default.AddUncountable("fish");
			_default.AddUncountable("sheep");
			_default.AddUncountable("deer");
			_default.AddUncountable("aircraft");
			_default.AddUncountable("oz");
			_default.AddUncountable("tsp");
			_default.AddUncountable("tbsp");
			_default.AddUncountable("ml");
			_default.AddUncountable("l");
			_default.AddUncountable("water");
			_default.AddUncountable("waters");
			_default.AddUncountable("semen");
			_default.AddUncountable("sperm");
			_default.AddUncountable("bison");
			_default.AddUncountable("grass");
			_default.AddUncountable("hair");
			_default.AddUncountable("mud");
			_default.AddUncountable("elk");
			_default.AddUncountable("luggage");
			_default.AddUncountable("moose");
			_default.AddUncountable("offspring");
			_default.AddUncountable("salmon");
			_default.AddUncountable("shrimp");
			_default.AddUncountable("someone");
			_default.AddUncountable("swine");
			_default.AddUncountable("trout");
			_default.AddUncountable("tuna");
			_default.AddUncountable("corps");
			_default.AddUncountable("scissors");
			_default.AddUncountable("means");
			_default.AddUncountable("mail");

			return _default;
		}
	}
}