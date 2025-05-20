using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Movies.Commands.SyncMovies;

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
        var films = await _swapi.GetAllFilmsAsync();

        foreach (var f in films)
        {
            var existing = await _uow.Movies.GetByTitleAsync(f.Title, ct);
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

        await _uow.CommitAsync(ct);
        return Unit.Value;
    }
}