using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace nodesharp.core
{
    //Huge shoutout to Matt Howells, https://stackoverflow.com/questions/3342941/kill-child-process-when-parent-process-is-killed
    internal class WinJob : IDisposable
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr CreateJobObject(IntPtr a, string lpName);
 
        [DllImport("kernel32.dll")]
        static extern bool SetInformationJobObject(
            IntPtr hJob, 
            JobObjectInfoType infoType, 
            IntPtr lpJobObjectInfo, 
            UInt32 cbJobObjectInfoLength);
 
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AssignProcessToJobObject(
            IntPtr job, 
            IntPtr process);
 
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);
 
        private IntPtr _handle;
        private bool _disposed;
 
        public WinJob()
        {
            _handle = CreateJobObject(IntPtr.Zero, null);
 
            var info = new JOBOBJECT_BASIC_LIMIT_INFORMATION
            {
                LimitFlags = 0x2000
            };
 
            var extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
            {
                BasicLimitInformation = info
            };
 
            var infoType = typeof (JOBOBJECT_EXTENDED_LIMIT_INFORMATION);
            var length = Marshal.SizeOf(infoType);
            var extendedInfoPtr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);
 
            var setResult = SetInformationJobObject(
                _handle,
                JobObjectInfoType.ExtendedLimitInformation,
                extendedInfoPtr,
                (uint) length);
 
            if (setResult)
                return;
 
            var lastError = Marshal.GetLastWin32Error();
            var message = "Unable to set information. Error: " + lastError;
            throw new Exception(message);
        }
 
        ~WinJob()
        {
            Dispose(false);
        }
 
        public void Dispose()
        {
            Dispose(true);
        }
 
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
 
            if (disposing)
                GC.SuppressFinalize(this);
 
            Close();
 
            _disposed = true;
        }
 
        public void Close()
        {
            CloseHandle(_handle);
            _handle = IntPtr.Zero;
        }
 
        public bool AddProcess(IntPtr processHandle)
        {
            return AssignProcessToJobObject(_handle, processHandle);
        }
 
        public bool AddProcess(int processId)
        {
            var process = Process.GetProcessById(processId);
            return AddProcess(process.Handle);
        }
 
    }

    #region Helper classes
 
    [StructLayout(LayoutKind.Sequential)]
    struct IO_COUNTERS
    {
        public UInt64 ReadOperationCount;
        public UInt64 WriteOperationCount;
        public UInt64 OtherOperationCount;
        public UInt64 ReadTransferCount;
        public UInt64 WriteTransferCount;
        public UInt64 OtherTransferCount;
    }
 
 
    [StructLayout(LayoutKind.Sequential)]
    struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
        public Int64 PerProcessUserTimeLimit;
        public Int64 PerJobUserTimeLimit;
        public UInt32 LimitFlags;
        public UIntPtr MinimumWorkingSetSize;
        public UIntPtr MaximumWorkingSetSize;
        public UInt32 ActiveProcessLimit;
        public UIntPtr Affinity;
        public UInt32 PriorityClass;
        public UInt32 SchedulingClass;
    }
 
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public UInt32 nLength;
        public IntPtr lpSecurityDescriptor;
        public Int32 bInheritHandle;
    }
 
    [StructLayout(LayoutKind.Sequential)]
    struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
        public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
        public IO_COUNTERS IoInfo;
        public UIntPtr ProcessMemoryLimit;
        public UIntPtr JobMemoryLimit;
        public UIntPtr PeakProcessMemoryUsed;
        public UIntPtr PeakJobMemoryUsed;
    }
 
    public enum JobObjectInfoType
    {
        AssociateCompletionPortInformation = 7,
        BasicLimitInformation = 2,
        BasicUIRestrictions = 4,
        EndOfJobTimeInformation = 6,
        ExtendedLimitInformation = 9,
        SecurityLimitInformation = 5,
        GroupInformation = 11
    }
 
    #endregion
}
