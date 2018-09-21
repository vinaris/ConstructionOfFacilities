using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ConstructionOfFacilities.Data;
using ConstructionOfFacilities.Models;

namespace ConstructionOfFacilities.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class AdministratorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;


        public AdministratorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ActionResult> ResetUserPassword(string id)
        {
           var user = await _userManager.FindByIdAsync(id);
           var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
           var result = _userManager.ResetPasswordAsync(user, resetToken, "!123Qwe");
            if (result.Result.Succeeded)
            {
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("UserList");

        }
      
        public async Task<ActionResult> DeleteUser(string id, IFormCollection collection)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return View("UserList");
                }
                var user = await _userManager.FindByIdAsync(id);
                var logins = user.Logins;
                var rolesForUser = await _userManager.GetRolesAsync(user);

                using (var transaction = _context.Database.BeginTransaction())
                {
                    foreach (var login in logins.ToList())
                    {
                        await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                    }

                    if (rolesForUser.Any())
                    {
                        foreach (var item in rolesForUser.ToList())
                        {
                            await _userManager.RemoveFromRoleAsync(user, item);
                        }
                    }
                    await _userManager.DeleteAsync(user);
                    transaction.Commit();
                }
                return RedirectToAction("UserList");
            }
            return View("UserList");
        }

        public ActionResult UserList()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }
    }
}