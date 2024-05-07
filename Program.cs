using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

class Program
{
    static void Main()
    {
        if (!IsAdministrator())
        {
            Console.WriteLine("Для работы программы требуются права администратора");
            return;
        }

        while (true)
        {
            Console.WriteLine("1. Подключиться к сети техникума (через Орион)");
            Console.WriteLine("2. Подключиться к сети техникума (через Ростелеком)");
            Console.WriteLine("0. Выход");

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
                    break;
            }
        }
    }

    static void ConnectToNetwork(string network)
    {
        var adapters = NetworkInterface.GetAllNetworkInterfaces();
        for (int i = 0; i < adapters.Length; i++)
        {
            Console.WriteLine("Выберите сетевой адаптер, которому необходимо сменить IP:");
            Console.WriteLine($"{i + 1}. {adapters[i].Name}");
        }

        int adapterIndex = Convert.ToInt32(Console.ReadLine()) - 1;
        var selectedAdapter = adapters[adapterIndex];

        if (network == "Орион")
        {
            Console.WriteLine("Выбрано подключение к сети техникума через Орион");
            var ipV4Address = selectedAdapter.GetIPProperties().UnicastAddresses.FirstOrDefault(x => x.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.Address.ToString();
            Console.WriteLine($"Ваш текущий IP: {ipV4Address}");
            Console.WriteLine("Вы можете использовать его в качестве статического IP для подключения к сети, либо же задать другой");

            Console.WriteLine("1. Использовать этот IP");
            Console.WriteLine("2. Изменить IP адрес");

            var ipChoice = Console.ReadLine();

            if (ipChoice == "1")
            {
                SetStaticIP(selectedAdapter.Name, ipV4Address, "255.255.254.0", "10.19.140.10", "10.19.140.3", "10.19.140.10");
            }
            else if (ipChoice == "2")
            {
                Console.WriteLine("Введите новый IP-адрес:");
                var newIp = Console.ReadLine();

                SetStaticIP(selectedAdapter.Name, newIp, "255.255.254.0", "10.19.140.10", "10.19.140.3", "10.19.140.10");
            }
        }
        else if (network == "Ростелеком")
        {
            Console.WriteLine("Выбрано подключение к сети техникума через Ростелеком");
            SetDHCP(selectedAdapter.Name);
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
