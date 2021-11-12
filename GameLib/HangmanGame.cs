using System;
using System.Text;
using System.Threading.Tasks;
using GameLib.rsc;

namespace GameLib
{
    public class HangmanGame : GameRules<HangmanGame>
    {
        private char _lastCharDiscovered { get; set; }
        private Word _hiddenWord { get; set; }
        public StringBuilder HashedWord { get; set; }

        public HangmanGame()
        {
            NewGame();
        }

        private void HashWord()
        {
            try
            {
                for (int i = 1; i < HashedWord.Length - 1; i++)
                {
                    HashedWord[i] = '\u005F';
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<bool> TryAppendCharacter(char character)
        {
            bool result = false;
            try
            {
                if (_hiddenWord.Text.Contains(character) && character != _lastCharDiscovered)
                {
                    _lastCharDiscovered = character;
                    result = true;
                    for (int i = 0; i <= _hiddenWord.Text.Length - 1; i++)
                    {
                        if (character == _hiddenWord.Text[i])
                            HashedWord[i] = character;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result;
        }

        public void NewGame()
        {
            _lastCharDiscovered = char.MinValue;
            _hiddenWord = Word.SelectRandomWord().Result;
            HashedWord = new StringBuilder(_hiddenWord.Text);
            HashWord();
        }
    }
}