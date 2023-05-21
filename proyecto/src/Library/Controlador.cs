namespace Proyecto;

public class Controlador
{
    /// <summary>
    /// Define la cantidad de ascensores que tiene el edificio.
    /// </summary>
    private static int CANTIDAD_ASCENSORES = 5;
    /// <summary>
    /// Variable que modifica el estado de operación del controlador.
    /// </summary>
    /// <value>True o False</value>
    private bool stopControlador { set; get; }
    /// <summary>
    /// Lista que almacena los ascensores que controla.
    /// </summary>
    /// <typeparam name="Ascensor"></typeparam>
    /// <returns>Lista de ascensores</returns>
    private static List<Ascensor> listaAscensores = new List<Ascensor>(CANTIDAD_ASCENSORES);

    private static Controlador instance = new Controlador();
    private Controlador()
    {
        if (instance != null)
        {
            throw new Exception("Already instantiated");
        }
        stopControlador = false;
        iniciarAscensores();
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
    /// Lista de ascensores
    /// </summary>
    /// <value></value>
    private List<Ascensor> ascensores { get; set; }
    /// <summary>
    /// Lista de solicitudes
    /// </summary>
    /// <value></value>
    private List<Solicitud> solicitudes { get; set; }

    public Controlador(List<Ascensor> ascensores, List<Solicitud> solicitudes)
    {
        this.ascensores = ascensores;
        this.solicitudes = solicitudes;
    }
    private void iniciarAscensores()
    {
        for (int i = 0; i < CANTIDAD_ASCENSORES; i++)
        {
            Ascensor ascensor = new Ascensor(i);
            // Thread asc = new Thread(new ThreadStart(aversiestocompila));
            // asc.Start();

        }


    }

/* HAY QUE PONER UN SEMAFOROOOOOO
    public static void updateElevatorLists(Elevator elevator)
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
