using Microsoft.EntityFrameworkCore;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Models;

namespace MovieLibraryEntities.Dao
{
    public class Repository : IRepository, IDisposable
    {
        private readonly IDbContextFactory<MovieContext> _contextFactory;
        private readonly MovieContext _context;

        public Repository(MovieContext dbContext)
        {
            _context = dbContext;
        }
        //public Repository(IDbContextFactory<MovieContext> contextFactory)
        //{
        //    _contextFactory = contextFactory;
        //    _context = _contextFactory.CreateDbContext();
        //}

        public void Dispose()
        {
            _context.Dispose();
        }

        public IEnumerable<Movie> GetAll()
        {
            return _context.Movies.ToList();
        }

        public IEnumerable<Movie> Search(string searchString)
        {
            var allMovies = _context.Movies;
            var listOfMovies = allMovies.ToList();
            var temp = listOfMovies.Where(x => x.Title.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));

            return temp;
        }

        public IEnumerable<Movie> AddMovie(string addString, DateTime date)
        {
            var newMovie = new Movie();
            newMovie.Title = addString;
            newMovie.ReleaseDate = date;
            _context.Movies.Add(newMovie);
            _context.SaveChanges();
            return Search(addString);
        }
    }
}
