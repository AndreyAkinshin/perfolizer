using System.Collections;
using System.Text;
using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Metrology;
using Perfolizer.Models;

namespace Perfolizer.Json;

public class LightJsonSerializer
{
    private const int IndentStep = 2;

    public static string Serialize(object obj, LightJsonSettings? settings = null) =>
        new LightJsonSerializer(settings).Append(obj).ToString();

    private readonly LightJsonSettings settings;
    private int currentIndent;
    private readonly StringBuilder builder = new ();

    private LightJsonSerializer(LightJsonSettings? settings = null)
    {
        this.settings = settings ?? new LightJsonSettings();
    }

    public override string ToString() => builder.ToString();

    private LightJsonSerializer Append(object obj)
    {
        switch (obj)
        {
            case IDictionary dictionary:
                AppendDictionary(dictionary);
                break;
            case string stringValue:
                AppendString(stringValue);
                break;
            case int intValue:
                AppendInt(intValue);
                break;
            case long longValue:
                AppendLong(longValue);
                break;
            case double doubleValue:
                AppendDouble(doubleValue);
                break;
            case bool boolValue:
                AppendBool(boolValue);
                break;
            case Guid guidValue:
                AppendGuidValue(guidValue);
                break;
            case DateTimeOffset dateTimeOffsetValue:
                AppendDateTimeOffset(dateTimeOffsetValue);
                break;
            case MeasurementUnit measurementUnit:
                AppendMeasurementUnit(measurementUnit);
                break;
            case MetricInfo metric:
                AppendMetric(metric);
                break;
            case ICollection collection:
                AppendCollection(collection);
                break;
            case Enum enumValue:
                AppendEnum(enumValue);
                break;
            case AbstractInfo perfonarObject:
                AppendPerfonarObject(perfonarObject);
                break;
            default:
                throw new NotSupportedException($"Unsupported type: {obj.GetType()}");
        }

        return this;
    }

    private void AppendPerfonarObject(AbstractInfo abstractInfo)
    {
        builder.Append('{');
        currentIndent += IndentStep;

        bool isFirst = true;
        foreach (var property in abstractInfo.GetType().GetProperties())
        {
            object? value;
            try
            {
                value = property.GetValue(abstractInfo);
            }
            catch (Exception)
            {
                continue;
            }
            if (value == null || IsEmptyValue(value))
                continue;

            string key = property.Name.ToCamelCase();
            AppendValue(isFirst, key, value);
            isFirst = false;
        }
        currentIndent -= IndentStep;
        AppendNextLine();
        AppendIndent();
        builder.Append("}");
    }

    private void AppendDictionary(IDictionary dictionary)
    {
        builder.Append('{');
        currentIndent += IndentStep;

        bool isFirst = true;
        foreach (DictionaryEntry dictionaryEntry in dictionary)
        {
            string? key = dictionaryEntry.Key.ToString();
            if (key == null)
                continue;

            object? value = dictionaryEntry.Value;
            if (value == null || IsEmptyValue(value))
                continue;

            AppendValue(isFirst, key.ToLowerInvariant(), value);
            isFirst = false;
        }
        currentIndent -= IndentStep;
        AppendNextLine();
        AppendIndent();
        builder.Append("}");
    }

    private void AppendValue(bool isFirst, string key, object value)
    {
        if (!isFirst)
            builder.Append(',');
        AppendNextLine();
        AppendIndent();
        builder.Append('\"');
        builder.Append(key);
        builder.Append('\"');
        builder.Append(':');
        AppendSpace();
        Append(value);
    }

    private static bool IsEmptyValue(object? value)
    {
        if (value == null)
            return true;
        if (value is string stringValue && stringValue.IsBlank())
            return true;
        if (value is NumberUnit)
            return true;
        if (value is ICollection { Count: 0 })
            return true;
        if (value is MetricInfo metric && metric.IsEmpty)
            return true;
        return false;
    }

    private void AppendCollection(ICollection collection)
    {
        builder.Append('[');
        currentIndent += IndentStep;
        bool isFirst = true;
        foreach (object value in collection)
        {
            if (value == null)
                continue;

            if (!isFirst)
                builder.Append(',');
            AppendNextLine();
            AppendIndent();
            Append(value);
            isFirst = false;
        }
        currentIndent -= IndentStep;
        AppendNextLine();
        AppendIndent();
        builder.Append(']');
    }

    private void AppendString(string value)
    {
        builder.Append('\"');
        builder.Append(value);
        builder.Append('\"');
    }

    private void AppendBool(bool value) => builder.Append(value ? "true" : "false");
    private void AppendEnum(Enum enumValue) => AppendString(enumValue.ToString().ToCamelCase());
    private void AppendGuidValue(Guid value) => AppendString(value.ToString());
    private void AppendDateTimeOffset(DateTimeOffset value) => AppendLong(value.ToUnixTimeMilliseconds());

    private void AppendMeasurementUnit(MeasurementUnit unit)
    {
        AppendString(unit.AbbreviationAscii);
    }

    private void AppendMetric(MetricInfo metric)
    {
        if (metric.IsEmpty)
            builder.Append("\"\"");
        else if (metric.Version == null && metric.Id != null)
            AppendString(metric.Id);
        else
            AppendPerfonarObject(metric);
    }

    private void AppendInt(int value) => builder.Append(value.ToString(DefaultCultureInfo.Instance));
    private void AppendLong(long value) => builder.Append(value.ToString(DefaultCultureInfo.Instance));
    private void AppendDouble(double value) => builder.Append(value.ToString(DefaultCultureInfo.Instance));

    private void AppendIndent() => builder.Append(' ', settings.Indent ? currentIndent : 0);

    private void AppendNextLine()
    {
        if (settings.Indent)
            builder.AppendLine();
    }

    private void AppendSpace()
    {
        if (settings.Indent)
            builder.Append(' ');
    }
}