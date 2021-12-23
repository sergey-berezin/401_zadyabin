using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace ThirdTask.Database
{
    class ImageContext : DbContext
    {
        public DbSet<AnalyzedImage> AnalyzedImages { get; set; }

        public string DbPath { get; }

        public ImageContext()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            DbPath = Path.Join(projectDirectory, "image.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder o)
            => o.UseLazyLoadingProxies().UseSqlite($"Data Source={DbPath}");
    }
}
