using System.Text.Json.Serialization;

namespace RucSu.Models;

public sealed class Lesson : TemplateModel
{
    private static readonly string[] LessonsStartTimes =
    {
        "9:00",
        "10:45",
        "12:40",
        "14:30",
        "16:20",
    };

    private static readonly string[] LessonsEndTimes =
    {
        "10:35",
        "12:20",
        "14:15",
        "16:05",
        "17:55",
    };

    public Lesson(byte id, string name, string teacher, string position)
    {
        Id = id;
        Name = name;
        Teacher = teacher;
        Position = position;
    }

    public byte Id { get; set; }

    /// <summary>
    /// Название предмета.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Имя учителя.
    /// </summary>
    public string Teacher { get; set; }

    /// <summary>
    /// Места где будет проводится занятие.
    /// </summary>
    public string Position { get; set; }

    [JsonIgnore]
    /// <summary>
    /// Время начала занятия.
    /// </summary>
    public string Start
    {
        get { return LessonsStartTimes[Id-1]; }
    }

    [JsonIgnore]
    /// <summary>
    /// Время начала конца.
    /// </summary>
    public string End
    {
        get { return LessonsEndTimes[Id - 1]; }
    }

    [JsonIgnore]
    /// <summary>
    /// Редактированное место проведения занятия.
    /// </summary>
    public string PositionEdited
    {
        get
        {
            return Position.Replace("ауд. л.з.", "Л.з.").Replace("ауд.", "Ауд.");
        }
    }

    [JsonIgnore]
    /// <summary>
    /// Время занятия.
    /// </summary>
    public string Time
    {
        get
        {
            return $"{Start}—{End}";
        }
    }

    [JsonIgnore]
    public override string[] Parameters => _Parameters;
    public static readonly string[] _Parameters = new string[]
    {
        "Id",
        "Name",
        "Teacher",
        "Position",

        "Start",
        "End",

        "PositionEdited",
        "Time",
    };

    public override string? GetValue(string name)
    {
        return (name) switch
        {
            "Id" => Id.ToString(),
            "Name" => Name,
            "Teacher" => Teacher,
            "Position" => Position,

            "Start" => Start,
            "End" => End,

            "PositionEdited" => PositionEdited,
            "Time" => Time,
            _ => null
        };
    }

    /// <summary>
    /// Текст занятия.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Id}. {Name}.{Environment.NewLine}{PositionEdited}.{Environment.NewLine}Время: {Time}.";
    }
}
