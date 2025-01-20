using System.Threading.Tasks;

namespace DryIocEx.Core.Extensions;

public static class TaskExtension
{
    public static void DoNotAwait(this Task task)
    {
    }

    public static void DoNotAwait(this ValueTask task)
    {
    }
}