using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
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

        public async Task<Word> SelectRandomWord()
        {
            await using (ProgramDbContext context = new ProgramDbContext())
            {
                try
                {
                    Random random = new Random();
                    int index = random.Next(await context.Words.CountAsync());
                    List<Word> words = await context.Words.ToListAsync();
                    Word currentWord = words[index];

                    return currentWord;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }


    public enum Language
    {
        EN_US,
        FR
    }
}