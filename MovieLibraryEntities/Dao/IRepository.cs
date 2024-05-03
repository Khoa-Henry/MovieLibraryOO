using MovieLibraryEntities.Models;

namespace MovieLibraryEntities.Dao
{
    public interface IRepository
    {
        IEnumerable<Movie> GetAll();
        Movie Search(string searchString);
        Movie AddMovie(string addString, DateTime date, string[] inputedGenres);
        User AddUser(string fName, string lName, int age, string gender, string street, string city, string state, string zCode, string occupationName);
    }
}
