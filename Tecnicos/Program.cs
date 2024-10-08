using Renci.SshNet;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.IO;
using DotNetEnv;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


class Program
{
    
    static void Menu()
    {
        while (true)
        {
            Console.Clear();


            Console.ForegroundColor = ConsoleColor.Cyan;

            
            string border = new string('═', 50);
            Console.WriteLine("╔" + border + "╗");

            
            string title = "Menú Principal";
            int padding = (50 - title.Length) / 2;
            Console.WriteLine("║" + new string(' ', padding) + title + new string(' ', padding) + "║");

            
            Console.WriteLine("╠" + border + "╣");

            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("║ 1) Ping                                          ║");
            Console.WriteLine("║ 2) Cargar Backup                                 ║");
            Console.WriteLine("║ 3) Marcar/Desmarcar Tarjeta de Red               ║");
            Console.WriteLine("║ 4) Realizar Compliance                           ║");
            Console.WriteLine("║ 5) Establecer Hora                               ║");
            Console.WriteLine("║ 6) Detectar Cantidad de Clientes                 ║");
            Console.WriteLine("║ 0) Salir                                         ║");

           
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╚" + border + "╝");

           
            Console.ResetColor();
            Console.Write("\n Ingrese su opción: ");

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
                    MenuTarjetaRed();   
                    break;
                case "4":

                    if (Compliance())
                    {
                        Console.WriteLine("Se Ha ReaLizado El Compliance!!!");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("No se pudo realizar el Compliance");
                        Console.ReadKey();
                    }
                    break;
                case "5":
                    try
                    {
                       
                        DateTime horaInternet = ObtenerHoraDeInternet();
                       
                        EstablecerHora(horaInternet);
                        Console.ForegroundColor= ConsoleColor.Green;
                        Console.WriteLine("Se Ha Establecido La Hora Correctamente!!!");
                        Console.ResetColor();
                        Console.ReadKey();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }

                    break;
                case "6":
                    CantidadClientes();
                    break;
                case "0":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Saliendo...");
                    Console.ResetColor();
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }

    }



    

    static DateTime ObtenerHoraDeInternet()
    {
        using (HttpClient client = new HttpClient())
        {
            
            string url = "http://worldtimeapi.org/api/timezone/America/Argentina/San_Juan";

            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            JObject json = JObject.Parse(jsonResponse);

            string datetimeString = json["datetime"].ToString();
            DateTime horaLocal = DateTime.Parse(datetimeString);

           

           
            return horaLocal;
        }
    }

    static void EstablecerHora(DateTime hora)
    {
        try
        {
            
            string nuevaHora = hora.ToString("HH:mm:ss");
            string nuevaFecha = hora.ToString("dd-MM-yyyy");


            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C date {nuevaFecha} && time{nuevaHora}";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;  // No mostrar la ventana de la consola
            process.StartInfo.UseShellExecute = false;

            process.Start();
            
            process.WaitForExit();
          

            Console.WriteLine($"La hora ha sido establecida a {nuevaHora}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al establecer la hora: {ex.Message}");
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
            Console.WriteLine("MENU PING\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1) Antena");
            Console.WriteLine("2) Ubiquiti");
            Console.WriteLine("3) Furukawa");
            Console.WriteLine("4) IP Personalizada");
            Console.WriteLine("5) Volver al Menu Principal\n");
            Console.ResetColor();
            Console.Write("Ingrese su opción: ");
            try
            {
                opcion = int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ingreso una opcion no valida!!!");
                Console.ResetColor();
                opcion = 30;
            }

            if (opcion >= 1 && opcion <= 3)
            {
                Console.Clear();
                SendPing(ip_device[opcion]);

            }
            else if (opcion == 4)
            {
                string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Ingrese la IP: ");
                Console.ResetColor();
                string ip_custom = Console.ReadLine();
                if (Regex.IsMatch(ip_custom, pattern))
                {
                    SendPing(ip_custom);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("IP no valida!!!");
                    Console.ReadKey();
                    Console.ResetColor();
                }

            }
            else if (opcion == 5)
            {
                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                Console.ReadKey();


            }
        }
    }


    static void SendPing(string direccion)
    {


        using (Ping ping = new Ping())
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            while (true)
            {
                try
                {
                    PingReply respuesta = ping.Send(direccion);
                    if (respuesta.Status == IPStatus.Success)
                    {
                        Console.ForegroundColor  = ConsoleColor.Green;
                        Console.WriteLine($"Respuesta de {direccion}: Tiempo={respuesta.RoundtripTime}ms");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Ping fallido: {respuesta.Status}");
                        Console.ResetColor ();  

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
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"\nFinalizando el Ping a la IP: {direccion}");
                        break;
                    }
                }

                Thread.Sleep(1000);

            }
        }



        Console.WriteLine("Ping finalizado.");
        Console.ResetColor();
        Console.ReadKey();

    }


    static bool HacerPing(string direccion)
    {
        using (Ping ping = new Ping())
        {

            PingReply respuesta = ping.Send(direccion);

            bool status = respuesta.Status == IPStatus.Success ? true : false;
            return status;
        }
    }
    static bool TestPing(string direccion)

    {

        int maxIntentos = 5;
        for (int intento = 1; intento <= maxIntentos; intento++)
        {

            if (HacerPing(direccion))
            {
                return true; // Salir si el ping es exitoso
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Intento {intento} fallido. Reintentando en 10 segundos...");
                Console.ForegroundColor = ConsoleColor.Yellow;
                for (int i = 10; i >= 0; i--)
                {
                    Console.Write($"\rReintentando en {i} segundos...");
                    Task.Delay(1000);

                }

                Console.WriteLine();
                Console.ResetColor();
            }
        }
        return false;
    }





    static void CargarBackup()
    {
        Console.Clear();
        

        Console.WriteLine("Ingrese el tipo de tecnologia con el que hara el backup");
        Console.WriteLine("1) M5");
        Console.WriteLine("2) M2");
        Console.WriteLine("3) AC");
        Console.WriteLine("4) Volver al Menu Principal");
        string option = Console.ReadLine();
        string path = OpenFileDialog2();

        if (path!="")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[+] Se Fijara la Tarjeta...");
            Console.ResetColor();
            MenuTarjetaRed();

            Console.WriteLine("[+] Se hara prueba de conexion con la antena:\n");
            Task.Delay(10000);
            bool conexionExitosa = TestPing("192.168.1.20");

            if (conexionExitosa)
            {
                bool request = Transfercfg(path);

                if (request)
                {
                   if (SaveConfig())
                    {
                        if (option  != "3")
                        {   
                            Console.ForegroundColor= ConsoleColor.Yellow;
                            Console.WriteLine("[!] Se Hará El Compliance.");
                            Console.ResetColor();
                            Task.Delay(15000);
                            conexionExitosa = TestPing("192.168.1.20");
                            if (conexionExitosa)
                            {
                                Task.Delay(15000);
                               if(Compliance())
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Se Ha Realizado el Compliance, Esperando Por la Conexión con la Antena");
                                    Console.ResetColor();
                                    Task.Delay(15000);
                                    TestPing("192.168.1.20");
                                    

                                }
                            }
                        }
                        
                        
                    }
                }
            }
            else
            {
                Console.WriteLine("No se Pudo realizar la tarea de BackUp");
               
            }

        }
        else if (option == "4")
        {
            return;

        }
        else
        {
            Console.WriteLine("La opcion no es válida");
        }

        Console.ReadKey(true);
    }


    static bool Compliance()
    {
        Env.Load();
        string host = Env.GetString("HOST");
        string username = Env.GetString("USERNAME");
        string password = Env.GetString("PASSWORD");
        int puerto = int.Parse(Env.GetString("PORT"));
        string comando = "touch /etc/persistent/ct && save";
        try
        {
            using (var ssh = new SshClient(host, puerto, username, password))
            {
                ssh.Connect();
                var cmd = ssh.RunCommand(comando);
                cmd = ssh.RunCommand("reboot");
                ssh.Disconnect();
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al ejecutar el comando SSH: {ex.Message}");
            return false;
        }

    }
    static bool SaveConfig()
    {
        Env.Load();
        string host = Env.GetString("HOST");
        string username = Env.GetString("USERNAME");
        string password = Env.GetString("PASSWORDF"); 
        int puerto = int.Parse(Env.GetString("PORTF")); 
        string comando = "cfgmtd -wp /etc && reboot";
        try
        {
            using (var ssh = new SshClient(host, puerto, username, password))
            {
                ssh.Connect();
                var cmd = ssh.RunCommand(comando);
                ssh.Disconnect();
            }
            return true; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al ejecutar el comando SSH: {ex.Message}");
            
            return false; 
        }
    }

    static bool Transfercfg(string path)
    {
        Env.Load();
        string host = Env.GetString("HOST");
        string username = Env.GetString("USERNAME");
        string password = Env.GetString("PASSWORDF");
        int puerto = int.Parse(Env.GetString("PORTF"));

        string localFilePath = path;
        string remoteFilePath = "/tmp/system.cfg";

        SftpClient sftp = null;

        try
        {
            sftp = new SftpClient(host, puerto, username, password);
            sftp.Connect();

            using (var fileStream = new FileStream(localFilePath, FileMode.Open))
            {
                sftp.UploadFile(fileStream, remoteFilePath);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[+] Archivo de configuración caragado correctamente!!!");
                Console.ResetColor();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor= ConsoleColor.Red;  
            Console.WriteLine($"Error al transferir el archivo: {ex.Message}");
            Console.ResetColor();
            return false; 
        }
        finally
        {
            
            if (sftp != null && sftp.IsConnected)
            {
                sftp.Disconnect();
                
            }
        }
    }


    static void MenuTarjetaRed()
    {
        
        while (true)
            
        {
            Console.Clear();
            Console.WriteLine("MENU TARJETA DE RED\n");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("1) Fijar / Desmarcar ip (default)");
            Console.WriteLine("2) Fijar ip en una ip Custom");
            Console.WriteLine("3) Volver al menu principal\n");
            Console.ResetColor();
            string op = Console.ReadLine();

            switch (op) {
                case "1":
                    TarjetaRed();
                    break;
                case "2":
                    Console.Clear();
                    string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Ingrese una IP de Dispositivo");
                    
                    string ip = Console.ReadLine();
                    Console.WriteLine("Ingrese el prefijo de red (24 o 22)");
                    string prefix = Console.ReadLine();

                    Console.ResetColor();

                    if (Regex.IsMatch(ip, pattern)) {
                        string[] parts = ip.Split('.');
                        parts[3] = "153";
                        ip = string.Join(".", parts);
                        if (prefix == "24" || prefix == "22")
                        {
                            string mascara = prefix == "24" ? "255.255.255.0" : "255.255.252.0";
                            TarjetaRed(ip, mascara);
                        }
                        else
                        {
                            Console.ForegroundColor= ConsoleColor.Red;
                            Console.WriteLine("Se Ingreso un prefijo de red no valido. Presione una tecla para continuar");
                            Console.ResetColor();
                            Console.ReadKey();
                        }
                        


                    }
                    else
                    {
                        Console.ForegroundColor= ConsoleColor.Red;
                        Console.WriteLine("IP No es válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                    }
                    break;
                case "3":
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;

            }
        }



    }


    static void TarjetaRed(string ip="192.168.1.53", string mascarared="255.255.255.0")
    {
        Console.Clear();
        string nombreInterfaz = "Ether0";
        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
        bool usandoDHCP = false;

        foreach (NetworkInterface iface in interfaces)
        {
            if (iface.Name == nombreInterfaz)
            {
                IPInterfaceProperties ipProperties = iface.GetIPProperties();

                if (iface.Supports(NetworkInterfaceComponent.IPv4))
                {
                    IPv4InterfaceProperties ipv4Properties = ipProperties.GetIPv4Properties();
                    if (ipv4Properties != null)
                    {
                        usandoDHCP = ipv4Properties.IsDhcpEnabled;
                    }
                }
            }
        }

        if (usandoDHCP)
        {
            FijarTarjeta(ip,mascarared);
            Console.WriteLine("El comando no se ejecutó correctamente, intente nuevamente...");
            Console.ReadKey();
        }
        else
        {
            DesmarcarTarjeta();
            Console.WriteLine("Surgio un Error, vuelva a intentarlo...");
            Console.ReadKey();
        }
    }


    static bool FijarTarjeta(string ip, string mascaradered)
    {
        string comando = $"interface ip set address \"Ether0\" static {ip} {mascaradered}";
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "netsh",
            Arguments = comando,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false, // No usar la shell del sistema
            CreateNoWindow = true // No mostrar una ventana de consola
        };


        try
        {
            using (Process process = Process.Start(psi))
            {
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    
                    Console.ReadKey();
                    Console.ResetColor();
                    return true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    
                    
                    Console.ResetColor();
                    return false;
                }
            }
        }
        catch
        {
            
            return false;
        }
    }


    static void DesmarcarTarjeta()
    {
        string comando = "interface ip set address \"Ether0\" dhcp";
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "netsh",
            Arguments = comando,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        try
        {
            using (Process process = Process.Start(psi))
            {
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Se ha Descmarcado la tarjeta de red con Exito!!");
                    Console.ReadKey();
                    Console.ResetColor();
                    return;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("El comando no se ejecutó correctamente, intente nuevamente...");
                    Console.ReadKey();
                    Console.ResetColor();
                    return;
                }
            }
        }
        catch
        {
            Console.WriteLine("Surgio un Error, vuelva a intentarlo...");
            Console.ReadKey();
        }
    }

    
    static string OpenFileDialog2()
    {
        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string downloadsPath = Path.Combine(userProfile, "Downloads");

        OpenFileDialog openFileDialog = new OpenFileDialog();


        openFileDialog.InitialDirectory = downloadsPath;
        openFileDialog.Filter = "Todos los archivos (*.*)|*.*|Archivos de texto (*.txt)|*.txt";
        openFileDialog.FilterIndex = 1;
        openFileDialog.RestoreDirectory = true;

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
          
            string filePath = openFileDialog.FileName;
           return filePath;
        }
        else
        {
            
            return "";
        }

       
     
    }

    static void CantidadClientes ()
    {
        Console.Clear();
        string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Ingrese la IP del Panel: ");
        Console.ResetColor();
        string host = Console.ReadLine();

        if (Regex.IsMatch(host, pattern))
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Ingrese la Contraseña del dispositivo: ");
                Console.ResetColor();
                string password = ReadPassword();
                string user = "ubnt";
                int port = 23;
                string command= "wstalist |grep \"mac\" |wc -l ; mca-status | grep \"deviceName=\" | sed -n 's/.*deviceName=\\([^,]*\\).*/\\1/p'";

                using (var client = new SshClient(host, port,user, password))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                                                
                        var sshCommand = client.CreateCommand(command);
                        var result = sshCommand.Execute();

                        string[] lines = result.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Clear();
                        if (lines.Length >= 2)
                        {
                            Console.WriteLine($"Panel: {lines[1].Trim()}");
                            Console.WriteLine($"IP: {host}");
                            Console.WriteLine($"Cantidad de Clientes: {lines[0]}");
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Presione alguna Tecla para Continuar...");
                            Console.ResetColor();
                            Console.ReadKey();

                        }
                            
                    }
                    else
                    {
                        Console.WriteLine("No se pudo establecer la conexión.");
                        Console.ReadKey();
                    }

                    
                    client.Disconnect();
                    
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine("\nError: " + ex.Message);
                Console.ResetColor();
                Console.ReadKey();
            }
        }


            
            
        
        

    }

    static string ReadPassword()
    {
        string password = string.Empty;
        ConsoleKeyInfo key;

       
        do
        {
            key = Console.ReadKey(true); 

            
            if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password = password[..^1]; 
                    Console.Write("\b \b"); 
                }
            }
            
            else if (key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar; 
                Console.Write("*");
            }
        }
        while (key.Key != ConsoleKey.Enter); 

        return password;
    }

    [STAThread]
    static void Main(string[] args)
    {
        Menu();
        
    }
}
