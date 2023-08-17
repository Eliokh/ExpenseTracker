using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        //public TransactionController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}
        public TransactionController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Transaction
        public async Task<IActionResult> Index(string query)
        {
            var userId = _userManager.GetUserId(User);
            foreach (var i in _context.Transactions)
            {
                i.Category = _context.Categories.Where(x => x.CategoryId == i.CategoryId).FirstOrDefault();
            }

            if (!String.IsNullOrEmpty(query))
            {
                var allTransactionsByUser = _context.Transactions.Where(x => x.UserId == userId);
                query = query.Trim().ToLower();

                var filteredTransactions = allTransactionsByUser.Where(x => x.Amount.ToString() == query || x.Note.ToLower().Contains(query) || x.Date.ToString().ToLower().Contains(query) || x.Category.Title.ToLower().Contains(query) || x.Category.Icon.ToLower().Contains(query) || x.Category.Type.ToLower().Contains(query));
                return View(filteredTransactions);

            }
              return View(await _context.Transactions.Where(x => x.UserId == userId).ToListAsync());
        }



        // GET: Transaction/AddorEdit
        public IActionResult AddorEdit(int id=0)
        {
            var userId = _userManager.GetUserId(User);

            ViewBag.UserID = userId;

            PopulateCategories();
           if(id== 0)
            {
                return View(new Transaction());
            }
            else
            {
                var transactionFromDb = _context.Transactions.Find(id);

                if (transactionFromDb == null)
                {
                    return NotFound();
                }
                return View(transactionFromDb);
            } 
        }

        // POST: Transaction/AddorEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> AddorEdit([Bind("Transactionid,CategoryId,Amount,Note,Date,UserId")] Transaction transaction)
         {
             if (ModelState.IsValid)
             {
                 if(transaction.Transactionid == 0)
                 {
                    int maxID = _context.Transactions.Max(x => x.Transactionid);
                    transaction.Transactionid = maxID + 1;
                    _context.Add(transaction);
                 }
                 else
                 {
                     _context.Update(transaction);
                 }
                 await _context.SaveChangesAsync();
                 return RedirectToAction(nameof(Index));
             }
             PopulateCategories();
             return View(transaction);
         }
        


   
                // POST: Transaction/Delete/5
                [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [NonAction]
        public void PopulateCategories()
        {
            var userId = _userManager.GetUserId(User);
            var CategoryCollection = _context.Categories.Where(x => x.UserId == userId).ToList();
            Category defaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
            CategoryCollection.Insert(0, defaultCategory);
            ViewBag.Categories = CategoryCollection; 
        }


        //[HttpPost]
        //public IActionResult Search(string searchQuery)
        //{
        //    var userId = _userManager.GetUserId(User);
        //    //var CategoryCollection = _context.Categories.Where(x => x.UserId == userId).ToList();
        //    //Category defaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
        //    //CategoryCollection.Insert(0, defaultCategory);
        //    //ViewBag.Categories = CategoryCollection;

        //    return View("Index");
        //}
    }
}
