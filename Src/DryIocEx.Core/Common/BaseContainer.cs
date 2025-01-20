using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DryIocEx.Core.IOC;

/// <summary>
///     让类继承的的简单容器
///     为了让Option能够注入各种泛型类，同时兼容各种扩展
///     如果在Action中要么使用容器，太重了
///     要么暴露各种泛型构建
///     这个方式相对优雅
/// </summary>
public abstract class BaseContainer<Self>
    where Self : BaseContainer<Self>
{
    private readonly ConcurrentDictionary<BaseInstanceKey, object> _instances = new();


    private readonly ConcurrentDictionary<BaseKeyInfo, BaseRegistryInfo> _registries = new();


    private object Create(Self self, Type totype, Type[] genericargs)
    {
        throw new NotImplementedException();
    }


    private Self Register(BaseRegistryInfo baseRegistry)
    {
        throw new NotImplementedException();
    }

    private object Resolve(BaseKeyInfo baseKey)
    {
        throw new NotImplementedException();
    }

    private object InnerGetService(BaseRegistryInfo baseRegistry, Type[] genericargs)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     创建对象信息
    ///     并不是所有对象都需要具体保存，也可以封装工厂
    /// </summary>
    private class BaseRegistryInfo
    {
        public BaseRegistryInfo(BaseKeyInfo keyinfo, EnumBaseLifetime baseLifetime, Func<Self, Type[], object> factory)
        {
            throw new NotImplementedException();
        }

        public BaseKeyInfo BaseKeyInfo { get; }

        public EnumBaseLifetime BaseLifetime { get; }

        /// <summary>
        ///     这边Factory内部封装了(container,type[])=> return Create(container,ToType,genericargs)
        /// </summary>
        public Func<Self, Type[], object> Factory { get; }


        public BaseRegistryInfo Next { set; get; }

        public IEnumerable<BaseRegistryInfo> AsEnumerable()
        {
            throw new NotImplementedException();
        }
    }

    private class BaseInstanceKey : IEquatable<BaseInstanceKey>
    {
        public BaseInstanceKey(BaseRegistryInfo baseRegistryInfo, Type[] genericArgs)
        {
            throw new NotImplementedException();
        }

        public Type[] GenericArgs { get; }
        public BaseRegistryInfo BaseRegistryInfo { get; }

        public bool Equals(BaseInstanceKey other)
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

        public static bool operator ==(BaseInstanceKey left, BaseInstanceKey right)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(BaseInstanceKey left, BaseInstanceKey right)
        {
            throw new NotImplementedException();
        }
    }


    private class BaseKeyInfo : IEquatable<BaseKeyInfo>
    {
        public BaseKeyInfo(Type fromType) : this(fromType, string.Empty)
        {
        }

        /// <summary>
        ///     创建对象Key
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="name"></param>
        public BaseKeyInfo(Type fromType, string name)
        {
            throw new NotImplementedException();
        }

        public string Name { get; }

        public Type FromType { get; }

        public bool Equals(BaseKeyInfo other)
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

        public static bool operator ==(BaseKeyInfo left, BaseKeyInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseKeyInfo left, BaseKeyInfo right)
        {
            return !Equals(left, right);
        }
    }

    #region PublicBuilder

    public Self Register(Type from, Type to, EnumBaseLifetime lifetime, string name = "")
    {
        throw new NotImplementedException();
    }


    public Self Register<TFrom, TTo>(EnumBaseLifetime lifetime, string name = "")
    {
        throw new NotImplementedException();
    }


    public Self Register(Type fromto, EnumBaseLifetime lifetime, string name = "")
    {
        throw new NotImplementedException();
    }


    public Self Register<TFromTo>(EnumBaseLifetime lifetime, string name = "")
    {
        throw new NotImplementedException();
    }

    public Self Register(Type fromtype, object instance, string name = "")
    {
        throw new NotImplementedException();
    }


    public Self Register<TFrom>(TFrom instance, string name = "")
    {
        throw new NotImplementedException();
    }

    public Self Register(Type fromtype, Func<Self, object> factory, EnumBaseLifetime lifetime, string name = "")
    {
        throw new NotImplementedException();
    }


    public Self Register<TFrom>(Func<Self, TFrom> factory,
        EnumBaseLifetime lifetime, string name = "")
    {
        throw new NotImplementedException();
    }

    public TFrom Resolve<TFrom>(string name = "")
    {
        throw new NotImplementedException();
    }

    public IEnumerable<TFrom> Resolves<TFrom>(string name = "")
    {
        throw new NotImplementedException();
    }

    #endregion
}

public enum EnumBaseLifetime
{
    Singleton,
    Transient
}