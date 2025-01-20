#if NET


using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DryIocEx.Core.Network
{
    public interface IServerMiddleware<TPackage>
    {
        IServer Server { get; set; }
        int Order { get; }
        void Start();
        void Stop();
        ValueTask<bool> Register(ISession<TPackage> channel);
        ValueTask<bool> Unregister(ISession<TPackage> channel);

        ValueTask Handle(ISession<TPackage> session, TPackage pacakge);
    }


    public class BaseSessionMiddle<TPackage>
    {
        public IServer Server { get; set; }
        public int Order { get; }

        public virtual void Start()
        {
        }


        public virtual void Stop()
        {
        }

        public virtual ValueTask<bool> Register(ISession<TPackage> channel)
        {
            throw new NotImplementedException();
        }

        public virtual ValueTask<bool> Unregister(ISession<TPackage> channel)
        {
            throw new NotImplementedException();
        }

        public virtual ValueTask Handle(ISession<TPackage> session, TPackage pacakge)
        {
            throw new NotImplementedException();
        }
    }


    public interface IUdpSessionContainerMiddleware<TPackage> : IServerMiddleware<TPackage>
    {
        ISession GetSession(string key);
    }

    public class UdpSessionMiddleware<TPackage> : BaseSessionMiddle<TPackage>, IUdpSessionContainerMiddleware<TPackage>
    {
        private readonly Dictionary<string, ISession<TPackage>> _sessions = new();


        public override ValueTask<bool> Register(ISession<TPackage> session)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<bool> Unregister(ISession<TPackage> session)
        {
            throw new NotImplementedException();
        }

        public ISession GetSession(string key)
        {
            throw new NotImplementedException();
        }


        private string GetSessionKey(ISession<TPackage> session)
        {
            throw new NotImplementedException();
        }
    }
}
#endif