namespace Proyecto;
public class Simulacion
{
    Controlador controlador = Controlador.GetInstance();
    public void procesarSolicitudes(String archivo)
    {
        int PISO_ACUAL = 0;
        int PISO_DESTINO = 1;
        int PESO = 2;
        int PRIORIDAD = 3;
        int ID = 4;
        int ESPERA = 5;

        String line;
        try
        {
            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr = new StreamReader(archivo);
            //Read the first line of text

            //Continue to read until you reach end of file
            while ((line = sr.ReadLine()) != null)
            {
                String[] sol = line.Split(",");
                //Solicitud = int pisoActual, int pisoDestino, int peso, int priodidad
                if (sol[0].Contains("esperar"))
                {
                    Int32.TryParse(sol[1],out int res);
                    controlador.agregarEspera(res);
                }
                else
                {
                    Solicitud solicitud = new Solicitud(Int32.Parse(sol[PISO_ACUAL]), Int32.Parse(sol[PISO_DESTINO]), Int32.Parse(sol[PESO]), Int32.Parse(sol[PRIORIDAD]), sol[ID], Int32.Parse(sol[ESPERA]));
                    Thread solThread = new Thread(new ThreadStart(solicitud.Run));
                    solThread.Start();
                }

            }
            //close the file
            sr.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }
}