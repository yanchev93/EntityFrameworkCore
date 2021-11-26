namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            var dbContext = new BookShopContext();
            //DbInitializer.ResetDatabase(dbContext);
            //var input = Console.ReadLine();

            var result = CountCopiesByAuthor(dbContext);

            Console.WriteLine(result);
        }



        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var bookCopies = context.Authors
                .Select(x => new
                {
                    bookCopies = x.Books.Sum(x => x.Copies),
                    name = x.FirstName + " " + x.LastName
                })
                .OrderByDescending(x => x.bookCopies)
                .ToArray();

            return string.Join(Environment.NewLine, bookCopies.Select(x => $"{x.name} - {x.bookCopies}"));
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                .Where(x => x.Title.Length > lengthCheck)
                .Select(x => x.Title)
                .ToArray();

            return books.Count();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var authors = context.Books
                .Include(x => x.Author)
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(x => x.BookId)
                .Select(x => new
                {
                    x.Title,
                    authorName = x.Author.FirstName + " " + x.Author.LastName
                })
                .ToArray();

            var output = string.Join(Environment.NewLine, authors.Select(x => $"{x.Title} ({x.authorName})"));

            //.Authors
            //    .Include(x => x.Books)
            //    .Where(x => x.LastName.ToLower().StartsWith(input.ToLower()))
            //    .Select(x => new
            //    {
            //        Author = x.Books
            //                .Select(x => x.Author),
            //        Books = x.Books
            //                .Where(x => x.BookId == x.AuthorId)
            //                .Select(x => x.BookId),
            //        BookId = x.Books.Select(x => x.BookId)
            //    }).OrderBy(x => x.BookId);

            return output;
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .ToArray()
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(x => x.Title)
                .Select(x => new { x.Title })
                .ToArray();

            return string.Join(Environment.NewLine, books.Select(x => x.Title));
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(x => x.FirstName.ToLower().EndsWith(input.ToLower()))
                .Select(x => new
                {
                    FullName = x.FirstName + " " + x.LastName
                })
                .ToArray()
                .OrderBy(x => x.FullName);

            var output = string.Join(Environment.NewLine, authors.Select(x => x.FullName));

            return output;
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .OrderByDescending(x => x.ReleaseDate)
                .ToArray();

            var output = string.Join(Environment.NewLine, books.Select(x => $"{x.Title} - {x.EditionType} - ${x.Price:F2}"));

            return output;
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();
            var books = context.Books
                .Where(x => x.BookCategories
                        .Any(b => categories.Contains(b.Category.Name.ToLower())))
                .OrderBy(x => x.Title)
                .Select(x => x.Title)
                .ToArray();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToArray();

            var output = string.Join(Environment.NewLine, books);

            return output;
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.Price > 40)
                .OrderByDescending(x => x.Price)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                }).ToArray();

            var output = string.Join(Environment.NewLine, books.Select(x => $"{x.Title} - ${x.Price:F2}"));

            return output;
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenbooks = context.Books
                                .Where(x => x.Copies < 5000 && x.EditionType == EditionType.Gold)
                                .OrderBy(b => b.BookId)
                                .Select(x => x.Title)
                                .ToArray();

            var output = string.Join(Environment.NewLine, goldenbooks);

            return output;
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            var titles = context.Books
                         .Where(x => x.AgeRestriction == ageRestriction)
                         .Select(x => x.Title)
                         .OrderBy(title => title)
                         .ToArray();

            var output = string.Join(Environment.NewLine, titles);

            return output;
        }
    }
}
