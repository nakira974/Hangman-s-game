using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GameLib.rsc;

namespace GameLib
{
    /// <summary>
    /// Classe comportant les règles du jeu du pendu avec sélection d'un mot en base et hashage de ce dernier
    /// </summary>
    public class HangmanGame : GameRules<HangmanGame>
    {
        /// <summary>
        /// Derniers char decouvert
        /// </summary>
        public List<char>LastCharDiscovered { get; set; }
        /// <summary>
        /// Mort caché selectionner en base
        /// </summary>
        private Word HiddenWord { get; set; }
        /// <summary>
        /// Mot hashé en stringbuilder
        /// </summary>
        public StringBuilder HashedWord { get; set; }

        public HangmanGame()
        {
            LastCharDiscovered = new List<char>(); 
            NewGame();
        }

        /// <summary>
        /// Hash le mot caché
        /// </summary>
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

        /// <summary>
        /// Essaie d'ajouter un char au mot caché si c'est bon retourne true
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public async Task<bool> TryAppendCharacter(char character)
        {
            bool result = false;
            try
            {
                if (HiddenWord.Text.Contains(character) && !LastCharDiscovered.Contains(character))
                {
                    LastCharDiscovered.Add(character);
                    result = true;
                    for (int i = 0; i <= HiddenWord.Text.Length - 1; i++)
                    {
                        if (character == HiddenWord.Text[i])
                            HashedWord[i] = character;
                        IsGameWon = HashedWord.ToString() == HiddenWord.Text;
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

        /// <summary>
        /// Lance une nouvelle partie et selectionne un mot.
        /// </summary>
        public void NewGame()
        {
            IsGameWon = false;
            LastCharDiscovered = new List<char>();
            HiddenWord = Word.SelectRandomWord().Result;
            WriteFile(HiddenWord.Text);
            HashedWord = new StringBuilder(HiddenWord.Text);
            HashWord();
        }

        /// <summary>
        /// Retourne le mot caché en entier.
        /// </summary>
        /// <returns></returns>
        public string GetHiddenWord()
        {
            return HiddenWord.Text;
        }
        
        private static Task WriteFile(string content)
        {
            try
            {
                using (StreamWriter outputWriter = File.AppendText(@"C:\temp\debugWord.txt"))
                {
                    string? tempLineValue;
                
                    outputWriter.WriteLine(content);


                    outputWriter.Dispose();
                }
            
                Console.Write($"{content} has been added to C:\\temp\\debugWord \n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Task.CompletedTask;
        }
    }
}