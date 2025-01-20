using System;
using System.Collections.Generic;
using DryIocEx.Core.IOC;

namespace DryIocEx.Core.Manager;

public interface IOption
{
}

public interface IOptionManager
{
    TOption GetOption<TOption>() where TOption : IOption;
    void RegisterOption<T>(T option) where T : IOption;
}

public class OptionManager : IOptionManager
{
    private readonly Dictionary<Type, OptionInfo> _OptionDict = new();


    public TOption GetOption<TOption>() where TOption : IOption
    {
        throw new NotImplementedException();
    }

   


    public void RegisterOption<T>(T option) where T : IOption
    {
        throw new NotImplementedException();
    }


    private class OptionInfo
    {
        private IOption _instance;

        public OptionInfo(Func<IOption> factory)
        {
            throw new NotImplementedException();
        }

        public Func<IOption> Factory { get; }

        public IOption Instance
        {
            get=> throw new NotImplementedException();
        }
    }
}

public static class OptionLocator
{
    private static IOptionManager _manager;

    public static IOptionManager Manager
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public static void SetOptionManager(IOptionManager manager)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
///     添加该特性会自动注入到ToolStoreStore,必须实现无产构造函数
///     会注入第一个接口
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class OptionAttribute : Attribute
{
}

public static class OptionExtension
{
    public static TOption GetOption<TOption>(this object obj) where TOption : IOption
    {
        throw new NotImplementedException();
    }

    public static void RegisterOption<TOption>(this IContainer container, TOption option) where TOption : IOption
    {
        throw new NotImplementedException();
    }

    public static void RegisterOption<TOption>(this IOptionManager store, TOption option) where TOption : IOption
    {
        throw new NotImplementedException();
    }
}