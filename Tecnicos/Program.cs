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
        ip_device.Add(3, "192.168.100.1");
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
                    MenuTarjetaRed();

                    Console.WriteLine("[+] Se hará prueba de conexión con el dispositivo:\n");
                    Task.Delay(10000);
                    bool conexionExitosa = TestPing("192.168.1.20");

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
                    FijarTarjeta();
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
                            FijarTarjeta(ip, mascara);
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


   

    static bool FijarTarjeta(string ip="192.168.1.20", string mascaradered="255.255.255.0")
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
                string command= "wstalist |grep \"mac\" |wc -l ; mca-status | grep essid | awk -F '=' '{print $2}'";

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
