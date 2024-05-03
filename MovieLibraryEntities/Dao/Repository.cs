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

        public Movie Search(string searchString)
        {
            var matchingMovies = _context.Movies.Include(m => m.MovieGenres)
                        .ThenInclude(mg => mg.Genre).ToList().Where(m => m.Title.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));

            var movie = matchingMovies.FirstOrDefault();
            return movie;
        }

        public Movie AddMovie(string addString, DateTime date, string[] inputedGenres)
        {
            // Create movie object
            var newMovie = new Movie
            {
                Title = addString,
                ReleaseDate = date
            };

            // Lookup or create genres and associate with the movie
            foreach (var genreName in inputedGenres)
            {
                var genre = _context.Genres.FirstOrDefault(g => g.Name.ToLower() == genreName.Trim().ToLower());
                if (genre == null)
                {
                    genre = new Genre { Name = genreName.Trim() };
                    _context.Genres.Add(genre);
                }
                _context.MovieGenres.Add(new MovieGenre { Genre = genre, Movie = newMovie });
            }


            _context.Movies.Add(newMovie);
            _context.SaveChanges();
            return Search(addString);
        }

        public User AddUser(string fName, string lName, int age, string gender, string street, string city, string state, string zCode, string occupationName)
        {
            //instead of new user, use new userDetail
            var newUser = new User
            {
                Age = age,
                Gender = gender,
                ZipCode = zCode,
                UserDetail = new UserDetail
                {
                    FirstName = fName,
                    LastName = lName,
                    StreetAddress = street,
                    City = city,
                    State = state
                }
            };

            // Lookup or create occupation
            var occupation = _context.Occupations.FirstOrDefault(o => o.Name.ToLower() == occupationName.Trim().ToLower());
            if (occupation == null)
            {
                occupation = new Occupation { Name = occupationName };
                _context.Occupations.Add(occupation);
            }
            newUser.Occupation = occupation;

            // Add user to database
            _context.Users.Add(newUser);
            Console.WriteLine(newUser.Id);
            Console.WriteLine(newUser.Occupation.Name);
            Console.WriteLine(newUser.UserDetail.FirstName);
            _context.SaveChanges();
            return newUser;
        }
    }
}
