using RucSu.Logic;
using System.Text.Json.Serialization;

namespace RucSu.Models;

public sealed class Day : TemplateModel
{
    public Day(DateTime date, string dayOfWeek)
    {
        Date = date;
        DayOfWeek = dayOfWeek;
    }

    public DateTime Date { get; set; }

    /// <summary>
    /// День недели
    /// </summary>
    public string DayOfWeek { get; set; }

    /// <summary>
    /// Занятия
    /// </summary>
    public List<Lesson> Lessons { get; set; } = new List<Lesson>();

    // Короткая дата: Завтра.
    [JsonIgnore]
    public string ShortDate
    {
        get
        {
            string? value = StringFormater.ShortDateName(Date);
            if (value == null) return WeekDate;
            return $"{value} ({Date.ToString("dd.MM")})";
        }
    }

    // День недели с датой: Понедельник (26.07).
    [JsonIgnore]
    public string WeekDate
    {
        get
        {
            return Date.ToString($"{DayOfWeek} (dd.MM)");
        }
    }

    [JsonIgnore]
    public override string[] Parameters => _Parameters;
    private static readonly string[] _Parameters =
    {
        "Date",
        "DayOfWeek",
        "ShortDate",
        "WeekDate"
    };

    public override string? GetValue(string name)
    {
        return name switch
        {
            "Date" => Date.ToShortDateString(),
            "DayOfWeek" => DayOfWeek,
            "ShortDate" => ShortDate,
            "WeekDate" => WeekDate,
            _ => null
        };
    }

    /// <summary>
    /// День недели с датой: Понедельник (26.07).
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return WeekDate;
    }
}
