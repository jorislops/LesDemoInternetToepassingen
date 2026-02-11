using EFLesDemo.Entities;
using Spectre.Console;

namespace EFLesDemo;

public class ScrumboardConsoleUtils
{
    public static void RenderScrumboardGrid(Scrumboard board)
    {
        var grid = new Grid();

        // 1. Add all columns first
        foreach (var _ in board.Columns)
            grid.AddColumn();

        // 2. Build the panels for each column
        var panels = board.Columns
            // .OrderBy(c => c.Order)
            .Select(col =>
            {
                var cardList = string.Join("\n",
                    col.Cards
                        // .OrderBy(c => c.Order)
                        .Select(c => $"{c.Id} - ({c.Order}) {c.Name}"));

                var content = string.IsNullOrWhiteSpace(cardList)
                    ? "[grey]No cards[/]"
                    : cardList;

                return new Panel(content)
                {
                    Header = new PanelHeader($"{col.Id} ({col.Order}) {col.Name}"),
                    Border = BoxBorder.Rounded
                };
            })
            .ToArray();

        // 3. Add a single row containing all panels
        grid.AddRow(panels);

        AnsiConsole.Write(grid);
    }
    
    public static void RenderScrumboard(Scrumboard board)
    {
        var table = new Table()
            .Title($"[yellow]{board.Name}[/]")
            .Border(TableBorder.Rounded);

        table.AddColumn("[bold]Column[/]");
        table.AddColumn("[bold]Cards[/]");

        foreach (var col in board.Columns.OrderBy(c => c.Order))
        {
            var cardsText = string.Join("\n", 
                col.Cards
                    .OrderBy(c => c.Order)
                    .Select(c => $"- {c.Name}"));

            table.AddRow(
                $"[green]{col.Name}[/]",
                string.IsNullOrWhiteSpace(cardsText) ? "[grey]No cards[/]" : cardsText
            );
        }

        AnsiConsole.Write(table);
    }
}