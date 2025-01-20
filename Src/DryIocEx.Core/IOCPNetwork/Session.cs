using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuddenGale.Core.Event;
using SuddenGale.Core.IOC;

namespace SuddenGale.Core.IOCPNetwork
{
    public class SessionEventStop<TPackage> : PubSubEvent<ISession<TPackage>, StopReason>
    {

    }

    public interface ISession<TPackage>
    {
        bool IsStop { get; }
        SessionEventStop<TPackage> Stopped { get; }
        ValueTask SendAsync(TPackage package);
        ValueTask SendAsync(byte[] buffer);
        void Start();
        void Stop(StopReason reason = StopReason.LocalClosing);
        IAsyncEnumerable<TPackage> RunAsync();
        ValueTask<TPackage> ReceiveAsync();
    }


    public class Session<TPackage>:ISession<TPackage>
    {
        public bool IsStop => _channel.IsStop;
        public IContainer Container { get;  }

        private IChannel<TPackage> _channel;

        public SessionEventStop<TPackage> Stopped { get; }
        public Session(IChannel<TPackage> channel)
        {
            Container = channel.Container;
            _channel = channel;
            Stopped=new SessionEventStop<TPackage>();
            _channel.Stopped.Subscribe(OnStopped,EnumThreadType.Publisher,keepalive:false);
        }

        private void  OnStopped(IChannel<TPackage> channel, StopReason reason)
        {
            Stopped.Publish(this,reason);
        }

        public ValueTask SendAsync(TPackage package)
        {
            return _channel.SendAsync(package);
        }

        public ValueTask SendAsync(byte[] buffer)
        {
            return _channel.SendAsync(buffer);
        }

        public void Start()
        {
            _channel.Start();
        }

        public void Stop(StopReason reason = StopReason.LocalClosing)
        {
            _channel.Stop(reason);
        }

        public IAsyncEnumerable<TPackage> RunAsync()
        {
            return _channel.RunAsync();
        }

        public ValueTask<TPackage> ReceiveAsync()
        {
            return _channel.ReceiveAsync();
        }
    }
}
