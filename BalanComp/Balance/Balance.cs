using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace accountingProject.Balance
{
    public class Balance
    {
        public class Cuenta
        {
            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public decimal Monto { get; set; }
            public string Clasificacion { get; private set; }
            public string? Tipo { get; set; }

            public Cuenta(string codigo, string nombre, string clasificacion, string? tipo)
            {
                Codigo = codigo?.Trim() ?? string.Empty;
                Nombre = nombre.Trim();
                Clasificacion = clasificacion?.Trim() ?? "Sin Clasificación.";
                Tipo = tipo;
            }

            public override string ToString() => $"{Codigo} {Nombre}: {Monto:C2}";
        }

        public class GrupoCuenta
        {
            public string Nombre { get; private set; }
            public List<Cuenta> Cuentas { get; private set; } = new();

            public GrupoCuenta(string nombre) => Nombre = nombre;

            public void AñadirCuenta(Cuenta c) => Cuentas.Add(c);

            public decimal Subtotal() => Cuentas.Sum(c => c.Monto);
        }

        public class BalanceGeneral
        {
            public List<GrupoCuenta> ActivoCirculante { get; private set; } = new();
            public List<GrupoCuenta> ActivoNoCirculante { get; private set; } = new();
            public List<GrupoCuenta> PasivoCirculante { get; private set; } = new();
            public List<GrupoCuenta> PasivoNoCirculante { get; private set; } = new();
            public GrupoCuenta Capital { get; private set; } = new("Capital Contable");

            public CultureInfo Moneda { get; set; }

            public BalanceGeneral(CultureInfo moneda)
            {
                Moneda = moneda ?? CultureInfo.CurrentCulture;
                InicializarCuentas();
            }

            /// <summary>Inicializa el Balance General con una lista completa de cuentas.</summary>
            private void InicializarCuentas()
            {
                // --- ACTIVOS CIRCULANTES ---

                var efectivo = new GrupoCuenta("Efectivo y Equivalentes");

                efectivo.AñadirCuenta(new Cuenta("1.1.1", "Caja", "AC", "Activo"));
                efectivo.AñadirCuenta(new Cuenta("1.1.2", "Bancos", "AC", "Activo"));
                ActivoCirculante.Add(efectivo);

                var cxp = new GrupoCuenta("Cuentas por Cobrar");
                cxp.AñadirCuenta(new Cuenta("1.1.3", "Clientes", "AC", "Activo"));
                cxp.AñadirCuenta(new Cuenta("1.1.4", "Documentos por Cobrar", "AC", "Activo"));
                cxp.AñadirCuenta(new Cuenta("1.1.5", "IVA Acreditable (Impuestos)", "AC", "Activo"));
                ActivoCirculante.Add(cxp);

                var inventarios = new GrupoCuenta("Inventarios");
                inventarios.AñadirCuenta(new Cuenta("1.1.6", "Inventario de Mercancía", "AC", "Activo"));
                inventarios.AñadirCuenta(new Cuenta("1.1.7", "Material de Oficina", "AC", "Activo"));
                ActivoCirculante.Add(inventarios);

                // --- ACTIVOS NO CIRCULANTES ---
                var propEquipo = new GrupoCuenta("Propiedad, Planta y Equipo");
                propEquipo.AñadirCuenta(new Cuenta("1.2.1", "Terrenos", "ANC", "Activo"));
                propEquipo.AñadirCuenta(new Cuenta("1.2.2", "Edificios", "ANC", "Activo"));
                propEquipo.AñadirCuenta(new Cuenta("1.2.3", "Equipo de Transporte", "ANC", "Activo"));
                propEquipo.AñadirCuenta(new Cuenta("1.2.4", "Depreciación Acumulada (NEGATIVO)", "ANC", "Activo"));
                ActivoNoCirculante.Add(propEquipo);

                var intangibles = new GrupoCuenta("Activos Intangibles");
                intangibles.AñadirCuenta(new Cuenta("1.2.5", "Patentes y Marcas", "ANC", "Activo"));
                intangibles.AñadirCuenta(new Cuenta("1.2.6", "Crédito Mercantil (Plusvalía)", "ANC", "Activo"));
                ActivoNoCirculante.Add(intangibles);

                // --- PASIVOS CIRCULANTES ---
                var cxpagar = new GrupoCuenta("Cuentas por Pagar a Corto Plazo");
                cxpagar.AñadirCuenta(new Cuenta("2.1.1", "Proveedores o cuentas por pagar", "PC", "Pasivo"));
                cxpagar.AñadirCuenta(new Cuenta("2.1.2", "Documentos por Pagar CP", "PC", "Pasivo"));
                cxpagar.AñadirCuenta(new Cuenta("2.1.3", "Impuestos por Pagar (Retenciones)", "PC", "Pasivo"));
                cxpagar.AñadirCuenta(new Cuenta("2.1.4", "Sueldos por Pagar", "PC", "Pasivo"));
                cxpagar.AñadirCuenta(new Cuenta("2.1.5", "", "PC", "Pasivo"));
                PasivoCirculante.Add(cxpagar);

                // --- PASIVOS NO CIRCULANTES ---
                var lxpagar = new GrupoCuenta("Obligaciones a Largo Plazo");
                lxpagar.AñadirCuenta(new Cuenta("2.2.1", "Hipotecas por Pagar LP", "PNC", "Pasivo"));
                lxpagar.AñadirCuenta(new Cuenta("2.2.2", "Documentos por Pagar LP", "PNC", "Pasivo"));
                lxpagar.AñadirCuenta(new Cuenta("2.2.3", "Reservas Técnicas (Provisiones)", "PNC", "Pasivo"));
                PasivoNoCirculante.Add(lxpagar);

                // --- CAPITAL CONTABLE ---
                Capital.AñadirCuenta(new Cuenta("3.1", "Capital Social", "CAP", "Capital"));
                Capital.AñadirCuenta(new Cuenta("3.2", "Aportaciones para Futuros Aumentos", "CAP", "Capital"));
                Capital.AñadirCuenta(new Cuenta("3.3", "Utilidad del Ejercicio", "CAP", "Capital"));
                Capital.AñadirCuenta(new Cuenta("3.4", "Reservas de Capital", "CAP", "Capital"));
            }

            public decimal TotalActivo() =>
                ActivoCirculante.Sum(g => g.Subtotal()) + ActivoNoCirculante.Sum(g => g.Subtotal());

            public decimal TotalPasivo() =>
                PasivoCirculante.Sum(g => g.Subtotal()) + PasivoNoCirculante.Sum(g => g.Subtotal());

            public decimal TotalCapital() => Capital.Subtotal();


            public decimal ObtenerDiferenciaCuadre()
            {
                var activo = TotalActivo();
                var pasycap = TotalPasivo() + TotalCapital();
                // Redondeo para evitar problemas de precisión decimal
                return Math.Round(activo - pasycap, 2);
            }

            public List<Cuenta> ObtenerTodasCuentas()
            {
                var lista = new List<Cuenta>();
                lista.AddRange(ActivoCirculante.SelectMany(g => g.Cuentas));
                lista.AddRange(ActivoNoCirculante.SelectMany(g => g.Cuentas));
                lista.AddRange(PasivoCirculante.SelectMany(g => g.Cuentas));
                lista.AddRange(PasivoNoCirculante.SelectMany(g => g.Cuentas));
                lista.AddRange(Capital.Cuentas);
                return lista;
            }

            public string Formatear(decimal monto) => monto.ToString("C2", Moneda);
        }


        public class BalanceCLI
        {
            private readonly BalanceGeneral balance;
            // Regex para asegurar que solo haya números o el símbolo decimal (y opcionalmente signo -)
            private static readonly Regex NumberRegex = new Regex(@"^[-\s]*\d*(\.\d*)?[\s]*$", RegexOptions.Compiled);

            public BalanceCLI(BalanceGeneral balance)
            {
                this.balance = balance;
            }

            public void Iniciar()
            {
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n==== MENÚ PRINCIPAL ====");
                    Console.WriteLine("1. Ingresar montos por cuenta");
                    Console.WriteLine("2. Mostrar Balance y Resumen Final");
                    Console.WriteLine("3. Salir");
                    Console.Write("Seleccione una opción: ");
                    string opcion = Console.ReadLine()?.Trim() ?? "";

                    switch (opcion)
                    {
                        case "1": IngresarMontos(); break;
                        case "2": MostrarBalanceCompleto(); break;
                        case "3": return;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Opción no válida. Intente de nuevo.");
                            break;
                    }
                }
            }

            private void IngresarMontos()
            {
                Console.WriteLine("\nINGRESO DE MONTOS (Deje vacío para omitir):");


                string tipo1 = null;
                string tipo2 = null;
                string tipo3 = null;
                foreach (var c in balance.ObtenerTodasCuentas())
                {
                    // La Depreciación Acumulada es la única cuenta de Activo que permite saldo negativo
                    bool permiteNegativo = c.Codigo == "1.2.4";

                    while (true)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                      
                        if ((c.Tipo != "Activo" && c.Tipo != "Capital"))
                        {
                            if (tipo2 == null)
                            {
                                tipo2 = "Pasivos";
                                Console.WriteLine("\nPasivos");
                            }

                        }
                        else if ((c.Tipo != "Activo" && c.Tipo != "Pasivo"))
                        {
                            if (tipo3 == null)
                            {
                                tipo3 = "Capital";
                                Console.WriteLine("Capital");
                            }
                        }
                        else
                        {
                            if (tipo1 == null)
                            {
                                tipo1 = "Activos";
                                Console.WriteLine("Activos");
                            }
                        }
                        Console.Write($"{c.Codigo} {c.Nombre} [{balance.Formatear(c.Monto)}]: ");
                        string entrada = Console.ReadLine()?.Trim() ?? "";

                        if (string.IsNullOrEmpty(entrada))
                            break; // Sale del loop interno para pasar a la siguiente cuenta

                        // 1. Verificar si la entrada contiene solo números, puntos/comas decimales y el signo de menos
                        if (!NumberRegex.IsMatch(entrada))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(" Valor inválido. Solo se permiten números o dejar vacío.");
                            continue; // Vuelve a pedir el valor para la misma cuenta
                        }

                        // 2. Intentar parsear el valor
                        // Usa InvariantCulture para forzar el punto como separador decimal si es necesario
                        if (decimal.TryParse(entrada.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
                        {
                            // 3. Validar si el valor no debe ser negativo
                            if (!permiteNegativo && valor < 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(" Error: Esta cuenta no puede tener un valor negativo.");
                                continue;
                            }

                            if (permiteNegativo && valor > 0)
                            {

                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(" Error: Esta cuenta no puede tener un valor Positivo.");
                                continue;
                            }

                            c.Monto = valor;

                            break;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("  Valor inválido. Por favor, ingrese un número decimal válido.");
                        }
                    }
                }
            }

            private void ImprimirGrupo(string titulo, List<GrupoCuenta> grupos, Func<decimal> totalFn)
            {
                Console.WriteLine($"\n--- {titulo} ---");
                foreach (var g in grupos)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"   {g.Nombre}");
                    Console.ForegroundColor = ConsoleColor.White;
                    foreach (var c in g.Cuentas)
                    {
                        Console.WriteLine($"     {c.Codigo} {c.Nombre,-30} {balance.Formatear(c.Monto),20}");
                    }
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"   SUBTOTAL {g.Nombre,-30} {balance.Formatear(g.Subtotal()),20}");
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n**TOTAL {titulo}** {balance.Formatear(totalFn()),47}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            private void MostrarBalanceCompleto()
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n========== BALANCE GENERAL ==========");
                Console.ForegroundColor = ConsoleColor.White;

                // --- ACTIVO ---
                ImprimirGrupo("ACTIVO CIRCULANTE", balance.ActivoCirculante, () => balance.ActivoCirculante.Sum(g => g.Subtotal()));
                ImprimirGrupo("ACTIVO NO CIRCULANTE", balance.ActivoNoCirculante, () => balance.ActivoNoCirculante.Sum(g => g.Subtotal()));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nTOTAL ACTIVO: {balance.Formatear(balance.TotalActivo()),60}");
                Console.WriteLine("---------------------------------------------------------------------------------");

                // --- PASIVO ---
                ImprimirGrupo("PASIVO CIRCULANTE", balance.PasivoCirculante, () => balance.PasivoCirculante.Sum(g => g.Subtotal()));
                ImprimirGrupo("PASIVO NO CIRCULANTE", balance.PasivoNoCirculante, () => balance.PasivoNoCirculante.Sum(g => g.Subtotal()));
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\nTOTAL PASIVO: {balance.Formatear(balance.TotalPasivo()),60}");
                Console.WriteLine("---------------------------------------------------------------------------------");

                // --- CAPITAL ---
                ImprimirGrupo("CAPITAL CONTABLE", new List<GrupoCuenta> { balance.Capital }, () => balance.Capital.Subtotal());
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"\nTOTAL CAPITAL: {balance.Formatear(balance.TotalCapital()),59}");
                Console.WriteLine("---------------------------------------------------------------------------------");

                // --- RESUMEN FINAL DE CUADRE (OBLIGATORIO) ---
                MostrarResumenCuadre();
            }

            private void MostrarResumenCuadre()
            {
                var activo = balance.TotalActivo();
                var pasivo = balance.TotalPasivo();
                var capital = balance.TotalCapital();
                var pasycap = pasivo + capital;
                var diff = balance.ObtenerDiferenciaCuadre();
                var cuadra = Math.Abs(diff) < 0.01m;

                Console.WriteLine("\n========== RESUMEN FINAL ==========");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"TOTAL ACTIVO (A): {balance.Formatear(activo),59}");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"TOTAL PASIVO (P): {balance.Formatear(pasivo),59}");
                Console.WriteLine($"TOTAL CAPITAL (C): {balance.Formatear(capital),58}");
                Console.WriteLine($"TOTAL P + C: {balance.Formatear(pasycap),62}");

                Console.WriteLine("---------------------------------------------------------------------------------");

                if (cuadra)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"¡EL BALANCE CUADRA! (A = P + C): {balance.Formatear(diff),47}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" EL BALANCE NO CUADRA. DIFERENCIA (A - (P + C)): {balance.Formatear(diff),35}");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }

            private void CambiarMoneda()
            {

                Console.WriteLine($"Moneda cambiada a: {balance.Moneda.DisplayName} ({balance.Moneda.NumberFormat.CurrencySymbol})");
            }
        }
    }
}
