using DryIocEx.Core.Common;

namespace DryIocEx.Prism;

public interface IDialogControl : IControl
{
    IWaitBox GetWaitBox(string title, string content);
}

public interface IEnsureBox
{
    string MyTitle { set; get; }

    string MyContent { set; get; }

    bool? ShowDialog();
}

public interface IEnsureCancelBox
{
    string MyTitle { set; get; }

    string MyContent { set; get; }

    bool? ShowDialog();
}

public interface IWaitBox
{
    string MyTitle { set; get; }

    string MyContent { set; get; }

    bool? ShowDialog();
    void CallClosed();
}

public interface ISetupView
{
    bool? ShowDialog();
}