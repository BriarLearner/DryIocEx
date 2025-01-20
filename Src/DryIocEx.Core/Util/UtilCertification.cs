using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DryIocEx.Core.Util;

public interface IUtilCertification : IUtil
{
    string GetCertificateName(string name, string country = "", string state = "", string area = "", string unit = "",
        string organization = "");

    X509Certificate2 CreateCertificate(string certificatename, DateTime starttime, DateTime endtime,
        string insecurepassword);
}

//https://stackoverflow.com/questions/48196350/generate-and-sign-certificate-request-using-pure-net-framework
//
[Util]
public class UtilCertification : IUtilCertification
{
    public string GetCertificateName(string name, string country = "", string state = "", string area = "",
        string unit = "", string organization = "")
    {
        throw new NotImplementedException();
    }

    public X509Certificate2 CreateCertificate(string certificatename, DateTime starttime, DateTime endtime,
        string insecurepassword)
    {
        throw new NotImplementedException();
    }


    public byte[] CreatePFXCertificate(string certificatename, DateTime starttime, DateTime endtime,
        string insecurepassword)
    {
        throw new NotImplementedException();
    }


    public byte[] CreatePFXCertificate(
        string certificatename,
        DateTime starttime,
        DateTime endtime,
        SecureString password)
    {
        throw new NotImplementedException();
    }

    private SystemTime ToSystemTime(DateTime dateTime)
    {
        throw new NotImplementedException();
    }

    private void Check(bool nativeCallSucceeded)
    {
        throw new NotImplementedException();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SystemTime
    {
        public readonly short Year;
        public readonly short Month;
        public readonly short DayOfWeek;
        public readonly short Day;
        public readonly short Hour;
        public readonly short Minute;
        public readonly short Second;
        public readonly short Milliseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CryptoApiBlob
    {
        public readonly int DataLength;
        public IntPtr Data;

        public CryptoApiBlob(int dataLength, IntPtr data)
        {
            throw new NotImplementedException();
        }
    }

    //https://learn.microsoft.com/zh-cn/windows/win32/api/wincrypt/ns-wincrypt-crypt_key_prov_info
    [StructLayout(LayoutKind.Sequential)]
    private struct CryptKeyProviderInformation
    {
        [MarshalAs(UnmanagedType.LPWStr)] public string ContainerName; //LPWSTR                pwszContainerName;
        [MarshalAs(UnmanagedType.LPWStr)] public readonly string ProviderName;
        public int ProviderType; //DWORD                 dwProvType;
        public readonly int Flags;
        public readonly int ProviderParameterCount;
        public readonly IntPtr ProviderParameters; // PCRYPT_KEY_PROV_PARAM 加密密钥验证参数  

        public int
            KeySpec; //AT_KEYEXCHANGE 用于加密/解密会话密钥的密钥。 AT_SIGNATURE用于创建和验证数字签名的密钥。为零时，此值作为 dwLegacyKeySpec 参数传递给 NCryptOpenKey 函数。
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CryptAlgorithmIdentifier
    {
        [MarshalAs(UnmanagedType.LPStr)] public string ObjId;
        public CryptoapiBlob Parameters;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CryptoapiBlob
    {
        public uint CbData;
        public IntPtr PbData;
    }

    private static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileTimeToSystemTime(
            [In] ref long fileTime,
            out SystemTime systemTime);

        [DllImport("AdvApi32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptAcquireContextW(
            out IntPtr providerContext,
            [MarshalAs(UnmanagedType.LPWStr)] string container,
            [MarshalAs(UnmanagedType.LPWStr)] string provider,
            int providerType,
            int flags);

        [DllImport("AdvApi32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptReleaseContext(
            IntPtr providerContext,
            int flags);

        [DllImport("AdvApi32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptGenKey(
            IntPtr providerContext,
            int algorithmId,
            int flags,
            out IntPtr cryptKeyHandle);

        [DllImport("AdvApi32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptDestroyKey(
            IntPtr cryptKeyHandle);

        [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CertStrToNameW(
            int certificateEncodingType,
            IntPtr x500,
            int strType,
            IntPtr reserved,
            [MarshalAs(UnmanagedType.LPArray)] [Out]
            byte[] encoded,
            ref int encodedLength,
            out IntPtr errorString);

        [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr CertCreateSelfSignCertificate(
            IntPtr providerHandle,
            [In] ref CryptoApiBlob subjectIssuerBlob,
            int flags,
            [In] ref CryptKeyProviderInformation keyProviderInformation,
            [In] ref CryptAlgorithmIdentifier signatureAlgorithm,
            [In] ref SystemTime startTime,
            [In] ref SystemTime endTime,
            IntPtr extensions);

        [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CertFreeCertificateContext(
            IntPtr certificateContext);

        [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr CertOpenStore(
            [MarshalAs(UnmanagedType.LPStr)] string storeProvider,
            int messageAndCertificateEncodingType,
            IntPtr cryptProvHandle,
            int flags,
            IntPtr parameters);

        [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CertCloseStore(
            IntPtr certificateStoreHandle,
            int flags);

        [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CertAddCertificateContextToStore(
            IntPtr certificateStoreHandle,
            IntPtr certificateContext,
            int addDisposition,
            out IntPtr storeContextPtr);

        [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CertSetCertificateContextProperty(
            IntPtr certificateContext,
            int propertyId,
            int flags,
            [In] ref CryptKeyProviderInformation data);

        [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PFXExportCertStoreEx(
            IntPtr certificateStoreHandle,
            ref CryptoApiBlob pfxBlob,
            IntPtr password,
            IntPtr reserved,
            int flags);
    }
}