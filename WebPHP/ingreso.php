<?php
session_start();

$host = "localhost";
$db = "mi_banco_db";
$user = "root";
$pass = "Novi3mbr3s11.";

$conn = new mysqli($host, $user, $pass, $db);

if ($conn->connect_error) {
    die("Error de conexión: " . $conn->connect_error);
}

if (isset($_POST["ingresar"])) {

    $tipo_doc = $_POST["tipo_doc"];
    $documento = $_POST["documento"];
    $usuario = $_POST["usuario"];
    $password = $_POST["password"];

    $stmt = $conn->prepare(
        "SELECT *
         FROM usuarios
         WHERE documento = ?
         AND tipo_doc = ?
         AND usuario = ?
         AND password = ?
         LIMIT 1"
    );
    $stmt->bind_param("ssss", $documento, $tipo_doc, $usuario, $password);
    $stmt->execute();
    $result = $stmt->get_result();

    if ($result && $result->num_rows == 1) {

        $userData = $result->fetch_assoc();

        $_SESSION["usuario"] = $userData["usuario"];
        $_SESSION["documento"] = $userData["documento"];

        echo "
        <script>
            alert('Login exitoso');
            window.location.href = 'resumen.php';
        </script>
        ";

    } else {
        echo "
        <script>
            alert('Credenciales incorrectas');
            window.location.href = 'ingreso.html';
        </script>
        ";
    }
}

$conn->close();
?>