using ChessMemoryApp.Model.File_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Memory_Techniques
{
    public class NumberToWordConverter
    {
        private readonly List<string> words = new List<string>(410652);

        public NumberToWordConverter() { }

        public async void LoadWordsFromFile(string file)
        {
            List<string> lines = await FileHelper.GetLinesFromFileList(file);
            foreach (string line in lines)
                words.Add(line);
        }

        public List<string> GetWordsFromChessCoordinates(string coordinate)
        {
            char letter = coordinate[0];
            string digit = coordinate[1].ToString();

            if (letter.IsVowel())
            {
                if (letter == 'a')
                    letter = '1';
                else if (letter == 'e')
                    letter = '5';
            }
            else
                letter = ConsonantSoundHelper.ConvertConsonantToDigit(letter);

            return GetWordsFromNumber(letter + digit);
        }

        public List<string> GetWordsFromNumber(string number)
        {
            return GetWordsFromNumber(words, number);
        }

        public static List<string> GetWordsFromNumber(List<string> words, string number)
        {
            if (words.Count == 0)
                return new List<string>();

            var foundWords = new List<string>();
            List<string[]> soundsFromNumber = ConsonantSoundHelper.GetSoundsFromNumber(number);

            foreach (string word in words)
            {
                List<string> consonantSounds = ConsonantSoundHelper.GetConsonantSoundsFromWord(word);
                if (consonantSounds.Count != soundsFromNumber.Count)
                    continue;

                int index = 0;

                for (int i = 0; i < consonantSounds.Count; i++)
                {
                    if (soundsFromNumber[index].Any(x => x == consonantSounds[index]))
                        index++;
                    else
                        break;
                }

                if (index == consonantSounds.Count)
                    foundWords.Add(word);
            }

            return foundWords;
        }
    }
}
