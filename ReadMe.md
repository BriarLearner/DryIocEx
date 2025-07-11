# DryIocEx

包含DryIocEx.Core和DryIocEx.Prism

DryIocEx.Core:参考DryIoc

DryIocEx.Prism:参考Prism

## 提示

1. 非必要所有功能写在前面，这样只要输入相关功能就能智能提示，虽然有一点反人类，但是用习惯之后提高效率

2. 每个组件的一般接口写在一个Interfaces文件里

3. 每个组件的一般扩展写在一个Extensions文件里

4. 为了不破坏原来的程序架构，如果改变接口都会加上Pro，这种方式不太好，通过以下方式进行约束
   1. 通过版本管理
   2. 尽量不改变接口命令，至少优化内部逻辑，或者将内部逻辑封装成Core/Internal方法
   3. 尽量实现接口

# 组件

## Algorithm

**ObjectPipe**

异步读取队列，线程安全，内部实现IAsyncEnumerable异步读取

**SupplyPipe**

主要逻辑入下图所示，在Write前调用`await supplyrequire`会抑制写的速度，使得写的速度与读的速度保持一致。

## Base

**BaseDispose//Dispose基类**

抽象方法

+ void CleanUnmanaged();//清理非托管资源

+ void CleanManaged();//清理托管资源

**BaseSave//保存基类**

System.Private.CoreLib.dll:

+ Converter<String,String>转换委托

属性：

+ long Id { get; }//Id自增

如果没有文件，构造函数中创建文件目录和默认文件

抽象方法

+ string ToSaveString();//需要保存的字符串
+ void LoadByString(string content);//加载读取的字符串

方法：

+ void LoadByFile(Converter<string, string> decrypt)//从文件加载方法
+ void SaveToFile(Converter<string, string> encrypt)//保存到文件

**Singleton\<T>//单例模式**

将单例和具体类分开，既可以作为基类，也可以作为包装类

属性：

T Current{get}//获取实例

枚举：

EnumSingleCreateRace

+ Optimistic//使用原子锁Interlocked.CompareExchange创建
+ Block//使用lock创建

**SingletonManager//单例集合**

内部放了EventManager和FactoryLogNet单例

静态方法：

+ void Register\<T>(Singleton\<T>.Creater creater)//注入单例
+ void Register\<T>(string name, Singleton\<T>.Creater creater)//名称单例
+ T Resolve\<T>(string name)//解析实例
+ T Resolve\<T>()//解析实例

## Events

**Events**

DelegateReference:委托的引用

+ 构造函数传入委托和keepAlive
+ Delegate Method属性：

SubscriptionToken：Token用来标识DelegateReference

+ unsubscribeaction:在取消订阅的时候调用方法

EventSubscription:包装DelegateReference和SubscriptionToken

+ 执行逻辑封装
+ Action<Object[]> GetExecutionAction();//获取订阅的方法并包装成Action<Object[]>
+ void InvokeAction(Action action);//执行逻辑

DispatcherEventSubscription:包装SynchronizationContext

BaseEvent:集合IEventSubscription

+ InternalSubscribe(IEventSubscription eventsubscription)方法，添加到EventSubscription集合中，如果Dispose从集合中移除
+ InternalPublish(params object[] arguments)：先修建集合

PubSubEvent:包装Event,所有实现的基类

+ Subscribe(Action action),默认发布线程，默认keepalive为false
+ Unsubscribe(Action action):取消订阅

EventManager:BaseEvent字典，类名，事件

+  GetEvent\<TEventType>();//方法

## IOC

由于dotnet中实例是没办法计算自身被引用的次数，所以就没办法做一个定时器，当检测到该对象引用次数只有1的时候自动销毁该对象，所以只能在Container销毁的时候销毁其创建的所有对象，所以通过容器创建的实现Dispose对象不要自己调用dispose。所以相关组件最好创建ContainerScope

**核心实现**

**ContainerExtensions**

对Container的扩展

**ContainerManager**

框架相关

**ContainerServiceProviderFactory**

接入AspNet依赖注入框架类

**AutoInjectionAttribute**

特性，如果载入Assembly会自动注入

**InjectionAttribute**

自动注入类时会优先加载这个特性，如果没有，则会将在所有构造函数的第一个

ContainerLocator 容器定位模式

```C#
ContainerLocator.Register()
ContainerLocator.Resolve()
this.Register()
this.Resolve()
扩展方法实现解析和注入
```



## Locks

**FactoryHybridLock//所工厂**

**LockHybrid//简单混合锁**

```C#
//使用
Enter
Leave
```

**LockSpinHybrid//简单自旋锁**

**LockSpinRecursionHybrid//自选递归锁**

## LogNet

**BaseLogNet//日志基类**

属性

+ _fileLock：文件锁
+ _queue：写入队列
+ Filter:过滤器

方法

+ Log(string text, EnumLogDegree degree)
+ startSaveFile();//启用线程保存日志
+ GetFileName();//获取文件名

**LogDate//日志子类，按日期**

```C#
GregorianCalendar gc = new GregorianCalendar();
int weekOfYear = gc.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
logname = header + DateTime.Now.Year + "_W" + weekOfYear;
```

**LogSingle//日志子类，单个文件**

**LogSize//日志子类，单个大小**

**UtilLog//日志扩展方法**

```C#
        public static void LogInfo(string info, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var fileinfo = new FileInfo(sourceFilePath);
            SingletonManager.Resolve<IEventManager>().GetEvent<EventLog>()
                .Publish($"[{fileinfo.Name}] [{memberName}()] [{sourceLineNumber}] {info}", EnumLogDegree.Info);
        }

        public static void Log(string info, EnumLogDegree degree, [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var fileinfo = new FileInfo(sourceFilePath);
            SingletonManager.Resolve<IEventManager>().GetEvent<EventLog>()
                .Publish($"[{fileinfo.Name}] [{memberName}()] [{sourceLineNumber}] {info}", degree);
        }

        public static void LogFatal(string info, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var fileinfo = new FileInfo(sourceFilePath);
            SingletonManager.Resolve<IEventManager>().GetEvent<EventLog>()
                .Publish($"[{fileinfo.Name}] [{memberName}()] [{sourceLineNumber}] {info}", EnumLogDegree.Fatal);
        }

        public static void LogDebug(string info, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var fileinfo = new FileInfo(sourceFilePath);
            SingletonManager.Resolve<IEventManager>().GetEvent<EventLog>()
                .Publish($"[{fileinfo.Name}] [{memberName}()] [{sourceLineNumber}] {info}", EnumLogDegree.Debug);
        }
        public static void LogWarn(string info, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var fileinfo = new FileInfo(sourceFilePath);
            SingletonManager.Resolve<IEventManager>().GetEvent<EventLog>()
                .Publish($"[{fileinfo.Name}] [{memberName}()] [{sourceLineNumber}] {info}", EnumLogDegree.Warn);
        }
```

## Network

**Server**

服务器

**Session**

统一会话层，封装各类业务层与channel业务交互

**Channel**

连路包装，用于封装各类连路，调用解析器处理粘包，分包等问题，封装Socket，Serial等

**Listener**

服务监听包装

**Client**

客户端

**Connector**

客户连接实现

## Broker

**BrokerCacheOperator\<T>//耗时操作队列**

构造函数传入处理实体的委托Action\<T> operate

AddCacheEntry(T entry),添加实体，单线程操作实体

**BrokerTimeWatcher//计时器**

方法：

+ void Start();//开始
+ void Stop();//结束
+ string GetStatistics();//获取统计信息
+ void WriteStatistics();//打印统计信息

+ Timer(string name, int iteration, Action action);//时间显示

## Util

**UtilStream//操作Stream**

静态方法

+ Copy(Stream source, byte[] destination, int offset, int count)//从流读取数据到数组中
+ Copy(Stream source, Stream destination, byte[] buffer)//将Source流中数据读取到数组中

+ Copy(Stream source, Stream destination, byte[] buffer, ProgressHandler progressHandler,
      TimeSpan updateInternal, object sender, string name)
  + 使用了ProgressEventArgs事件参数，该参数对象带三个属性，名称，进度，目标值
  + ProgressHandler:updateInternal时间到了之后使用该委托progressHandler(sender, args);

**UtilByte//字节**

方法

+ IsBytesEqual：判断字节是否相等
+ GetUshortArray：bytes转换成2个字节的ushort的数组，会大小端转换
+ GetShortArray:bytes转换成2个字节的short的数组，会大小端转换
+ GetBoolArray：bytes转换成bool的数组
+ GetByteArray：Bool转化成Bytes数组
+ GetBitString:将Bool转换成String，可以添加分割符
+ GetHexString:将bytes转换成Hex
+ GetFullHexString:将bytes转换成Hex 加上0x
+ GetHexString：将字符串转换成HexString
+ GetByteArray:将Hex字符串转换成Byte数组

**UtilCallContext//线程槽**

需要重新设计

+ void SetData(string name, object data);//设置值
+ object GetData(string name)获取值

**UtilConsole//控制台打印不同颜色**

静态方法方法

+ WriteColorLine(string str, ConsoleColor color)
+ void WriteErrorLine(this string str, ConsoleColor color = ConsoleColor.Red)
+ void WriteWarningLine(this string str, ConsoleColor color = ConsoleColor.Yellow)
+ void WriteInfoLine(this string str, ConsoleColor color = ConsoleColor.White)
+ void WriteSuccessLine(this string str, ConsoleColor color = ConsoleColor.Green)
+ void WriteConfigLine(this string str, ConsoleColor color = ConsoleColor.Cyan)

**UtilConverter//转换**

+ ToBool//字符串
+ ToBool//object
+ IsNotEmptyOrNull//不为空

**UtilDataTime//日期**

+ GetFormString(this DateTime date);//date.ToString("yyyy-MM-dd HH:mm:ss");
+ GetPrecisionString(this DateTime date);//date.ToString("yyyy-MM-dd HH:mm:ss:fff");
+ GetUnderLineString(this DateTime date);//date.ToString("yyyy_MM_dd_HH_mm_ss");

**UtilDesEncrypt//加解密DESCrypto对称**

+ GetDesEncrypt();//字符串加密
+ GetDesDecrypt();//字符串解密

**UtilEndian//字符帮助类**

静态方法 字序转化

+ GetBoolsByByte//单字节转化成Bool[8]
+ GetBoolsByInt16//

**UtilFiles//文件操作类**

**UtilGuard//参数简单验证**

**UtilGUID//获取GUID**

**UtilMD5//MD5操作**

**UtilReflection//反射操作**

+ LoadSpecialInstanceByAssemblyPath\<T>:获取Assembly中所有T的子类
+ LoadSpecialInstanceByAssembly\<T>：从程序集中加载ClassDependencyAttribute特性，所有类都会创建

**UtilSaveXML//保存到XML**

+ void WritXMLFile\<T>(T entity,string filepath)：将实例的属性转化成XML保存到文件
+ T ReadXMLFile<T>(string filepath)：将XML转换成实例

**UtilSaveJson//保存json文件**

+ WriteJsonFile\<T>(T entity,string filepath)//使用JsonConvert.SerializeObject
+ T ReadJsonFile\<T>(string filepath)//读取json字符串为实例

**UtilSimpleEncrypt//简单加密解密**

+ GetSimpleEncrypt(this byte[] enBytes)//简单加密
+ GetSimpleDecrypt(this byte[] deBytes)//简单解密

**UtilSingle//单个实例**

+ CheckSingleton(string processname, out bool exit)//通过Mutex检测单例

**UtilTask//tasklog**

+ Log\<TResult>();//task完成日志
+ AsAsync();//异步

使用

```C#
        public async Task UtilTaskTest()
        {
            Task.Delay(2000).ContinueWith((t) =>
            {
                UtilTask.CompletedAsync("SimpleTest");
            }).Continue();
            DateTime.Now.GetPrecisionString().WriteConfigLine();
            await UtilTask.AsAsync("SimpleTest",1000);
            DateTime.Now.GetPrecisionString().WriteConfigLine();
        }
```

+ WithCancellation(this Task task, CancellationToken ct)//展示了取消，传统的取消需要在逻辑代码中不停的检查ThrowIfCancellationRequested
+ AddTask(Func\<Task> taskfactory, bool start)
+ RunTasks(int maxDegreeOfParallelism = 8)

```C#
foreach (var photo in photos)
{
    UtilTask.AddTask(async () =>
    {
        await Download(photo);
    });
}

foreach (var photo in photos)
{
    UtilTask.AddTask(async () =>
    {
        await Download(photo);
    }, startTheEngine: false); // Don't start the task pool.
}

// Finally, start the task pool with 30 tasks running at the same time.
await UtilTask.RunTasks(30);
```

**UtilXML//设置XML**

**UtilValidator//验证类**

验证邮箱

验证网址

验证日期

验证手机号

验证IP

验证身份证

验证邮政

验证小时

验证数字

验证中文

# ASTM通讯组件

## 使用

```
var Server = new ASTMServer();//创建server，该服务实现链路连接通知，数据包收发
 Server.SetSend(5, 10); //发送间隔设置和发送消息超时时间设置
 Server.ConnectState += Server_ConnectState;//链路连接通知
 Server.ReceiveGroupPackage += Server_ReceiveGroupPackage;//包接收通知
 //创建服务,服务Ip和端口 Tag是服务标识key，当创建多个服务的时候，用Tag标识来区分服务
 var ipendpoint = CreateEndPoint(ServerIp, ServerPort);
 var client = new HISASTMServerBuilder(Server).UseTcp(ServerIp, ServerPort)
    .UseSession(o => o.Tag = ipendpoint).UseLog(c => logger).Build();
  client.StartAsync();
 //创建客户端
  var client = new HISASTMClientBuilder(server).UseTcp(RemoteIp, RemotePort)
     .UseSession(o => o.Tag = clientendpoint)
     .UseLog(c => logger).Build();
 client.StartAsync();
 
 
 void Server_ConnectState(IPEndPoint endpoint, bool isconnect)
 {
  var ipendpoint = Server.GetSessionOption(endpoint).Tag.To<IPEndPoint>();//获取标识Key
 }
 
 void Server_ReceiveGroupPackage(IPEndPoint endpoint, List<HISASTMPackage> list,
        EnumGroupReason reason)
        {
        var ipendpoint = Server.GetSessionOption(endpoint).Tag.To<IPEndPoint>();//获取标识Key
        }
        
```

## 协议包

### HISASTMPackage

实现ASTM协议，解析包HISASTMPackage，该包采用池化技术，降低内存消耗

```C#
/// <summary>
///     ASTM包
/// </summary>
public class HISASTMPackage : IDisposable
{
    ///创建时间
    public DateTime CreateTime { get; private set; }

    /// <summary>
    ///     帧序号
    /// </summary>
    public int FrameIndex { set; get; }

    /// <summary>
    ///     包类型
    /// </summary>
    public EnumASTMPackageType Type { get; set; }

    /// <summary>
    ///     消息类型 消息结构中
    /// </summary>
    public string Identify { set; get; }

    /// <summary>
    ///     包序号
    /// </summary>
    public int PackageIndex { set; get; }

    /// <summary>
    ///     数据域
    /// </summary>
    public List<string> Fields { get; } = new();
}
```

```C#
var package = HISASTMPackage.Pool.Rent();//创建包
package.Type = type;

使用完后请调用package.Dispose();//返回内存池
```

## 协议解析HISASTMPackageFilter

```C#
public class HISASTMPackageFilter : IPackageFilter<HISASTMPackage>
{
    public HISASTMPackage Filter(ref SequenceReader<byte> reader)
    {
        //1.检测第一个字节是否是 识别的字节
        var type = GetType(reader);
        //2.不是过滤掉
        if (type == EnumASTMPackageType.Unknown) //过滤掉未知数据
        {
            reader.Advance(1);
            return null;
        }

        //3.第一个字节是STX,解析STX
        if (type == EnumASTMPackageType.STX)
        {
#if NET
            var temsequence = reader.UnreadSequence;
#else
            var temsequence = reader.Sequence.Slice(reader.Consumed);
#endif

            var readlean = TryReadTo(temsequence);
            if (readlean < 0)
            {
                reader.Advance(reader.Remaining);
                return null;
            }

            if (readlean == 0) return null;
            reader.Advance(readlean);
            var spack = temsequence.Slice(0, readlean);

            
            if (spack.Length < 10) return null; 
            //3.3验证crc
#if NET
            var crc = Encoding.ASCII.GetString(spack.Slice(spack.Length - 4, 2));
#else
            var crc = Encoding.UTF8.GetString(spack.Slice(spack.Length - 4, 2).ToArray()); 
#endif

            var keyvaluereader = new SequenceReader<byte>(spack.Slice(0, spack.Length - 4)); 
            var sum = 0;
            byte temvalue = 0;
            while (keyvaluereader.TryRead(out temvalue)) sum += temvalue;
            var calcrc = (sum % 0x100).ToString("X2"); 
            if (!calcrc.Equals(crc, StringComparison.OrdinalIgnoreCase)) return null; 
            return Decoder(ref spack, null);
        }

        var package = HISASTMPackage.Pool.Rent();
        package.Type = type;
        reader.Advance(1);
        return package;
    }

    public ReadOnlyMemory<byte> Converter(HISASTMPackage pack)
    {
        if (pack == null) return Array.Empty<byte>();
        switch (pack.Type)
        {
            case EnumASTMPackageType.ENQ:
                return new[] { ASTMConst.ENQ };
            case EnumASTMPackageType.ACK:
                return new[] { ASTMConst.ACK };
            case EnumASTMPackageType.NAK:
                return new[] { ASTMConst.NAK };
            case EnumASTMPackageType.EOT:
                return new[] { ASTMConst.EOT };
            case EnumASTMPackageType.HT:
                return new[] { ASTMConst.HT };
            case EnumASTMPackageType.STX:
                var list = new List<byte>();
                list.Add(ASTMConst.STX);
                list.Add((byte)(pack.FrameIndex % 8).ToString("D1").First());
                list.Add((byte)pack.Identify.ToUpper().First());
                list.Add(ASTMConst.charField);
                list.Add((byte)pack.PackageIndex.ToString("D1").First());

                if (pack.Fields.Any())
                {
                    list.Add(ASTMConst.charField);
                    list.AddRange(Encoding.UTF8.GetBytes(string.Join("|", pack.Fields)));
                }

                list.Add(ASTMConst.CR);
                list.Add(ASTMConst.ETX);
                list.AddRange(Encoding.UTF8.GetBytes((list.Sum(s => s) % 0x100).ToString("X2")));
                list.Add(ASTMConst.CR);
                list.Add(ASTMConst.LF);
                return list.ToArray();
            default:
                return Array.Empty<byte>();
        }
    }

    public void Reset()
    {
    }

    private long TryReadTo(ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        try
        {
            var exit = false;
            do
            {
                var find = reader.TryReadTo(out ReadOnlySequence<byte> spack, ASTMConst.CRLF);
                if (!find)
                {
                    if (reader.Length < 4 * 1024) //一个包的长度不能超过4K
                    {
                        exit = true;
                        return 0; //有可能分包了
                    }

                    exit = true;
                    return -1; //异常包 全部丢弃
                }

                if (spack.Length < 4)
                {
                }
                else
                {
                    var slice = spack.Slice(spack.Length - 4, 2);
                    var temre = new SequenceReader<byte>(slice);
                    temre.TryRead(out var temcr);
                    temre.TryRead(out var temetx);
                    if (temcr == ASTMConst.CR && (temetx == ASTMConst.ETX || temetx == ASTMConst.ETB))
                    {
                        exit = true;
                        return reader.Consumed;
                    }
                }
            } while (!exit);
        }
        catch (Exception e)
        {
        }

        return -1; //异常结束全部丢弃
    }

    private HISASTMPackage Decoder(ref ReadOnlySequence<byte> buffer, object context)
    {
        try
        {
            var content = buffer.Slice(1, buffer.Length - 7);
#if NET
            var parameters = Encoding.UTF8.GetString(content).Split('|');
#else
            var parameters = Encoding.UTF8.GetString(content.ToArray()).Split('|');
#endif

            if (parameters.Length < 2 || parameters[0].Length < 2) return null; //无效包 
            var package = HISASTMPackage.Pool.Rent();
            package.Type = EnumASTMPackageType.STX;
            package.FrameIndex = int.Parse(parameters[0][0].ToString());
            package.Identify = parameters[0][1].ToString();
            package.PackageIndex = int.Parse(parameters[1]);
            if (parameters.Length > 2) package.Fields.AddRange(parameters.Skip(2).ToArray());
            return package;
        }
        catch (Exception e)
        {
        }

        return null;
    }

    private EnumASTMPackageType GetType(SequenceReader<byte> reader)
    {
        if (reader.IsNext(ASTMConst.ENQ)) return EnumASTMPackageType.ENQ;
        if (reader.IsNext(ASTMConst.ACK)) return EnumASTMPackageType.ACK;
        if (reader.IsNext(ASTMConst.NAK)) return EnumASTMPackageType.NAK;
        if (reader.IsNext(ASTMConst.EOT)) return EnumASTMPackageType.EOT;
        if (reader.IsNext(ASTMConst.STX)) return EnumASTMPackageType.STX;
        if (reader.IsNext(ASTMConst.HT)) return EnumASTMPackageType.HT;
        return EnumASTMPackageType.Unknown;
    }
}
```

## ASTMServer

接收发送包

```C#
    public void ReceivePackage(ISession<HISASTMPackage> session, HISASTMPackage package)
    {
        try
        {
            var ipendpoint = session.GetKey<IPEndPoint>();
            if (ipendpoint == null) return;

            try
            {
                ReceiveSinglePackage?.Invoke(ipendpoint, package);
            }
            catch (Exception e)
            {
            }

            if (package.Type == EnumASTMPackageType.ENQ)
            {
                SendACK(session);
                //如果包含旧数据
                if (CacheGroupPackage.ContainsKey(ipendpoint))
                {
                    if (CacheGroupPackage.TryRemove(ipendpoint, out var group))
                    {
                        group.SetGroupPackage(EnumGroupReason.BreakUp);
                    }

                }

                //接收到新的数据开始
                var newgroup = new GroupASTMPackage(ipendpoint, HandleAutoGroup);
                newgroup.AddPackage(package);
                CacheGroupPackage[ipendpoint] = newgroup;
                return;
            }

            if (package.Type == EnumASTMPackageType.EOT)
            {
                if (CacheGroupPackage.ContainsKey(ipendpoint))
                {
                    //正常结束
                    if (CacheGroupPackage.TryRemove(ipendpoint, out var temgroup))
                    {
                        temgroup.AddPackage(package);
                        temgroup.SetGroupPackage(EnumGroupReason.Success);
                    }
                }

                return;
            }

            //心跳
            if (package.Type == EnumASTMPackageType.HT)
            {
                package.Dispose();
                return;
            }

            if (package.Type == EnumASTMPackageType.ACK)
            {
                //发送完等待ACK,通知发送成功
                //没有发送，收到到ACK，丢弃
                if (WaitACK.ContainsKey(ipendpoint)) WaitACK[ipendpoint].TrySetResult(1);
                return;
            }

            if (package.Type == EnumASTMPackageType.NAK)
            {
                if (WaitACK.ContainsKey(ipendpoint)) WaitACK[ipendpoint].TrySetResult(3);
                return;
            }

            if (package.Type == EnumASTMPackageType.STX)
            {
                SendACK(session);
                if (CacheGroupPackage.ContainsKey(ipendpoint))
                {
                    var group = CacheGroupPackage[ipendpoint];
                    group.AddPackage(package);
                }
                return;
            }

            HandleUnknownPackage(ipendpoint, package);
        }
        catch (Exception e)
        {
        }
    }

    public async Task<EnumSendResult> SendGroupASTMPackage(IPEndPoint ipendpoint, List<HISASTMPackage> list)
    {
        if (!Clients.ContainsKey(ipendpoint)) return EnumSendResult.NotFind;
        if (list != null && list.Any())
        {
            if (!SendQueue.ContainsKey(ipendpoint))
                try
                {
                    LockSendQueue.Enter();
                    if (!SendQueue.ContainsKey(ipendpoint))
                    {
                        SendQueue[ipendpoint] = new ConcurrentQueue<SendASTMInfo>();
                        SendFlag[ipendpoint] = new SendFlagModel();
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    LockSendQueue.Leave();
                }

            var sendinfo = new SendASTMInfo(list);
            var queue = SendQueue[ipendpoint];
            queue.Enqueue(sendinfo);

            InnerStartSend(ipendpoint);
            await sendinfo.WaitTask.Task;
            return sendinfo.Result;
        }

        return EnumSendResult.None;
    }

    private void InnerStartSend(IPEndPoint ipendpoint)
    {
        if (Interlocked.CompareExchange(ref SendFlag[ipendpoint].IsSending, 1, 0) == 0)
            Task.Run(() =>
            {
                try
                {
                    InnerSend(ipendpoint);
                }
                finally
                {
                    Interlocked.Exchange(ref SendFlag[ipendpoint].IsSending, 0); //正在发送
                }
            });
    }

    private void InnerSend(IPEndPoint ipendpoint)
    {
        if (SendQueue.TryGetValue(ipendpoint, out var queue))
            try
            {
                
                while (queue.TryDequeue(out var sendinfo) && sendinfo != null)
                   
                {
                    try
                    {
                        var success = true;
                        if (sendinfo.Packages != null && sendinfo.Packages.Any())
                            foreach (var package in sendinfo.Packages)
                            {
                                var result = InnerSendPackage(ipendpoint, package).Result;
                                if (result != EnumSendResult.Success)
                                {
                                    sendinfo.Result = result;
                                    success = false;
                                    break;
                                }
                            }

                        if (success) sendinfo.Result = EnumSendResult.Success;
                    }
                    catch (Exception e)
                    {
                    }
                    finally
                    {
                        sendinfo.WaitTask.TrySetResult(true);
                    }

                    //上位机解析每次接收延迟10ms 用我自己的解析器没有问题，真不知道当时是怎么想的
                    if (_sendInternal > 0)
                        Task.Delay(_sendInternal).Wait();
                }
            }
            catch (Exception e)
            {
            }
    }

    private async Task<EnumSendResult> InnerSendPackage(IPEndPoint ipendpoint, HISASTMPackage package)
    {
        if (Clients.ContainsKey(ipendpoint))
        {
            var session = Clients[ipendpoint];
            try
            {
                if (package.Type == EnumASTMPackageType.EOT ||
                    package.Type == EnumASTMPackageType.ACK ||
                    package.Type == EnumASTMPackageType.NAK ||
                    package.Type == EnumASTMPackageType.HT
                   )
                {
                    await session.SendAsync(package);
                    return EnumSendResult.Success;
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                WaitACK.TryRemove(ipendpoint, out var obcts);
            }

            return EnumSendResult.Fail;
        }

        return EnumSendResult.NotFind;
    }
```



