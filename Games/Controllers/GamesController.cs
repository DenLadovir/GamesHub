using Games.Application.Queries;
using Games.Constants;
using Games.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("Games")]
public class GamesController : Controller
{
    private readonly IMediator _mediator;

    public GamesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ViewGame(int id)
    {
        var game = await _mediator.Send(new GetGameByIdQuery(id));
        if (game == null) return NotFound();
        return View(game);
    }

    [HttpGet("AddGame")]
    [Authorize(Policy = "ModeratorOrAdmin")]
    public async Task<IActionResult> AddGame()
    {
        ViewBag.AllGenres = await _mediator.Send(new GetAllGenresQuery());

        var model = new Game
        {
            SelectedGenreIds = new List<int>()
        };

        return View(model);
    }

    [HttpPost("AddGame")]
    [Authorize(Policy = "ModeratorOrAdmin")]
    public async Task<IActionResult> AddGame(Game game, int[] selectedGenres)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AllGenres = await _mediator.Send(new GetAllGenresQuery());
            return View(game);
        }

        var newGame = await _mediator.Send(new AddGameCommand(game, selectedGenres));
        return RedirectToAction("ViewGame", new { id = newGame.Id });
    }

    [HttpGet("AddGenre")]
    [Authorize(Policy = "ModeratorOrAdmin")]
    public IActionResult AddGenre() => View();

    [HttpPost("AddGenre")]
    [Authorize(Policy = "ModeratorOrAdmin")]
    public async Task<IActionResult> AddGenre(Genre genre)
    {
        if (!ModelState.IsValid) return View(genre);

        await _mediator.Send(new AddGenreCommand(genre));
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("Edit/{id}")]
    [Authorize(Policy = "ModeratorOrAdmin")]
    public async Task<IActionResult> EditGame(int id)
    {
        var game = await _mediator.Send(new GetGameByIdQuery(id));
        if (game == null) return NotFound();

        ViewBag.AllGenres = await _mediator.Send(new GetAllGenresQuery());
        return View(game);
    }

    [HttpPost("Edit/{id}")]
    [Authorize(Policy = "ModeratorOrAdmin")]
    public async Task<IActionResult> EditGame(int id, Game updatedGame, int[] selectedGenres)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AllGenres = await _mediator.Send(new GetAllGenresQuery());
            return View(updatedGame);
        }

        var result = await _mediator.Send(new EditGameCommand(id, updatedGame, selectedGenres));
        if (result == null) return NotFound();

        return RedirectToAction("ViewGame", new { id = result.Id });
    }

    [HttpGet("Delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var game = await _mediator.Send(new GetGameByIdQuery(id));
        if (game == null) return NotFound();

        return View(game);
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGameConfirmed(int id)
    {
        var result = await _mediator.Send(new DeleteGameCommand(id));
        if (!result) return NotFound();

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("Filter")]
    public async Task<IActionResult> Filtering(GameFilterViewModel filter)
    {
        var result = await _mediator.Send(new FilterGamesQuery(filter));
        return View(result);
    }

    [HttpGet("TestError")]
    public IActionResult TestError()
    {
        throw new Exception(Messages.TestError);
    }
}