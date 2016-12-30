using Android.App;
using Android.Widget;
using Android.OS;
using System.IO;
using Android.Content.Res;
using System;

namespace MnemonicPwManager
{
    [Activity(Label = "Mnemonic Password Manager", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public string Iliad { get; private set; }
        NumberPicker wordLengthPicker;
        Spinner bookSpinner;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.Main);
            Button generateWordButton = FindViewById<Button>(Resource.Id.generateButton);
            generateWordButton.Click += OnGenerateWordButtonClick;

         
            bookSpinner = FindViewById<Spinner>(Resource.Id.bookSpinner);
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this,
            Resource.Array.BookChoices, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
            bookSpinner.Adapter = adapter;
           

            wordLengthPicker = FindViewById<NumberPicker>(Resource.Id.numberPicker);
            wordLengthPicker.MaxValue = 80;
            wordLengthPicker.MinValue = 8;
           
            NumberPicker numbers = FindViewById<NumberPicker>(Resource.Id.numberPickerNumbers);
            numbers.MaxValue = wordLengthPicker.Value;
            numbers.MinValue = 0;
            numbers.Enabled = false;

            Switch numberSwitch = FindViewById<Switch>(Resource.Id.numberSwitch);
            numberSwitch.CheckedChange += delegate
            {
                if (numberSwitch.Checked == true)
                    numbers.Enabled = true;
                else
                    numbers.Enabled = false;
            };

            wordLengthPicker.ValueChanged += delegate
            {
                numbers.MaxValue = wordLengthPicker.Value;
            };

           
            //generateWordButton.Click += delegate
            //{
               
            //    RandomWord randomWord = new RandomWord(NumberOfLetters(), Iliad,UseNumbersInPassword());
            //    randomWordText.Text = randomWord.NewWord;
            //    mnemonicSentence.Text = randomWord.NewSentence;
            //};

            
        }

        private void OnGenerateWordButtonClick(object sender, EventArgs e)
        {
            InitializeTextFile();
            EditText randomWordText = FindViewById<EditText>(Resource.Id.passwordText);
            EditText mnemonicSentence = FindViewById<EditText>(Resource.Id.mnemonicText);
            RandomWord randomWord = new RandomWord(NumberOfLetters(), Iliad, UseNumbersInPassword(),UseNumbersInPassword() ? AmountOfNUmbers() : 0 );
            randomWordText.Text = randomWord.NewWord;
            mnemonicSentence.Text = randomWord.NewSentence;
        }

        protected int NumberOfLetters()
        {
            return wordLengthPicker.Value;
        }

        protected void InitializeTextFile()
        {
            AssetManager assets = this.Assets;
            if (bookSpinner.SelectedItem.Equals("The Iliad by Homer"))
            using (StreamReader sr = new StreamReader(assets.Open("Iliad.txt")))
            {
                Iliad = sr.ReadToEnd();
            }
            else if(bookSpinner.SelectedItem.Equals("Crime and punishment by Fyodor Dostoyevsky"))
                using (StreamReader sr = new StreamReader(assets.Open("CrimeAndPunishment.txt")))
                {
                    Iliad = sr.ReadToEnd();
                }
            
        }
        protected bool UseNumbersInPassword()
        {
            Switch numberSwitch = FindViewById<Switch>(Resource.Id.numberSwitch);
            if (numberSwitch.Checked == true)
                return true;
            else
                return false;
        }

        protected int AmountOfNUmbers()
        {
            NumberPicker numbers = FindViewById<NumberPicker>(Resource.Id.numberPickerNumbers);
            return numbers.Value;
        }


    }
}

