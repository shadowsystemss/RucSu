using RucSu.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace RucSu.Logic;

public static class Parser
{
    /// <summary>
    /// Шаблон дня.
    /// </summary>
    private readonly static Regex _dayRegex = new Regex("bold\">\\s+(.*?)\\s+\\((.*?)\\)\\s+</div.*?b>(.*?)</div>\\s+</div>", RegexOptions.Compiled | RegexOptions.Singleline);

    /// <summary>
    /// Шаблон занятия.
    /// </summary>
    private readonly static Regex _lessonRegex = new Regex("([0-5])\\. (.*?)<.*?/>\\s+(.*?)<br/>\\s+(.*?)<", RegexOptions.Compiled | RegexOptions.Singleline);

    /// <summary>
    /// Получить расписание на неделю.
    /// </summary>
    /// <param name="parameters">параметры запроса</param>
    /// <param name="employee">для преподавателей?</param>
    public static async Task<List<Day>?> ScheduleAsync(string parameters, bool employee = false, CancellationToken? cancel = null)
    {
        string url = "https://schedule.ruc.su/";
        if (employee) url += "employee/";

        // Получение данных.
        string raw;
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage hrm;
            if (cancel.HasValue) hrm = await client.PostAsync(url,
                new StringContent(parameters,
                    Encoding.UTF8, "application/x-www-form-urlencoded"),
                cancel.Value);
            else hrm = await client.PostAsync(url,
                new StringContent(parameters,
                    Encoding.UTF8, "application/x-www-form-urlencoded"));
            if (hrm.IsSuccessStatusCode) raw = await hrm.Content.ReadAsStringAsync();
            else return null;
        }

        // Парсинг.
        var matches = _dayRegex.Matches(raw);
        var schedule = new List<Day>();

        // Моделирование.
        foreach (Match match in matches)
        {
            // День.
            var day = new Day(DateTime.Parse(match.Groups[1].Value), match.Groups[2].Value);

            // Занятия.
            var lessonMatches = _lessonRegex.Matches(match.Groups[3].Value);
            foreach (Match lessonMatch in lessonMatches)
            {
                var id = Convert.ToByte(lessonMatch.Groups[1].Value);
                var name = lessonMatch.Groups[2].Value;
                var lesson = day.Lessons.Find(x => x.Id == id && x.Name == name);
                var position = lessonMatch.Groups[4].Value;
                if (lesson is null)
                    day.Lessons.Add(new Lesson(
                        id,
                        name,
                        lessonMatch.Groups[3].Value,
                        position
                    ));
                else
                    lesson.Position += Environment.NewLine + position;
            }

            schedule.Add(day);
        }
        return schedule;
    }

    private readonly static Regex _valuesRegex = new Regex("lg\" name=\"(.*?)\".*?>(.*?)</select>", RegexOptions.Compiled | RegexOptions.Singleline);
    private readonly static Regex _selectRegex = new Regex("value=\"(.+?)\".*?>(.*?)</option>", RegexOptions.Compiled | RegexOptions.Singleline);

    /// <summary>
    /// Получить возможные значения.
    /// </summary>
    /// <param name="employee">для преподавателей?</param>
    public async static Task<Dictionary<string, Dictionary<string, string>>?> GetValues(bool employee = false, string? branch = null, string? year = null, CancellationToken? cancel = null)
    {
        string url = "https://schedule.ruc.su/";
        if (employee) url += "employee/";

        // Параметры
        string requestParameters = "";
        if (!string.IsNullOrWhiteSpace(branch)) requestParameters += "branch=" + branch;
        if (!string.IsNullOrWhiteSpace(year))
            requestParameters += string.IsNullOrWhiteSpace(requestParameters) ? '&' : "" + "year=" + year;


        // Запрос
        string raw;
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage hrm;
            if (cancel.HasValue) hrm = await client.PostAsync(url,
                new StringContent(requestParameters, Encoding.UTF8, "application/x-www-form-urlencoded"), cancel.Value);
            else hrm = await client.PostAsync(url,
                new StringContent(requestParameters, Encoding.UTF8, "application/x-www-form-urlencoded"));
            if (hrm.IsSuccessStatusCode) raw = await hrm.Content.ReadAsStringAsync();
            else return null;
        }

        // Моделирование
        var matches = _valuesRegex.Matches(raw);
        var result = new Dictionary<string, Dictionary<string, string>>();

        foreach (Match match in matches)
        {
            var select = new Dictionary<string, string>();

            var selectMatches = _selectRegex.Matches(match.Groups[2].Value);
            foreach (Match? selectMatch in selectMatches)
                if (selectMatch != null)
                {
                    string key = selectMatch.Groups[2].Value;

                    // Иногда имена преподавателей повторяются.
                    if (select.ContainsKey(key))
                    {
                        int n = 0;
                        while (select.ContainsKey(key + n)) n++;
                        key += n;
                    }

                    select.Add(key, selectMatch.Groups[1].Value);
                }
            result.Add(match.Groups[1].Value, select);
        }

        return result;
    }
}
