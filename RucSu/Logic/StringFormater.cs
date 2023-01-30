using RucSu.Models;

namespace RucSu.Logic;

public static class StringFormater
{
    /// <summary>
    /// Дата вида "Завтра".
    /// </summary>
    /// <param name="date">Дата</param>
    /// <returns>Дата в строке</returns>
    public static string? ShortDateName(DateTime? date)
    {
        var today = DateTime.Today;
        if (date is null) return null;
        if (date == today) return "Сегодня";
        if (date == today.AddDays(1)) return "Завтра";
        if (date == today.AddDays(2)) return "Послезавтра";
        return null;
    }

    /// <summary>
    /// Занятие в строку.
    /// </summary>
    /// <param name="lesson">Занятие</param>
    /// <returns>Результат</returns>
    public static string LessonAsString(Lesson lesson, string? template = null)
    {
        if (template == null) return lesson.ToString();
        return lesson.GetByTemplate(template);
    }

    /// <summary>
    /// День в строку.
    /// </summary>
    /// <param name="day">день</param>
    /// <param name="template">шаблон</param>
    /// <param name="date">замена даты</param>
    /// <param name="lessons">добавить занятия?</param>
    /// <param name="lessonTemplate">шаблон для занятий</param>
    public static string DayAsString(Day day, string? template = null, string? date = null, bool lessons = true, string? lessonTemplate = null)
    {
        // Чтение шаблона.
        if (template is null) template = date ?? day.ToString();
        else
        {
            if (date != null)
                template = template.Replace("<$ShortDate>", date)
                                  .Replace("<$WeekDate>", date);
            template = day.GetByTemplate(template); // Замена.
        }
        if (lessons)
            foreach (Lesson lesson in day.Lessons)
                template += Environment.NewLine + LessonAsString(lesson, lessonTemplate);
        return template;
    }
}
