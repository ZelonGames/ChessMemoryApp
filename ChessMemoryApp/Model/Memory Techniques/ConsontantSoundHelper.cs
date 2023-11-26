using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Memory_Techniques
{
    public static class ConsonantSoundHelper
    {
        public static readonly string[][] soundCollection = new string[][]
        {
            new string[]{"ss", "s", "c", "z"},
            new string[]{"tt", "t", "dd", "d"},
            new string[]{"nn", "ng", "n"},
            new string[]{"mm", "m" },
            new string[]{"rr", "r"},
            new string[]{"ll", "l"},
            new string[]{"j", "sch", "sj", "h"},
            new string[]{"ck", "kk", "k", "gg", "g"},
            new string[]{"ff", "f", "vv", "v"},
            new string[]{"pp", "p", "bb", "b"},
        };

        public static char ConvertConsonantToDigit(char consonant)
        {
            for (int i = 0; i < soundCollection.Length; i++)
            {
                if (soundCollection[i].Any(x => x == consonant.ToString()))
                    return i.ToString()[0];
            }

            return ' ';
        }

        public static bool IsVowel(this char character)
        {
            return character is
                'a' or 'e' or 'i' or 'o' or 'u' or 'y' or 'å' or 'ä' or 'ö' or
                'A' or 'E' or 'I' or 'O' or 'U' or 'Y' or 'Å' or 'Ä' or 'Ö';
        }

        public static bool IsConsonant(this char character)
        {
            return character is
                'b' or 'c' or 'd' or 'f' or 'g' or 'h' or 'j' or 'k' or 'l' or 'm' or 'n' or 'p' or 'q' or 'r' or 's' or 't' or 'v' or 'w' or 'x' or 'z' or
                'B' or 'C' or 'D' or 'F' or 'G' or 'H' or 'J' or 'K' or 'L' or 'M' or 'N' or 'P' or 'Q' or 'R' or 'S' or 'T' or 'V' or 'W' or 'X' or 'Z';
        }

        public static bool IsFirstVowelEqualTo(this string word, char vowel)
        {
            foreach (char c in word)
            {
                if (c == vowel)
                    return true;

                if (c.IsVowel())
                    return false;
            }

            return false;
        }

        public static List<string> GetConsonantSoundsFromWord(string word)
        {
            var sounds = new List<string>();

            string currentSound = "";
            foreach (char character in word)
            {
                if (character.IsConsonant())
                    currentSound += character;
                else
                {
                    if (currentSound.Length > 0)
                        sounds.Add(currentSound);

                    currentSound = "";
                }
            }

            if (currentSound.Length > 0)
                sounds.Add(currentSound);

            return sounds;
        }

        public static string RemoveVowels(this string word)
        {
            string newWord = "";

            foreach (char c in word)
            {
                if (c.IsConsonant())
                    newWord += c;
            }

            return newWord;
        }

        public static List<string[]> GetSoundsFromNumber(string number)
        {
            var sounds = new List<string[]>(number.Length);

            foreach (char digit in number)
            {
                string[] soundAlternatives = soundCollection[(int)char.GetNumericValue(digit)];
                sounds.Add(soundAlternatives);
            }

            return sounds;
        }
    }
}
