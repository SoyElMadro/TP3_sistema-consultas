<?php
session_start();

if (!isset($_SESSION["usuario"]) || !isset($_SESSION["documento"])) {
    header("Location: ingreso.html");
    exit;
}

$host = "localhost";
$db = "mi_banco_db";
$user = "root";
$pass = "Novi3mbr3s11.";

$conn = new mysqli($host, $user, $pass, $db);

if ($conn->connect_error) {
    die("Error de conexión: " . $conn->connect_error);
}

$documento = $_SESSION["documento"];
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Panel del Cliente</title>
    <script src="https://cdn.tailwindcss.com"></script>
</head>

<body class="bg-gray-100 font-sans">

<header class="bg-[#004691] text-white p-4 text-center">
    <h1 class="text-xl font-bold">Panel del Cliente</h1>
</header>

<main class="max-w-5xl mx-auto p-6 space-y-8">

<?php

$sqlUltima = "
    SELECT l.*
    FROM liquidaciones l
    INNER JOIN tarjetas t ON l.num_cuenta = t.num_cuenta
    WHERE t.dni_titular = '$documento'
    ORDER BY l.id_liquidacion DESC
    LIMIT 1
";

$resultUltima = $conn->query($sqlUltima);

if ($resultUltima && $resultUltima->num_rows > 0) {
    $ultima = $resultUltima->fetch_assoc();

    echo "
    <div class='bg-white p-6 rounded shadow border-l-4 border-blue-600'>
        <h2 class='text-xl font-bold text-[#004691] mb-4'>Última Liquidación</h2>

        <p><strong>Cuenta:</strong> {$ultima['num_cuenta']}</p>
        <p><strong>Período:</strong> {$ultima['periodo']}</p>
        <p><strong>Vencimiento:</strong> {$ultima['fecha_vencimiento']}</p>
        <p><strong>Monto a pagar:</strong> $ {$ultima['total_a_pagar']}</p>
        <p><strong>Monto mínimo:</strong> $ {$ultima['pago_minimo']}</p>
    </div>
    ";
} else {
    echo "<p>No hay liquidaciones disponibles.</p>";
}

$sqlHistorial = "
    SELECT l.*
    FROM liquidaciones l
    INNER JOIN tarjetas t ON l.num_cuenta = t.num_cuenta
    WHERE t.dni_titular = '$documento'
    ORDER BY l.id_liquidacion DESC
";

$resultHistorial = $conn->query($sqlHistorial);

echo "
<div class='bg-white p-6 rounded shadow'>
    <h2 class='text-xl font-bold text-[#004691] mb-4'>Historial de Liquidaciones</h2>
";

if ($resultHistorial && $resultHistorial->num_rows > 0) {

    echo "<table class='w-full text-sm border'>
            <thead>
                <tr class='bg-gray-300'>
                    <th class='p-2'>Cuenta</th>
                    <th class='p-2'>Período</th>
                    <th class='p-2'>Vencimiento</th>
                    <th class='p-2'>Monto a pagar</th>
                    <th class='p-2'>Monto mínimo</th>
                </tr>
            </thead>
            <tbody>";

    while ($row = $resultHistorial->fetch_assoc()) {
        echo "
        <tr class='border-t text-center'>
            <td class='p-2'>{$row['num_cuenta']}</td>
            <td class='p-2'>{$row['periodo']}</td>
            <td class='p-2'>{$row['fecha_vencimiento']}</td>
            <td class='p-2'>$ {$row['total_a_pagar']}</td>
            <td class='p-2'>$ {$row['pago_minimo']}</td>
        </tr>
        ";
    }

    echo "</tbody></table>";

} else {
    echo "<p>No hay historial de liquidaciones.</p>";
}

echo "</div>";

$conn->close();
?>

</main>

<footer class="text-center text-xs text-gray-500 p-4">
    Sistema Mis Tarjetas - Panel de Cliente
</footer>

</body>
</html>