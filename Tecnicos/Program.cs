using Renci.SshNet;
using System;
using System.Net;
using System.Net.NetworkInformation;

class Program
{

    static void Menu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Seleccione una opción:");
            Console.WriteLine("1) Ping");
            Console.WriteLine("2) Cargar Backup");
            Console.WriteLine("3) Marcar/Desmarcar Tarjeta de Red");
            Console.WriteLine("4) Realizar Compliance");
            Console.WriteLine("5) Establecer Hora");
            Console.WriteLine("0) Salir");
            Console.Write("Ingrese su opción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    MenuPing();
                    break;
                case "2":
                    CargarBackup();
                    break;
                case "3":
                    //MarcarDesmarcarTarjetaRed();
                    break;
                case "4":
                    //RealizarCompliance();
                    break;
                case "5":
                    //EstablecerHora();
                    break;
                case "0":
                    Console.WriteLine("Saliendo...");
                    return;
                default:
                    Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }

    }

    static void MenuPing()
    {
        Dictionary<int, string> ip_device = new Dictionary<int, string>();
        ip_device.Add(1, "192.168.1.20");
        ip_device.Add(2, "192.168.88.1");
        ip_device.Add(3, "192.168.1.100");
        int opcion;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1) Antena");
            Console.WriteLine("2) Ubiquiti");
            Console.WriteLine("3) Furukawa");
            Console.WriteLine("4) IP Personalizada");
            Console.WriteLine("5) Volver al Menu Principal");
            try
            {
                opcion = int.Parse(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ingreso una opcion no valida!!!");
                opcion = 30;
            }

            if (opcion >= 1 && opcion <= 3)
            {   
                Console.Clear();   
                SendPing(ip_device[opcion]);
            
            }else if(opcion == 4){
                Console.Clear();
                Console.WriteLine("Ingrese la IP: ");
                string ip_custom = Console.ReadLine();
                SendPing(ip_custom);   
            }else if (opcion == 5)
            {
                return;
            }
            else
            {
                Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                Console.ReadKey();
                

            }
        }
    }

   
    static void SendPing(string direccion){


        using (Ping ping = new Ping())
        {
            Console.Clear();
            
            while (true)
            {
                try
                {
                    PingReply respuesta = ping.Send(direccion);
                    if (respuesta.Status == IPStatus.Success)
                    {
                        Console.WriteLine($"Respuesta de {direccion}: Tiempo={respuesta.RoundtripTime}ms");
                    }
                    else
                    {
                        Console.WriteLine($"Ping fallido: {respuesta.Status}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al hacer ping: {ex.Message}");
                }

                if (Console.KeyAvailable)
                {
                    
                    var key = Console.ReadKey(intercept: true).Key;

                    
                    if (key == ConsoleKey.Escape)
                    {
                        Console.WriteLine($"\nFinalizando el Ping a la IP: {direccion}");
                        break;
                    }
                }

                Thread.Sleep(1000);
               
            }
        }

        
        Console.WriteLine("Ping finalizado.");
        Console.ReadKey();
    
    }

    static bool TestPing(string direccion) {
        using (Ping ping = new Ping())
        {

            PingReply respuesta = ping.Send(direccion);

            bool status = respuesta.Status == IPStatus.Success ? true : false;
            return status;
        }

     }


    static void CargarBackup()
    {
        Console.Clear();
        Dictionary<string, string> path = new Dictionary<string, string>();
        path.Add("1", "C:\\Backups\\sytemM5.cfg");
        path.Add("2", "C:\\Backups\\sytemM2.cfg");
        path.Add("3", "C:\\Backups\\sytemAC.cfg");

        Console.WriteLine("Ingrese el tipo de tecnologia con el que hara el backup");
        Console.WriteLine("1) M5");
        Console.WriteLine("2) M2");
        Console.WriteLine("3) AC");
        Console.WriteLine("4) Volver al Menu Principal");
        string option = Console.ReadLine();

        if (path.TryGetValue(option, out string valor))
        {
            Console.WriteLine(valor);
        }
        else if (option == "4")
        {
            return;

        }else{
            Console.WriteLine("La opcion no es válida");
        }

        Console.ReadKey(true);
    }
    static void Main(string[] args)
    {
        Menu();
    }
}
