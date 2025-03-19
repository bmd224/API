
namespace MVC.Data
{
    public class EFRepository_mini_InMemory : EFRepository_mini<ApplicationDbContextInMemory>
    {
        public EFRepository_mini_InMemory(ApplicationDbContextInMemory context) : base(context) { }
    }
}
