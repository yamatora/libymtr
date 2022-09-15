using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Linq;

namespace libymtr.IO {
    public static class File {
        /// <summary>
        /// Concat directory and filename to filepath
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string ConcatPath(string directory, string filename) {
            return Path.Combine(directory, filename);
        }

        /// <summary>
        /// Write binary to file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filepath"></param>
        /// <param name="force"></param>
        public static void WriteBinary(byte[] data, string filepath, bool force = true) {
            FileMode mode = force ? FileMode.Create : FileMode.CreateNew;
            using (var fs = new FileStream(filepath, mode))
            using (var writer = new BinaryWriter(fs)) {
                writer.Write(data);
            }
        }
    }
}
