namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            var dbContext = new BookShopContext();
            //DbInitializer.ResetDatabase(dbContext);
            //var input = Console.ReadLine();

            var result = GetBooksByCategory(dbContext, "hoRRor mystery drama");

            Console.WriteLine(result);
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
