using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
//using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

namespace libymtr.Windows.Registry {
    public static class Environment {
        //  Variable name
        private const string NAME_PATH = "Path";
        //  Message
        private const string MSG_ADMIN_ONLY = "Only executable as admin";

        /// <summary>
        /// Get Environment variables from Registry
        /// </summary>
        /// <param name="varName">Variable name: ex. "Path"</param>
        /// <param name="target">Environment variable's target</param>
        /// <returns></returns>
        public static string[] GetEnvironmentVariable(string varName, EnvironmentVariableTarget target = EnvironmentVariableTarget.User) {
            Windows.CheckOS();

            //  Get values
            return System.Environment.GetEnvironmentVariable(varName, target).Split(";");
        }
        /// <summary>
        /// Set Environment variables to Registry
        /// </summary>
        /// <param name="varName">Variable name: ex. "Path"</param>
        /// <param name="valueArray">Environment variable's value array</param>
        /// <param name="target">Environment variable's target</param>
        /// <param name="forceReboot">Whether reboot or not</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static bool SetEnvirionmentVariable(string varName, string[] valueArray, EnvironmentVariableTarget target = EnvironmentVariableTarget.User, bool forceReboot = false) {
            Windows.CheckOS();

            ////  Check running as admin
            //var principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            //if (!principal.IsInRole(WindowsBuiltInRole.Administrator)) {
            //    if (forceReboot) {
            //        throw new Exception(MSG_ADMIN_ONLY);
            //    }
            //    return false;
            //}

            //  Set values
            var value = Generic.ConcatByChar(';', valueArray);
            System.Environment.SetEnvironmentVariable(varName, value, target);
            return true;
        }

        /// <summary>
        /// Add dir to "Path"
        /// </summary>
        /// <param name="dirPath">Directory path to add to "Path"</param>
        /// <param name="target">Environment variable's target</param>
        /// <param name="forceReboot">Whether reboot or not</param>
        /// <returns></returns>
        public static bool AddPath(string dirPath, EnvironmentVariableTarget target = EnvironmentVariableTarget.User, bool forceReboot = false) {
            var list = GetEnvironmentVariable(NAME_PATH, target).ToList();
            if (list.Contains(dirPath)) {
                return true;
            }
            list.Add(dirPath);
            return SetEnvirionmentVariable(NAME_PATH, list.ToArray(), target, forceReboot);
        }
        /// <summary>
        /// Remove dir from "Path"
        /// </summary>
        /// <param name="dirPath">Directory path to remove from "path"</param>
        /// <param name="target">Environment variable's target</param>
        /// <param name="forceReboot">Whether reboot or not</param>
        /// <returns></returns>
        public static bool RemovePath(string dirPath, EnvironmentVariableTarget target = EnvironmentVariableTarget.User, bool forceReboot = false) {
            var list = GetEnvironmentVariable(NAME_PATH, target).ToList();
            if (!list.Contains(dirPath)) {
                return true;
            }
            list.Remove(dirPath);
            return SetEnvirionmentVariable(NAME_PATH, list.ToArray(), target, forceReboot);
        }
    }
}
