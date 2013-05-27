using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MiningAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(45, 10);
            Console.Title = "Mining Adapter";
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Starting Automation");
            Console.Clear();

            int lengthP = System.IO.File.ReadLines("Automate\\Presets.txt").Count();
            string[] Presets  = System.IO.File.ReadAllLines("Automate\\Presets.txt");
            string[] Settings = System.IO.File.ReadAllLines("Automate\\Settings.txt");

            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "poclbm\\poclbm.ini");
            string[] GUISettings =   {
                                      "{",
                                      "    \"profiles\": [",
                                      "        {",
                                      "            \"username\": \"" + Settings[2] + "\",",
                                      "            \"balance_auth_token\": \"\",",
                                      "            \"name\": \"Slush\",",
                                      "            \"hostname\": \"" + Settings[0] + "\",",
                                      "            \"external_path\": \"\",",
                                      "            \"affinity_mask\": 0,",
                                      "            \"flags\": \"-v -w256 -f40\",",
                                      "            \"autostart\": true,",
                                      "            \"device\": 0,",
                                      "            \"password\": \"" + Settings[3] + "\",",
                                      "            \"port\": \"" + Settings[1] + "\"",
                                      "        }",
                                      "    ],",
                                      "    \"show_opencl_warning\": true,",
                                      "    \"start_minimized\": true",
                                      "}",
                                    };
            System.IO.File.WriteAllLines(path, GUISettings);

            Console.WriteLine("Running  ");
            Console.WriteLine("Miner    ");
            Console.WriteLine("Clock    ");

            string minerOld = "None";
            string clockOld = "None";
            string miner = "CG";
            string clock = "Mine";
            int clockSpeed = System.Convert.ToInt32(Settings[5]);
            double count = 1;

            KillMiner();

            while (!Console.KeyAvailable)
            {
                int n = 0;
                string[] Running = new string[lengthP/5];

                miner = "CG";
                clock = "Mine";

                for (int i = 2; i < lengthP; i = i + 5)
                {
                    Process[] pname = Process.GetProcessesByName(Presets[i]);
                    if (pname.Length == 1)
                    {
                        Running[n] = Presets[i - 1];

                        if (miner != "GUI" || miner != "None")
                        {
                            if (miner != "None")
                            {
                                if (Presets[i + 1] == "None")
                                {
                                    miner = "None";
                                }
                                if (Presets[i + 1] == "GUI")
                                {
                                    miner = "GUI";
                                }
                            }
                        }

                        if (Presets[i + 2] == "Stock")
                        {
                            clock = "Stock";
                        }

                        n++;
                    }
                }

                if(clock == "Mine")
                {
                    clockSpeed=System.Convert.ToInt32(Settings[5]);
                }
                else
                {
                    clockSpeed=System.Convert.ToInt32(Settings[4]);
                }

                if (clock != clockOld)
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "Automate\\BarelyClocked.exe";
                    process.StartInfo.Verb = "runas";
                    process.StartInfo.Arguments = "gpu=0 core=" + clockSpeed;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.Start();
                }

                if (miner == "CG" && miner == minerOld)
                {
                    count++;
                }

                if (miner != minerOld)
                {
                    count = 0;

                    KillMiner();
                }

                if (count == 360)
                {
                    count = 0;
                    Process[] cgminer = Process.GetProcessesByName("cgminer");
                    foreach (Process p in cgminer)
                    {
                        p.Kill();
                    }
                    minerOld = "GUI";
                }

                if (miner == "CG" && minerOld != "CG")
                {
                    Process cgminer = new Process();
                    cgminer.StartInfo.FileName = "CGMiner\\cgminer.exe";
                    cgminer.StartInfo.Verb = "runas";
                    cgminer.StartInfo.Arguments = "-o " + Settings[0] +
                                                 ":" + Settings[1] +
                                                 " -u " + Settings[2] +
                                                 " -p " + Settings[3] +
                                                 " -I 9 -k diablo -v 1 -w 256";
                    cgminer.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    cgminer.Start();
                }

                if (miner == "GUI" && minerOld != "GUI")
                {
                    Process.Start("GUIMiner\\guiminer.exe");
                }

                Console.Clear();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Running  ");
                for (int i = 0; i < n; i++)
                {
                    Console.SetCursorPosition(9, Console.CursorTop - 1);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(Running[i]);
                    Console.WriteLine("         ");
                }

                Console.ForegroundColor = ConsoleColor.White;
                if (n == 0)
                    n = 1;
                Console.SetCursorPosition(0, n);
                Console.WriteLine("Miner    ");

                if (miner == "CG")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (miner == "GUI")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.SetCursorPosition(9, Console.CursorTop - 1);
                if (miner == "None")
                {
                    Console.WriteLine(miner);
                }
                else
                {
                    Console.WriteLine(miner + "Miner");
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, n + 1);
                Console.WriteLine("Clock    ");

                if (clock == "Mine")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                Console.SetCursorPosition(9, Console.CursorTop - 1);
                Console.WriteLine(clockSpeed + "Mhz");

                System.Threading.Thread.Sleep(1000);
                minerOld = miner;
                clockOld = clock;
            }
        }
        public static void KillMiner()
        {
            Process[] cgminer = Process.GetProcessesByName("cgminer");
            foreach (Process p in cgminer)
            {
                p.Kill();
            }
            Process[] guiminer = Process.GetProcessesByName("guiminer");
            foreach (Process p in guiminer)
            {
                p.Kill();
            }
            Process[] poclbm = Process.GetProcessesByName("poclbm");
            foreach (Process p in poclbm)
            {
                p.Kill();
            }
        }
    }
}