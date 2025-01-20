using System;

namespace DryIocEx.Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class KeyAttribute : Attribute
{
    public KeyAttribute(int key, string group)
    {
        throw new NotImplementedException();
    }

    public int Key { set; get; }

    public string Group { set; get; }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class NameAttribute : Attribute
{
    public NameAttribute(string name, string group)
    {
        throw new NotImplementedException();
    }

    public string Name { set; get; }

    public string Group { set; get; }
}