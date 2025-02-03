using System;
using System.IO.Pipes;
using System.IO;
using System.Text;

class ClienteMadLibs
{
    static void Main(string[] args)
    {
        string pipeName = "MadLibsPipe";

        // Crear el pipe para conectarse al servidor
        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
        {
            Console.WriteLine("Conectándose al servidor...");
            pipeClient.Connect();
            Console.WriteLine("Conexión establecida con el servidor.");

            // Crear lector y escritor para el pipe
            using (StreamReader reader = new StreamReader(pipeClient, Encoding.UTF8))
            using (StreamWriter writer = new StreamWriter(pipeClient, Encoding.UTF8))
            {
                writer.AutoFlush = true; // Asegurar que los datos se envíen de inmediato

                while (true) // Bucle principal
                {
                    try
                    {
                        // Pedir al usuario el nombre del cuento
                        Console.Write("Escribe el nombre del cuento (ejemplo: cuento2): ");
                        string? nombreCuento = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(nombreCuento))
                        {
                            Console.WriteLine("El nombre del cuento no puede ser vacío. Intenta nuevamente.");
                            continue; // Repite el bucle para pedir un nombre válido
                        }
                        writer.WriteLine(nombreCuento);

                        // Leer mensajes del servidor (solicitudes de palabras o errores)
                        string? mensaje;
                        while ((mensaje = reader.ReadLine()) != null)
                        {
                            if (mensaje.StartsWith("Rellena este hueco"))
                            {
                                Console.WriteLine(mensaje);
                                string? palabra = Console.ReadLine();
                                palabra ??= ""; // Si es null, reemplaza con cadena vacía
                                writer.WriteLine(palabra);
                            }
                            else
                            {
                                Console.WriteLine(mensaje);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
        }
    }
}

