namespace RucSu.Models;

public abstract class TemplateModel
{
    public abstract string[] Parameters { get; }

    public abstract string? GetValue(string name);

    public virtual string GetByTemplate(string template)
    {
        foreach (var parameter in Parameters)
        {
            var start = $"<s${parameter}>";
            var end = $"<e${parameter}>";

            var value = GetValue(parameter);
            if (value == null) template = RemoveByLabels(template, start, end);
            else template = template.Replace(start, "")
                                    .Replace(end, "")
                                    .Replace($"<${parameter}>", value);
        }
        return template;
    }

    public static string RemoveByLabels(string text, string start, string end)
    {
        while (text.Contains(start))
        {
            if (!text.Contains(end)) break;
            var startPosition = text.IndexOf(start);
            text = text.Remove(startPosition, text.IndexOf(end) + end.Length - startPosition);
        }
        return text;
    }
}
