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

    /// <summary>
    /// Дата
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// День недели
    /// </summary>
    public string DayOfWeek { get; set; }

    /// <summary>
    /// Занятия
    /// </summary>
    public List<Lesson> Lessons { get; set; } = new List<Lesson>();

    [JsonIgnore]
    /// <summary>
    /// Короткая дата: Завтра.
    /// </summary>
    public string ShortDate
    {
        get
        {
            var value = StringFormater.ShortDateName(Date);
            if (value == null) return WeekDate;
            return $"{value} ({Date.ToString("dd.MM")})";
        }
    }

    [JsonIgnore]
    /// <summary>
    /// День недели с датой: Понедельник (26.07).
    /// </summary>
    /// <returns></returns>
    public string WeekDate
    {
        get
        {
            return Date.ToString($"{DayOfWeek} (dd.MM)");
        }
    }

    [JsonIgnore]
    public override string[] Parameters => _Parameters;
    public static string[] _Parameters = new string[]
    {
        "Date",
        "DayOfWeek",
        "ShortDate",
        "WeekDate",
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
