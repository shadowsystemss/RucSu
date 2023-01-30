namespace RucSu.Models;

public abstract class TemplateModel
{
    public abstract string[] Parameters { get; }

    public abstract string? GetValue(string name);

    public virtual string GetByTemplate(string template)
    {
        foreach (string parameter in Parameters)
        {
            var start = $"<s${parameter}>";
            var end = $"<e${parameter}>";

            var name = $"<${parameter}>";

            string? value = GetValue(parameter);
            if (value == null) template = RemoveByLabels(template, start, end)
                                                        .Replace(name, "");
            else template = template.Replace(start, "")
                                    .Replace(end, "")
                                    .Replace(name, value);
        }
        return template;
    }

    public static string RemoveByLabels(string text, string start, string end)
    {
        int startPosition;
        int endPosition;

        while (true)
        {
            startPosition = text.IndexOf(start);
            if (startPosition == -1) return text;

            endPosition = text.IndexOf(end, startPosition);
            if (endPosition == -1) return text;
            
            text = text.Remove(startPosition, endPosition + end.Length - startPosition);
        }
    }
}