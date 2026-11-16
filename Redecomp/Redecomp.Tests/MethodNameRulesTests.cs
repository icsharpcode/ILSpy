// Copyright (c) 2026 Franck
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using Redecomp.Core.Naming;

namespace Redecomp.Tests;

public static class MethodNameRulesTests
{
	[Test]
	public static void StripsLeadingVerbInFrenchAndEnglish()
	{
		Assert.AreEqual("totalTtc", MethodNameRules.NameForResultOf("CalculerTotalTtc", false), "CalculerTotalTtc");
		Assert.AreEqual("rapport", MethodNameRules.NameForResultOf("GenererRapport", false), "GenererRapport");
		Assert.AreEqual("ruptures", MethodNameRules.NameForResultOf("CompterRuptures", false), "CompterRuptures");
		Assert.AreEqual("customerOrders", MethodNameRules.NameForResultOf("LoadCustomerOrders", false), "LoadCustomerOrders");
		Assert.AreEqual("total", MethodNameRules.NameForResultOf("GetTotal", false), "GetTotal");
	}

	[Test]
	public static void DropsAsyncSuffix()
	{
		Assert.AreEqual("orders", MethodNameRules.NameForResultOf("FetchOrdersAsync", false), "FetchOrdersAsync");
	}

	[Test]
	public static void NamesBooleanResultOfAVerbAfterItsParticiple()
	{
		Assert.AreEqual("sauvegarde", MethodNameRules.NameForResultOf("SauvegarderAsync", true), "SauvegarderAsync");
		Assert.AreEqual("envoye", MethodNameRules.NameForResultOf("EnvoyerAsync", true), "EnvoyerAsync");
		Assert.AreEqual("saved", MethodNameRules.NameForResultOf("Save", true), "Save");
		Assert.AreEqual("sent", MethodNameRules.NameForResultOf("Send", true), "Send");
		Assert.AreEqual("processed", MethodNameRules.NameForResultOf("Process", true), "Process");
	}

	[Test]
	public static void NamesBooleanResultOfAVerbWithObjectAfterTheObject()
	{
		Assert.AreEqual("itemRemoved", MethodNameRules.NameForResultOf("RemoveItem", true), "RemoveItem");
		Assert.AreEqual("factureValide", MethodNameRules.NameForResultOf("ValiderFacture", true), "ValiderFacture");
		Assert.AreEqual("isValid", MethodNameRules.NameForResultOf("IsValid", true), "IsValid");
	}

	[Test]
	public static void SuffixesNonBooleanResultOfAVerb()
	{
		Assert.AreEqual("processResult", MethodNameRules.NameForResultOf("Process", false), "Process");
		Assert.AreEqual("executerResult", MethodNameRules.NameForResultOf("ExecuterAsync", false), "ExecuterAsync");
	}

	[Test]
	public static void KeepsNounMethodNamesAndRejectsUninformativeOnes()
	{
		Assert.AreEqual("totalTtc", MethodNameRules.NameForResultOf("TotalTtc", false), "TotalTtc");
		Assert.AreEqual(null, MethodNameRules.NameForResultOf("ToString", false), "ToString");
		Assert.AreEqual(null, MethodNameRules.NameForResultOf("Invoke", false), "Invoke");
		Assert.AreEqual(null, MethodNameRules.NameForResultOf("Async", false), "Async");
	}
}
