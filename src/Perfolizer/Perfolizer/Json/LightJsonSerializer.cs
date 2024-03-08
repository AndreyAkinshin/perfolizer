using System.Collections;
using System.Text;
using Perfolizer.Common;
using Perfolizer.Extensions;
using Perfolizer.Phd.Base;

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
            case PhdEntry phdEntry:
                Append(phdEntry.ToRaw());
                break;
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
            case ICollection collection:
                AppendCollection(collection);
                break;
            case Enum enumValue:
                AppendEnum(enumValue);
                break;
            default:
                AppendObject(obj);
                break;
        }

        return this;
    }

    private void AppendObject(object obj)
    {
        builder.Append('{');
        currentIndent += IndentStep;

        bool isFirst = true;
        foreach (var property in obj.GetType().GetProperties())
        {
            object? value;
            try
            {
                value = property.GetValue(obj);
            }
            catch (Exception)
            {
                continue;
            }
            if (IsEmptyValue(value))
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
            object? value = dictionaryEntry.Value;
            if (IsEmptyValue(value))
                continue;

            string key = dictionaryEntry.Key.ToString().ToLowerInvariant();
            AppendValue(isFirst, key, value);
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
        if (value is ICollection { Count: 0 })
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