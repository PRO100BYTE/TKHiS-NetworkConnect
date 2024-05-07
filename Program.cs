using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

class Program
{
    static void Main()
    {
        if (!IsAdministrator())
        {
            Console.WriteLine("Для работы программы требуются права администратора");
            Thread.Sleep(10000);
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("TKHiS NetworkConnect by #TheDayG0ne");
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("1. Подключиться к сети техникума через Орион");
            Console.WriteLine("2. Подключиться к сети техникума через Ростелеком");
            Console.WriteLine("0. Выход");
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("Выберите необходимое действие");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ConnectToNetwork("Орион");
                    break;
                case "2":
                    ConnectToNetwork("Ростелеком");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                    Thread.Sleep(3000);
                    break;
            }
        }
    }

    static void ConnectToNetwork(string network)
    {
        var adapters = NetworkInterface.GetAllNetworkInterfaces();
        Console.Clear();
        Console.WriteLine("TKHiS NetworkConnect by #TheDayG0ne"); 
        Console.WriteLine("-----------------------------------------------------");
        Console.WriteLine("Выберите сетевой адаптер, которому необходимо сменить IP:");
        for (int i = 0; i < adapters.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {adapters[i].Name}");
        }

        int adapterIndex = Convert.ToInt32(Console.ReadLine()) - 1;
        var selectedAdapter = adapters[adapterIndex];

        if (network == "Орион")
        {
            Console.Clear();
            Console.WriteLine("TKHiS NetworkConnect by #TheDayG0ne");
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("Выбрано: Подключение к сети техникума через Орион");
            Console.WriteLine($"Выбран сетевой адаптер: {selectedAdapter.Name}");
            var ipV4Address = selectedAdapter.GetIPProperties().UnicastAddresses.FirstOrDefault(x => x.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.Address.ToString();
            Console.WriteLine(" ");
            Console.WriteLine($"Ваш текущий IP: {ipV4Address}");
            Console.WriteLine("Вы можете использовать его в качестве статического IP для подключения к сети, либо же задать другой");
            Console.WriteLine(" ");
            Console.WriteLine("1. Использовать этот IP");
            Console.WriteLine("2. Изменить IP адрес");

            var ipChoice = Console.ReadLine();

            if (ipChoice == "1")
            {
                Console.Clear();
                Console.WriteLine("TKHiS NetworkConnect by #TheDayG0ne");
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("Устанавливаю следующие параметры:");
                Console.WriteLine($"- IP: {ipV4Address}");
                Console.WriteLine("- Маска подсети: 255.255.254.0");
                Console.WriteLine("- DNS: 10.19.140.3, 10.19.140.10");
                SetStaticIP(selectedAdapter.Name, ipV4Address, "255.255.254.0", "10.19.140.10", "10.19.140.3", "10.19.140.10");
                Thread.Sleep(3000);
                Console.WriteLine(" ");
                Console.WriteLine("Готово! :)");
                Thread.Sleep(1000);
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine(" ");
                Console.WriteLine("Возврат в главное меню через 10 секунд...");
                Thread.Sleep(10000);
            }
            else if (ipChoice == "2")
            {
                Console.Clear();
                Console.WriteLine("TKHiS NetworkConnect by #TheDayG0ne");
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("Введите новый IP-адрес:");
                var newIp = Console.ReadLine();

                Console.Clear();
                Console.WriteLine("TKHiS NetworkConnect by #TheDayG0ne");
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("Устанавливаю следующие параметры:");
                Console.WriteLine($"- IP: {newIp}");
                Console.WriteLine("- Маска подсети: 255.255.254.0");
                Console.WriteLine("- DNS: 10.19.140.3, 10.19.140.10");
                SetStaticIP(selectedAdapter.Name, newIp, "255.255.254.0", "10.19.140.10", "10.19.140.3", "10.19.140.10");
                Thread.Sleep(3000);
                Console.WriteLine(" ");
                Console.WriteLine("Готово! :)");
                Thread.Sleep(1000);
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine(" ");
                Console.WriteLine("Возврат в главное меню через 10 секунд...");
                Thread.Sleep(10000);
            }
        }
        else if (network == "Ростелеком")
        {
            Console.Clear();
            Console.WriteLine("TKHiS NetworkConnect by #TheDayG0ne");
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("Выбрано: Подключение к сети техникума через Ростелеком");
            Console.WriteLine($"Выбран сетевой адаптер: {selectedAdapter.Name}");
            Thread.Sleep(3000);
            Console.Clear();
            Console.WriteLine("TKHiS NetworkConnect by #TheDayG0ne");
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("Получаю параметры IP через DHCP...");
            SetDHCP(selectedAdapter.Name);
            Thread.Sleep(3000);
            Console.WriteLine(" ");
            Console.WriteLine("Готово! :)");
            Thread.Sleep(1000);
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine(" ");
            Console.WriteLine("Возврат в главное меню через 10 секунд...");
            Thread.Sleep(10000);
        }
    }

    static void SetStaticIP(string networkInterface, string ipAddress, string subnetMask, string defaultGateway, string preferredDns, string alternateDns)
    {
        ProcessStartInfo psi = new ProcessStartInfo("netsh", $"interface ip set address \"{networkInterface}\" static {ipAddress} {subnetMask} {defaultGateway} 1")
        {
            Verb = "runas",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        Process.Start(psi);

        psi = new ProcessStartInfo("netsh", $"interface ip set dns \"{networkInterface}\" static {preferredDns} primary")
        {
            Verb = "runas",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        Process.Start(psi);

        psi = new ProcessStartInfo("netsh", $"interface ip add dns \"{networkInterface}\" {alternateDns} index=2")
        {
            Verb = "runas",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        Process.Start(psi);
    }

    static void SetDHCP(string networkInterface)
    {
        ProcessStartInfo psi = new ProcessStartInfo("netsh", $"interface ip set address \"{networkInterface}\" dhcp")
        {
            Verb = "runas",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        Process.Start(psi);

        psi = new ProcessStartInfo("netsh", $"interface ip set dns \"{networkInterface}\" dhcp")
        {
            Verb = "runas",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        Process.Start(psi);
    }

    static bool IsAdministrator()
    {
        var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
        var principal = new System.Security.Principal.WindowsPrincipal(identity);
        return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
    }
}
