﻿namespace WordGame
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public class GameViewModel : IGamePage, INotifyPropertyChanged
    {
        private string a1BoxText;
        private string a2BoxText;
        private string a3BoxText;
        private string a4BoxText;
        private string a5BoxText;

        private string b1BoxText;
        private string b2BoxText;
        private string b3BoxText;
        private string b4BoxText;
        private string b5BoxText;

        private string c1BoxText;
        private string c2BoxText;
        private string c3BoxText;
        private string c4BoxText;
        private string c5BoxText;

        private string d1BoxText;
        private string d2BoxText;
        private string d3BoxText;
        private string d4BoxText;
        private string d5BoxText;

        private string e1BoxText;
        private string e2BoxText;
        private string e3BoxText;
        private string e4BoxText;
        private string e5BoxText;

        public event PropertyChangedEventHandler PropertyChanged;

        private (int xCoord, int yCoord) EarliestStartingLetterCoords;
        private readonly Dictionary<(int xcoord, int ycoord), char> LettersEntered;

        public GameViewModel()
        {
            this.a1BoxText = string.Empty;
            this.a2BoxText = string.Empty;
            this.a3BoxText = string.Empty;
            this.a4BoxText = string.Empty;
            this.a5BoxText = string.Empty;

            this.b1BoxText = string.Empty;
            this.b2BoxText = string.Empty;
            this.b3BoxText = string.Empty;
            this.b4BoxText = string.Empty;
            this.b5BoxText = string.Empty;

            this.c1BoxText = string.Empty;
            this.c2BoxText = string.Empty;
            this.c3BoxText = string.Empty;
            this.c4BoxText = string.Empty;
            this.c5BoxText = string.Empty;

            this.d1BoxText = string.Empty;
            this.d2BoxText = string.Empty;
            this.d3BoxText = string.Empty;
            this.d4BoxText = string.Empty;
            this.d5BoxText = string.Empty;

            this.e1BoxText = string.Empty;
            this.e2BoxText = string.Empty;
            this.e3BoxText = string.Empty;
            this.e4BoxText = string.Empty;
            this.e5BoxText = string.Empty;

            //string[,] grid = new string[5,5];

            //grid[0, 0] = this.a1BoxText;
            //grid[1, 0] = this.a2BoxText;
            //grid[2, 0] = this.a3BoxText;
            //grid[3, 0] = this.a4BoxText;
            //grid[4, 0] = this.a5BoxText;

            //grid[0, 1] = this.b1BoxText;
            //grid[1, 1] = this.b2BoxText;
            //grid[2, 1] = this.b3BoxText;
            //grid[3, 1] = this.b4BoxText;
            //grid[4, 1] = this.b5BoxText;

            //grid[0, 2] = this.c1BoxText;
            //grid[1, 2] = this.c2BoxText;
            //grid[2, 2] = this.c3BoxText;
            //grid[3, 2] = this.c4BoxText;
            //grid[4, 2] = this.c5BoxText;

            //grid[0, 3] = this.d1BoxText;
            //grid[1, 3] = this.d2BoxText;
            //grid[2, 3] = this.d3BoxText;
            //grid[3, 3] = this.d4BoxText;
            //grid[4, 3] = this.d5BoxText;

            //grid[0, 4] = this.e1BoxText;
            //grid[1, 4] = this.e2BoxText;
            //grid[2, 4] = this.e3BoxText;
            //grid[3, 4] = this.e4BoxText;
            //grid[4, 4] = this.e5BoxText;

            // TODO: When a letter is set, check it's coords against this and set it if it's closer to the top left of the grid.
            // This might be hard because if the player deletes the starting letter, will need to work out from the reamining entered letters which is now the start...
            this.EarliestStartingLetterCoords = (0, 0);

            this.LettersEntered = new Dictionary<(int xcoord, int ycoord), char>();

            this.OnGoButtonClicked = new DelegateCommand<object>(this.ValidateWord);
        }

        public ICommand OnGoButtonClicked { get; }

        private bool wordIsValid;

        public bool WordIsValid
        {
            get => this.wordIsValid;
            set
            {
                this.wordIsValid = value;
                this.OnPropertyChanged(nameof(this.WordIsValid));
            }
        }

        public void ValidateWord(object obj)
        {
            if (this.LettersEntered.Count <= 1)
            {
                this.WordIsValid = false;
                ShowInvalidWordMessage();
                return;
            }

            (int xcoord, int ycoord) firstLetterCoords = this.GetFirstLetterCoords();

            bool wordIsHorizontal = false;
            bool wordIsVertical = false;

            foreach (KeyValuePair<(int xcoord, int ycoord), char> letter in this.LettersEntered)
            {
                if (letter.Key.xcoord != firstLetterCoords.xcoord
                    && letter.Key.ycoord != firstLetterCoords.ycoord)
                {
                    // Found a letter that is not in the same row/column as the first letter.
                    this.WordIsValid = false;
                    ShowInvalidWordMessage();
                    return;
                }

                if (letter.Key.ycoord == firstLetterCoords.ycoord
                    && letter.Key.xcoord != firstLetterCoords.xcoord)
                {
                    wordIsHorizontal = true;
                }

                if (letter.Key.xcoord == firstLetterCoords.xcoord
                    && letter.Key.ycoord != firstLetterCoords.ycoord)
                {
                    wordIsVertical = true;
                }
            }

            if (wordIsHorizontal && wordIsVertical)
            {
                // Word must be either horizontal or vertical.
                this.WordIsValid = false;
                ShowInvalidWordMessage();
                return;
            }

            string word = string.Empty;

            if (wordIsHorizontal)
            {
                List<int> xcoords = this.LettersEntered.Keys.Select(k => k.xcoord).ToList();
                xcoords.Sort();

                int lengthOfWord = xcoords.Count;

                int expectedLastXCoord = xcoords[0] + lengthOfWord - 1;

                if (xcoords.Last() != expectedLastXCoord)
                {
                    // If these don't match, there must be a space in the word somewhere.
                    this.WordIsValid = false;
                    ShowInvalidWordMessage();
                    return;
                }

                foreach (int letterXCoord in xcoords)
                {
                    (int letterXCoord, int ycoord) coord = (letterXCoord, firstLetterCoords.ycoord);
                    char letter = this.LettersEntered[coord];

                    word += letter;
                }
            }
            else
            {
                List<int> ycoords = this.LettersEntered.Keys.Select(k => k.ycoord).ToList();
                ycoords.Sort();

                int lengthOfWord = ycoords.Count;

                int expectedLastYCoord = ycoords[0] + lengthOfWord - 1;

                if (ycoords.Last() != expectedLastYCoord)
                {
                    // If these don't match, there must be a space in the word somewhere.
                    this.WordIsValid = false;
                    ShowInvalidWordMessage();
                    return;
                }

                foreach (int letterYCoord in ycoords)
                {
                    (int xcoord, int letterYCoord) coord = (firstLetterCoords.xcoord, letterYCoord);
                    char letter = this.LettersEntered[coord];

                    word += letter;
                }
            }

            // If here, word is in a single line, we know if it's vertrical or horizontal, and we know there are no spaces in the word.
            // Next, check word in dictionary.
            this.WordIsValid = this.WordIsInDictionary(word);

            if (!this.WordIsValid)
            {
                ShowInvalidWordMessage();
                return;
            }
        }

        private static void ShowInvalidWordMessage()
        {
            // Note - if you set an icon on this message box, it will do the annoying beep sound when the message box appears.
            MessageBox.Show("The word you entered is not a real word!\nPlease try again.", "Not a real word!", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
        }

        private bool WordIsInDictionary(string word)
        {
            // TODO: validate word in dictioanry.

            int initialCapacity = 82765;
            int maxEditDistanceDictionary = 2; //maximum edit distance per dictionary precalculation
            SymSpell symSpell = new SymSpell(initialCapacity, maxEditDistanceDictionary);

            //load dictionary
            //string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //string dictionaryPath = baseDirectory + "../../../../SymSpell/frequency_dictionary_en_82_765.txt";
            string dictionaryPath = @"D:\Dev\Scrabbleships\C#\WordGame\WordGame\frequency_dictionary_en_82_765.txt";
            int termIndex = 0; //column of the term in the dictionary text file
            int countIndex = 1; //column of the term frequency in the dictionary text file
            if (!symSpell.LoadDictionary(dictionaryPath, termIndex, countIndex))
            {
                Console.WriteLine("File not found!");
                //press any key to exit program
                Console.ReadKey();
                return false;
            }

            int maxEditDistanceLookup = 1; //max edit distance per lookup (maxEditDistanceLookup<=maxEditDistanceDictionary)
            SymSpell.Verbosity suggestionVerbosity = SymSpell.Verbosity.Top; //Top, Closest, All
            List<SymSpell.SuggestItem> suggestions = symSpell.Lookup(word.ToLower(), suggestionVerbosity, maxEditDistanceLookup);

            //display suggestions, edit distance and term frequency
            //foreach (var suggestion in suggestions)
            //{
            //    Console.WriteLine(suggestion.term + " " + suggestion.distance.ToString() + " " + suggestion.count.ToString("N0"));
            //}

            if (!suggestions.Any())
            {
                return false;
            }

            if (suggestions.First().term.ToUpper() == word.ToUpper())
            {
                return true;
            }

            return false;
        }

        private (int xcoord, int ycoord) GetFirstLetterCoords()
        {
            bool gotAFirstLetter = false;
            (int xcoord, int ycoord) firstLetter = (-1, -1);

            foreach ((int xcoord, int ycoord) coords in this.LettersEntered.Keys)
            {
                if (!gotAFirstLetter)
                {
                    firstLetter = coords;
                    gotAFirstLetter = true;
                }
                else if (coords.xcoord <= firstLetter.xcoord
                    && coords.ycoord <= firstLetter.ycoord)
                {
                    firstLetter = coords;
                }
            }

            return firstLetter;
        }

        private void SetEarliestLetterIfNeeded(int x, int y)
        {
            if (x <= this.EarliestStartingLetterCoords.xCoord && y <= this.EarliestStartingLetterCoords.yCoord)
            {
                this.EarliestStartingLetterCoords = (x, y);
            }
        }

        private void AddValidLetter(int x, int y, char letter)
        {
            (int x, int y) coords = (x, y);
            this.LettersEntered.Add(coords, letter);
        }

        private void RemoveLetter(int x, int y)
        {
            (int x, int y) coords = (x, y);
            this.LettersEntered.Remove(coords);
        }

        public string A1BoxText
        {
            get => this.a1BoxText;
            set
            {
                this.a1BoxText = value;
                this.OnPropertyChanged(nameof(this.A1BoxText));
                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(0, 0);
                    this.AddValidLetter(0, 0, value[0]);
                }
                else
                {
                    this.RemoveLetter(0, 0);
                }
            }
        }

        public string A2BoxText
        {
            get => this.a2BoxText;
            set
            {
                this.a2BoxText = value;
                this.OnPropertyChanged(nameof(this.A2BoxText));
                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(1, 0);
                    this.AddValidLetter(1, 0, value[0]);
                }
                else
                {
                    this.RemoveLetter(1, 0);
                }
            }
        }

        public string A3BoxText
        {
            get => this.a3BoxText;
            set
            {
                this.a3BoxText = value;
                this.OnPropertyChanged(nameof(this.A3BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(2, 0);
                    this.AddValidLetter(2, 0, value[0]);
                }
                else
                {
                    this.RemoveLetter(2, 0);
                }
            }
        }

        public string A4BoxText
        {
            get => this.a4BoxText;
            set
            {
                this.a4BoxText = value;
                this.OnPropertyChanged(nameof(this.A4BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(3, 0);
                    this.AddValidLetter(3, 0, value[0]);
                }
                else
                {
                    this.RemoveLetter(3, 0);
                }
            }
        }

        public string A5BoxText
        {
            get => this.a5BoxText;
            set
            {
                this.a5BoxText = value;
                this.OnPropertyChanged(nameof(this.A5BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(4, 0);
                    this.AddValidLetter(4, 0, value[0]);
                }
                else
                {
                    this.RemoveLetter(4, 0);
                }
            }
        }

        public string B1BoxText
        {
            get => this.b1BoxText;
            set
            {
                this.b1BoxText = value;
                this.OnPropertyChanged(nameof(this.B1BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(0, 1);
                    this.AddValidLetter(0, 1, value[0]);
                }
                else
                {
                    this.RemoveLetter(0, 1);
                }
            }
        }

        public string B2BoxText
        {
            get => this.b2BoxText;
            set
            {
                this.b2BoxText = value;
                this.OnPropertyChanged(nameof(this.B2BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(1, 1);
                    this.AddValidLetter(1, 1, value[0]);
                }
                else
                {
                    this.RemoveLetter(1, 1);
                }
            }
        }

        public string B3BoxText
        {
            get => this.b3BoxText;
            set
            {
                this.b3BoxText = value;
                this.OnPropertyChanged(nameof(this.B3BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(2, 1);
                    this.AddValidLetter(2, 1, value[0]);
                }
                else
                {
                    this.RemoveLetter(2, 1);
                }
            }
        }

        public string B4BoxText
        {
            get => this.b4BoxText;
            set
            {
                this.b4BoxText = value;
                this.OnPropertyChanged(nameof(this.B4BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(3, 1);
                    this.AddValidLetter(3, 1, value[0]);
                }
                else
                {
                    this.RemoveLetter(3, 1);
                }
            }
        }

        public string B5BoxText
        {
            get => this.b5BoxText;
            set
            {
                this.b5BoxText = value;
                this.OnPropertyChanged(nameof(this.B5BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(4, 1);
                    this.AddValidLetter(4, 1, value[0]);
                }
                else
                {
                    this.RemoveLetter(4, 1);
                }
            }
        }

        public string C1BoxText
        {
            get => this.c1BoxText;
            set
            {
                this.c1BoxText = value;
                this.OnPropertyChanged(nameof(this.C1BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(0, 2);
                    this.AddValidLetter(0, 2, value[0]);
                }
                else
                {
                    this.RemoveLetter(0, 2);
                }
            }
        }

        public string C2BoxText
        {
            get => this.c2BoxText;
            set
            {
                this.c2BoxText = value;
                this.OnPropertyChanged(nameof(this.C2BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(1, 2);
                    this.AddValidLetter(1, 2, value[0]);
                }
                else
                {
                    this.RemoveLetter(1, 2);
                }
            }
        }

        public string C3BoxText
        {
            get => this.c3BoxText;
            set
            {
                this.c3BoxText = value;
                this.OnPropertyChanged(nameof(this.C3BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(2, 2);
                    this.AddValidLetter(2, 2, value[0]);
                }
                else
                {
                    this.RemoveLetter(2, 2);
                }
            }
        }

        public string C4BoxText
        {
            get => this.c4BoxText;
            set
            {
                this.c4BoxText = value;
                this.OnPropertyChanged(nameof(this.C4BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(3, 2);
                    this.AddValidLetter(3, 2, value[0]);
                }
                else
                {
                    this.RemoveLetter(3, 2);
                }
            }
        }

        public string C5BoxText
        {
            get => this.c5BoxText;
            set
            {
                this.c5BoxText = value;
                this.OnPropertyChanged(nameof(this.C5BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(4, 2);
                    this.AddValidLetter(4, 2, value[0]);
                }
                else
                {
                    this.RemoveLetter(4, 2);
                }
            }
        }

        public string D1BoxText
        {
            get => this.d1BoxText;
            set
            {
                this.d1BoxText = value;
                this.OnPropertyChanged(nameof(this.D1BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(0, 3);
                    this.AddValidLetter(0, 3, value[0]);
                }
                else
                {
                    this.RemoveLetter(0, 3);
                }
            }
        }

        public string D2BoxText
        {
            get => this.d2BoxText;
            set
            {
                this.d2BoxText = value;
                this.OnPropertyChanged(nameof(this.D2BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(1, 3);
                    this.AddValidLetter(1, 3, value[0]);
                }
                else
                {
                    this.RemoveLetter(1, 3);
                }
            }
        }

        public string D3BoxText
        {
            get => this.d3BoxText;
            set
            {
                this.d3BoxText = value;
                this.OnPropertyChanged(nameof(this.D3BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(2, 3);
                    this.AddValidLetter(2, 3, value[0]);
                }
                else
                {
                    this.RemoveLetter(2, 3);
                }
            }
        }

        public string D4BoxText
        {
            get => this.d4BoxText;
            set
            {
                this.d4BoxText = value;
                this.OnPropertyChanged(nameof(this.D4BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(3, 3);
                    this.AddValidLetter(3, 3, value[0]);
                }
                else
                {
                    this.RemoveLetter(3, 3);
                }
            }
        }

        public string D5BoxText
        {
            get => this.d5BoxText;
            set
            {
                this.d5BoxText = value;
                this.OnPropertyChanged(nameof(this.D5BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(4, 3);
                    this.AddValidLetter(4, 3, value[0]);
                }
                else
                {
                    this.RemoveLetter(4, 3);
                }
            }
        }

        public string E1BoxText
        {
            get => this.e1BoxText;
            set
            {
                this.e1BoxText = value;
                this.OnPropertyChanged(nameof(this.E1BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(0, 4);
                    this.AddValidLetter(0, 4, value[0]);
                }
                else
                {
                    this.RemoveLetter(0, 4);
                }
            }
        }

        public string E2BoxText
        {
            get => this.e2BoxText;
            set
            {
                this.e2BoxText = value;
                this.OnPropertyChanged(nameof(this.E2BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(1, 4);
                    this.AddValidLetter(1, 4, value[0]);
                }
                else
                {
                    this.RemoveLetter(1, 4);
                }
            }
        }

        public string E3BoxText
        {
            get => this.e3BoxText;
            set
            {
                this.e3BoxText = value;
                this.OnPropertyChanged(nameof(this.E3BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(2, 4);
                    this.AddValidLetter(2, 4, value[0]);
                }
                else
                {
                    this.RemoveLetter(2, 4);
                }
            }
        }

        public string E4BoxText
        {
            get => this.e4BoxText;
            set
            {
                this.e4BoxText = value;
                this.OnPropertyChanged(nameof(this.E4BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(3, 4);
                    this.AddValidLetter(3, 4, value[0]);
                }
                else
                {
                    this.RemoveLetter(3, 4);
                }
            }
        }

        public string E5BoxText
        {
            get => this.e5BoxText;
            set
            {
                this.e5BoxText = value;
                this.OnPropertyChanged(nameof(this.E5BoxText));

                if (value != string.Empty)
                {
                    this.SetEarliestLetterIfNeeded(4, 4);
                    this.AddValidLetter(4, 4, value[0]);
                }
                else
                {
                    this.RemoveLetter(4, 4);
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
