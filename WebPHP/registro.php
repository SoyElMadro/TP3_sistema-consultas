<?php
$host = "localhost";
$db   = "mi_banco_db";
$user = "root";
$pass = "Novi3mbr3s11.";

$conn = new mysqli($host, $user, $pass, $db);

if ($conn->connect_error) {
    die("Error de conexión: " . $conn->connect_error);
}

if (isset($_POST["registrar"])) {

    $documento = $_POST["documento"];
    $email     = $_POST["email"];
    $usuario   = $_POST["usuario"];
    $passwordA = $_POST["passwordA"];
    $passwordB = $_POST["passwordB"];

    if ($passwordA != $passwordB) {
        echo "<script>
                alert('Las contraseñas no coinciden');
                window.location.href = 'registro.html';
              </script>";
        exit;
    }

    $checkCliente = $conn->prepare(
        "SELECT documento, usuario FROM usuarios
         WHERE documento = ? AND email = ?
         LIMIT 1"
    );
    $checkCliente->bind_param("ss", $documento, $email);
    $checkCliente->execute();
    $resultCliente = $checkCliente->get_result();

    if ($resultCliente->num_rows === 0) {
        echo "<script>
                alert('No existe un cliente con ese documento y email. El cliente debe ser dado de alta por el banco antes de registrarse.');
                window.location.href = 'registro.html';
              </script>";
        exit;
    }

    $cliente = $resultCliente->fetch_assoc();

    if (!empty($cliente["usuario"])) {
        echo "<script>
                alert('Este cliente ya posee un usuario registrado.');
                window.location.href = 'ingreso.html';
              </script>";
        exit;
    }

    $checkUsuario = $conn->prepare(
        "SELECT documento FROM usuarios WHERE usuario = ? LIMIT 1"
    );
    $checkUsuario->bind_param("s", $usuario);
    $checkUsuario->execute();
    if ($checkUsuario->get_result()->num_rows > 0) {
        echo "<script>
                alert('El nombre de usuario ya está en uso. Elegí otro.');
                window.location.href = 'registro.html';
              </script>";
        exit;
    }

    $update = $conn->prepare(
        "UPDATE usuarios
         SET usuario = ?, password = ?
         WHERE documento = ?"
    );
    $update->bind_param("sss", $usuario, $passwordA, $documento);

    if ($update->execute()) {
        echo "<script>
                alert('Usuario creado correctamente. Ya podés iniciar sesión.');
                window.location.href = 'ingreso.html';
              </script>";
    } else {
        echo "Error: " . $conn->error;
    }
}

$conn->close();
?>
