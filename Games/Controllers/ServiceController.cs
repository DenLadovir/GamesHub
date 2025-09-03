using Games.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IActionResult Index()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }

    public IActionResult ListRoles()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    public IActionResult CreateUser() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(string email, string password, string role)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", Messages.LoginAndPassRequired);
            return View();
        }

        var user = new IdentityUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(role) && await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            TempData["Success"] = string.Format(Messages.UserCreated, email);
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["Error"] = Messages.UserNotFound;
            return RedirectToAction("Index");
        }
        Console.WriteLine(string.Format(Messages.DeletingMessage, user.Id, user.UserName));

        var result = await _userManager.DeleteAsync(user);
        TempData["Success"] = result.Succeeded
            ? string.Format(Messages.UserDeleted, user.Email)
            : string.Join(", ", result.Errors.Select(e => e.Description));

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> EditRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var roles = _roleManager.Roles.ToList();
        var model = new EditRolesViewModel
        {
            UserId = user.Id,
            UserEmail = user.Email,
            UserRoles = await _userManager.GetRolesAsync(user),
            AllRoles = roles
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRoles(string userId, List<string> roles)
    {
        roles ??= new List<string>();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);

        var addedRoles = roles.Except(userRoles);
        var removedRoles = userRoles.Except(roles);

        await _userManager.AddToRolesAsync(user, addedRoles);
        await _userManager.RemoveFromRolesAsync(user, removedRoles);

        TempData["Success"] = string.Format(Messages.UserRolesUpdated, user.Email);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ChangePassword(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var model = new ChangePasswordViewModel
        {
            UserId = user.Id,
            UserEmail = user.Email
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            TempData["Error"] = Messages.UserNotFound;
            return RedirectToAction("Index");
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

        if (!result.Succeeded)
        {
            TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
            return View(model);
        }

        TempData["Success"] = string.Format(Messages.PasswordChanged, user.Email);
        return RedirectToAction("Index");
    }
}