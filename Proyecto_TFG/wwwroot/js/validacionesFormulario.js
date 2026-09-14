function validarDatosEntrega() {
    let direccionEntrega = document.getElementById("direccionEntrega");
    let codigoPostal = document.getElementById("codigoPostal");
    let provincia = document.getElementById("selectProvincias");
    let municipio = document.getElementById("municipio");
    let telefono = document.getElementById("telefono");
    let metodoPago = document.getElementById("metodoPago");
    let totalPedido = document.getElementById("totalPedido");

    // Limpiar los mensajes de error anteriores
    document.querySelectorAll(".error").forEach((errorDiv) => (errorDiv.innerHTML = ""));

    let isValid = true;

    // Dirección de Entrega (letras, números y símbolos)
    let regexDireccion = /^[a-zA-Z0-9áéíóúÁÉÍÓÚüÜ\s,.'-]{1,100}$/;
    if (!direccionEntrega.value || !regexDireccion.test(direccionEntrega.value)) {
        document.getElementById("errorDireccionEntrega").innerText = "Introduce una dirección válida.";
        isValid = false;
    }

    // Total Pedido (no puede ser menor o igual a 0)
    if (totalPedido.value <= 0) {
        document.getElementById("errorTotalPedido").innerText = "No ha seleccionado ningún producto.";
        isValid = false;
    }

    // Código Postal (debe ser un número de 5 dígitos)
    let regexCodigoPostal = /^\d{5}$/;
    if (!codigoPostal.value || !regexCodigoPostal.test(codigoPostal.value)) {
        document.getElementById("errorCodigoPostal").innerText = "Introduce un código postal válido.";
        isValid = false;
    }

    // Provincia (selección obligatoria)
    if (!provincia.value) {
        document.getElementById("errorProvincia").innerText = "Selecciona una provincia.";
        isValid = false;
    }

    // Municipio (solo letras y máximo 20 caracteres)
    let regexMunicipio = /^[a-zA-ZáéíóúÁÉÍÓÚüÜ\s]{1,20}$/;
    if (!municipio.value || !regexMunicipio.test(municipio.value)) {
        document.getElementById("errorMunicipio").innerText = "Introduce un municipio válido (solo letras, máximo 20 caracteres).";
        isValid = false;
    }

    // Teléfono (debe empezar con 6, 7 o 9 y ser un número válido)
    let regexTelefono = /^[679]\d{8}$/;
    if (!telefono.value || !regexTelefono.test(telefono.value)) {
        document.getElementById("errorTelefono").innerText = "Introduce un número de teléfono válido (debe empezar con 6, 7 o 9).";
        isValid = false;
    }

    // Método de pago (obligatorio)
    if (!metodoPago.value) {
        document.getElementById("errorMetodoPago").innerText = "Selecciona un método de pago.";
        isValid = false;
    }

    //Validar email solicitante
    let emailSolicitante = document.getElementById("txtEmailSolicitante");
    let regexEmailSolicitante = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailSolicitante.value || !regexEmailSolicitante.test(emailSolicitante.value)) {
        document.getElementById("errorEmailSolicitante").innerHTML = "Intruduce un email válido.";
        isValid = false;
    }

    if (isValid) {

        let jsonDatosSolicitudPedido = {};

        let arrayInputsFormulario = $("input").not("#tarjetaModal input, #transferenciaModal input").toArray(); //.not es para que no me coja los inputs de las tarjetas modales de boostrap y el .toArray() para cogerlo como array y poder recorrelo

        arrayInputsFormulario.forEach(input => {

            if (input.id != "") {
                jsonDatosSolicitudPedido[input.id] = input.value;
            }
        });

        let select2Proveedor = document.getElementById("select2-selectProveedor-container");
        jsonDatosSolicitudPedido["nombreProveedor"] = select2Proveedor.title;

        let nifProveedor = document.getElementById("tdNifProveedor");
        jsonDatosSolicitudPedido[nifProveedor.id] = nifProveedor.innerHTML;

        let direccionProveedor = document.getElementById("tdDireccionProveedor");
        jsonDatosSolicitudPedido[direccionProveedor.id] = direccionProveedor.innerHTML;

        let telefonoProveedor = document.getElementById("tdTelefonoProveedor");
        jsonDatosSolicitudPedido[telefonoProveedor.id] = telefonoProveedor.innerHTML;

        let emailProveedor = document.getElementById("tdEmailProveedor");
        jsonDatosSolicitudPedido[emailProveedor.id] = emailProveedor.innerHTML;

        let select2Provincias = document.getElementById("select2-selectProvincias-container");
        jsonDatosSolicitudPedido["provincia"] = select2Provincias.title;

        let metodoPago = document.getElementById("metodoPago");
        jsonDatosSolicitudPedido[metodoPago.id] = metodoPago.value;

        // Inicializamos el array de productos
        let productos = [];

        // Recorrer cada fila del tbody
        $('#detallePedido tr').each(function () {
            let producto = {};

            // Obtener valores de cada columna
            let codigoProducto = $(this).find('input[name="codigoProducto"]').val() || "";
            let descripcionProducto = $(this).find('.select2-selection__rendered').attr('title') || ""; // Obtiene el título del select2
            let cantidadProducto = $(this).find('input[name="cantidadProducto"]').val() || "";
            let precioUnitarioProducto = $(this).find('input[name="precioUnitarioProducto"]').val() || "";
            let subtotalProducto = $(this).find('input[name="subtotalProducto"]').val() || "";

            // Agregar datos al objeto producto
            producto.codigo = codigoProducto;
            producto.descripcion = descripcionProducto;
            producto.CantidadProducto = cantidadProducto;
            producto.precio_unitario = precioUnitarioProducto;
            producto.SubtotalProducto = subtotalProducto;

            // Añadir el producto al array
            productos.push(producto);
        });

        // Añadir productos al JSON existente
        jsonDatosSolicitudPedido.productos = productos;

        console.log(jsonDatosSolicitudPedido);

        fetch('/api/Api/enviarSolicitudDocuware', {
            method: 'POST', // Establecer el método de la solicitud
            headers: {
                'Content-Type': 'application/json' // enviar los datos en formato JSON
            },
            body: JSON.stringify(jsonDatosSolicitudPedido) // Convertir el objeto a una cadena JSON
        })
            .then(response => {
                if (response.ok) {
                    return response.json(); // Si la respuesta es exitosa, obtener el cuerpo de la respuesta
                } else {
                    throw new Error('Error en la solicitud: ' + response.statusText); // Si ocurre un error
                }
            })
            .then(data => {
                console.log('Éxito:', data); // manejar la respuesta de la API
            })
            .catch(error => {
                console.error('Error:', error); // Manejar errores en la solicitud
            });


        Swal.fire({
            title: '¡Éxito!',
            text: 'Se ha creado su solicitud correctamente',
            icon: 'success',
            confirmButtonText: 'Aceptar',
            customClass: {
                popup: 'swal-wide'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                // Recargar la página para que se vacien los campos
                location.reload();
            }
        });;

    } else {
        Swal.fire({
            title: 'Error',
            text: 'Lo sentimos, no se ha podido procesar su solicitud',
            icon: 'error',
            confirmButtonText: 'Aceptar',
            customClass: {
                popup: 'swal-wide2'
            }
        });

    }
}

// Validar si la tarjeta está caducada
function validarFechaExpiracion(fecha) {
    const [mes, anio] = fecha.split("/").map(Number);
    const fechaActual = new Date();
    const anioActual = fechaActual.getFullYear() % 100; // Últimos 2 dígitos del año
    const mesActual = fechaActual.getMonth() + 1;

    // Verificar si el mes es válido
    if (mes < 1 || mes > 12) {
        return false;
    }

    // Verificar si la fecha
    return !(anio < anioActual || (anio === anioActual && mes < mesActual));
}


// Validar el nombre del titular de la tarjeta
function validarNombreTarjeta(nombre) {
    const nombreRegex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]{2,50}$/; // Letras y espacios, 2-50 caracteres
    return nombreRegex.test(nombre);
}

// Validar el formato del número de la tarjeta (solo números)
function validarNumeroTarjeta(numero) {
    const tarjetaRegex = /^[0-9]{16}$/; // 16 dígitos
    return tarjetaRegex.test(numero.replace(/\s/g, "")); // Quitar espacios antes de validar
}

// Validar formato del BIC (SWIFT)
function validarBIC(bic) {
    const bicRegex = /^[A-Z]{4}[A-Z0-9]{2}[A-Z0-9]{2}([A-Z0-9]{3})?$/; // BIC de 8 o 11 caracteres
    return bicRegex.test(bic.toUpperCase()); // Convierte a mayúsculas antes de validar
}

// Validar nombre del banco (solo letras y espacios)
function validarNombreBanco(banco) {
    const bancoRegex = /^[a-zA-Z\s]{2,100}$/; // Solo letras y espacios, entre 2 y 100 caracteres
    return bancoRegex.test(banco);
}

// Validar el IBAN
function validarIBAN(iban) {
    const ibanRegex = /^[A-Z]{2}[0-9]{2}[A-Z0-9]{1,30}$/; // Formato genérico del IBAN
    return ibanRegex.test(iban.replace(/\s/g, "").toUpperCase()); // Quita espacios y lo pone en mayúsculas
}

// Validar monto de la transferencia (solo números positivos)
function validarMontoTransferencia(monto) {
    const montoRegex = /^[0-9]+(\.[0-9]{1,2})?$/; // Solo números enteros o decimales con hasta 2 dígitos
    return montoRegex.test(monto) && parseFloat(monto) > 0;
}
