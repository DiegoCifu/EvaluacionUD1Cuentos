using System;
using System.IO.Pipes;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class ServidorMadLibs
{
    static void Main(string[] args)
    {
        string pipeName = "MadLibsPipe";

        // Crear el pipe del servidor
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut))
        {
            Console.WriteLine("Esperando conexión con el cliente...");
            pipeServer.WaitForConnection();
            Console.WriteLine("Cliente conectado.");

            // Crear lector y escritor para el pipe
            using (StreamReader reader = new StreamReader(pipeServer, Encoding.UTF8))
            using (StreamWriter writer = new StreamWriter(pipeServer, Encoding.UTF8))
            {
                writer.AutoFlush = true; // Asegurar que los datos se envíen de inmediato

                while (true) // Bucle principal
                {
                    try
                    {
                        // Leer el nombre del cuento desde el cliente
                        string? nombreCuento = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(nombreCuento))
                        {
                            Console.WriteLine("El cliente envió un nombre de cuento vacío o nulo.");
                            writer.WriteLine("ERROR: El nombre del cuento no es válido.");
                            continue; // Repite el bucle principal
                        }

                        // Leer el archivo del cuento
                        string filePath = $"{nombreCuento}.txt";
                        if (!File.Exists(filePath))
                        {
                            writer.WriteLine("ERROR: El archivo del cuento no existe.");
                            continue;
                        }

                        string cuento = File.ReadAllText(filePath);
                        Console.WriteLine("Archivo del cuento leído correctamente.");

                        // Detectar las etiquetas en el cuento
                        Regex regex = new Regex("<(.*?)>");
                        MatchCollection matches = regex.Matches(cuento);

                        foreach (Match match in matches)
                        {
                            string etiqueta = match.Value; // Ejemplo: <sustantivo-masculino>
                            writer.WriteLine($"Rellena este hueco ({etiqueta}):");
                            string? palabra = reader.ReadLine();
                            palabra ??= ""; // Si es null, reemplaza con cadena vacía
                            cuento = cuento.Replace(etiqueta, palabra);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        writer.WriteLine("ERROR: Ocurrió un problema en el servidor.");
                    }
                }
            }
        }
    }
}



