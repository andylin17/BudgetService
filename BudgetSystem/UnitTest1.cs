using NSubstitute;
namespace BudgetSystem;

public class Tests
{
    private IBudgetRepo _budgetRepo;
    private BudgetService _budgetService;

    [SetUp]
    public void Setup()
    {
        _budgetRepo = NSubstitute.Substitute.For<IBudgetRepo>();

        _budgetService = new BudgetService(_budgetRepo);

    }

    [Test]
    public void SameYearSameMonthSameDay()
    {
        GetData();
        var s = new DateTime(2024, 01, 01);
        var e = new DateTime(2024, 01, 01);
        var actual = _budgetService.Query(s, e);
        Assert.AreEqual(10, actual);
    }
    [Test]
    public void SameYearSameMonthFullMonth()
    {
        GetData();
        var s = new DateTime(2024, 01, 01);
        var e = new DateTime(2024, 01, 31);
        var actual = _budgetService.Query(s, e);
        Assert.AreEqual(310, actual);
    }
    [Test]
    public void SameYearSameMonthPartialMonth()
    {
        GetData();
        var s = new DateTime(2024, 01, 03);
        var e = new DateTime(2024, 01, 17);
        var actual = _budgetService.Query(s, e);
        Assert.AreEqual(150, actual);
    }

    [Test]
    public void NoData()
    {
        GetData();
        var s = new DateTime(2024, 03 ,03);
        var e = new DateTime(2024, 03, 17);
        var actual = _budgetService.Query(s, e);
        Assert.AreEqual(0, actual);
    }

    [Test]
    public void InvalidDay()
    {
        GetData();
        var s = new DateTime(2024, 06, 03);
        var e = new DateTime(2024, 04, 17);
        var actual = _budgetService.Query(s, e);
        Assert.AreEqual(0, actual);
    }


    private void GetData()
    {

        _budgetRepo.GetAll().Returns(new List<Budget>()
        {
            new Budget(){YearMonth = "202312",Amount = 310}, //10
            new Budget(){YearMonth = "202401",Amount = 310}, //10
            new Budget(){YearMonth = "202402",Amount = 2900}, //100
            new Budget(){YearMonth = "202404",Amount = 60000}, //2000
            new Budget(){YearMonth = "202405",Amount = 31}, //1
            new Budget(){YearMonth = "202406",Amount = 300000} // 10000
        }
        );
    }
}