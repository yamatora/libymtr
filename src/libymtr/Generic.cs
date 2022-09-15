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
        /// Return subarray after offset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static T[] GetPart<T>(T[] src, int offset) {
            return GetPart(src, offset, src.Length - offset);
        }
        /// <summary>
        /// Return subarray of specified length from offset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T[] GetPart<T>(T[] src, int offset, int length) {
            if (src.Length - length < offset) {
                throw new Exception("GetPart: Over size");
            }
            T[] result = new T[length];
            Buffer.BlockCopy(src, offset, result, 0, length);
            return result;
        }
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
        /// <summary>
        /// Compare T[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsEqualArray<T>(T[] a, T[] b) {
            return Enumerable.SequenceEqual(a, b);
        }
    }
}
