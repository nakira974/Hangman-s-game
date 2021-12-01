using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GameLib.rsc
{
    /// <summary>
    /// Classe permettant de selectionner une entité en base grâce à Entity-Framework 6 
    /// </summary>
    public class Word
    {
        /// <summary>
        /// Id en base
        /// </summary>
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        /// <summary>
        /// Mot du dictionnaire
        /// </summary>
        [Required] public string Text { get; init; }

        /// <summary>
        /// Enum sur la langue du mot (au cas où...)
        /// </summary>
        [Required] public Language Language { get; init; }

        /// <summary>
        /// Selectionne un mot aléatoirement en base
        /// </summary>
        /// <returns></returns>
        public static async Task<Word> SelectRandomWord()
        {
            Word result = null;

            try
            {
                await using (ProgramDbContext context = new ProgramDbContext())
                {
                    Random random = new Random();
                    int index = random.Next(await context.Words.CountAsync());
                    List<Word> words = await context.Words.ToListAsync();
                    result = words[index];
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


    /// <summary>
    /// Enum sur la langue du mot choisi
    /// </summary>
    public enum Language
    {
        EN_US,
        FR
    }
}