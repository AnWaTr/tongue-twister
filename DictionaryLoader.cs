using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace TroelsenPronTool
{
	class DictionaryLoader
	{
		// I'm going to hold all data in memory...RAM is cheap right? 
		private Dictionary<string, string> entries = new Dictionary<string, string> ();

		public DictionaryLoader ()
		{
			// Load the TXT file and loop over each line. 
			foreach (string line in File.ReadLines("CMU.dict.SAMPA.txt"))
			{
				// OK, we want to split this line such that the first item goes up to the 
				// very first whitespace, and the second is everything afterwards. 
				string[] lineSplit = line.Split (new char[]{' '}, 2);

				// Now, we want to strip out all white spaces from the second item of the split. 
				lineSplit[1] = Regex.Replace(lineSplit[1], @"\s+", "");

				// Add to dictionary:
				// Key 		= 	English orthography.
				// Value 	= 	SAMPA
				try
				{
					entries.Add (lineSplit[0], lineSplit[1]);
				}
				catch(Exception ex) // the file seems to have some duplicate ENG entries...
				{
					#if _DEBUG
					Console.WriteLine (ex.Message);
					Console.WriteLine (line);
					#endif
				}
			}
		}

		public Dictionary<string, string> GetDictionary()
		{
			return entries;
		}
	}
}