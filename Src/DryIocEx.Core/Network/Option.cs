#if NET


using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace DryIocEx.Core.Network;

public interface IServerOption
{
    string Name { get; set; }
}

public class ServerOption : IServerOption
{
    /// <summary>
    ///     清除空闲Session，定时时间
    ///     如果为0就不进行检测
    ///     单位S
    /// </summary>
    public int ClearIdleSessionInterval { set; get; } = 0;

    /// <summary>
    ///     多长时间判定为空闲
    ///     默认300S
    /// </summary>
    public int IdleSessionTimeOut { set; get; } = 300;


    public string Name { set; get; }
}

public interface IChannelCreatorOption
{
}

public class ChannelCreatorOption : IChannelCreatorOption
{
    public string Ip { get; set; }

    public int Port { get; set; }

    /// <summary>
    ///     指定流 Socket 是否正在使用 Nagle 算法
    /// </summary>
    /// <remarks>
    ///     Nagle 算法旨在通过使套接字缓冲小数据包，然后在特定情况下将它们合并并发送到一个数据包，从而减少网络流量。 TCP 数据包包含40字节的标头以及要发送的数据。 当使用 TCP 发送小型数据包时，TCP
    ///     标头产生的开销可能会成为网络流量的重要部分。 在负载较重的网络上，由于这种开销导致的拥塞会导致丢失数据报和重新传输，以及拥塞导致的传播时间过大。 如果在连接上以前传输的数据保持未确认的情况，则 Nagle 算法将禁止发送新的
    ///     TCP 段.大多数网络应用程序都应使用 Nagle 算法。(UDP) 套接字在用户数据报协议上设置此属性将不起作用。
    /// </remarks>
    public bool NoDelay { get; set; }
}

/// <summary>
///     看成ChannelCreatorOption
/// </summary>
public class TcpChannelCreatorOption : ChannelCreatorOption
{
    /// <summary>
    ///     挂起连接队列的最大长度
    /// </summary>
    public int BackLog { set; get; } = 100;

    public SslProtocols Security { get; set; }
    public CertificateOption CertificateOptions { get; set; }
}

public class CertificateOption
{
    public X509Certificate Certificate { get; set; }

    /// <summary>
    ///     Gets the certificate file path (pfx).
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    ///     Gets the password.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    ///     Gets the the store where certificate locates.
    /// </summary>
    /// <value>
    ///     The name of the store.
    /// </value>
    public string StoreName { get; set; } = "My"; //The X.509 certificate store for personal certificates.

    /// <summary>
    ///     Gets the thumbprint.
    /// </summary>
    public string Thumbprint { get; set; }


    /// <summary>
    ///     Gets the store location of the certificate.
    /// </summary>
    /// <value>
    ///     The store location.
    /// </value>
    public StoreLocation StoreLocation { get; set; } =
        StoreLocation.CurrentUser; //The X.509 certificate store used by the current user.

    /// <summary>
    ///     Gets a value that will be used to instantiate the X509Certificate2 object in the CertificateManager
    /// </summary>
    public X509KeyStorageFlags KeyStorageFlags { get; set; }

    /// <summary>
    ///     Gets a value indicating whether [client certificate required].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [client certificate required]; otherwise, <c>false</c>.
    /// </value>
    public bool ClientCertificateRequired { get; set; }

    public RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; set; }
}

#endif