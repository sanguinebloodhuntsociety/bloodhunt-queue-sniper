using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace bloodhunt_queue_sniper
{
    public class Util
    {

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, IntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const uint MOUSEEVENTF_LEFTUP = 0x0004;
        public const uint MOUSEEVENTF_MOVE = 0x0001;
        public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;


        const int SW_RESTORE = 9;
        const uint WM_KEYDOWN = 0x0100;
        const int VK_RETURN = 0x0D;

        public static string ReadBloodhuntConfigToFindOutBinding(string action)
        {
            string filePath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tiger", "Settings"), "InputBindings.json");


            if (File.Exists(filePath))
            {
                try
                {
                    string fileContents = File.ReadAllText(filePath);
                    JObject inputBindingsObject = JObject.Parse(fileContents);
                    JArray keyboardMouseMappings = (JArray)inputBindingsObject["keyboardMouseMappings"];
                    foreach (JObject keyboardMouseMapping in keyboardMouseMappings)
                    {
                        if (keyboardMouseMapping["actionName"].ToString() == action)
                        {
                            string bindedKey = keyboardMouseMapping["key"].ToString().ToLower();

                            Dictionary<string, string> numberMap = new Dictionary<string, string>
                            {
                                ["one"] = "1",
                                ["two"] = "2",
                                ["three"] = "3",
                                ["four"] = "4",
                                ["five"] = "5",
                                ["six"] = "6",
                                ["seven"] = "7",
                                ["eight"] = "8",
                                ["nine"] = "9"
                            };

                            if (numberMap.TryGetValue(bindedKey, out string tryNumber) != false)
                            {
                                return numberMap.TryGetValue(bindedKey, out string parsedNumber) ? "D" + parsedNumber : "null";
                            }
                            else
                                return bindedKey.ToUpper();
                            {
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    return "null";
                }
            }
            return "null";
        }

        public static void EnableAllPartyManagingKeys()
        {
            foreach (MenuItem item in Main.Instane.trayIcon.ContextMenu.MenuItems)
            {
                if (item.Text == "Party")
                {
                    foreach (MenuItem subItem in item.MenuItems)
                    {
                        if (!subItem.Text.Contains("Leave"))
                        {
                            subItem.Enabled = true;
                        }
                        else
                        {
                            subItem.Enabled = false;
                        }
                    }
                    break;
                }
            }
        }

        public static void DisableAllPartyManagingKeys()
        {
            foreach (MenuItem item in Main.Instane.trayIcon.ContextMenu.MenuItems)
            {
                if (item.Text == "Party")
                {
                    foreach (MenuItem subItem in item.MenuItems)
                    {
                        if (!subItem.Text.Contains("Leave"))
                        {
                            subItem.Enabled = false;
                        }
                        else
                        {
                            subItem.Enabled = true;
                        }
                    }
                    break;
                }
            }
        }

        public static void OpenBloodhuntAndOpenGameModeWindowToQueue(string mode)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName.Contains("Tiger"))
                {
                    IntPtr hWnd = process.MainWindowHandle;
                    ShowWindow(hWnd, SW_RESTORE);
                    SetForegroundWindow(hWnd);
                    SetFocus(hWnd);
                    SetCursorPos(100, 100);
                    mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_LEFTDOWN, 100, 100, 0, IntPtr.Zero);
                    PostMessage(hWnd, WM_KEYDOWN, (int)Enum.Parse(typeof(Keys), Main.ChangeGameModekey), 0);
                    mode = mode.ToLower();
                    Thread.Sleep(100);
                    if (mode.Contains("trio"))
                    {
                        int x = (1268 * 65535) / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                        int y = (582 * 65535) / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_LEFTDOWN, x, y, 0, IntPtr.Zero);
                        mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_LEFTUP, x, y, 0, IntPtr.Zero);
                    }
                    else if (mode.Contains("duo"))
                    {
                        int x = (942 * 65535) / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                        int y = (530 * 65535) / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_LEFTDOWN, x, y, 0, IntPtr.Zero);
                        mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_LEFTUP, x, y, 0, IntPtr.Zero);
                    }
                    else if (mode.Contains("solo"))
                    {
                        int x = (624 * 65535) / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                        int y = (582 * 65535) / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_LEFTDOWN, x, y, 0, IntPtr.Zero);
                        mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_LEFTUP, x, y, 0, IntPtr.Zero);
                    } else if (mode.Contains("tdm"))
                    {
                        int x = (1594 * 65535) / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                        int y = (582 * 65535) / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_LEFTDOWN, x, y, 0, IntPtr.Zero);
                        mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | MOUSEEVENTF_LEFTUP, x, y, 0, IntPtr.Zero);
                    }
                    break;
                }
            }
        }

        public static void OpenBloodhuntAndPressDeQueueButton()
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName.Contains("Tiger"))
                {
                    IntPtr hWnd = process.MainWindowHandle;
                    SetForegroundWindow(hWnd);
                    ShowWindow(hWnd, SW_RESTORE);
                    PostMessage(hWnd, WM_KEYDOWN, (int)Enum.Parse(typeof(Keys), Main.QueueKey), 0);
                    break;
                }
            }
        }

        public static string GetHardwareID()
        {
            string processorId = "";
            string motherboardId = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            ManagementObjectCollection collection = searcher.Get();
            foreach (ManagementObject obj in collection)
            {
                processorId = obj["ProcessorID"].ToString();
                break;
            }
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            collection = searcher.Get();
            foreach (ManagementObject obj in collection)
            {
                motherboardId = obj["SerialNumber"].ToString();
                break;
            }
            string combinedID = processorId + motherboardId;
            return combinedID.GetHashCode().ToString();
        }
    }
}
