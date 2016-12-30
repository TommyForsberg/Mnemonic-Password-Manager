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
    public class RandomWord
    {
        List<string[]> SplittedText { get; set; }
        public int WordLength { get; set; }
        public bool Numbers { get; set; }
        public int AmountOfNumbers { get; set; }
        public string NewWord { get; set; }
        public string NewSentence { get; set; }
        
        public RandomWord(int WordLength, string text, bool Numbers, int AmountOfNumbers)
        {

            SplitTextIntoSentenceArrays(text);
            this.AmountOfNumbers = AmountOfNumbers;
            this.Numbers = Numbers;
            if (Numbers == true)
                this.WordLength = WordLength - AmountOfNumbers;
            else
                this.WordLength = WordLength;

            GenerateRandomSentence();
        }

        public void SplitTextIntoSentenceArrays(string text)
        {
            string[] separators = { "!", ".", "?" };
            var sentences = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            SplittedText = new List<string[]>();
            int index = 0;
            foreach (var sentence in sentences)
            {
                SplittedText.Add(sentences[index].Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                index++;
            }
        }

        void GenerateRandomSentence()
        {
            var selectedArrays = SplittedText.Where(array => array.Count().Equals(WordLength)).ToArray();

            Random rnd = new Random();
            int number = rnd.Next(0, selectedArrays.Length);
            var randomSentence = selectedArrays[number];
           
                
            var expandedArray = new List<string>(); //insertnumbers test

            for (int i = 0; i < randomSentence.Length; i++)
            {
                expandedArray.Add(randomSentence[i]);
            }
                #region insert random number
                if (Numbers)
            {
                for (int i = 0; i < AmountOfNumbers; i++)
                {
                    int randomNumber = rnd.Next(0, 9);
                    int randomPlace = rnd.Next(0, expandedArray.Count);
                    expandedArray.Insert(randomPlace, randomNumber.ToString());
                }
            }
            #endregion
            string newSentence = string.Empty;
            foreach (var word in expandedArray) //change to random sentence to roll back
            {
                newSentence = string.Format(newSentence + word + " ");
            }

            NewSentence = newSentence;
            NewWord = new string(expandedArray.Select(x => x[0]).ToArray()); //change to randomsentence to roll back

        }
        //public string Randomizer() //Not In use
        //{
        //    string newWord = string.Empty;
        //    char randomChar;
        //    Random rnd = new Random();
        //    int index= rnd.Next(0, Captions.Length);
        //    for(int i = 0; i< WordLength; i++)
        //    {
        //        randomChar = Captions[rnd.Next(0, Captions.Length)];
        //        newWord = newWord += randomChar;
        //    }
        //    return newWord;
        //}
    }
}