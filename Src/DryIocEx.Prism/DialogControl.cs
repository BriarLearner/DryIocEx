using System;
using System.Threading.Tasks;
using System.Windows;
using DryIocEx.Core.Common;
using DryIocEx.Core.Extensions;
using DryIocEx.Core.IOC;
using DryIocEx.Core.Log;

namespace DryIocEx.Prism;

[AutoRegister(typeof(IDialogControl), EnumLifetime.Singleton)]
public class DialogControl : BaseControl, IDialogControl
{
    public override Operate Execute(Operate operate)
    {
        throw new NotImplementedException();
    }


    public override Task<Operate> ExecuteAsync(Operate operate)
    {
        throw new NotImplementedException();
    }

    public IWaitBox GetWaitBox(string title, string content)
    {
        throw new NotImplementedException();
    }
}

public class DialogInfo
{
    public string Title { set; get; }

    public string Content { set; get; }
}