namespace P01_StudentSystem
{
    using P01_StudentSystem.Data;
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            var db = new StudentSystemContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

        }
    }
}
