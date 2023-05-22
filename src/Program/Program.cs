using Proyecto;

Simulacion sim = new Simulacion();
sim.cargarSolicitudes("solicitudes.txt");
Controlador controlador = Controlador.GetInstance();
controlador.run();