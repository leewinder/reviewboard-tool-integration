using System;
using System.Management;

namespace Create_Review.Utilities
{
    // 
    // Generates unique identifiers for the PC the app is running on
    //
    class Identifiers
    {
        //
        // Requests the ID of the PC
        //
        public static string UUID
        {
            get
            {
                // Build up the ID every time
                string combinedId = GetCpuId() + GetCpuName() + GetSystemName() + GetHddId();
                return combinedId;
            }
        }

        //
        // Generates the CPU ID
        //
        private static string GetCpuId()
        {
            ManagementObject processorObject = GetProcessorManagementObject();

            string cpuInfo = string.Empty;
            try
            {
                // Not always valid
                cpuInfo = processorObject.Properties["processorID"].Value.ToString();
            }
            catch (Exception)
            {
                cpuInfo = @"Unknown Processor ID";
            }
            
            // Return the ID
            return cpuInfo;
        }

        //
        // Generates the CPU Name
        //
        private static string GetCpuName()
        {
            ManagementObject processorObject = GetProcessorManagementObject();

            string cpuInfo = string.Empty;
            try
            {
                // Not always valid
                cpuInfo = processorObject.Properties["Name"].Value.ToString();
            }
            catch (Exception)
            {
                cpuInfo = @"Unknown Processor Name";
            }

            // Return the ID
            return cpuInfo;
        }

        //
        // Generates the CPU Name
        //
        private static string GetSystemName()
        {
            ManagementObject processorObject = GetProcessorManagementObject();

            string cpuInfo = string.Empty;
            try
            {
                // Not always valid
                cpuInfo = processorObject.Properties["SystemName"].Value.ToString();
            }
            catch (Exception)
            {
                cpuInfo = @"Unknown System Name";
            }

            // Return the ID
            return cpuInfo;
        }

        //
        // Generates the ID of the hard drive
        //
        private static string GetHddId()
        {
            string drive = "C";

            ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            dsk.Get();

            string hddId = string.Empty;
            try
            {
                hddId = dsk["VolumeSerialNumber"].ToString();
            }
            catch (Exception)
            {
                hddId = @"Unknown HDD ID";
            }

            // Return the ID
            return hddId;
        }

        //
        // Returns the processor management class
        //
        private static ManagementObject GetProcessorManagementObject()
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                // Just return the first one
                return mo;
            }

            // Nothing
            return null;
        }
    }
}
