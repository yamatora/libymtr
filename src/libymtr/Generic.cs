using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace libymtr {
    /// <summary>
    /// Generic type function
    /// </summary>
    public static class Generic {
        /// <summary>
        /// Concat <T> array with splitter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="splitter"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ConcatByChar<T>(char splitter, params T[] array) {
            string result = "";
            if (array == null || array.Length == 0) {
                return result;
            }
            result = array[0].ToString();
            for (int i = 1; i < array.Length; i++) {
                result += splitter;
                result += array[i].ToString();
            }
            return result;
        }

        /// <summary>
        /// Concat T[] array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrays"></param>
        /// <returns></returns>
        public static T[] ConcatArray<T>(params T[][] arrays) {
            T[] result = new T[arrays.Select(array => array.Length).Sum()];
            int typeSize = Marshal.SizeOf(typeof(T));
            int p = 0;
            foreach (T[] array in arrays) {
                Buffer.BlockCopy(array, 0, result, p, array.Length);
                p += array.Length;
            }
            return result;
        }
    }
}
