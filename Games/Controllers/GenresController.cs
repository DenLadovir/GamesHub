using Games.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Games.Controllers
{
    public class GenresController : Controller
    {
        private readonly IMediator _mediator;

        public GenresController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.AllGenres = await _mediator.Send(new GetAllGenresQuery());
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllGenres = await _mediator.Send(new GetAllGenresQuery());
                return View(genre);
            }

            var addedGenre = await _mediator.Send(new AddGenreCommand(genre));
            return RedirectToAction("Index", "Home");
        }
    }
}