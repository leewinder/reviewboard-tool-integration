using System.Diagnostics;
using System.Threading;

namespace RBProc.Utilities
{
    //
    // Locks one instance of a process type for the duration of the objects life
    //
    class ProcessLock : System.IDisposable
    {
        //
        // Constructor
        //
        public ProcessLock()
        {
            // Get the name of our process mutex
            string mutexName = GetMutexName();

            // Create our unique mutex
            m_ownerMutex = new System.Threading.Mutex(false, mutexName);
            m_ownerMutex.WaitOne();
        }

        //
        // Disposes of this object
        //
        public void Dispose()
        {
            // Close the mutex so we don't block on it
            if (m_ownerMutex != null)
            {
                m_ownerMutex.ReleaseMutex();
                m_ownerMutex.Close();

                m_ownerMutex = null;
            }
        }

        // Class Properties
        private readonly string OwnerMutexName = @"process-lock-{0}-owner-mutex-e4e434fe-5f1f-47a5-9922-fb79bf4b798e";

        // Private Properties
        private Mutex           m_ownerMutex = null;      
        
        // 
        // Returns the name of the process mutex
        // 
        private string GetMutexName()
        {
            // Get our process
            Process thisProcess = Process.GetCurrentProcess();

            // Build up the mutex name
            string mutexName = string.Format(OwnerMutexName, thisProcess.ProcessName);
            return mutexName;
        }
    }
}
