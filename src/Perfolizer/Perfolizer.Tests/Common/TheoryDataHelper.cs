namespace Perfolizer.Tests.Common;

public static class TheoryDataHelper
{
    public static TheoryData<string> Create(IEnumerable<string> values)
    {
        var data = new TheoryData<string>();
        foreach (string value in values)
            data.Add(value);
        return data;
    }
}