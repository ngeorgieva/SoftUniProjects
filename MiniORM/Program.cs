namespace MiniORM
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Entities;

    public class Program
    {
        public static void Main()
        {
            string connectionString = new ConnectionStringBuilder("MyWebSiteDatabase").ConnectionString;
            IDbContext context = new EntityManager(connectionString, true);

            // Test Framework

            #region //Task 11: Fetch Users

            //User user = new User("Gosho", "asd", 28, DateTime.Now);
            //context.Persist(user);
            //User stephen = new User("stefcho", "dragon123", 15, DateTime.Now);
            //context.Persist(stephen);
            //
            //stephen.Password = "newDragon123";
            //stephen.Username = "Stefchy";
            //context.Persist(stephen);
            //
            //User steffy = context.FindById<User>(1);
            //Console.WriteLine(steffy.Username);

            //User ivan = new User("ivancho", "pass123", 18, DateTime.Now);
            //context.Persist(ivan);
            //
            //User parvan = new User("parvancho", "pass1234", 18, DateTime.Now);
            //context.Persist(ivan);
            //context.Persist(parvan);

            //IEnumerable<User> rusersRegAfter2010Aged18 = context.FindAll<User>("YEAR(RegistrationDate) > 2010 AND [Age] /  = /18");
            //
            //foreach (User u in rusersRegAfter2010Aged18)
            //{
            //    Console.WriteLine(u.Username);
            //}

            #endregion

            #region//Task 12: Add New Entity

            //List<Book> books = new List<Book>()
            //{
            //    new Book("Harry Potter and the Cursed Child - Parts I & II", "J.K. Rowling , Jack Thorne , John Tiffany", new DateTime(2015, 10, 2), "English", true, 1),
            //    new Book("Merchant of Venice", "Shakespeare W.", new DateTime(2013, 11, 3), "English", false, 2.3m),
            //    new Book("Short Stories from the Nineteenth Century", "Davies D.S.(Ed.)", new DateTime(2011, 12, 4), "English", false, 4),
            //    new Book("The Horror in the Museum: Collected Short Stories Vol.2", "Lovecraft H.P.", new DateTime(2004, 1, 5), "English", true, 6.4m),
            //    new Book("Twenty Thousand Leagues Under the Sea", "Verne J.", new DateTime(2042, 7, 6), "English", false, 7),                          
            //    new Book("Mansfield Park", "Austen J.", new DateTime(2003, 6, 7), "English", true, 9.2m),
            //    new Book("Adventures & Memoirs of Sherlock Holmes", "Doyle A.C.", new DateTime(2023, 2, 8), "English", true, 10),
            //    new Book("Lord Jim", "Conrad J.", new DateTime(2052, 4, 9), "English", false, 8.2m),
            //    new Book("Three Musketeers", "Dumas A.", new DateTime(2012, 1, 30), "English", true, 5.2m),
            //    new Book("Tale of Two Cities", "Dickens C.", new DateTime(2005, 5, 21), "English", false, 2.1m),
            //};
            //
            //foreach (var book in books)
            //{
            //    context.Persist(book);
            //}
            //
            //int titleLength = int.Parse(Console.ReadLine());
            //var wantedBooks = context.FindAll<Book>($"LEN(Title) >= {titleLength} AND IsHardCovered = 1");
            //int numberOfBooksWithNTitleLength = 0;
            //foreach (Book book in wantedBooks)
            //{
            //    book.Title = book.Title.Substring(0, titleLength);
            //    context.Persist(book);
            //    numberOfBooksWithNTitleLength++;
            //}
            //Console.WriteLine(numberOfBooksWithNTitleLength);

            #endregion

            #region//Task 13: Update Entity

            //List<Book> books = new List<Book>()
            //{
            //    new Book("Harry Potter and the Cursed Child - Parts I & II", "J.K. Rowling , Jack Thorne , John Tiffany", new DateTime(2015, 10, 2), "English", true, 1),
            //    new Book("Merchant of Venice", "Shakespeare W.", new DateTime(2013, 11, 3), "English", false, 2.3m),
            //    new Book("Short Stories from the Nineteenth Century", "Davies D.S.(Ed.)", new DateTime(2011, 12, 4), "English", false, 4),
            //    new Book("The Horror in the Museum: Collected Short Stories Vol.2", "Lovecraft H.P.", new DateTime(2004, 1, 5), "English", true, 6.4m),
            //    new Book("Twenty Thousand Leagues Under the Sea", "Verne J.", new DateTime(2042, 7, 6), "English", false, 7),                          
            //    new Book("Mansfield Park", "Austen J.", new DateTime(2003, 6, 7), "English", true, 9.2m),
            //    new Book("Adventures & Memoirs of Sherlock Holmes", "Doyle A.C.", new DateTime(2023, 2, 8), "English", true, 10),
            //    new Book("Lord Jim", "Conrad J.", new DateTime(2052, 4, 9), "English", false, 8.2m),
            //    new Book("Three Musketeers", "Dumas A.", new DateTime(2012, 1, 30), "English", true, 5.2m),
            //    new Book("Tale of Two Cities", "Dickens C.", new DateTime(2005, 5, 21), "English", false, 2.1m),
            //};
            //
            //foreach (Book book in books)
            //{
            //    context.Persist(book);
            //}
            //
            //var wantedBooks = context.FindAll<Book>();
            //var topRatingBooks = wantedBooks.OrderByDescending(b => b.Rating).ThenBy(b => b.Title).Take(3);
            //foreach (var book in topRatingBooks)
            //{
            //    Console.WriteLine($"{book.Title} ({book.Author}) - {book.Rating:f1}/10");
            //}

            #endregion

            #region//Task 14: Update Records
            //int year = int.Parse(Console.ReadLine());
            //var booksToBeChanged = context.FindAll<Book>($"YEAR(PublishedOn) > {year} AND IsHardCovered = 1");
            //int count = 0;
            // 
            //foreach (var book in booksToBeChanged)
            //{
            //    book.Title = book.Title.ToUpper(CultureInfo.InvariantCulture);
            //    context.Persist(book);
            //    count++;
            //}
            //
            //Console.WriteLine($"Books released after 2000 year: {booksToBeChanged.Count()}");
            //foreach (var book in booksToBeChanged.OrderBy(b => b.Title))
            //{
            //    Console.WriteLine(book.Title);
            //}
            #endregion

            #region//Task 15: Delete Records
            //var lowRatingBooks = context.FindAll<Book>("Rating < 2");
            //int deletedBooksCount = 0;
            //
            //foreach (var book in lowRatingBooks)
            //{
            //    context.Delete<Book>(book);
            //    deletedBooksCount++;
            //}
            //
            //Console.WriteLine($"{deletedBooksCount} books have been deleted from the database");
            #endregion
        }
    }
}
