using System;
using MySql.Data.MySqlClient;

namespace Progra3Card.Administrativo
{
    class Program
    {
        private static string connectionString = "Server=localhost;Port=3306;Database=mi_banco_db;Uid=root;Pwd=papita123;";

        static void Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    SISTEMA ADMINISTRATIVO PROGRA3CARD   ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Emitir Nueva Tarjeta (Alta de Cliente)");
                Console.WriteLine("2. Listar Tarjetas");
                Console.WriteLine("3. Ver Detalle de una Tarjeta / Cliente");
                Console.WriteLine("4. Eliminar Tarjeta (Baja de Sistema)");
                Console.WriteLine("5. Emitir Nueva Liquidación Mensual");
                Console.WriteLine("6. Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1": MenuEmitirTarjeta(); break;
                    case "2": MenuListarTarjetas(); break;
                    case "3": MenuVerDetalleTarjeta(); break;
                    case "4": MenuEliminarTarjeta(); break;
                    case "5": MenuEmitirLiquidacion(); break;
                    case "6": salir = true; break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void MenuListarTarjetas()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE TARJETAS ---");
            Console.WriteLine("{0,-12} | {1,-18} | {2,-20} | {3,-15}", "Nro Cuenta", "Nro Tarjeta", "Banco Emisor", "DNI Titular");
            Console.WriteLine("----------------------------------------------------------------------");

            ObtenerYMostrarTarjetas();

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuVerDetalleTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- DETALLE DE TARJETA Y CLIENTE ---");
            Console.Write("Ingrese el Número de Cuenta a consultar: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            MostrarDetalleCompleto(numCuenta);

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEliminarTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR TARJETA DEL SISTEMA ---");
            Console.Write("Ingrese el Número de Cuenta de la tarjeta a dar de baja: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n⚠️ ADVERTENCIA: Se eliminará la tarjeta, sus liquidaciones y los datos de acceso web vinculados.");
            Console.ResetColor();
            Console.Write("¿Está seguro de continuar? (S/N): ");

            if (Console.ReadLine().ToUpper() == "S")
            {
                bool exito = DarDeBajaTarjeta(numCuenta);

                if (exito)
                    Console.WriteLine("\nTarjeta eliminada correctamente del sistema.");
                else
                    Console.WriteLine("\nError al intentar eliminar la tarjeta. Verifique el número de cuenta.");
            }
            else
            {
                Console.WriteLine("\nOperación cancelada.");
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEmitirTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- EMITIR NUEVA TARJETA ---");

            Console.Write("DNI Titular: ");
            string dni = Console.ReadLine();

            string tipoDoc = "";
            while (true)
            {
                Console.WriteLine("\nTipo de documento:");
                Console.WriteLine("1 - DNI");
                Console.WriteLine("2 - PASAPORTE");
                Console.Write("Opción: ");
                string opcionTipoDoc = Console.ReadLine();

                if (opcionTipoDoc == "1") { tipoDoc = "DNI"; break; }
                if (opcionTipoDoc == "2") { tipoDoc = "PASAPORTE"; break; }
                Console.WriteLine("❌ Opción inválida. Intente nuevamente.");
            }

            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();

            Console.Write("Apellido: ");
            string apellido = Console.ReadLine();

            DateTime fechaNacimientoDate;
            string fechaNacimiento;
            while (true)
            {
                Console.Write("Fecha de nacimiento (YYYY-MM-DD): ");
                string input = Console.ReadLine();
                if (DateTime.TryParse(input, out fechaNacimientoDate))
                {
                    fechaNacimiento = fechaNacimientoDate.ToString("yyyy-MM-dd");
                    break;
                }
                Console.WriteLine("❌ Fecha inválida. Use el formato YYYY-MM-DD (ej: 1990-05-23).");
            }

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Saldo inicial: ");
            string saldo = Console.ReadLine();

            Console.Write("Número de tarjeta: ");
            string numeroTarjeta = Console.ReadLine();

            string banco = "";

            while (true)
            {
                Console.WriteLine("\nBanco emisor:");
                Console.WriteLine("1 - Banco Nación");
                Console.WriteLine("2 - Banco Santander");
                Console.WriteLine("3 - Banco Galicia");
                Console.WriteLine("0 - Cancelar operación");
                Console.Write("Opción: ");

                string opcionBanco = Console.ReadLine();

                switch (opcionBanco)
                {
                    case "1":
                        banco = "Banco Nación";
                        break;

                    case "2":
                        banco = "Banco Santander";
                        break;

                    case "3":
                        banco = "Banco Galicia";
                        break;

                    case "0":
                        Console.WriteLine("Operación cancelada.");
                        Console.ReadKey();
                        return;

                    default:
                        Console.WriteLine("❌ Opción inválida. Intente nuevamente.");
                        continue;
                }

                break;
            }

            using (MySqlConnection conexion = new MySqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();

                    string insertUsuario =
                        "INSERT INTO usuarios (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email) " +
                        "VALUES (@dni, @tipoDoc, @nombre, @apellido, @fechaNacimiento, @email)";

                    using (MySqlCommand cmd1 = new MySqlCommand(insertUsuario, conexion))
                    {
                        cmd1.Parameters.AddWithValue("@dni", dni);
                        cmd1.Parameters.AddWithValue("@tipoDoc", tipoDoc);
                        cmd1.Parameters.AddWithValue("@nombre", nombre);
                        cmd1.Parameters.AddWithValue("@apellido", apellido);
                        cmd1.Parameters.AddWithValue("@fechaNacimiento", fechaNacimiento);
                        cmd1.Parameters.AddWithValue("@email", email);

                        cmd1.ExecuteNonQuery(); // --> funcion para ejecutrar sin datos (como es un INSERT no me devuelve nada )
                    }

                    string insertTarjeta =
                        "INSERT INTO tarjetas (numero_tarjeta, banco_emisor, saldo, dni_titular) " +
                        "VALUES (@numeroTarjeta, @banco, @saldo, @dni)";

                    using (MySqlCommand cmd2 = new MySqlCommand(insertTarjeta, conexion))
                    {
                        cmd2.Parameters.AddWithValue("@numeroTarjeta", numeroTarjeta);
                        cmd2.Parameters.AddWithValue("@banco", banco);
                        cmd2.Parameters.AddWithValue("@saldo", saldo);
                        cmd2.Parameters.AddWithValue("@dni", dni);

                        cmd2.ExecuteNonQuery();
                    }

                    Console.WriteLine("\nTarjeta emitida correctamente.");
                    Console.WriteLine($"Número de tarjeta: {numeroTarjeta}");
                    Console.WriteLine($"Titular: {nombre} {apellido}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error:");
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }

            Console.WriteLine("\nPresione una tecla para volver...");
            Console.ReadKey();
        }

        static void MenuEmitirLiquidacion()
        {
            Console.Clear();
            Console.WriteLine("--- EMITIR LIQUIDACIÓN MENSUAL ---");

            Console.Write("Número de cuenta: ");
            string cuenta = Console.ReadLine();

            Console.Write("Período (ej: 2026-06): ");
            string periodo = Console.ReadLine();

            Console.Write("Fecha de vencimiento (YYYY-MM-DD): ");
            string vencimiento = Console.ReadLine();

            Console.Write("Monto total a pagar: ");
            string montoTotal = Console.ReadLine();

            Console.Write("Pago mínimo: ");
            string pagoMinimo = Console.ReadLine();

            using (MySqlConnection conexion = new MySqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();

                    string insert =
                        "INSERT INTO liquidaciones (num_cuenta, periodo, fecha_vencimiento, total_a_pagar, pago_minimo) " +
                        "VALUES (@cuenta, @periodo, @vencimiento, @montoTotal, @pagoMinimo)";

                    using (MySqlCommand cmd = new MySqlCommand(insert, conexion))
                    {
                        cmd.Parameters.AddWithValue("@cuenta", cuenta);
                        cmd.Parameters.AddWithValue("@periodo", periodo);
                        cmd.Parameters.AddWithValue("@vencimiento", vencimiento);
                        cmd.Parameters.AddWithValue("@montoTotal", montoTotal);
                        cmd.Parameters.AddWithValue("@pagoMinimo", pagoMinimo);

                        int filas = cmd.ExecuteNonQuery();

                        if (filas > 0)
                                Console.WriteLine("\nLiquidación generada correctamente.");
                            else
                                Console.WriteLine("\nNo se pudo generar la liquidación.");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error:");
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }

            Console.WriteLine("\nPresione una tecla para volver...");
            Console.ReadKey();
        }

        static void ObtenerYMostrarTarjetas()
        {
            using (MySqlConnection conexion = new MySqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();

                    string consulta = "SELECT * FROM tarjetas";

                    using (MySqlCommand comando = new MySqlCommand(consulta, conexion))
                    {
                        using (MySqlDataReader lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                string num_cuenta = lector["num_cuenta"].ToString() ?? "";
                                string num_tarj = lector["numero_tarjeta"].ToString() ?? "";
                                string banco_emisor = lector["banco_emisor"].ToString() ?? "";
                                string dni_titular = lector["dni_titular"].ToString() ?? "";
                                Console.WriteLine(string.Format("{0,-12} | {1,-18} | {2,-20} | {3,-15}",
                                    num_cuenta, num_tarj, banco_emisor, dni_titular));
                            }
                            Console.WriteLine("==============================================================\n");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ocurrió un error al intentar operar con la base de datos:");
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
        }

        static void MostrarDetalleCompleto(int cuenta)
        {
            using (MySqlConnection conexion = new MySqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();

                    string consulta =
                        "SELECT t.num_cuenta, t.numero_tarjeta, t.banco_emisor, " +
                        "t.dni_titular, u.nombre, u.apellido, u.email, t.saldo " +
                        "FROM tarjetas t " +
                        "INNER JOIN usuarios u ON t.dni_titular = u.documento " +
                        "WHERE t.num_cuenta = @cuenta";

                    using (MySqlCommand comando = new MySqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@cuenta", cuenta);

                        using (MySqlDataReader lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                Console.WriteLine("\n--- INFORMACIÓN COMPLETA ---");
                                Console.WriteLine($"Cuenta: {lector["num_cuenta"]}");
                                Console.WriteLine($"Tarjeta: {lector["numero_tarjeta"]}");
                                Console.WriteLine($"Banco: {lector["banco_emisor"]}");
                                Console.WriteLine($"DNI Titular: {lector["dni_titular"]}");
                                Console.WriteLine($"Nombre: {lector["nombre"]}");
                                Console.WriteLine($"Apellido: {lector["apellido"]}");
                                Console.WriteLine($"Email: {lector["email"]}");
                                Console.WriteLine($"Saldo: {lector["saldo"]}");
                            }
                            else
                            {
                                Console.WriteLine("\nNo se encontró una tarjeta con ese número de cuenta.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error al obtener el detalle:");
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
        }

        static bool DarDeBajaTarjeta(int cuenta)
        {
            using (MySqlConnection conexion = new MySqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();

                    string selectDni =
                       "SELECT dni_titular FROM tarjetas WHERE num_cuenta = @cuenta";

                    object resultDni = null;
                    using (MySqlCommand cmdSelect = new MySqlCommand(selectDni, conexion))
                    {
                        cmdSelect.Parameters.AddWithValue("@cuenta", cuenta);

                        resultDni = cmdSelect.ExecuteScalar(); // --> funcion para leer una sola celda
                    }

                    if (resultDni == null)
                    {
                        return false;
                    }

                    string dniTitular = resultDni.ToString();

                    string deleteTarjeta =
                        "DELETE FROM tarjetas WHERE num_cuenta = @cuenta";

                    int filasAfectadas = 0;
                    using (MySqlCommand cmdDelete = new MySqlCommand(deleteTarjeta, conexion))
                    {
                        cmdDelete.Parameters.AddWithValue("@cuenta", cuenta);

                        filasAfectadas = cmdDelete.ExecuteNonQuery();
                    }

                    if (filasAfectadas > 0)
                    {
                        string updateUsuario =
                            "UPDATE usuarios SET usuario = NULL, password = NULL WHERE documento = @dni";

                        using (MySqlCommand cmdUpdate = new MySqlCommand(updateUsuario, conexion))
                        {
                            cmdUpdate.Parameters.AddWithValue("@dni", dniTitular);

                            cmdUpdate.ExecuteNonQuery();
                        }
                    }

                    return filasAfectadas > 0;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error al eliminar la tarjeta:");
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();

                    return false;
                }
            }
        }
    }
}