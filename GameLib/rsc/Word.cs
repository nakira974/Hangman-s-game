using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GameLib.rsc
{
    public class Word
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        [Required] public string Text { get; init; }

        [Required] public Language Language { get; init; }

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


    public enum Language
    {
        EN_US,
        FR
    }
}