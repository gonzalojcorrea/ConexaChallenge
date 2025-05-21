using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Movies.Commands.SyncMovies;

/// <summary>
/// Command to synchronize movies.
/// </summary>
public class SyncMoviesCommandHandler : IRequestHandler<SyncMoviesCommand, Unit>
{
    private readonly ISwapiClient _swapi;
    private readonly IUnitOfWork _uow;

    public SyncMoviesCommandHandler(
        ISwapiClient swapi,
        IUnitOfWork uow)
    {
        _swapi = swapi;
        _uow = uow;
    }

    public async Task<Unit> Handle(SyncMoviesCommand request, CancellationToken ct)
    {
        // 1. Fetch all movies from the SWAPI
        var films = await _swapi.GetAllFilmsAsync();

        // 2. Check if any films were fetched
        foreach (var f in films)
        {
            // 3. Check if the film already exists in the database
            var existing = await _uow.Movies.GetByTitleAsync(f.Title, ct);

            // 4. If it doesn't exist, add it; otherwise, update it
            if (existing is null)
            {
                await _uow.Movies.AddAsync(new Movie
                {
                    Title = f.Title,
                    Director = f.Director,
                    Producer = f.Producer,
                    ReleaseDate = f.ReleaseDate,
                    OpeningCrawl = f.OpeningCrawl,
                    Characters = f.Characters.ToList()
                });
            }
            else
            {
                existing.Director = f.Director;
                existing.Producer = f.Producer;
                existing.ReleaseDate = f.ReleaseDate;
                existing.OpeningCrawl = f.OpeningCrawl;
                existing.Characters = f.Characters.ToList();
            }
        }

        // 5. Commit the changes to the database
        await _uow.CommitAsync(ct);
        return Unit.Value;
    }
}
