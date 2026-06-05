namespace PcfSpec.Tests;

public class PcfPropertyValueTests
{
    [Fact]
    public void TestValue()
    {
        {
            var value = new PcfPropertyValue(42);
            Assert.True(value.IsInt);
            Assert.False(value.IsString);
            Assert.Equal(42, value.AsInt());
            Assert.Throws<InvalidOperationException>(() => value.AsString());
        }
        {
            var value = new PcfPropertyValue("hello");
            Assert.True(value.IsString);
            Assert.False(value.IsInt);
            Assert.Equal("hello", value.AsString());
            Assert.Throws<InvalidOperationException>(() => value.AsInt());
        }
        {
            Assert.Throws<ArgumentNullException>(() => new PcfPropertyValue(null!));
        }
        {
            PcfPropertyValue value = default;
            Assert.True(value.IsInt);
            Assert.Equal(0, value.AsInt());
        }
        {
            var value = new PcfPropertyValue(42);
            var (intValue, stringValue) = value;
            Assert.Equal(42, intValue);
            Assert.Null(stringValue);
        }
        {
            var value = new PcfPropertyValue("hello");
            var (intValue, stringValue) = value;
            Assert.Equal("hello", stringValue);
            Assert.Null(intValue);
        }
        {
            PcfPropertyValue value = 42;
            Assert.True(value.IsInt);
            Assert.Equal(42, value.AsInt());
        }
        {
            PcfPropertyValue value = "hello";
            Assert.True(value.IsString);
            Assert.Equal("hello", value.AsString());
        }
        {
            var value = (int)new PcfPropertyValue(42);
            Assert.Equal(42, value);
        }
        {
            var value = (string)new PcfPropertyValue("hello");
            Assert.Equal("hello", value);
        }
        {
            Assert.Throws<InvalidOperationException>(() => (string)new PcfPropertyValue(42));
            Assert.Throws<InvalidOperationException>(() => (int)new PcfPropertyValue("hello"));
        }
    }

    [Fact]
    public void TestEquals()
    {
        {
            var a = new PcfPropertyValue(42);
            var b = new PcfPropertyValue(42);
            Assert.True(a.Equals(b));
            Assert.True(a.Equals((object)b));
            Assert.True(a == b);
            Assert.False(a != b);
        }
        {
            var a = new PcfPropertyValue(1);
            var b = new PcfPropertyValue(42);
            Assert.False(a.Equals(b));
            Assert.False(a.Equals((object)b));
            Assert.False(a == b);
            Assert.True(a != b);
        }
        {
            var a = new PcfPropertyValue("hello");
            var b = new PcfPropertyValue("hello");
            Assert.True(a.Equals(b));
            Assert.True(a.Equals((object)b));
            Assert.True(a == b);
            Assert.False(a != b);
        }
        {
            var a = new PcfPropertyValue("hello");
            var b = new PcfPropertyValue("world");
            Assert.False(a.Equals(b));
            Assert.False(a.Equals((object)b));
            Assert.False(a == b);
            Assert.True(a != b);
        }
        {
            var a = new PcfPropertyValue(42);
            var b = new PcfPropertyValue("hello");
            Assert.False(a.Equals(b));
            Assert.False(a.Equals((object)b));
            Assert.False(a == b);
            Assert.True(a != b);
        }
        {
            var a = new PcfPropertyValue("hello");
            var b = new PcfPropertyValue(42);
            Assert.False(a.Equals(b));
            Assert.False(a.Equals((object)b));
            Assert.False(a == b);
            Assert.True(a != b);
        }
    }

    [Fact]
    public void TestGetHashCode()
    {
        {
            var a = new PcfPropertyValue(42);
            var b = new PcfPropertyValue(42);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }
        {
            var a = new PcfPropertyValue("hello");
            var b = new PcfPropertyValue("hello");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }
        {
            var a = new PcfPropertyValue(42);
            var b = new PcfPropertyValue("hello");
            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }

    [Fact]
    public void TestToString()
    {
        {
            var value = new PcfPropertyValue(42);
            Assert.Equal("42", value.ToString());
        }
        {
            var value = new PcfPropertyValue("hello");
            Assert.Equal("hello", value.ToString());
        }
    }
}
