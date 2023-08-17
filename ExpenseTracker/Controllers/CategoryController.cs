using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Data;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        //public CategoryController(ApplicationDbContext context)
        public CategoryController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Category
        public async Task<IActionResult> Index(string filter)
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.IsCategories = true;

            if (!String.IsNullOrEmpty(filter))
            {
                var categories = _context.Categories.Where(x => x.UserId == userId && x.Type == filter).ToList();
                return View(categories);
            }
            return View(await _context.Categories.Where(x => x.UserId == userId).ToListAsync());
        }



        // GET: Category/AddorEdit
        public IActionResult AddorEdit(int id=0)
        {
            var userId = _userManager.GetUserId(User);
            
            ViewBag.UserID = userId;

            if (id == 0)
            {
            return View(new Category());
            }
            else
            {
                var categoryFromDb = _context.Categories.Find(id);
                if (categoryFromDb == null)
                {
                    return NotFound();
                }
                return View(categoryFromDb);
            }
        }

        // POST: Category/AddorEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddorEdit([Bind("CategoryId,Title,Icon,Type,UserId")] Category category)
        {
            //if(category.Title == category.Icon.ToString())
            //{
            //    ModelState.AddModelError("Title", "the Icon cannot match the Title");
            //}

            //var currentUser = await _userManager.GetUserAsync(User);
            //var userId = currentUser.Id;
            //category.User = currentUser;
            //category.UserId = userId;

            if (ModelState.IsValid)
            {
                if(category.CategoryId == 0)
                {
                    int maxID = _context.Categories.Max(x => x.CategoryId);
                    category.CategoryId = maxID + 1;
                    
                    _context.Add(category); //User.Identity.Name()
                }
                else
                {
                    _context.Update(category);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        /*
        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Title,Icon,Type")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }*/


        [HttpGet]
        public IActionResult DeleteConfirmation(int categoryID)
        {
            var transactions = _context.Transactions.Where(x => x.CategoryId == categoryID).ToList();
            if (transactions.Any())
                return Ok(true);

            return Ok(false);
        }



        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                var transactions = _context.Transactions.Where(x => x.CategoryId == category.CategoryId).ToList();
                if (!transactions.Any())
                    _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

   
    }
}
