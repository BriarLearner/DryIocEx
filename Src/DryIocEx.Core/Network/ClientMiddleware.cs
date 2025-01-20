#if NET


using System;
using System.Threading.Tasks;

namespace DryIocEx.Core.Network
{
    public interface IClientMiddleware<TPackage>
    {
        int Order { get; }

        IClient Client { get; set; }

        ValueTask<bool> Connected(ISession<TPackage> session);

        ValueTask<bool> Disconnected(ISession<TPackage> session);
    }

    public interface IReconnectMiddleware<TPackage> : IClientMiddleware<TPackage>
    {
    }


    public class ClientReconnectMiddleware<TPackage> : IReconnectMiddleware<TPackage>
    {
        public int Order { get; }
        public IClient Client { get; set; }

        public ValueTask<bool> Connected(ISession<TPackage> session)
        {
            return ValueTask.FromResult(true);
        }

        public ValueTask<bool> Disconnected(ISession<TPackage> session)
        {
            throw new NotImplementedException();
        }
    }
}
#endif