using ConsoleTables;
using Microsoft.Extensions.Logging;
using MovieLibraryEntities.Dao;
using MovieLibraryOO.Dao;
using MovieLibraryOO.Dto;
using MovieLibraryOO.Mappers;
using System;

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
                    case Menu.MenuOptions.Add:
                        _logger.LogInformation("Adding a new movie");
                        // ask for movie name and date or use today's date???
                        var name = menu.GetUserResponse("Enter your", "movie title:", "green");
                        var year = menu.GetUserResponse("Enter the", "year of the movie:", "green");
                        var month = menu.GetUserResponse("Enter the", "month of the movie", "green");
                        var day = menu.GetUserResponse("Enter the", "day of the movie:", "green");

                        var fullDate = DateTime.Parse($"{month} {day}, {year}");
                        // call repository
                        var addedMovie = _repository.AddMovie(name, fullDate);
                        movies = _movieMapper.Map(addedMovie);
                        ConsoleTable.From<MovieDto>(movies).Write();
                        break;
                    case Menu.MenuOptions.Update:
                        _logger.LogInformation("Updating an existing movie");
                        break;
                    case Menu.MenuOptions.Delete:
                        _logger.LogInformation("Deleting a movie");
                        break;
                    case Menu.MenuOptions.Search:
                        _logger.LogInformation("Searching for a movie");
                        var userSearchTerm = menu.GetUserResponse("Enter your", "search string:", "green");
                        var searchedMovies = _repository.Search(userSearchTerm);
                        movies = _movieMapper.Map(searchedMovies);
                        ConsoleTable.From<MovieDto>(movies).Write();
                        break;
                }
            }
            while (menuChoice != Menu.MenuOptions.Exit);

            menu.Exit();


            Console.WriteLine("\nThanks for using the Movie Library!");

        }
    }
}