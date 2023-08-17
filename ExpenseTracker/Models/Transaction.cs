using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Transaction
    {
        //private readonly ApplicationDbContext _context;


        //public Transaction(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        [Key]
        public int Transactionid { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a new category.")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        //CategoryId
        [Range(1,int.MaxValue,ErrorMessage ="Amount should be greater than 0.")]
        public int Amount { get; set; }
        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryTitleWithIcon
        {
            get { return Category == null ? "Empty" : Category.TitleWithIcon; }


        }
        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                return (( Category==null || Category.Type=="Expense")? "-  " : "+  " ) + Amount.ToString("C");
            }
        }

        //public Category getCategory()
        //{
        //    ApplicationDbContext _context;
        //    Category category = _context.Categories.Where(x => x.CategoryId == this.CategoryId).FirstOrDefault();
        //    return category;
        //}

        //public static generalSearch(string query)
        //{
        //    var userId = _userManager.GetUserId(User);
        //    var allTransactionsByUser = _context.Transactions.Where(x => x.UserId == userID).ToList();
        //}

    }
}
