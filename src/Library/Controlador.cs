namespace Proyecto;

public class Controlador
{

    /// <summary>
    /// Variable que modifica el estado de operación del controlador.
    /// </summary>
    /// <value>True o False</value>
    private bool stopControlador { set; get; }

    /// <summary>
    /// Cantidad de ascensores que controla.
    /// </summary>
    /// <typeparam name="Ascensor"></typeparam>
    /// <returns>Numero de ascensores</returns>
    private int cantAscensores;

    private static Controlador instance = new Controlador();
    private Controlador()
    {
        if (instance != null)
        {
            throw new Exception("Already instantiated");
        }
        stopControlador = false;
        IniciarAscensores();
    }

    /// <summary>
    /// Singleton para el Controlador
    /// </summary>
    /// <returns>Instancia del controlador</returns>
    public static Controlador getInstance()
    {
        return instance;
    }

    /// <summary>
    /// Lista de solicitudes
    /// </summary>
    /// <value></value>
    // private List<Solicitud> solicitudes { get; set; }

    // public Controlador(int cantAscensores, List<Solicitud> solicitudes)
    public Controlador(int cantAscensores)
    {
        this.cantAscensores = cantAscensores;
        // this.solicitudes = solicitudes;
    }
    public void IniciarAscensores()
    {
        for (int i = 0; i < cantAscensores; i++)
        {
            Ascensor ascensor = new Ascensor(i);
            Thread asc = new Thread(new ThreadStart(ascensor.AscensorEjecutando));
            asc.Start();
        }


    }

/* HAY QUE PONER UN SEMAFOROOOOOO
    public static void ActualizarListaAsscensores(Elevator elevator)
    {
        if (elevator.getElevatorState().equals(ElevatorState.UP))
        {
            upMovingMap.put(elevator.getId(), elevator);
            downMovingMap.remove(elevator.getId());
        }
        else if (elevator.getElevatorState().equals(ElevatorState.DOWN))
        {
            downMovingMap.put(elevator.getId(), elevator);
            upMovingMap.remove(elevator.getId());
        }
        else if (elevator.getElevatorState().equals(ElevatorState.STATIONARY))
        {
            upMovingMap.put(elevator.getId(), elevator);
            downMovingMap.put(elevator.getId(), elevator);
        }
        else if (elevator.getElevatorState().equals(ElevatorState.MAINTAINANCE))
        {
            upMovingMap.remove(elevator.getId());
            downMovingMap.remove(elevator.getId());
        }
    }*/
}
