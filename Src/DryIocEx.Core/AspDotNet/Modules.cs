using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DryIocEx.Core.AspDotNet;

public static class ModuleInitializerExtensions
{
    /// <summary>
    ///     每个项目中都可以自己写一些实现了IModuleInitializer接口的类，在其中注册自己需要的服务，这样避免所有内容到入口项目中注册
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    public static IServiceCollection RunModuleInitializers(this IServiceCollection services,
        IEnumerable<Assembly> assemblies)
    {
        foreach (var asm in assemblies)
        {
            var types = asm.GetTypes();
            var moduleInitializerTypes =
                types.Where(t => !t.IsAbstract && typeof(IModuleInitializer).IsAssignableFrom(t));
            foreach (var implType in moduleInitializerTypes)
            {
                var initializer = (IModuleInitializer?)Activator.CreateInstance(implType);
                if (initializer == null) throw new ApplicationException($"Cannot create ${implType}");
                initializer.Initialize(services);
            }
        }

        return services;
    }
}

/// <summary>
///     所有项目中的实现了IModuleInitializer接口都会被调用，请在Initialize中编写注册本模块需要的服务。
///     一个项目中可以放多个实现了IModuleInitializer的类。不过为了集中管理，还是建议一个项目中只放一个实现了IModuleInitializer的类
/// </summary>
public interface IModuleInitializer
{
    public void Initialize(IServiceCollection services);
}