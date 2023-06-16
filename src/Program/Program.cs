using Proyecto;
Simulacion sim = new Simulacion();
sim.procesarSolicitudes("escFiesta.txt");
Controlador controlador = Controlador.GetInstance();
controlador.run();