using Android.App;
using Android.Widget;
using Android.OS;
using System.IO;
using Android.Content.Res;
using System;
using System.Net;
using System.Threading.Tasks;
using Android.Content;

namespace MnemonicPwManager
{
    [Activity(Label = "Mnemonic Password Manager", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        NumberPicker _wordLengthPicker;
        Spinner _bookSpinner;
        string _textMaterial;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //Assigns delegate method to button click.
            Button generateWordButton = FindViewById<Button>(Resource.Id.generateButton); 
            generateWordButton.Click += OnGenerateWordButtonClick;

            Button copyToClipBoard = FindViewById<Button>(Resource.Id.copyButton);
            copyToClipBoard.Click += OnCopyToClip;                

            //Add string-array with book titles to spinner.
            _bookSpinner = FindViewById<Spinner>(Resource.Id.bookSpinner);
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this,
            Resource.Array.BookChoices, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
            _bookSpinner.Adapter = adapter;
            _bookSpinner.ItemSelected += InitializeTextFile;
           
            _wordLengthPicker = FindViewById<NumberPicker>(Resource.Id.numberPicker);
            _wordLengthPicker.MaxValue = 80;
            _wordLengthPicker.MinValue = 8;
            NumberPicker numbers = FindViewById<NumberPicker>(Resource.Id.numberPickerNumbers);
            numbers.MaxValue = _wordLengthPicker.Value;
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
            _wordLengthPicker.ValueChanged += delegate
            {
                numbers.MaxValue = _wordLengthPicker.Value;
            };
        }

        protected void OnGenerateWordButtonClick(object sender, EventArgs e)
        {
            EditText randomWordText = FindViewById<EditText>(Resource.Id.passwordText);
            EditText mnemonicSentence = FindViewById<EditText>(Resource.Id.mnemonicText);
            RandomWord randomWord = new RandomWord(NumberOfLetters(), _textMaterial, UseNumbersInPassword(),UseNumbersInPassword() ? AmountOfNumbers() : 0 );
            randomWordText.Text = randomWord.NewWord;
            mnemonicSentence.Text = randomWord.NewSentence;
        }
        protected void OnCopyToClip(object sender, EventArgs e)
        {
            EditText randomWordText = FindViewById<EditText>(Resource.Id.passwordText);
            var clipboard = (ClipboardManager)GetSystemService(ClipboardService);
            var clip = ClipData.NewPlainText("Password", randomWordText.Text);
            clipboard.PrimaryClip = clip;
        }
        protected int NumberOfLetters()
        {
            return _wordLengthPicker.Value;
        }
        protected bool UseNumbersInPassword()
        {
            Switch numberSwitch = FindViewById<Switch>(Resource.Id.numberSwitch);
            return numberSwitch.Checked ? true : false;
        }
        protected int AmountOfNumbers()
        {
            NumberPicker numbers = FindViewById<NumberPicker>(Resource.Id.numberPickerNumbers);
            return numbers.Value;
        }
        protected void InitializeTextFile(object sender, EventArgs e)
        {
            AssetManager assets = this.Assets;
            if (_bookSpinner.SelectedItem.Equals("The Iliad by Homer (Available offline)"))
                using (StreamReader sr = new StreamReader(assets.Open("Iliad.txt")))
                {
                    _textMaterial = sr.ReadToEnd();
                }
            else if (_bookSpinner.SelectedItem.Equals("Crime and punishment by Fyodor Dostoyevsky (Available offline)"))
                using (StreamReader sr = new StreamReader(assets.Open("CrimeAndPunishment.txt")))
                {
                    _textMaterial = sr.ReadToEnd();
                }
            else
                DownloadAsync(_bookSpinner.SelectedItem.ToString());
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
                        case "Siddhartha by Hermann Hesse (Online resource)":
                            getString = client.DownloadStringTaskAsync("http://www.gutenberg.org/cache/epub/2500/pg2500.txt");
                            _textMaterial = await getString;
                            break;
                        case "The Trial by Franz Kafka (Online resource)":
                            getString = client.DownloadStringTaskAsync("http://www.gutenberg.org/cache/epub/7849/pg7849.txt");
                            _textMaterial = await getString;
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
    }
}

