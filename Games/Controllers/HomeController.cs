using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Games.Application.Queries;
using Games.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(string? title, int[]? selectedGenreIds, int? year, bool upcomingOnly)
    {
        var filter = new GameFilterViewModel
        {
            Title = title,
            Year = year,
            SelectedGenreIds = selectedGenreIds?.ToList() ?? new List<int>(),
            UpcomingOnly = upcomingOnly
        };

        var model = await _mediator.Send(new FilterGamesQuery(filter));
        return View(model);
    }

    public IActionResult Feedback()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}