using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds all quotes
// Returns a random one

public static class Quotes {

	//static List<string> quotes = new List<string>();
	static string[] quotes = new string[] {
		"The first step towards failure is trying",
		"Those who doubt your ability probably have a valid reason",
		"If at first you don’t succeed give up and try something else",
		"Always believe that something wonderful will probably never happen",
		"Every corpse on mount Everest was once an extremely motivated person",
		"Everything happens for a reasons. Sometimes the reason is that you’re stupid and make bad decisions",
		"Always remember you’re someone’s reason to smile because you’re a joke",
		"Some are born great, some achieve greateness, and some wind up like you",
		"Everyone is good at something, and you're the best at not being good at anything",
		"When life gives you lemons..."
	};


	public static string GetRandom() {
		int len = quotes.Length;
		int ran = Mathf.FloorToInt(Random.Range(0, len));
		return quotes[ran];
	}
}
