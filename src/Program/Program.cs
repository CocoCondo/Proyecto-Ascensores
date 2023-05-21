using Proyecto;
Simulacion sim = new Simulacion();
sim.procesarSolicitudes("solicitudes.txt");
Controlador controlador = Controlador.GetInstance();
controlador.run();