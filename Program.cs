// #define _DEBUG

using System;
using System.Collections.Generic;
using System.Linq;

/* GoPost Note on what constitutes a rhyme:
 * 
 * Rhyme by the rhyme of the last syllable. 
 * Including the complexity of handling stress information as you mention is not required. 
 * I'll also only test with words of one or two syllables. 
 * 
 */

namespace TroelsenPronTool
{
	class MainClass
	{
		// In my implementation, I am not checking for any diphthongs.
		// I will only worry about the following SAMPA vowel characters. 
		private static char[] sampaVowels = {'I', 'E', '{', '}', '|', 'A', 'V', 'U', 'i', 
			'e', 'u', 'o', 'O', '@', '3', '2', '6', '9', 'Y'};

		private static Dictionary<string, string> data;

		public static void Main (string[] args)
		{
			DisplayBanner ();

			// First we need to load the CMU file into memory.
			// This is a dictionary of <ENG Orthography, SAMPA> entries. 
			DictionaryLoader dl = new DictionaryLoader ();
			data = dl.GetDictionary ();

			// Ask the user for a word until they enter 'q' to quit.  
			while (true)
			{
				Console.Write ("Please enter a word (q to quit): ");
				string wordToRhyme = Console.ReadLine ();
				string sampaValue = null;

				if (wordToRhyme.ToLower () == "q")
					break;

				// Let's see if the word is in our dictionary, if so, get the VALUE
				// of the KEY. 
				if (!data.TryGetValue (wordToRhyme.ToUpper(), out sampaValue))
				{
					Console.WriteLine ("Sorry, I don't know that word...try another?");
				} 
				else
				{
					Console.WriteLine ("Here are some words that rhyme with {0}:\n", wordToRhyme);

					#region Find and display rhyming words!
					// Split SAMPA into syllables and get last syllable. 
					string lastSyllable = sampaValue.Split ('.').Last<string>();

					// Get index of where VOWEL symbol is in the last syllable string. 
					int vowelPos = lastSyllable.IndexOfAny(sampaVowels); 

					// Now search the dictionary for words which have the same final syllable vowel.  
					var rhymeWords = (from kvp in data where 
						kvp.Value.EndsWith(lastSyllable.Substring(vowelPos)) select kvp);

					// Print out first 10...remove .Take(10) from this to see all of them. 
					foreach (var item in rhymeWords.Take(10))
					{
						// Don't show any item with has a (number) in it...
						if(!item.Key.Contains("("))
							Console.WriteLine ("Word: {0, -30} SAMPA: {1, -30}", 
								item.Key.ToLower(), item.Value);
					}
					#endregion

					#region Now make a tongue twister!
					Console.WriteLine ("\nHere is a tongue twister based on {0}:", wordToRhyme);

					// let's start by getting first syllable of the word. 
					string firstSyllable = sampaValue.Split('.').First<string>();

					#if _DEBUG
					Console.WriteLine ("First syllable: {0}", firstSyllable);
					#endif

					// Now, look for words in dictionary which have same onset. 
					var alliterations = (from kvp in data where 
						kvp.Value.StartsWith(firstSyllable) select kvp);
					List<string> batchZero = new List<string>();
					foreach (var item in alliterations) {
						#if _DEBUG
						Console.WriteLine ("Word: {0, -30} SAMPA: {1, -30}", item.Key, item.Value);
						#endif

						// Don't add any item with has a (<NUM>) in it.
						if(!item.Key.Contains("("))
							batchZero.Add (item.Key.ToLower());
					}	

					if(batchZero.Count == 0)
						batchZero.Add("");

					// Get new set of alliterations. 
					List<string> batchOne = GetNewRandomAlliterations(firstSyllable);

					// Finally, pick something from each batch.
					Random r = new Random();
					List<string> tongueTwister = new List<string>()
					{
						wordToRhyme, 
						batchZero[r.Next(batchZero.Count)],
						batchOne[r.Next(batchOne.Count)],
						batchZero[r.Next(batchZero.Count)],
						batchOne[r.Next(batchOne.Count)]
					};

					// Remove any dups. 
					foreach (var item in tongueTwister.Distinct()) {
						Console.Write("{0} ", item);
					}
					Console.WriteLine ("\n***************************************\n");
					#endregion
				}
			}
		}

		#region Helper function to get new alliterations
		private static List<string> GetNewRandomAlliterations(string oldSyllable)
		{
			int count = 0;
			char newVowel; 
			List<string> newAlliterations = new List<string>(); 

			// Now let's replace the vowel of the first syllable with a random new one.  
			// We could get no results- so keep looping until we find something or exhausted all possibilities. 
			while(true)
			{
				// Make sure we don't randomly get the same vowel. 
				while(true)
				{
					Random r = new Random();
					newVowel = sampaVowels[r.Next(sampaVowels.Length)];

					if(!oldSyllable.Contains(newVowel))
						break;
				}

				string newSyllable = oldSyllable.Replace(oldSyllable[oldSyllable.IndexOfAny(sampaVowels)], 
					newVowel);

				#if _DEBUG
				Console.WriteLine ("New First Syllable: {0}", newFirstSyllable);
				#endif

				// Now get new batch of words from dictionary based on this new first syllable.
				var alliterations = (from kvp in data where 
					(kvp.Value.StartsWith(newSyllable) ) select kvp);


				foreach (var item in alliterations) {
					#if _DEBUG
					Console.WriteLine ("Word: {0, -30} SAMPA: {1, -30}", item.Key, item.Value);
					#endif

					// Don't add any item with has a (number) in it...
					if(!item.Key.Contains("("))
						newAlliterations.Add (item.Key.ToLower());
				}				

				count++;

				if(alliterations.Count() > 1 || count == sampaVowels.Length)
					break; 
			}

			// This is just in case we really can't generate anything.
			if(newAlliterations.Count == 0)
				newAlliterations.Add("");

			return newAlliterations;
		}
		#endregion

		private static void DisplayBanner()
		{
			Console.WriteLine ("***************************************");
			Console.WriteLine ("Rhymes and Tongue twister Program");
			Console.WriteLine ("Troelsen - LING 575, Winter 2015");
			Console.WriteLine ("***************************************\n");
		}
	}
}
