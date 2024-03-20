using System.Text.RegularExpressions;

namespace Maurice.Scheduler;

public enum CronFieldType
{
    Minutes,
    Hours,
    DayOfMonth,
    Month,
    DayOfWeek
}

internal class CronExpressionParser
{
    private static readonly Regex FieldRegex = new(@"^(\*|(\d+|\*/\d+)(-(\d+|\*/\d+))?)(\/\d+)?(,(\d+|\*/\d+)(-(\d+|\*/\d+))?)*$", RegexOptions.Compiled);

    private static readonly Dictionary<string, string> SpecialStrings = new()
    {
        { "@yearly", "0 0 1 1 *" },
        { "@monthly", "0 0 1 * *" },
        { "@weekly", "0 0 * * 0" },
        { "@daily", "0 0 * * *" },
        { "@hourly", "0 * * * *" }
    };

    public static List<DateTime> Parse(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new ArgumentException("Cron expression cannot be empty");

        if (SpecialStrings.ContainsKey(expression))
            expression = SpecialStrings[expression];

        var fields = expression.Split(' ');

        if (fields.Length != 5)
            throw new ArgumentException("Invalid number of fields in cron expression");

        var cronFields = new List<int>[5];

        for (int i = 0; i < fields.Length; i++)
        {
            if (!FieldRegex.IsMatch(fields[i]))
                throw new ArgumentException($"Invalid cron field '{fields[i]}'");

            cronFields[i] = ParseField(fields[i], (CronFieldType)i);
        }

        return GenerateSchedule(cronFields);
    }

    private static List<int> ParseField(string field, CronFieldType fieldType)
    {
        var values = new List<int>();

        var tokens = field.Split(',');

        foreach (var token in tokens)
        {
            if (token.Contains("/"))
            {
                var stepTokens = token.Split('/');
                var start = stepTokens[0] == "*" ? 0 : int.Parse(stepTokens[0]);
                var step = int.Parse(stepTokens[1]);

                for (var i = start; i <= GetMaxValue(fieldType); i += step)
                {
                    values.Add(i);
                }
            }
            else if (token.Contains("-"))
            {
                var rangeTokens = token.Split('-');
                var start = int.Parse(rangeTokens[0]);
                var end = int.Parse(rangeTokens[1]);

                for (var i = start; i <= end; i++)
                {
                    values.Add(i);
                }
            }
            else if (token == "*")
            {
                for (var i = 0; i <= GetMaxValue(fieldType); i++)
                {
                    values.Add(i);
                }
            }
            else
            {
                values.Add(int.Parse(token));
            }
        }

        return values;
    }

    private static List<DateTime> GenerateSchedule(List<int>[] cronFields)
    {
        var schedule = new List<DateTime>();

        var currentDate = DateTime.Now;

        for (int year = currentDate.Year; year <= currentDate.Year + 1; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
                {
                    for (int hour = 0; hour < 24; hour++)
                    {
                        for (int minute = 0; minute < 60; minute++)
                        {
                            bool match = true;

                            foreach (var cronField in cronFields)
                            {
                                //if (!cronField.Contains(year, month, day, hour, minute))
                                //{
                                //    match = false;
                                //    break;
                                //}
                            }

                            if (match)
                            {
                                schedule.Add(new DateTime(year, month, day, hour, minute, 0));
                            }
                        }
                    }
                }
            }
        }

        return schedule;
    }

    private static int GetMaxValue(CronFieldType fieldType)
    {
        return fieldType switch
        {
            CronFieldType.Minutes => 59,
            CronFieldType.Hours => 23,
            CronFieldType.DayOfMonth => 31,
            CronFieldType.Month => 12,
            CronFieldType.DayOfWeek => 6,
            _ => throw new ArgumentOutOfRangeException(nameof(fieldType), fieldType, null)
        };
    }
}