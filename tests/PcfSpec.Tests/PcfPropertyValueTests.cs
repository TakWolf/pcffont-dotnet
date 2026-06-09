namespace PcfSpec.Tests;

public class PcfPropertyValueTests
{
    [Fact]
    public void TestValue1()
    {
        var value = new PcfPropertyValue(42);
        Assert.True(value.IsInt);
        Assert.False(value.IsString);
        Assert.Equal(42, value.AsInt());
        Assert.Throws<InvalidOperationException>(() => value.AsString());
    }

    [Fact]
    public void TestValue2()
    {
        var value = new PcfPropertyValue("hello");
        Assert.True(value.IsString);
        Assert.False(value.IsInt);
        Assert.Equal("hello", value.AsString());
        Assert.Throws<InvalidOperationException>(() => value.AsInt());
    }

    [Fact]
    public void TestValue3()
    {
        Assert.Throws<ArgumentNullException>(() => new PcfPropertyValue(null!));
    }

    [Fact]
    public void TestValue4()
    {
        PcfPropertyValue value = default;
        Assert.True(value.IsInt);
        Assert.Equal(0, value.AsInt());
    }

    [Fact]
    public void TestValue5()
    {
        var value = new PcfPropertyValue(42);
        var (intValue, stringValue) = value;
        Assert.Equal(42, intValue);
        Assert.Null(stringValue);
    }

    [Fact]
    public void TestValue6()
    {
        var value = new PcfPropertyValue("hello");
        var (intValue, stringValue) = value;
        Assert.Equal("hello", stringValue);
        Assert.Null(intValue);
    }

    [Fact]
    public void TestValue7()
    {
        PcfPropertyValue value = 42;
        Assert.True(value.IsInt);
        Assert.Equal(42, value.AsInt());
    }

    [Fact]
    public void TestValue8()
    {
        PcfPropertyValue value = "hello";
        Assert.True(value.IsString);
        Assert.Equal("hello", value.AsString());
    }

    [Fact]
    public void TestValue9()
    {
        var value = (int)new PcfPropertyValue(42);
        Assert.Equal(42, value);
    }

    [Fact]
    public void TestValue10()
    {
        var value = (string)new PcfPropertyValue("hello");
        Assert.Equal("hello", value);
    }

    [Fact]
    public void TestValue11()
    {
        Assert.Throws<InvalidOperationException>(() => (string)new PcfPropertyValue(42));
        Assert.Throws<InvalidOperationException>(() => (int)new PcfPropertyValue("hello"));
    }

    [Fact]
    public void TestEquals1()
    {
        var a = new PcfPropertyValue(42);
        var b = new PcfPropertyValue(42);
        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void TestEquals2()
    {
        var a = new PcfPropertyValue(1);
        var b = new PcfPropertyValue(42);
        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
        Assert.False(a == b);
        Assert.True(a != b);
    }

    [Fact]
    public void TestEquals3()
    {
        var a = new PcfPropertyValue("hello");
        var b = new PcfPropertyValue("hello");
        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void TestEquals4()
    {
        var a = new PcfPropertyValue("hello");
        var b = new PcfPropertyValue("world");
        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
        Assert.False(a == b);
        Assert.True(a != b);
    }

    [Fact]
    public void TestEquals5()
    {
        var a = new PcfPropertyValue(42);
        var b = new PcfPropertyValue("hello");
        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
        Assert.False(a == b);
        Assert.True(a != b);
    }

    [Fact]
    public void TestEquals6()
    {
        var a = new PcfPropertyValue("hello");
        var b = new PcfPropertyValue(42);
        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
        Assert.False(a == b);
        Assert.True(a != b);
    }

    [Fact]
    public void TestGetHashCode1()
    {
        var a = new PcfPropertyValue(42);
        var b = new PcfPropertyValue(42);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void TestGetHashCode2()
    {
        var a = new PcfPropertyValue("hello");
        var b = new PcfPropertyValue("hello");
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void TestGetHashCode3()
    {
        var a = new PcfPropertyValue(42);
        var b = new PcfPropertyValue("hello");
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void TestToString1()
    {
        var value = new PcfPropertyValue(42);
        Assert.Equal("42", value.ToString());
    }

    [Fact]
    public void TestToString2()
    {
        var value = new PcfPropertyValue("hello");
        Assert.Equal("hello", value.ToString());
    }
}
