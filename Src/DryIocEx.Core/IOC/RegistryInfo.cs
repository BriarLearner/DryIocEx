using System;
using System.Collections.Generic;
using System.Linq;

namespace DryIocEx.Core.IOC;
/// <summary>
/// 容器扩展
/// </summary>
internal static class IOCInfoExtension
{
    public static KeyInfo ToKeyInfo(this Type type)
    {
        return ToKeyInfo(type, string.Empty);
    }

    public static KeyInfo ToKeyInfo(this Type type, string name)
    {
        return new KeyInfo(type, name);
    }

    public static InstanceKey ToInstanceKey(this RegistryInfo registry, Type[] genericargs)
    {
        return new InstanceKey(registry, genericargs);
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
        KeyInfo = keyinfo;
        Lifetime = lifetime;
        Factory = factory;
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
        for (var self = this; self != null; self = self.Next) yield return self;
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
        Name = name;
        FromType = fromType;
    }

    public string Name { set; get; }

    public Type FromType { set; get; }

    public bool Equals(KeyInfo other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Equals(FromType, other.FromType);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((KeyInfo)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (FromType != null ? FromType.GetHashCode() : 0);
        }
    }

    public static bool operator ==(KeyInfo left, KeyInfo right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(KeyInfo left, KeyInfo right)
    {
        return !Equals(left, right);
    }
}

public class InstanceKey : IEquatable<InstanceKey>
{
    public InstanceKey(RegistryInfo registryInfo, Type[] genericArgs)
    {
        GenericArgs = genericArgs;
        RegistryInfo = registryInfo;
    }

    public Type[] GenericArgs { set; get; }
    public RegistryInfo RegistryInfo { set; get; }

    public bool Equals(InstanceKey other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (RegistryInfo != other.RegistryInfo) return false;
        if (GenericArgs.Length != other.GenericArgs.Length) return false;
        for (var i = 0; i < GenericArgs.Length; i++)
            if (GenericArgs[i] != other.GenericArgs[i])
                return false;
        return true;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((InstanceKey)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = RegistryInfo.GetHashCode();
        return GenericArgs.Aggregate(hashCode, (current, t) => current ^ t.GetHashCode());
    }

    public static bool operator ==(InstanceKey left, InstanceKey right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(InstanceKey left, InstanceKey right)
    {
        return !Equals(left, right);
    }
}