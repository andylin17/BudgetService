using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BudgetSystem;

public class BudgetService
{
	private IBudgetRepo _budgetRepo;

	public BudgetService(IBudgetRepo budgetRepo)
	{
		_budgetRepo = budgetRepo;
	}

	public decimal Query(DateTime start, DateTime end)
	{

		if (start > end)
		{
			return 0.0m;
		}

		var budgets = _budgetRepo.GetAll();
		var dateRange = new List<TempDataObj>();

		int overlappingDays = 0;
		var dailyAmount = 0;
		foreach (var budget in budgets)
		{
			var year = int.Parse(budget.YearMonth.Substring(0, 4));
			var month = int.Parse(budget.YearMonth.Substring(4, 2));
			var days = DateTime.DaysInMonth(year, month);
			var tempDataObj = new TempDataObj(year, month, budget.Amount / days);
			dateRange.Add(tempDataObj);
			
			if (start.Year == end.Year && start.Month == end.Month)
			{
				if(budget.YearMonth== start.ToString("yyyyMM"))
				{
					overlappingDays = end.Day - start.Day + 1;
					dailyAmount = tempDataObj.DailyAmount * overlappingDays;
				}

				continue;
			}
			else
			{

				for (var currentDate = start; currentDate < NextEndMonth(end); currentDate.AddMonths(1))
				{
					var startDays = DateTime.DaysInMonth(start.Year, start.Month);
					
					DateTime date;
					if (budget.YearMonth == start.ToString("yyyyMM"))
					{
						overlappingDays = startDays - start.Day + 1;
						date = start;
					}
					else if (budget.YearMonth == end.ToString("yyyyMM"))
					{
						overlappingDays = end.Day;
						date = end;
					}
					else
					{
						overlappingDays = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
						date = currentDate;
						
					}
					dailyAmount += tempDataObj.DailyAmount * overlappingDays;
				}
			}
		}
		return dailyAmount;
	}

	private static DateTime NextEndMonth(DateTime end)
	{
		return new DateTime(end.Year, end.Month, 1).AddMonths(1);
	}
}

public interface IBudgetRepo
{
	List<Budget> GetAll();
}

public class Budget
{
	public string YearMonth { get; set; }

	public int Amount { get; set; }
}

internal class TempDataObj
{
	public int Year { get; }
	public int Month { get; }
	public int DailyAmount { get; }

	public TempDataObj(int year, int month, int dailyAmount)
	{
		Year = year;
		Month = month;
		DailyAmount = dailyAmount;
	}

	public override bool Equals(object? obj)
	{
		return obj is TempDataObj other &&
			   EqualityComparer<object>.Default.Equals(Year, other.Year) &&
			   EqualityComparer<object>.Default.Equals(Month, other.Month) &&
			   EqualityComparer<object>.Default.Equals(DailyAmount, other.DailyAmount);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Year, Month, DailyAmount);
	}
}