using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Transactions;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        //public DashBoardController(ApplicationDbContext context)
        public DashBoardController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<ActionResult> Index()
        {

            //last 7 days:
            DateTime StartDate=DateTime.Today.AddDays(-6);
            DateTime EndDate=DateTime.Today;

            var userId = _userManager.GetUserId(User);

            List<Models.Transaction> SelectedTransactions = await _context.Transactions
             .Include(x => x.Category)
             .Where(y=>y.Date >= StartDate && y.Date <= EndDate && y.UserId == userId)
             .ToListAsync();

            //TotalIncome:
            int TotalIncome = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C");

            //TotalExpense
            int TotalExpense = SelectedTransactions
                .Where(a => a.Category.Type == "Expense")
                .Sum(b => b.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C");

            //balance  Amount
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture, "{0:C}", Balance);

            //donut chart _ Expense by category
            ViewBag.DoughnutChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    CategoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    FormattedAmount = k.Sum(j => j.Amount).ToString("C"),
                })
                .OrderByDescending(l => l.amount)
            .ToList();

            //spline chart: Income Vs Expense
            //Income:
            List<SplineChartData> IncomeSummary = SelectedTransactions
               .Where(i => i.Category.Type == "Income")
               .GroupBy(j => j.Date)
               .Select(k => new SplineChartData()
               {
                   day = k.First().Date.ToString("dd-MMM"),
                   income = k.Sum(l => l.Amount)
               })
               .ToList();


            //Expense:
            List<SplineChartData> ExpenseSummary = SelectedTransactions
               .Where(i => i.Category.Type == "Expense")
               .GroupBy(j => j.Date)
               .Select(k => new SplineChartData()
               {
                   day = k.First().Date.ToString("dd-MMM"),
                   expense = k.Sum(l => l.Amount)
               })
               .ToList();

            //Combine expense and income to pass the objects together by their date:
            string[] last7Days = Enumerable.Range(0,7)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            ViewBag.SplineChartData = from day in last7Days
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };

            //recent transactions
            ViewBag.RecentTransactions = await _context.Transactions
                .Include(i => i.Category)
                .Where(x => x.UserId == userId)
                .OrderByDescending(j => j.Date)
                .Take(5)
                .ToListAsync();


            return View();
        }
    }
    public class SplineChartData
    {
        public string day;
        public int income;
        public int expense; 
    }

}
