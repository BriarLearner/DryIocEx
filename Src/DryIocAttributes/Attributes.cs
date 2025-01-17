// <auto-generated/>
/*
The MIT License (MIT)

Copyright (c) 2013-2024 Maksim Volkau

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable disable

namespace DryIocAttributes;

/// <summary>List of supported DryIoc reuse types.</summary>
public enum ReuseType
{
    /// <summary>Means no reuse.</summary>
    Transient,
    /// <summary>subj.</summary>
    Singleton,
    /// <summary>Replaced the `CurrentScope`</summary>
    Scoped,
    /// <summary>subj.</summary>
    ResolutionScope,
    /// <summary>subj.</summary>
    ScopedOrSingleton
}

/// <summary>Specifies options to handle situation when registered service is already present in the registry.</summary>
public enum IfAlreadyExported
{
    /// <summary>Appends new default registration or throws registration with the same key.</summary>
    AppendNotKeyed,
    /// <summary>Throws if default or registration with the same key is already exist.</summary>
    Throw,
    /// <summary>Keeps old default or keyed registration ignoring new registration: ensures Register-Once semantics.</summary>
    Keep,
    /// <summary>Replaces old registration with new one.</summary>
    Replace,
    /// <summary>Adds new implementation or null (Made.Of),
    /// skips registration if the implementation is already registered.</summary>
    AppendNewImplementation
}

/// <summary>Provides whole set of possible/supported export options.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field,
    AllowMultiple = true, Inherited = false)]
public class ExportExAttribute : ExportAttribute
{
    /// <summary>Creates attribute.</summary>
    public ExportExAttribute(object contractKey = null, Type contractType = null,
        IfAlreadyExported ifAlreadyExported = IfAlreadyExported.AppendNotKeyed)
        : base(contractKey as string, contractType)
    {
        ContractKey = contractKey;
        IfAlreadyExported = ifAlreadyExported;
    }

    /// <summary>Creates export with specified service type.</summary>
    public ExportExAttribute(Type contractType,
        IfAlreadyExported ifAlreadyExported = IfAlreadyExported.AppendNotKeyed) :
        this(null, contractType, ifAlreadyExported)
    { }

    /// <summary>Creates export with handling existing export option.</summary>
    /// <param name="ifAlreadyExported">Handles export when other such export is already exist.</param>
    public ExportExAttribute(IfAlreadyExported ifAlreadyExported) :
        this(null, null, ifAlreadyExported)
    { }

    /// <summary>Optional service key or string <see cref="ExportAttribute.ContractName"/>.</summary>
    public object ContractKey { get; set; }

    /// <summary>Option to handle existing and duplicate exports.</summary>
    public IfAlreadyExported IfAlreadyExported { get; set; }
}

/// <summary>Base attribute to specify type of reuse for annotated class.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Method
    | AttributeTargets.Field
    | AttributeTargets.Property
    | AttributeTargets.Parameter,
    Inherited = false)]
public class ReuseAttribute : Attribute
{
    /// <summary>Implementation of reuse. Could be null to specify transient or no reuse.</summary>
    public readonly ReuseType ReuseType;

    /// <summary>Implementation type for reuse.</summary>
    public readonly Type CustomReuseType;

    /// <summary>Optional name, valid only for the Scoped Reuse. If specified, it overrides the <see cref="ScopeNames"/>.</summary>
    public readonly string ScopeName; // todo: @api convert to the property and change the type to object

    /// <summary>Optional names, valid only for the Scoped Reuse. May be overridden by the <see cref="ScopeName"/>.</summary>
    public readonly object[] ScopeNames;

    /// <summary>Specify the reuse type and the optional scope name.</summary>
    public ReuseAttribute(ReuseType reuseType, string scopeName = null)
    {
        ReuseType = reuseType;
        ScopeName = scopeName;
    }

    /// <summary>Specify the reuse via the `DryIoc.IReuse` implementation type and optional scope name</summary>
    public ReuseAttribute(Type customReuseType, string scopeName = null)
    {
        CustomReuseType = customReuseType;
        ScopeName = scopeName;
    }

    /// <summary>Specify the reuse type and the scope name.</summary>
    public ReuseAttribute(ReuseType reuseType, params object[] scopeNames)
    {
        ReuseType = reuseType;
        ScopeNames = scopeNames;
    }

    /// <summary>Specify the reuse via the `DryIoc.IReuse` implementation type and the scope name</summary>
    public ReuseAttribute(Type customReuseType, params object[] scopeNames)
    {
        CustomReuseType = customReuseType;
        ScopeNames = scopeNames;
    }
}

/// <summary>Defines the Transient reuse for exported service.</summary>
public class TransientReuseAttribute : ReuseAttribute
{
    /// <summary>Creates attribute by specifying null as <see cref="ReuseAttribute.ReuseType"/>.</summary>
    public TransientReuseAttribute() : base(ReuseType.Transient) { }
}

/// <summary>Denotes exported type with Singleton reuse.</summary>
public class SingletonReuseAttribute : ReuseAttribute
{
    /// <summary>Creates attribute.</summary>
    public SingletonReuseAttribute() : base(ReuseType.Singleton) { }
}

/// <summary>Denotes exported type with Current Scope Reuse.</summary>
public class CurrentScopeReuseAttribute : ReuseAttribute
{
    /// <summary>Creates attribute.</summary> <param name="scopeName">(optional)</param>
    public CurrentScopeReuseAttribute(string scopeName = null) : base(ReuseType.Scoped, scopeName) { }
}

/// <summary>Marks exported type with Reuse.InWebRequest.
/// Basically it is CurrentScopeReuse with predefined name Reuse.WebRequestScopeName.</summary>
public class WebRequestReuseAttribute : CurrentScopeReuseAttribute
{
    /// <summary>Default web reuse scope name. Just a convention supported by DryIoc.</summary>
    public static readonly string WebRequestScopeName = "WebRequestScopeName";

    /// <summary>Creates attribute.</summary>
    public WebRequestReuseAttribute() : base(WebRequestScopeName) { }
}

/// <summary>Marks exported type with Reuse.InThread.
/// Basically it is CurrentScopeReuse with predefined name ThreadScopeContext.ScopeContextName.</summary>
public class ThreadReuseAttribute : CurrentScopeReuseAttribute
{
    /// <summary>Name for root scope in thread context. Just a convention supported by DryIoc.</summary>
    public static readonly string ScopeContextName = "ThreadScopeContext";

    /// <summary>Creates attribute.</summary>
    public ThreadReuseAttribute() : base(ScopeContextName) { }
}

/// <summary>Denotes exported type with Resolution Scope Reuse.</summary>
public class ResolutionScopeReuseAttribute : ReuseAttribute
{
    /// <summary>Creates attribute.</summary>
    public ResolutionScopeReuseAttribute() : base(ReuseType.ResolutionScope) { }
}

/// <summary>Denotes exported type with Scoped Or Singleton Reuse.</summary>
public class ScopedOrSingletonReuseAttribute : ReuseAttribute
{
    /// <summary>Creates attribute.</summary>
    public ScopedOrSingletonReuseAttribute() : base(ReuseType.ScopedOrSingleton) { }
}

/// <summary>Specifies for export part to use the whatever reuse of importing site.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field)]
public class UseParentReuseAttribute : Attribute { }

/// <summary>Marks exported reused service to be stored as WeakReference</summary>
public class WeaklyReferencedAttribute : Attribute { }

/// <summary>Marks exported reused service to be Not disposed together with scope.</summary>
public class PreventDisposalAttribute : Attribute { }

/// <summary>Allows disposable transient to be exported.</summary>
public class AllowDisposableTransientAttribute : Attribute { }

/// <summary>Turns On tracking of disposable transient dependency in parent scope or in open scope if resolved directly.</summary>
public class TrackDisposableTransientAttribute : Attribute { }

/// <summary>OBSOLETE: Please use ExportExAttribute instead. ExportEx adds IfAlreadyExported option, plus may be extended with other options in future.</summary>
[Obsolete("Please use ExportExAttribute instead. ExportEx adds IfAlreadyExported option, plus may be extended with other options in future.")]
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field,
    AllowMultiple = true, Inherited = false)]
public class ExportWithKeyAttribute : ExportAttribute
{
    /// <remarks>Specifies service key if <see cref="ExportAttribute.ContractName"/> is not specified.</remarks>
    public object ContractKey { get; set; }

    /// <summary>Creates attribute.</summary>
    /// <param name="contractKey">Service key object, should implement <see cref="object.GetHashCode"/> and <see cref="object.Equals(object)"/></param>
    /// <param name="contractType">Service type.</param>
    public ExportWithKeyAttribute(object contractKey, Type contractType)
        : base(contractType)
    {
        ContractKey = contractKey;
    }

    /// <summary>Creates attribute using implementation type as <see cref="ExportAttribute.ContractType"/></summary>
    /// <param name="contractKey">Service key object, should implement <see cref="object.GetHashCode"/> and <see cref="object.Equals(object)"/></param>
    public ExportWithKeyAttribute(object contractKey) : this(contractKey, null) { }
}

/// <summary>Specifies to export all implemented contract types automatically.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field,
    Inherited = false)]
public class ExportManyAttribute : Attribute
{
    /// <summary>Specifies service key if <see cref="ContractName"/> is not specified.</summary>
    public object ContractKey { get; set; }

    // [Obsolete("Use ContractKey instead")]
    /// <summary>OBSOLETE: Use ContractKey instead.</summary>
    public string ContractName { get; set; }

    /// <summary>Excludes specified contract types.</summary>
    public Type[] Except { get; set; }

    /// <summary>Public types by default.</summary>
    public bool NonPublic { get; set; }

    /// <summary>Option to handle existing and duplicate exports.</summary>
    public IfAlreadyExported IfAlreadyExported { get; set; }
}

/// <summary>Obsolete: Is not required anymore, you can just put Export on member without marking the containing type with AsFactory.</summary>
[Obsolete("Is not required anymore, you can just put Export on member without marking the containing type with AsFactory.", error: false)]
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field,
    Inherited = false)]
public class AsFactoryAttribute : Attribute { }

/// <summary>Exports service as custom wrapper.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field,
    Inherited = false)]
public class AsWrapperAttribute : Attribute
{
    /// <summary>For open-generic wrapper indicates wrapped argument type index.</summary>
    public int WrappedServiceTypeArgIndex { get; set; }

    /// <summary>Per name.</summary>
    public bool AlwaysWrapsRequiredServiceType { get; set; }

    /// <summary>Creates attribute with <see cref="WrappedServiceTypeArgIndex"/>.</summary>
    /// <param name="wrappedServiceTypeArgIndex">(optional) To use single generic type arg.</param>
    /// <param name="alwaysWrapsRequiredServiceType">(optional) Required for generic wrapper to ignore its type arguments.</param>
    public AsWrapperAttribute(int wrappedServiceTypeArgIndex = -1, bool alwaysWrapsRequiredServiceType = false)
    {
        WrappedServiceTypeArgIndex = wrappedServiceTypeArgIndex;
        AlwaysWrapsRequiredServiceType = alwaysWrapsRequiredServiceType;
    }
}

/// <summary>Specifies that exported service is decorator of services of <see cref="ExportAttribute.ContractType"/>.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field,
    Inherited = false)]
public class AsDecoratorAttribute : Attribute
{
    /// <summary>If <see cref="ContractName"/> specified, it has more priority over <see cref="ContractKey"/>. </summary>
    public string ContractName { get; set; }

    /// <summary>Contract key of Decorated type, not for a decorator itself. Used to find the service to apply decorator to.</summary>
    public object ContractKey { get; set; }

    /// <summary>If provided specifies relative decorator position in decorators chain.
    /// Greater number means further from decoratee - specify negative number to stay closer.
    /// Decorators without order (Order is 0) or with equal order are applied in registration order
    /// - first registered are closer decoratee.</summary>
    public int Order { get; set; }

    /// <summary>Instructs to use decorated service reuse. Decorated service may be decorator itself.</summary>
    public bool UseDecorateeReuse { get; set; }

    /// <summary>Creates attribute by providing its optional properties.</summary>
    /// <param name="contractKey">(optional) Contract key of Decorated type, not for a decorator itself.
    /// Used to find the service to apply decorator to.</param>
    /// <param name="order">(optional)If provided specifies relative decorator position in decorators chain.
    /// Greater number means further from decoratee - specify negative number to stay closer.
    /// Decorators without order (Order is 0) or with equal order are applied in registration order
    /// - first registered are closer decoratee.</param>
    /// <param name="useDecorateeReuse">(optional) Instructs to use decorated service reuse.
    /// Decorated service may be decorator itself.</param>
    public AsDecoratorAttribute(object contractKey = null, int order = 0, bool useDecorateeReuse = false)
    {
        ContractKey = contractKey;
        Order = order;
        UseDecorateeReuse = useDecorateeReuse;
    }

    /// <summary>Creates attributes with <see cref="ContractName"/> and optional order.</summary>
    /// <param name="contractName"></param> <param name="order">(optional)</param>
    /// <param name="useDecorateeReuse">(optional)</param>
    public AsDecoratorAttribute(string contractName, int order = 0, bool useDecorateeReuse = false)
    {
        ContractName = contractName;
        Order = order;
        UseDecorateeReuse = useDecorateeReuse;
    }
}

/// <summary>Type of services supported by Container.</summary>
public enum FactoryType
{
    /// <summary>(default) Defines normal service factory</summary>
    Service,
    /// <summary>Defines decorator factory</summary>
    Decorator,
    /// <summary>Defines wrapper factory.</summary>
    Wrapper
};

/// <summary>Policy to handle unresolved service.</summary>
public enum IfUnresolved
{
    /// <summary>If service is unresolved for whatever means, it will throw the respective exception.</summary>
    Throw,
    /// <summary>If service is unresolved for whatever means, it will return default(serviceType) value.</summary>
    ReturnDefault,
    /// <summary>If service is not registered, then it will return default, for other errors it will throw.</summary>
    ReturnDefaultIfNotRegistered,
}

// todo: @simplify we are repeating the DryIoc.Request here, but we need just a little functionality of it
/// <summary>Dependency request path information.</summary>
public sealed class Request
{
    /// <summary>Represents empty info (indicated by null <see cref="ServiceType"/>).</summary>
    public static readonly Request Empty =
        new Request(null, null, null, null, null, IfUnresolved.Throw, -1, FactoryType.Service, null, 0, null);

    /// <summary>Returns true for an empty request.</summary>
    public bool IsEmpty => ServiceType == null;

    /// <summary>Returns true if request is the first in a chain.</summary>
    public bool IsResolutionRoot => !IsEmpty && DirectParent.IsEmpty;

    /// <summary>Parent request or null for root resolution request.</summary>
    public readonly Request DirectParent;

    /// <summary>Returns service parent skipping wrapper if any. To get immediate parent us <see cref="DirectParent"/>.</summary>
    public Request Parent => IsEmpty ? Empty : DirectParent.FirstOrEmpty(p => p.FactoryType != FactoryType.Wrapper);

    /// <summary>Gets first request info starting with itself which satisfies the condition, or empty otherwise.</summary>
    /// <param name="condition">Condition to stop on. Should not be null.</param>
    /// <returns>Request info of found parent.</returns>
    public Request FirstOrEmpty(Func<Request, bool> condition)
    {
        var r = this;
        while (!r.IsEmpty && !condition(r))
            r = r.DirectParent;
        return r;
    }

    /// <summary>Asked service type.</summary>
    public readonly Type ServiceType;

    /// <summary>Required service type if specified.</summary>
    public readonly Type RequiredServiceType;

    /// <summary>Optional service key.</summary>
    public readonly object ServiceKey;

    /// <summary>Metadata key to find in metadata dictionary in resolved service.</summary>
    public readonly string MetadataKey;

    /// <summary>Metadata or the value (if key specified) to find in resolved service.</summary>
    public readonly object Metadata;

    /// <summary>Policy to deal with unresolved request.</summary>
    public readonly IfUnresolved IfUnresolved;

    /// <summary>Resolved factory ID, used to identify applied decorator.</summary>
    public readonly int FactoryID;

    /// <summary>False for Decorators and Wrappers.</summary>
    public readonly FactoryType FactoryType;

    /// <summary>Implementation type.</summary>
    public readonly Type ImplementationType;

    /// <summary>Relative number representing reuse lifespan.</summary>
    public readonly int ReuseLifespan;

    /// <summary>Creates info by supplying all the properties and chaining it with current (parent) info.</summary>
    public Request Push(
        Type serviceType, Type requiredServiceType, object serviceKey, string metadataKey, object metadata, IfUnresolved ifUnresolved,
        int factoryID, FactoryType factoryType, Type implementationType, int reuseLifespan) =>
        new Request(serviceType, requiredServiceType, serviceKey, metadataKey, metadata, ifUnresolved,
            factoryID, factoryType, implementationType, reuseLifespan, this);

    private Request(
        Type serviceType, Type requiredServiceType, object serviceKey, string metadataKey, object metadata, IfUnresolved ifUnresolved,
        int factoryID, FactoryType factoryType, Type implementationType, int reuseLifespan,
        Request directParent)
    {
        DirectParent = directParent;

        // Service info:
        ServiceType = serviceType;
        RequiredServiceType = requiredServiceType;
        ServiceKey = serviceKey;
        MetadataKey = metadataKey;
        Metadata = metadata;
        IfUnresolved = ifUnresolved;

        // Implementation info:
        FactoryID = factoryID;
        FactoryType = factoryType;
        ImplementationType = implementationType;
        ReuseLifespan = reuseLifespan;
    }

    /// <summary>Returns all request until the root - parent is null.</summary>
    /// <returns>Requests from the last to first.</returns>
    public IEnumerable<Request> Enumerate()
    {
        for (var i = this; !i.IsEmpty; i = i.DirectParent)
            yield return i;
    }

    /// <summary>Prints request with all its parents to string.</summary> <returns>The string.</returns>
    public override string ToString()
    {
        if (IsEmpty)
            return "{empty}";

        var s = new StringBuilder();

        if (FactoryType != FactoryType.Service)
            s.Append(FactoryType.ToString().ToLower()).Append(' ');

        if (ImplementationType != null && ImplementationType != ServiceType)
            s.Append(ImplementationType).Append(": ");

        s.Append(ServiceType);

        if (RequiredServiceType != null)
            s.Append(" with RequiredServiceType=").Append(RequiredServiceType);

        if (ServiceKey != null)
            s.Append(" with ServiceKey=").Append('{').Append(ServiceKey).Append('}');

        if (MetadataKey != null || Metadata != null)
            s.Append(" with Metadata=").Append(new KeyValuePair<string, object>(MetadataKey, Metadata));

        if (IfUnresolved != IfUnresolved.Throw)
            s.Append(" if unresolved ").Append(Enum.GetName(typeof(IfUnresolved), IfUnresolved));

        if (ReuseLifespan != 0)
            s.Append(" with ReuseLifespan=").Append(ReuseLifespan);

        if (!DirectParent.IsEmpty)
            s.AppendLine().Append("  in ").Append(DirectParent);

        return s.ToString();
    }

    /// <summary>Returns true if request info and passed object are equal, and their parents recursively are equal.</summary>
    /// <param name="obj"></param> <returns></returns>
    public override bool Equals(object obj) =>
        Equals(obj as Request);

    /// <summary>Returns true if request info and passed info are equal, and their parents recursively are equal.</summary>
    public bool Equals(Request other) =>
        other != null && EqualsWithoutParent(other)
        && (DirectParent == null && other.DirectParent == null
        || (DirectParent != null && DirectParent.EqualsWithoutParent(other.DirectParent)));

    /// <summary>Compares with other info taking into account the properties but not the parents and their properties.</summary>
    public bool EqualsWithoutParent(Request other) =>
        other.ServiceType == ServiceType

        && other.RequiredServiceType == RequiredServiceType
        && other.IfUnresolved == IfUnresolved
        && other.ServiceKey == ServiceKey

        && other.FactoryType == FactoryType
        && other.ImplementationType == ImplementationType
        && other.ReuseLifespan == ReuseLifespan;

    /// <summary>Calculates the combined hash code based on factory IDs.</summary>
    public override int GetHashCode()
    {
        if (IsEmpty)
            return 0;

        var hash = FactoryID;
        var parent = DirectParent;
        while (!parent.IsEmpty)
        {
            hash = CombineHashCodes(hash, parent.FactoryID);
            parent = parent.DirectParent;
        }

        return hash;
    }

    // Inspired by System.Tuple.CombineHashCodes
    private static int CombineHashCodes(int h1, int h2)
    {
        unchecked
        {
            return (h1 << 5) + h1 ^ h2;
        }
    }
}

/// <summary>Base type for exported type Setup Condition.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field,
    Inherited = false)]
public abstract class ExportConditionAttribute : Attribute
{
    /// <summary>Returns true to use exported service for request.</summary>
    /// <param name="request"></param> <returns>True to use exported service for request.</returns>
    public abstract bool Evaluate(Request request);
}

/// <summary>Please use ImportExAttribute instead. ImportEx adds ContractKey of arbitrary type, plus may be extended with other options in future.</summary>
[AttributeUsage(AttributeTargets.Parameter
    | AttributeTargets.Field
    | AttributeTargets.Property)]
public class ImportExAttribute : ImportAttribute
{
    /// <summary>Arbitrary object to match with service key.</summary>
    public object ContractKey { get; set; }

    /// <summary>Creates attribute object service key.</summary> <param name="contractKey"></param>
    /// <param name="contractType">(optional) If missing then imported member type will be used as service type.</param>
    public ImportExAttribute(object contractKey, Type contractType = null)
        : base(contractType)
    {
        ContractKey = contractKey;
    }

    /// <summary>Creates attribute with string service name.</summary> <param name="contractKey"></param>
    /// <param name="contractType">(optional) If missing then imported member type will be used as service type.</param>
    public ImportExAttribute(string contractKey, Type contractType = null)
        : base(contractKey, contractType)
    {
        ContractKey = contractKey;
    }
}

/// <summary>Exports service with associated metadata key and value.
/// Key can be skipped</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Method
    | AttributeTargets.Parameter
    | AttributeTargets.Field
    | AttributeTargets.Property,
    Inherited = false,
    AllowMultiple = true)]
public class WithMetadataAttribute : Attribute
{
    /// <summary>Metadata key in result metadata dictionary</summary>
    public readonly string MetadataKey;

    /// <summary>Metadata value.</summary>
    public readonly object Metadata;

    /// <summary>Creates attribute</summary>
    /// <param name="metadataKey"></param>
    /// <param name="metadata"></param>
    public WithMetadataAttribute(string metadataKey, object metadata)
    {
        MetadataKey = metadataKey;
        Metadata = metadata;
    }

    /// <summary>Creates attribute</summary> <param name="metadata"></param>
    public WithMetadataAttribute(object metadata) : this(null, metadata) { }
}

/// <summary>Imports the service.
/// But in case the service is not registered, attribute will exports the service in-place for registration.
/// Useful for ad-hoc registration of types from not controlled libraries.</summary>
[AttributeUsage(AttributeTargets.Parameter
    | AttributeTargets.Field
    | AttributeTargets.Property)]
public class ImportExternalAttribute : Attribute
{
    /// <summary>Implementation type of registered service.</summary>
    public Type ImplementationType { get; set; }

    /// <summary>Use specific constructor for registration.</summary>
    public Type[] ConstructorSignature { get; set; }

    /// <summary>Metadata key in result metadata dictionary</summary>
    public string MetadataKey { get; set; }

    /// <summary>Metadata value, can be specified with or without <see cref="MetadataKey"/>.</summary>
    public object Metadata { get; set; }

    /// <summary>Registering (and importing) with specified service key.</summary>
    public object ContractKey { get; set; }

    /// <summary>Registering (and importing) with specified service type.</summary>
    public Type ContractType { get; set; }

    /// <summary>Creates attributes.</summary>
    /// <param name="implementationType">(optional) Implementation type of registered service.</param>
    /// <param name="constructorSignature">(optional) Use specific constructor for registration.</param>
    /// <param name="metadata">(optional) Metadata associated with registration.</param>
    /// <param name="contractKey">(optional) Registering (and importing) with specified service key.</param>
    /// <param name="contractType">(optional) Registering (and importing) with specified service type.</param>
    public ImportExternalAttribute(Type implementationType = null, Type[] constructorSignature = null,
        object metadata = null, object contractKey = null, Type contractType = null)
    {
        ImplementationType = implementationType;
        ConstructorSignature = constructorSignature;
        Metadata = metadata;
        ContractType = contractType;
        ContractKey = contractKey;
    }
}

/// <summary>Exported type should open resolution scope when injected.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field)]
public class OpenResolutionScopeAttribute : Attribute { }

/// <summary>Does not add the resolution scope into the parent or singleton scope,
/// preventing possibly unwanted holding of the scope (and its services) for the lifespan of the container.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field)]
public class AvoidResolutionScopeTrackingAttribute : Attribute { }

/// <summary>When single service is resolved, but multiple candidates found, this setting will be used to prefer this one.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field)]
public class PreferInSingleServiceResolveAttribute : Attribute { }

/// <summary>Specifies that export should be imported as dynamic resolution call,
/// instead of in-lined creation expression.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field)]
public class AsResolutionCallAttribute : Attribute { }

/// <summary>Marker for resolution root exports.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field)]
public class AsResolutionRootAttribute : Attribute { }

/// <summary>Relative disposal order when defined. Greater number, later dispose.</summary>
[AttributeUsage(AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Method
    | AttributeTargets.Property
    | AttributeTargets.Field)]
public class DisposalOrderAttribute : Attribute
{
    /// <summary>Relative number of disposal</summary>
    public int RelativeValue { get; set; }
    /// <summary>Constructor</summary>
    public DisposalOrderAttribute(int relativeValue) => RelativeValue = relativeValue;
}

#nullable restore