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
using System.Diagnostics;
using System.ServiceProcess;
using DotNetEnv;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Microsoft.VisualBasic.ApplicationServices;
using System.Configuration;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

class Program
{
    [DllImport("iphlpapi.dll", ExactSpelling = true)]
    private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_MAXIMIZE = 3;

    static void Menu()
    {
        IntPtr hWndConsole = GetConsoleWindow();
        ShowWindow(hWndConsole, SW_MAXIMIZE);
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
            Console.WriteLine("║ 7) Detectar Dispositivos en la red               ║");
            Console.WriteLine("║ 8) Información de la antena del cliente          ║");
            Console.WriteLine("║ 9) Reiniciar Cliente                             ║");
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
                    EstablecerHora();

                    break;
                case "6":
                    CantidadClientes();
                    break;

                case "7":
                    Discovery();
                    Console.ReadKey(true);
                    break;
                case "8":
                    InfoCliente();
                    Console.ReadKey(true);
                    break;

                case "9":
                    ReiniciarCliente();
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



    public static bool SyncTime()
    {
        try
        {
            Process process = new Process();
            process.StartInfo.FileName = "w32tm";
            process.StartInfo.Arguments = "/resync";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();

            return process.ExitCode == 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool IsW32tmServiceRunning()
    {
        try
        {
            ServiceController sc = new ServiceController("w32time");
            return sc.Status == ServiceControllerStatus.Running;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static void StartW32tmService()
    {
        try
        {
            ServiceController sc = new ServiceController("w32time");
            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running);
            }
        }
        catch (Exception)
        {
            
        }
    }

    public static void EstablecerHora()
    {
        Console.Clear();
        Console.WriteLine("---- HORA ----");

        Console.WriteLine("Se establecerá la hora...");

        if (IsW32tmServiceRunning())
        {
            if (SyncTime())
            {
                Console.WriteLine("Se estableció correctamente la hora. Presione Enter para continuar.");
            }
            else
            {
                Console.WriteLine("No se pudo establecer la hora. Presione Enter para continuar.");
            }
        }
        else
        {
            StartW32tmService();
            if (SyncTime())
            {
                Console.WriteLine("Se estableció correctamente la hora. Presione Enter para continuar.");
            }
            else
            {
                Console.WriteLine("No se pudo establecer la hora. Presione Enter para continuar.");
            }
        }

        Console.ReadKey();
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


    static void ReiniciarCliente()
    {
        if (TestPing("192.168.1.20"))
        {
            Env.Load();
            string command = "reboot";
            string request = ExcecuteCommand(command : command,password : Env.GetString("PASSWORD"));

            if (request != null) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Se ha reiniciado con exito al cliente");
            }
            else
            {
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine("No se ha podido reiniciar al cliente");
            }


        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No hay conexion con la antena");
        }
        Console.ResetColor();
        return;
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
            try
            {
                if (HacerPing(direccion))
                {
                    return true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Intento {intento} fallido. Reintentando en 10 segundos...");
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    for (int i = 10; i >= 0; i--)
                    {
                        Console.Write($"\rReintentando en {i} segundos...");
                        Thread.Sleep(1000);
                    }

                    Console.WriteLine();
                    Console.ResetColor();
                }
            }
            catch (PingException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error al hacer ping: {ex.Message}. Intento {intento} fallido.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ocurrió un error inesperado: {ex.Message}");
                Console.ResetColor();
                return false;  // Si ocurre otro tipo de excepción, salir de la función
            }
        }
        return false;
    }





    static void CargarBackup()
    {
        Console.Clear();
        Console.WriteLine("MENU DE BACKUP\n");
        Console.ForegroundColor= ConsoleColor.Green;
        Console.WriteLine("1) Cargar Backup");
        Console.WriteLine("2) Volver al Menu Principal\n");
        Console.ResetColor();
        string option = "";
        Console.Write("Ingrese una opción: ");
        option = Console.ReadLine();
        
        switch (option)
        {

            case "1":
                string path = OpenFileDialog2();
                if (path != "")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[+] Se Fijara la Tarjeta...");
                    Console.ResetColor();
                    FijarTarjeta();

                    Console.WriteLine("[+] Se hará prueba de conexión con el dispositivo:\n");
                    Task.Delay(10000);
                    bool conexionExitosa = true;

                    if (conexionExitosa)
                    {
                        Console.WriteLine("[+] Se Transfirirá el archivo de configuración");
                        bool request = Transfercfg(path);

                        if (request)
                        {
                            if (SaveConfig())
                            {
                                Console.WriteLine("Se Ha cargado Correctamente el Backup");
                            }

                        }
                        else
                        {
                            Console.WriteLine("No se pudo transferir el archivo de configuración");
                            Console.ReadKey();
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No se pudo establecer conexion con el dispositivo");
                        Console.ReadKey(true);
                        return;
                    }


                }
                else
                {
                    Console.WriteLine("No se seleccionó ningun archivo de configuración");
                }

                break;
            
            case "2":
                return;

            default:
                Console.WriteLine("Ingreso una opcion no valida");
                Console.ReadKey();
                break;
        
        }
                

           
            
        
        

        Console.ReadKey(true);
    }


    static bool Compliance()
    {
        bool requeset = TestPing("192.168.1.20");

        if (requeset) {
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
        else
        {
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1) Fijar Tarjeta (antena)");
            Console.WriteLine("2) Fijar Tarjeta IP custom");
            Console.WriteLine("3) Desmarcar Tarjeta");
            Console.WriteLine("4) Volver al menu principal\n");
           
            Console.ResetColor();
            Console.Write("Ingrese una opción: ");
            string op = Console.ReadLine();

            switch (op) {
                case "1":
                    if (FijarTarjeta())

                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Se ha fijado la tarjeta de red Correctamente!!!");
                    }
                    else

                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No se ha podido fijar la tarjeta de red");
                    }
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
                            if(FijarTarjeta(ip, mascara))
                            {

                                Console.Clear();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Se ha fijado la tarjeta de red Correctamente!!!");
                            }
                            else

                            {
                                Console.Clear();
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("No se ha podido fijar la tarjeta de red");
                            }

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
                    DesmarcarTarjeta();
                    break;
                case "4":
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                    Console.ReadKey();
                    break;

            }
        }



    }


   

    static bool FijarTarjeta(string ip="192.168.1.153", string mascaradered="255.255.255.0")
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
        openFileDialog.Filter = "Archivos de configuración (*.cfg)|*.cfg";
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
        string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

        Console.Clear();
        Console.WriteLine("MENU CANTIDAD DE CLIENTES\n");
        Console.ForegroundColor= ConsoleColor.Green;
        Console.WriteLine("1) Cantidad de usuario en el cliente actual");
        Console.WriteLine("2) Cantidad de un panel distinto");
        Console.WriteLine("3) Volver al menu principal\n");
        Console.ResetColor();
        Console.Write("Ingrese una opcion: ");
        string option = Console.ReadLine();

        switch (option)

        {
            case "1":
                string currentDirectory = Directory.GetCurrentDirectory();
                string envFilePath = Path.Combine(currentDirectory, ".env");
                Env.Load(envFilePath);
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Espere mientras se obtiene información");
                Console.ResetColor();  
                string command = "wstalist";
                string request = ExcecuteCommand(command: command, password: Env.GetString("PASSWORD"));
                JArray jsonArray = JArray.Parse(request);
                JObject firstDevice = (JObject)jsonArray[0];

                string ipDevice = firstDevice["lastip"].ToString();
                string hostname = firstDevice["remote"]["hostname"].ToString();
                
                string passwordpanel = (firstDevice["remote"]["platform"].ToString()).Contains("AC") ? Env.GetString("PASSWORDAC") : Env.GetString("PASSWORD");
                string commandPanel = "wstalist |grep \"mac\" |wc -l";

                string response = ExcecuteCommand(host:ipDevice,command:commandPanel,password:passwordpanel);
                Console.Clear(); 
                Console.WriteLine($"Nombre del Panel: {hostname}");
                Console.WriteLine($"IP del Panel: {ipDevice}");
                Console.WriteLine($"Cantidad de Clientes: {response}");
                
                Console.ReadKey();
                break;


            case "2":
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
                        string command2 = "wstalist |grep \"mac\" |wc -l ; mca-status | grep essid | awk -F '=' '{print $2}'";

                        using (var client = new SshClient(host, port, user, password))
                        {
                            client.Connect();
                            if (client.IsConnected)
                            {

                                var sshCommand = client.CreateCommand(command2);
                                var result = sshCommand.Execute();

                                string[] lines = result.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Clear();
                                if (lines.Length >= 2)
                                {
                                    Console.Write("SSID Panel: ");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.Write($"{lines[1]}\n");
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write("IP: ");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.Write($"{host}\n");
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.Write("Cantidad de Clientes: ");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.Write($"{lines[0]}\n\n");
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
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nError: " + ex.Message);
                        Console.ResetColor();
                        Console.ReadKey();
                    }
                }

                return;
            case "3":
                return;
            default:
                Console.WriteLine("Ingreso una opcion no valida");
                Console.ReadKey();
                return;
        }


            
            
        
        

    }

    static (int, int, int, int) ConvertirSegundos(long segundos)
    {
        int dias = (int)(segundos / (24 * 3600));
        segundos %= (24 * 3600);
        int horas = (int)(segundos / 3600);
        segundos %= 3600;
        int minutos = (int)(segundos / 60);
        segundos %= 60;

        return (dias, horas, minutos, (int)segundos);
    }

    static void InfoCliente()

    {
        Env.Load();
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Tipo de Antena?\n");
        Console.WriteLine("1) AirMax");
        Console.WriteLine("2) AC");
        Console.ResetColor();
        Console.Write("Ingrese una opción: ");
        string op = Console.ReadLine();
        string pass = "";
        Console.ForegroundColor = ConsoleColor.Yellow;
        if (PingHost("192.168.1.20"))
        {
            
            if (op == "1")
            {
                pass = Env.GetString("PASSWORD");
            } else if (op == "2") { 
                pass = Env.GetString("PASSWORDAC");
            }
            Console.Clear();
            Console.WriteLine("Espere mientras se obtiene información del cliente...");
            string first_command = "mca-status | grep 'deviceName' | awk -F '[=,]' '{print $2\",\"$4\",\"$8\",\"$10}'";
            string second_command = "mca-status | grep -E 'essid|wlanPollingQuality|wlanPollingCapacity|lanSpeed|ccq|signal|uptime|distance' | awk -F '[,=]' '{if (NR == $1) {first=$2} else {print first $2 \",\"}}'| xargs";
            string info1 = ExcecuteCommand(host: "192.168.1.20", command: first_command, password: pass);
            string info2 = ExcecuteCommand(host: "192.168.1.20", command: second_command, password: pass);

            if (info1 != null && info2 != null)
            {
                try
                {
                    string[] campos1 = info1.Split(',');
                    string[] campos2 = info2.Split(",");
                    int ccq =  int.Parse(campos2[2].ToString().Trim())/10;
                    int signal = int.Parse( campos2[1].ToString().Trim().Replace("-", ""));
                    
                    int calidad = int.Parse(campos2[5].ToString().Trim());
                    int capacidad = int.Parse(campos2[6].ToString().Trim());
                   

                    (int dias, int horas, int minutos, int segundos) = ConvertirSegundos(long.Parse(campos2[3].Trim()));
                    string time = $"{dias} días : {horas} horas : {minutos} minutos";

                    float distance = int.Parse(campos2[4]) / 1000;
                    string distance_str = $"{distance} km";
                    ConsoleColor colorCCQ, colorSignal, colorCapacity,colorQuality;
                    
                    if (calidad > 60)
                    {
                        
                        colorQuality = ConsoleColor.Green;
                    }else if (calidad < 60 && calidad > 40)
                    {
                       
                        colorQuality = ConsoleColor.Yellow;
                    }
                    else
                    {
                        
                        colorQuality = ConsoleColor.Red;
                    }


                    if (capacidad > 60)
                    {

                        colorCapacity = ConsoleColor.Green;
                    }
                    else if (capacidad < 60 && capacidad > 40)
                    {

                        colorCapacity = ConsoleColor.Yellow;
                    }
                    else
                    {

                        colorCapacity = ConsoleColor.Red;
                    }



                    if (ccq >= 75)
                    {
                        colorCCQ = ConsoleColor.Green;
                    }
                    else if (ccq > 60)
                    {
                        colorCCQ = ConsoleColor.Yellow;
                    }
                    else
                    {
                        colorCCQ = ConsoleColor.Red;
                    }


                    if (signal < 65)
                    {
                        colorSignal = ConsoleColor.Green;
                    } else if(signal > 65 && signal < 72)
                    {
                        colorSignal = ConsoleColor.Yellow;
                    }
                    else
                    {
                        colorSignal = ConsoleColor.Red;
                    }

                    Console.Clear();
                    Console.WriteLine("ESTADÍSTICAS DEL CLIENTE\n");

                    
                    PrintHeader("Campo", "Valor");
                    PrintDivider();

                   
                    PrintRow("Cliente:", campos1[0]);
                    PrintRow("MAC:", campos1[1]);
                    PrintRow("Tipo de Antena:", campos1[2]);
                    PrintRow("IP:", campos1[3].Trim());
                    PrintRow("Panel:", campos2[0]);
                    PrintRowWithColor("Velocidad del cable:", campos2[7].Trim(), campos2[7].Contains("100") ? ConsoleColor.Green : ConsoleColor.Red);
                    PrintRowWithColor("CCQ:", ccq.ToString(), colorCCQ);
                    PrintRowWithColor("Señal:", signal.ToString(), colorSignal);
                    PrintRowWithColor("Capacidad:", capacidad.ToString(), colorCapacity);
                    PrintRowWithColor("Calidad:", calidad.ToString(), colorQuality);
                    PrintRow("Tiempo Activo:",time);
                    PrintRow("Distancia:", distance_str);

                    
                    Console.ResetColor();

                }
                catch { }
               

            }

        }
        else
        {
            Console.ForegroundColor= ConsoleColor.Red;
            Console.WriteLine("No hay conexion con el cliente");
        }
        
    }


    static void PrintRow(string label, string value)
    {
        
        Console.WriteLine($"{label.PadRight(20)} | {value.PadRight(30)}");
    }

    static void PrintRowWithColor(string label, string value, ConsoleColor color)
    {
        
        Console.Write($"{label.PadRight(20)} | ");
        Console.ForegroundColor = color;
        Console.WriteLine($"{value.PadRight(30)}");
        Console.ResetColor();
    }

    static void PrintHeader(string col1, string col2)
    {
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.ForegroundColor = ConsoleColor.Black;
       
        Console.WriteLine($"{col1.PadRight(20)} | {col2.PadRight(30)}");
        Console.ResetColor();
    }
    static void PrintDivider()
    {
        
        Console.WriteLine(new string('-', 20) + "-+-" + new string('-', 30));
    }
    static string ExcecuteCommand(string host="192.168.1.20", string command="",string password="",int port=23,string user="ubnt")
    {
        try
        {
            using (var client = new SshClient(host, port, user, password))
            {
                client.Connect();
                if (client.IsConnected)
                {

                    var sshCommand = client.CreateCommand(command);
                    string result = sshCommand.Execute();
                   
                    return result;

                    

                }
                else
                {
                    Console.WriteLine("No se pudo establecer la conexión.");
                    Console.ReadKey();
                    return "";
                }


                client.Disconnect();

            }
        }
        catch (Exception ex) {
            Console.WriteLine("Entro por la excepcion");
            return "";
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
       


    static string GetIP()
    {
        string interfaceName = "Ether0"; 

        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.Name == interfaceName && ni.OperationalStatus == OperationalStatus.Up)
            {
                

                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork) // Solo IPv4
                    {
                        
                        return ip.Address.ToString();

                    }
                }
            }
        }
        return "";
    }

    static string RemoveLastOctet(string ip)
    {
       
        string[] octets = ip.Split('.');

        if (octets.Length == 4)
        {
            
            return $"{octets[0]}.{octets[1]}.{octets[2]}.";
        }
        else
        {
            throw new ArgumentException("IP no válida. Debe tener 4 octetos.");
        }
    }

    static void Discovery()
    {
        Console.Clear();
        Console.WriteLine("MENU DE DESCUBRIMIENTO DE EQUIPOS\n");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Recuerde que el descubrimiento depende de la ip fijada en la tarjeta!!!");
        

        string ip_last = GetIP();

        ip_last = RemoveLastOctet(ip_last);

        string baseIp = ip_last;

        List<Tuple<string, string>> results = new List<Tuple<string, string>>();

        List<Thread> threads = new List<Thread>();

        Console.ForegroundColor= ConsoleColor.Green;
        for (int i = 1; i < 255; i++)
        {
            string ip = baseIp + i.ToString();
            var thread = new Thread(() => ScanIp(ip, results));
            thread.Start();
            threads.Add(thread);
        }

        
        foreach (var thread in threads)
        {
            thread.Join();
        }

        Console.Clear();
        Console.WriteLine($"{new string('-', 40)}");
        Console.WriteLine($"{"IP",-20} | {"MAC",-20}");
        Console.WriteLine($"{new string('-', 40)}");
        foreach (var result in results)
        {
            Console.WriteLine($"{result.Item1,-20} | {result.Item2,-20}");
        }
        Console.WriteLine($"{new string('-', 40)}");

        Console.ResetColor();
    }



    private static void ScanIp(string ipAddress, List<Tuple<string, string>> results)
    {
        if (PingHost(ipAddress))
        {
            string mac = GetMacAddress(ipAddress);
            if (!string.IsNullOrEmpty(mac))
            {
                lock (results) 
                {
                    results.Add(new Tuple<string, string>(ipAddress, mac));
                }
            }
        }
    }

    private static bool PingHost(string ipAddress)
    {
        using (var ping = new System.Net.NetworkInformation.Ping())
        {
            try
            {
                var reply = ping.Send(ipAddress, 100);
                return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }

    private static string GetMacAddress(string ipAddress)
    {
        IPAddress ipAddr = IPAddress.Parse(ipAddress);
        byte[] macAddr = new byte[6];
        uint macAddrLen = (uint)macAddr.Length;

        int result = SendARP((int)BitConverter.ToInt32(ipAddr.GetAddressBytes(), 0), 0, macAddr, ref macAddrLen);
        if (result == 0)
        {
            return BitConverter.ToString(macAddr).Replace("-", ":");
        }

        return null;
    }




    [STAThread]
    static void Main(string[] args)
    {
        Menu();
        
    }
}
