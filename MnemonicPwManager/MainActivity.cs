using Android.App;
using Android.Widget;
using Android.OS;
using System.IO;
using Android.Content.Res;
using System;
using System.Net;
using System.Threading.Tasks;


namespace MnemonicPwManager
{
    [Activity(Label = "Mnemonic Password Manager", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public string TextMaterial { get; private set; }
        NumberPicker wordLengthPicker;
        Spinner bookSpinner;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //Assigns delegate method to button click.
            Button generateWordButton = FindViewById<Button>(Resource.Id.generateButton); 
            generateWordButton.Click += OnGenerateWordButtonClick;                        

            //Add string-array with book titles to spinner.
            bookSpinner = FindViewById<Spinner>(Resource.Id.bookSpinner);
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this,
            Resource.Array.BookChoices, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
            bookSpinner.Adapter = adapter;
            bookSpinner.ItemSelected += InitializeTextFile;
           
            wordLengthPicker = FindViewById<NumberPicker>(Resource.Id.numberPicker);
            wordLengthPicker.MaxValue = 80;
            wordLengthPicker.MinValue = 8;
            NumberPicker numbers = FindViewById<NumberPicker>(Resource.Id.numberPickerNumbers);
            numbers.MaxValue = wordLengthPicker.Value;
            numbers.MinValue = 0;
            numbers.Enabled = false;

            //Enable numbers for selection.
            Switch numberSwitch = FindViewById<Switch>(Resource.Id.numberSwitch);
            numberSwitch.CheckedChange += (sender, e)=>
            {
                if (numberSwitch.Checked == true)
                    numbers.Enabled = true;
                else
                    numbers.Enabled = false;
            };

            //Changes numbers max-amount according to wordlength.
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
            //InitializeTextFile();
            EditText randomWordText = FindViewById<EditText>(Resource.Id.passwordText);
            EditText mnemonicSentence = FindViewById<EditText>(Resource.Id.mnemonicText);
            RandomWord randomWord = new RandomWord(NumberOfLetters(), TextMaterial, UseNumbersInPassword(),UseNumbersInPassword() ? AmountOfNUmbers() : 0 );
            randomWordText.Text = randomWord.NewWord;
            mnemonicSentence.Text = randomWord.NewSentence;
        }

        protected int NumberOfLetters()
        {
            return wordLengthPicker.Value;
        }

        protected void InitializeTextFile(object sender, EventArgs e)
        {
            AssetManager assets = this.Assets;
            if (bookSpinner.SelectedItem.Equals("The Iliad by Homer (Available offline)"))
                using (StreamReader sr = new StreamReader(assets.Open("Iliad.txt")))
                {
                    TextMaterial = sr.ReadToEnd();
                }
            else if (bookSpinner.SelectedItem.Equals("Crime and punishment by Fyodor Dostoyevsky (Available offline)"))
                using (StreamReader sr = new StreamReader(assets.Open("CrimeAndPunishment.txt")))
                {
                    TextMaterial = sr.ReadToEnd();
                }
            else
                DownloadAsync(bookSpinner.SelectedItem.ToString());
            //else if (bookSpinner.SelectedItem.Equals("The Odyssey by Homer (Online resource)"))
            //    DownloadAsync();
        }

        async void DownloadAsync(string bookTitle)
        {
            try
            {
                using (var client = new WebClient())
                {
                    Task<string> getString;
                    switch (bookTitle)
                    {
                        case "The Odyssey by Homer(Online resource)":
                            getString = client.DownloadStringTaskAsync("http://www.gutenberg.org/cache/epub/1727/pg1727.txt");
                            TextMaterial = await getString;
                            break;
                        case "The Trial by Franz Kafka (Online resource)":
                            getString = client.DownloadStringTaskAsync("http://www.gutenberg.org/cache/epub/7849/pg7849.txt");
                            TextMaterial = await getString;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                AlertDialog alert = builder.Create();
                alert.SetTitle("Network problem");
                alert.SetMessage("There was a problem when trying to reach online resources. " + bookTitle);
                alert.SetButton("OK",(sender, e) =>
                {

                });
                alert.Show();
            }
            
        }
        protected bool UseNumbersInPassword()
        {
            Switch numberSwitch = FindViewById<Switch>(Resource.Id.numberSwitch);
            return numberSwitch.Checked ? true : false;
        }

        protected int AmountOfNUmbers()
        {
            NumberPicker numbers = FindViewById<NumberPicker>(Resource.Id.numberPickerNumbers);
            return numbers.Value;
        }
    }
}

