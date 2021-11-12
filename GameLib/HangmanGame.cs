using System;
using System.Text;
using System.Threading.Tasks;
using GameLib.rsc;

namespace GameLib
{
    public class HangmanGame : GameRules<HangmanGame>
    {
        private Word _hiddenWord { get; init; }
        public StringBuilder HashedWord { get; set; }

        public HangmanGame()
        {
            _hiddenWord = Word.SelectRandomWord().Result;
            HashedWord = new StringBuilder(_hiddenWord.Text);
            HashWord();
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
                if (_hiddenWord.Text.Contains(character))
                {
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
    }
}