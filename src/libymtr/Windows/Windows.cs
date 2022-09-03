using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace libymtr.Windows {
    public static class Windows {
        private const string MSG_WINDOWS_ONLY = "Work on Windows only";
        private static Exception WindowsOnlyException {
            get {
                return new Exception(MSG_WINDOWS_ONLY);
            }
        }

        /// <summary>
        /// Check whether os is Windows
        /// </summary>
        public static void CheckOS() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return;
            }
            throw WindowsOnlyException;
        }
    }
}
