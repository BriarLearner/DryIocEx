using System;
using System.Collections.Generic;
using System.Linq;

namespace DryIocEx.Core.IOC;

internal static class IOCInfoExtension
{
    public static KeyInfo ToKeyInfo(this Type type)
    {
        throw new NotImplementedException();
    }

    public static KeyInfo ToKeyInfo(this Type type, string name)
    {
        throw new NotImplementedException();
    }

    public static InstanceKey ToInstanceKey(this RegistryInfo registry, Type[] genericargs)
    {
        throw new NotImplementedException();
    }
}

public enum EnumLifetime
{
    Singleton,
    Scoped,
    Transient
}

/// <summary>
///     创建对象信息
///     并不是所有对象都需要具体保存，也可以封装工厂
/// </summary>
public class RegistryInfo
{
    public RegistryInfo(KeyInfo keyinfo, EnumLifetime lifetime, Func<IContainer, Type[], object> factory)
    {
        throw new NotImplementedException();
    }

    public KeyInfo KeyInfo { get; set; }

    public EnumLifetime Lifetime { set; get; }

    /// <summary>
    ///     这边Factory内部封装了(container,type[])=> return Create(container,ToType,genericargs)
    /// </summary>
    public Func<IContainer, Type[], object> Factory { set; get; }


    public RegistryInfo Next { set; get; }

    public IEnumerable<RegistryInfo> AsEnumerable()
    {
        throw new NotImplementedException();
    }
}

public class KeyInfo : IEquatable<KeyInfo>
{
    public KeyInfo(Type fromType) : this(fromType, string.Empty)
    {
    }

    /// <summary>
    ///     创建对象Key
    /// </summary>
    /// <param name="fromType"></param>
    /// <param name="name"></param>
    public KeyInfo(Type fromType, string name)
    {
        throw new NotImplementedException();
    }

    public string Name { set; get; }

    public Type FromType { set; get; }

    public bool Equals(KeyInfo other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object obj)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public static bool operator ==(KeyInfo left, KeyInfo right)
    {
        throw new NotImplementedException();
    }

    public static bool operator !=(KeyInfo left, KeyInfo right)
    {
        throw new NotImplementedException();
    }
}

public class InstanceKey : IEquatable<InstanceKey>
{
    public InstanceKey(RegistryInfo registryInfo, Type[] genericArgs)
    {
        throw new NotImplementedException();
    }

    public Type[] GenericArgs { set; get; }
    public RegistryInfo RegistryInfo { set; get; }

    public bool Equals(InstanceKey other)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object obj)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public static bool operator ==(InstanceKey left, InstanceKey right)
    {
        throw new NotImplementedException();
    }

    public static bool operator !=(InstanceKey left, InstanceKey right)
    {
        throw new NotImplementedException();
    }
}