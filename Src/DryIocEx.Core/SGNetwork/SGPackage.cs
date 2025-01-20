#if NET


using System;
using System.Linq;
using System.Threading;
using DryIocEx.Core.Common;

namespace DryIocEx.Core.SGNetwork;

public struct SGPackageBuilder
{
    private readonly SGPackage package;

    public SGPackageBuilder(SGPackage package)
    {
        throw new NotImplementedException();
    }

    public SGPackageBuilder Set<T>(string str, T value)
    {
        throw new NotImplementedException();
    }

    public SGPackageBuilder Set(Action<SGPackage> action)
    {
        throw new NotImplementedException();
    }

    public SGPackage Build()
    {
        throw new NotImplementedException();
    }
}

public class SGPackage : IDisposable
{
    private static uint staticid;


    private readonly bool Disposable = false;


    public SGPackage()
    {
        throw new NotImplementedException();
    }

    public uint ID { set; get; }

    public int PFC { set; get; }

    public KeyValueCollection Data { get; }

    internal static IPool<SGPackage> Pool { get; } = new SGPackagePool();

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public static SGPackageBuilder Create(Action<SGPackage> action = null)
    {
        throw new NotImplementedException();
    }

    public void Initial()
    {
        throw new NotImplementedException();
    }
}

public class SGPackagePool : Pool<SGPackage>
{
    public SGPackagePool()
    {
        throw new NotImplementedException();
    }

    public override void Return(SGPackage sub)
    {
        throw new NotImplementedException();
    }


    public override SGPackage Rent()
    {
        throw new NotImplementedException();
    }
}

public class SGPFCStore
{
    public const int ACK = 1;
    public const int HeartBeat = 2;
}

#endif