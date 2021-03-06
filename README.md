# Home Budget Manager in C#

*Please note: The program is in Hungarian. 
A translation is underway, but it is not trivial since I also need to update the APIs that 
pull live currency exchange rates.*

A monetary manager utility to encourage setting monthly budgets and staying within them.

- prompts the user to make a list of their expenses and incomes every month

![add budget](https://raw.githubusercontent.com/zopad/HomeBudget/master/Documentation/AddBudgetScreen.png)

- provides statistics, charts and allows setting spending limits on categories to meet their saving goals.

![trends](https://raw.githubusercontent.com/zopad/HomeBudget/master/Documentation/TrendsScreen.png)

- features a priority system, ranking the importance of expenses 

![priorities](https://raw.githubusercontent.com/zopad/HomeBudget/master/Documentation/PrioritiesScreen.png)

- Expense cutback recommender
- Warnings about sudden overspending and going over set limits 
- An included currency exchange calculator that uses live data, 

![currency exchange](https://raw.githubusercontent.com/zopad/HomeBudget/master/Documentation/ValueExchange.png)

- can update in the background and send notifications if the wanted currency is over the wanted threshold.
- can export to Excel



Implemented using the MVVM design pattern, using Visual Studio 2017, in C#.

GUI is in WPF.

## Dependencies

Visual Studio should take care of this, otherwise mainly:

- [LiteDB](https://github.com/mbdavid/LiteDB)
- [LiveCharts](https://lvcharts.net/)
- [MaterialDesignThemes](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
- [EPPlus](https://github.com/JanKallman/EPPlus)

**License**

MIT