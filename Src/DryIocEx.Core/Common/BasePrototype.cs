using System;

namespace DryIocEx.Core.Common;

public abstract class BasePrototype<TSubject>
    where TSubject : class, new()
{
    public TSubject Copy()
    {
        throw new NotImplementedException();
    }

    protected abstract void Copy(TSubject subject);
}