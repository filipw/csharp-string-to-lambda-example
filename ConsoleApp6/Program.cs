using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConsoleApp6
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Roslyn approach:");
            var discountFilter = "album => album.Quantity > 100";
            var options = ScriptOptions.Default.AddReferences(typeof(Album).Assembly);
            Func<Album, bool> discountFilterExpression = await CSharpScript.EvaluateAsync<Func<Album, bool>>(discountFilter, options);

            var albums = new List<Album>
            {
                new Album { Quantity = 10, Artist = "Betontod", Title = "Revolution" },
                new Album { Quantity = 50, Artist = "The Dangerous Summer", Title = "The Dangerous Summer" },
                new Album { Quantity = 200, Artist = "Depeche Mode", Title = "Spirit" },
            };

            var discountedAlbums = albums.Where(discountFilterExpression);

            foreach(var album in discountedAlbums)
            {
                Console.WriteLine($"Discounted album: {album.Artist} - {album.Title}");
            }

            Console.WriteLine();
            Console.WriteLine("****");
            Console.WriteLine();

            // approach with manual expression building
            Console.WriteLine("Manual expression building approach:");
            var parameter = Expression.Parameter(typeof(Album), "album");
            var comparison = Expression.GreaterThan(Expression.Property(parameter, Type.GetType("ConsoleApp6.Album").GetProperty("Quantity")), Expression.Constant(100));
            var lambda = Expression.Lambda<Func<Album, bool>>(comparison, parameter).Compile();
            var discountedAlbums2 = albums.Where(lambda);

            foreach (var album in discountedAlbums2)
            {
                Console.WriteLine($"Discounted album: {album.Artist} - {album.Title}");
            }

            Console.ReadKey();
        }
    }

    public class Album
    {
        public int Quantity { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
    }
}
