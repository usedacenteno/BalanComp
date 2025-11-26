using System.Globalization;
using static accountingProject.Balance.Balance;
using static accountingProject.EstadoResultado.EstadoResultado;

namespace BalanceGeneralApp
{
    

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            MenuPrincipal();
           
        }

        static void MenuPrincipal()
        {
            Console.Clear();
            Console.WriteLine("Bienvenidos al Sistema de Cuentas");
            Console.WriteLine("===================================");
            Console.WriteLine("1. Iniciar Balance General");
            Console.WriteLine("2. Estado de Resultados");
            Console.WriteLine("3. Salir");
            Console.Write("Seleccione una opción: ");

            int.TryParse(Console.ReadLine(), out int opcion);

            switch (opcion)
            {
                case 1:
                    var balance = new BalanceGeneral(new CultureInfo("es-NI"));
                    var cli = new BalanceCLI(balance);
                    cli.Iniciar();
                    MenuPrincipal();
                    break;
                case 2:
                    decimal VentasTotales;
                    decimal Devoluciones;
                    decimal RebajasSobreVentas;
                    decimal MercanciaDisponible = 0;
                    decimal Descuentos;

                    //Entradas de Información

                    Ingresosleer(out VentasTotales, out Devoluciones, out RebajasSobreVentas, out Descuentos);
                    CostosLeer(out decimal Compras, out decimal GastosDeCompra, out decimal DevolucionesSobreCompra, out decimal RebajasSobreCompras, out decimal DescuentoSobreCompra, out decimal InventarioInicial, out decimal InventarioFinal);
                    GastosDeVentaLeer(out decimal SueldoDeLosJefesDeVenta, out decimal SueldoDeLosEmpleadosDeAlmacen, out decimal SueldoDeLosEmpleadosDeLaVenta, out decimal SueldosDeLosChoferesDelEquipoDeEntrega, out decimal ComisionesDeAgentesYDependientes, out decimal SeguroSocialesDelPersonalDeVenta, out decimal Propaganda, out decimal ConsumoDeEtiquetasEnvolturaYEmpaque, out decimal FletesYAcarreosDeLaMercanciasVendidas, out decimal GastosDeMantenimientoDelEquipoDeReparto, out decimal ImpuestosSobreIngresosMercantiles, out decimal RentaDeOficinaVentas, out decimal GastosDeDepreciacionDeVenta);
                    GastosDeAdministracionLeer(out decimal SueldoDeGerente, out decimal ServiciosBasicosVenta, out decimal SueldoDeViceGerente, out decimal SueldoDeContador, out decimal SueldoDelPersonalDeOficina, out decimal SeguroSocialDelPersonal, out decimal ConsumoDePapeleríaYUtilesDeEscritorio, out decimal GastosDeCoreoYTelegrafo, out decimal AlquilerDeOficina, out decimal GastosDeDepreciacion);
                    GastosFinancieros(out decimal InteresesSobreDocumentos, out decimal DescuentoPorPagoAnticipado, out decimal InteresesSobrePrestamosBancarios, out decimal PerdidasOUtilidadesQueResultenDelIntercambioDeMonedaExtrangera);
                    EgresosLeer(out decimal InteresesPagados, out decimal ServiciosBancarios, out decimal RentasCobradas, out decimal PerdidasoUtilidadesEnVentaDeVariosActivosFijo, out decimal PerdidasoUtilidadesEnCompraDeActivos, out decimal RentasPagadas, out decimal DividendoDeAcciones, out decimal ComisionesCobradas);
                    OtrosIngresosLeer(out decimal InteresesCobrados, out decimal DividendosCobrados);
                    Pregunta(out decimal ISR);

                    //Estado de resultado en si
                    TotalVentasNetas(VentasTotales, Devoluciones, RebajasSobreVentas, Descuentos, out decimal TotalVentasN);
                    TotalComprasNetas(Compras, GastosDeCompra, DevolucionesSobreCompra, RebajasSobreCompras, DescuentoSobreCompra, InventarioInicial, InventarioFinal, MercanciaDisponible, out decimal TotalComprasN);
                    CalcUtilidadBruta(TotalVentasN, TotalComprasN, out decimal UtilidadBruta);
                    TotalGastosDeVenta(SueldoDeLosJefesDeVenta, ServiciosBasicosVenta, SueldoDeLosEmpleadosDeAlmacen, SueldoDeLosEmpleadosDeLaVenta, SueldosDeLosChoferesDelEquipoDeEntrega, ComisionesDeAgentesYDependientes, SeguroSocialesDelPersonalDeVenta, Propaganda, ConsumoDeEtiquetasEnvolturaYEmpaque, FletesYAcarreosDeLaMercanciasVendidas, GastosDeMantenimientoDelEquipoDeReparto, ImpuestosSobreIngresosMercantiles, RentaDeOficinaVentas, GastosDeDepreciacionDeVenta, out decimal TotalGastosV);
                    TotalGastosDeAdministración(SueldoDeGerente, SueldoDeViceGerente, SueldoDeContador, SueldoDelPersonalDeOficina, SeguroSocialDelPersonal, ConsumoDePapeleríaYUtilesDeEscritorio, GastosDeCoreoYTelegrafo, AlquilerDeOficina, GastosDeDepreciacion, out decimal TotalGastosAdmin);
                    TotalGastosFinancieros(InteresesSobreDocumentos, DescuentoPorPagoAnticipado, InteresesSobrePrestamosBancarios, PerdidasOUtilidadesQueResultenDelIntercambioDeMonedaExtrangera, out decimal TotalGastosFi);
                    CalcUtilidadDeOperación(TotalGastosV, TotalGastosAdmin, TotalGastosFi, UtilidadBruta, out decimal UtilidadDeOperación);
                    TotalOtrosIngresos(InteresesCobrados, DividendosCobrados, out decimal TotalOI);
                    TotalOtrosegresos(InteresesPagados, ServiciosBancarios, RentasCobradas, PerdidasoUtilidadesEnVentaDeVariosActivosFijo, PerdidasoUtilidadesEnCompraDeActivos, RentasPagadas, DividendoDeAcciones, ComisionesCobradas, out decimal TotalOtrosGastos);
                    CalcUtilidadAntesdeISR(TotalOI, UtilidadDeOperación, TotalOtrosGastos, out decimal UtilidadAntesDeImpuesto);
                    CalcUtilidadNeta(ISR, UtilidadAntesDeImpuesto, out decimal UtilidadNeta);

                    MenuPrincipal();
                    break;
                case 3:

                    Console.WriteLine("Gracias por usar el sistema de cuentas. ¡Hasta luego!");
                    break;
                default:
                    Console.WriteLine("Opción no válida. Por favor, intente de nuevo.");
                    break;
            }

        }
    }

}