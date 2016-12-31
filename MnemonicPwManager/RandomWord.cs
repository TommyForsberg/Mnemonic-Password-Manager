using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.Res;
using System.IO;


namespace MnemonicPwManager
{
    class RandomWord
    {
        int _wordLength;
        List<string> _passwordMnemonics;
        bool _numbers;
        int _amountOfNumbers;
        public string NewWord { get; private set; }
        public string NewSentence { get; private set; }
        
        public RandomWord(int WordLength, string text, bool _numbers, int _amountOfNumbers)
        {
            this._amountOfNumbers = _amountOfNumbers;
            this._numbers = _numbers;
            if (_numbers == true)
                _wordLength = WordLength - _amountOfNumbers;
            else
                _wordLength = WordLength;

           _passwordMnemonics = GenerateRandomSentence(SplitTextIntoSentenceArrays(text));
            NewWord = GenerateMnemonicString();
        }

        List<string[]> SplitTextIntoSentenceArrays(string text)
        {
            string[] separators = { "!", ".", "?" };
            var sentences = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var splittedText = new List<string[]>();
            int index = 0;
            foreach (var sentence in sentences)
            {
                splittedText.Add(sentences[index].Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                index++;
            }
            return splittedText;
        }

        List<string> GenerateRandomSentence(List<string[]> SplittedText)
        {
            //Collects all sentences as arrays with as many words as password should be long in a list with arrays.
            var selectedArrays = SplittedText.Where(array => array.Count().Equals(_wordLength)).ToArray();

            //Picks one of the sentences at random
            Random rnd = new Random();
            int number = rnd.Next(0, selectedArrays.Length);
            var randomSentence = selectedArrays[number];

            //Transfers the words in array to an expandable List.
            var passwordMnemonics = new List<string>();
            for (int i = 0; i < randomSentence.Length; i++)
            {
                passwordMnemonics.Add(randomSentence[i]);
            }

            //Inserts numbers if true.
            #region insert random number
            if (_numbers)
            {
                for (int i = 0; i < _amountOfNumbers; i++)
                {
                    int randomNumber = rnd.Next(0, 9);
                    int randomPlace = rnd.Next(0, passwordMnemonics.Count);
                    passwordMnemonics.Insert(randomPlace, randomNumber.ToString());
                }
            }
            #endregion
            return passwordMnemonics;
        }
        string GenerateMnemonicString() //Takes inital letters and builds a sentence.
        { 
            string newSentence = string.Empty;
            foreach (var word in _passwordMnemonics)
            {
                newSentence = string.Format(newSentence + word + " ");
            }
            NewSentence = newSentence;
            return new string(_passwordMnemonics.Select(x => x[0]).ToArray());
        }
        }
    }
