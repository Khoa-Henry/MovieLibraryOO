using ConsoleTables;
using Microsoft.Extensions.Logging;
using MovieLibraryEntities.Dao;
using MovieLibraryOO.Dao;
using MovieLibraryOO.Dto;
using MovieLibraryOO.Mappers;
using MovieLibraryEntities.Models;
using System;
using System.Linq;

namespace MovieLibraryOO.Services
{
    public class MainService : IMainService
    {
        private readonly ILogger<MainService> _logger;
        private readonly IMovieMapper _movieMapper;
        private readonly IRepository _repository;
        private readonly IFileService _fileService;

        public MainService(ILogger<MainService> logger, IMovieMapper movieMapper, IRepository repository, IFileService fileService)
        {
            _logger = logger;
            _movieMapper = movieMapper;
            _repository = repository;
            _fileService = fileService;
        }

        public void Invoke()
        {
            var menu = new Menu();

            Menu.MenuOptions menuChoice;
            do
            {
                menuChoice = menu.ChooseAction();

                switch (menuChoice)
                {
                    case Menu.MenuOptions.ListFromDb:
                        _logger.LogInformation("Listing movies from database");
                        var allMovies = _repository.GetAll();
                        var movies = _movieMapper.Map(allMovies);
                        ConsoleTable.From<MovieDto>(movies).Write();
                        break;
                    case Menu.MenuOptions.AddMovie:
                        _logger.LogInformation("Adding a new movie");
                        // ask for movie name and date or use today's date???
                        var name = menu.GetUserResponse("Enter your", "movie title:", "green");
                        var date = menu.GetUserResponse("Enter the", "release date (MM/dd/yyyy):", "green");
                        if (!DateTime.TryParse(date, out DateTime releaseDate))
                        {
                            _logger.LogInformation("Invalid date format. Aborting.");
                            return;
                        }
                        var fullDate = DateTime.Parse(date);
                        var genres = menu.GetUserResponse("Enter the", "genres (comma-separated):", "green");
                        string[] genreNames = genres.Trim().Split(',');

                        // call repository
                        var addedMovie = _repository.AddMovie(name, fullDate, genreNames);
                        _logger.LogInformation("Movie added!!!");

                        PrintMovieResult(addedMovie);
                        break;
                    case Menu.MenuOptions.SearchMovie:
                        _logger.LogInformation("Searching for a movie");
                        var userSearchTerm = menu.GetUserResponse("Enter your", "search string:", "green");
                        var searchedMovies = _repository.Search(userSearchTerm);

                        PrintMovieResult(searchedMovies);
                        break;
                    case Menu.MenuOptions.AddUser:
                        _logger.LogInformation("Adding a new user...");

                        var firstName = menu.GetUserResponse("Enter user", "first name:", "green");
                        var lastName = menu.GetUserResponse("Enter user", "last name:", "green");
                        var age = menu.GetUserResponse("Enter user", "age:", "green");
                        if (!int.TryParse(age, out int ageInt))
                        {
                            _logger.LogInformation("Invalid age format. Aborting.");
                            return;
                        }
                        var gender = menu.GetUserResponse("Enter user", "gender (M/F):", "green");
                        var street = menu.GetUserResponse("Enter user", "street address:", "green");
                        var city = menu.GetUserResponse("Enter user", "city:", "green");
                        var state = menu.GetUserResponse("Enter user", "state:", "green");
                        var zCode = menu.GetUserResponse("Enter user", "zipcode:", "green");
                        var occupationName = menu.GetUserResponse("Enter user", "occupation:", "green");

                        _repository.AddUser(firstName,lastName, ageInt, gender.ToUpper(),street,city,state.ToUpper(),zCode, occupationName);
                        _logger.LogInformation("New user added!!!");
                        break;
                }
            }
            while (menuChoice != Menu.MenuOptions.Exit);

            menu.Exit();


            Console.WriteLine("\nThanks for using the Movie Library!");

        }

        public void PrintMovieResult(Movie movie)
        {
            string finalGenre = "";
            foreach (var genre in movie?.MovieGenres)
            {
                finalGenre += genre.Genre.Name;
                if (finalGenre != "" && genre != movie.MovieGenres.LastOrDefault())
                {
                    finalGenre += ",";
                }
            }


            // Display the header
            Console.WriteLine("|" + new string('-', 6) + "+" + new string('-', 32) + "+" + new string('-', 24) + "+" + new string('-', 32) + "|");
            Console.WriteLine($"| {"Id",4} | {"Title",-30} | {"ReleaseDate",22} | {"Genre",-30} |");
            Console.WriteLine("|" + new string('-', 6) + "+" + new string('-', 32) + "+" + new string('-', 24) + "+" + new string('-', 32) + "|");

            // data
            Console.WriteLine($"| {movie.Id,4} | {movie.Title,-30} | {movie.ReleaseDate.ToString("MM/dd/yyyy hh:mm:ss tt"),22} | {finalGenre,-30} |");

            // Display the bottom border
            Console.WriteLine("|" + new string('-', 6) + "+" + new string('-', 32) + "+" + new string('-', 24) + "+" + new string('-', 32) + "|");

        }
    }
}