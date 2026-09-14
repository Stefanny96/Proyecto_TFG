// Lista global para almacenar los IDs de los productos ya seleccionados
let productosSeleccionados = [];

// Llamar las funciones cuando la página esté completamente cargada
window.onload = function () {
    generarFechaSolicitud(); // Generar la fecha de solicituD
    inicializarSelect2Proveedores();
    inicializarSelect2Provincias();
    generarFechaEntregaAleatoria();
    rellenarDatosUsuarioLogueado();
    generarNumeroSolicitud(); // Generar el número de solicitud
};


// Función para generar el número de solicitud
function generarNumeroSolicitud() {
    /*const fecha = new Date();
      const numeroSolicitud = `SP-${fecha.getFullYear().toString().slice(-2)}${(fecha.getMonth() + 1).toString().padStart(2, '0')}${fecha.getDate().toString().padStart(2, '0')}${fecha.getHours().toString().padStart(2, '0')}${fecha.getMinutes().toString().padStart(2, '0')}${fecha.getSeconds().toString().padStart(2, '0')}`;
      document.getElementById("numeroSolicitud").value = numeroSolicitud;*/

    const email = document.getElementById("txtEmailSolicitante").value;

    // Obtener la fecha y hora actual
    const timestamp = Date.now(); // Número de milisegundos desde 1970

    // Generar un hash simple del correo electrónico
    const hash = email
        .split("")
        .reduce((acc, char) => acc + char.charCodeAt(0), 0) // Sumar códigos ASCII
        .toString(16); // Convertir a hexadecimal

    // Combinar los elementos para formar el número único
    const numeroSolicitud = `SOL-${hash}-${timestamp}`;

    document.getElementById("numeroSolicitud").value = numeroSolicitud;
}

// Función para generar la fecha de solicitud
function generarFechaSolicitud() {
    const fecha = new Date();
    const dia = fecha.getDate().toString().padStart(2, "0"); // Asegurarse de que el día tenga 2 dígitos
    const mes = (fecha.getMonth() + 1).toString().padStart(2, "0"); // El mes comienza en 0, así que sumamos 1
    const anio = fecha.getFullYear();
    document.getElementById("fechaSolicitud").value = `${anio}-${mes}-${dia}`; // Formato YYYY/MM/DD
}
function rellenarDatosUsuarioLogueado() {

    let nombreUsuarioSolicitante = document.getElementById("txtNombreSolicitante");
    //let emailUsuarioSolicitante = document.getElementById("txtEmailSolicitante");
    let cifEmpresaDelUsuarioSolicitante = document.getElementById("txtNifSolicitante");

    //datosUsuario es un variable goblal de javascript definida en la vista formulario.cshtml
    nombreUsuarioSolicitante.value = window.appData.datosUsuario.nombre;
    emailUsuarioSolicitante.value = window.appData.datosUsuario.email;
    cifEmpresaDelUsuarioSolicitante.value = window.appData.datosEmpresa.cif;

}

// Función para generar una fecha prevista de entrega aleatoria (máximo 2 semanas desde hoy)
function generarFechaEntregaAleatoria() {
    let hoy = new Date();
    let diasMaximos = 14; // Máximo 2 semanas
    let diasAleatorios = Math.floor(Math.random() * diasMaximos) + 1; // Número de días aleatorios entre 1 y 14

    // Crear la fecha de entrega sumando los días aleatorios
    let fechaEntrega = new Date(hoy);
    fechaEntrega.setDate(hoy.getDate() + diasAleatorios);

    // Formatear la fecha como DD/MM/YYYY
    let dia = fechaEntrega.getDate().toString().padStart(2, "0");
    let mes = (fechaEntrega.getMonth() + 1).toString().padStart(2, "0");
    let anio = fechaEntrega.getFullYear();

    let idFechaPrevistaEntrega = document.getElementById("fechaEntrega");
    idFechaPrevistaEntrega.value = `${anio}-${mes}-${dia}`;
}

// Función para inicializar Select2 de proveedores
function inicializarSelect2Proveedores() {
    leerJsonProveedores(function (data) {
        let proveedores = data.map((proveedor) => ({
            id: proveedor.proveedor_id, // ID para referencia
            text: proveedor.nombre_razon_social, // Nombre visible en el dropdown
        }));

        // Inicializa Select2
        $("#selectProveedor").select2({
            placeholder: "Buscar proveedor...",
            data: proveedores,
        });

        // Manejar selección de un proveedor
        $("#selectProveedor").on("select2:select", function (e) {
            let seleccionado = e.params.data;

            // Filtrar el proveedor seleccionado por su ID
            let proveedorSeleccionado = data.find(
                (proveedor) => proveedor.proveedor_id == seleccionado.id
            );

            // Mostrar la información del proveedor en un div
            mostrarInformacionProveedor(proveedorSeleccionado);

            // Eliminar lineas de producto cuando cambias de proveedor
            limpiarFilas();

            //Para que se muestre cuando seleccionas proveedor
            agregarLinea();
        });
    });
}

function mostrarInformacionProveedor(proveedor) {
    productosSeleccionados = [];
    const html = `
        <div class="proveedor-info mt-3">
            <table class="table">
            <thead>
                <tr>
                    <th>NIF/CIF</th>
                    <th>Dirección</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td id="tdNifProveedor">${proveedor.nif_cif}</td>
                    <td id="tdDireccionProveedor">${proveedor.direccion.calle}, ${proveedor.direccion.ciudad}, ${proveedor.direccion.pais}, ${proveedor.direccion.codigo_postal}</td>
                </tr>
            </tbody>
            </table>

            <table class="table">
            <thead>
                <tr>
                    <th>Teléfono</th>
                    <th>Email</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td id="tdTelefonoProveedor">${proveedor.contacto_principal.telefono}</td>
                    <td id="tdEmailProveedor">${proveedor.contacto_principal.correo_electronico}</td>
                </tr>
            </tbody>
            </table>
        </div>
    `;

    // Insertamos el HTML en un contenedor
    $("#divInformacionProveedor").html(html);
}
function agregarLinea() {
    let idProveedorSeleccionado =
        document.getElementById("selectProveedor").value;

    if (idProveedorSeleccionado != 0) {
        let fila = document.createElement("tr");
        fila.innerHTML = `
          <td><input type="text" class="form-control codigoProducto txtProductos" name="codigoProducto" placeholder="Código del producto" readonly /></td>
          <td>
              <select class="form-control select2 selectProducto" style="width: 100%;"></select>
          </td>
          <td><input type="number" class="form-control cantidadProducto txtProductos" name="cantidadProducto" placeholder="Cantidad" oninput="manejarCambioCantidad(this)"/></td>
          <td>
              <div class="input-precio">
                  <input type="number" class="form-control precioUnitarioProducto txtProductos" name="precioUnitarioProducto" placeholder="Precio unitario" oninput="actualizarSubtotal(this)" readonly />
                  <span class="simbolo-euro">€</span>
              </div>
          </td>
          <td>
              <div class="input-precio">
                  <input type="number" class="form-control subtotalProducto txtProductos" name="subtotalProducto" placeholder="Subtotal" readonly />
                  <span class="simbolo-euro">€</span>
              </div>
          </td>
          <td>
              <button type="button" class="btnEliminarProducto" onclick="eliminarFila(this)">Eliminar</button>
          </td>
      `;

        document.getElementById("detallePedido").appendChild(fila);

        rellenarSelectProducto(fila); // Cargar los productos disponibles
    } else {
        alert("No ha seleccionado ningún proveedor");
    }
}

function rellenarSelectProducto(fila) {
    let selectFilaProducto = fila.querySelector(".selectProducto");
    let idProveedorSeleccionado =
        document.getElementById("selectProveedor").value;

    leerJsonProductos(function (data) {
        // Filtrar productos del proveedor seleccionado
        let productosFiltrados = data.filter(
            (producto) =>
                producto.proveedor_id == idProveedorSeleccionado &&
                !productosSeleccionados.includes(producto.producto_id) // Excluir los seleccionados
        );

        // Convertir los productos filtrados en formato compatible con Select2
        let productos = productosFiltrados.map((producto) => ({
            id: producto.producto_id,
            text: producto.nombre,
        }));

        // Inicializar el Select2 con un placeholder
        $(selectFilaProducto).select2({
            placeholder: "Seleccione producto", // Placeholder visible por defecto
            data: productos,
        });

        // Forzar que no haya un producto seleccionado al inicio
        $(selectFilaProducto).val(null).trigger("change");

        // Configurar evento de selección
        $(selectFilaProducto)
            .off("select2:select")
            .on("select2:select", function (e) {
                let seleccionado = e.params.data;

                // Agregar el producto seleccionado a la lista global
                productosSeleccionados.push(seleccionado.id);

                // Mostrar información del producto en la fila actual
                let productoSeleccionado = productosFiltrados.find(
                    (producto) => producto.producto_id == seleccionado.id
                );
                if (productoSeleccionado) {
                    mostrarInformacionProducto(productoSeleccionado, fila);
                }
            });
    });
}

function actualizarTodosLosSelects() {
    // Obtener todas las filas de la tabla
    let filas = document.querySelectorAll("#detallePedido tr");

    // Leer el proveedor seleccionado
    let idProveedorSeleccionado = document.getElementById("selectProveedor").value;

    // Leer todos los productos una sola vez
    leerJsonProductos(function (data) {
        // Filtrar productos del proveedor seleccionado
        let productosFiltrados = data.filter(
            (producto) => producto.proveedor_id == idProveedorSeleccionado
        );

        filas.forEach((fila) => {
            let selectFilaProducto = fila.querySelector(".selectProducto");

            // Excluir productos ya seleccionados
            let productosDisponibles = productosFiltrados.filter(
                (producto) => !productosSeleccionados.includes(producto.producto_id)
            );

            // Crear opciones para Select2
            let productos = productosDisponibles.map((producto) => ({
                id: producto.producto_id,
                text: producto.nombre,
            }));

            // Obtener el valor seleccionado actualmente
            let valorSeleccionado = $(selectFilaProducto).val();

            // Actualizar el Select2 solo si el valor seleccionado no es válido
            if (
                !productos.find((producto) => producto.id == valorSeleccionado)
            ) {
                $(selectFilaProducto).val(null).trigger("change");
            }

            // Actualizar las opciones del Select2
            $(selectFilaProducto)
                .empty() // Vaciar opciones anteriores
                .select2({
                    data: productos,
                    placeholder:
                        productos.length > 0
                            ? "Seleccione producto"
                            : "No hay productos disponibles",
                });
        });
    });
}


function mostrarInformacionProducto(producto, fila) {
    // Seleccionar elementos por clase y establecer los valores
    let codigoProducto = fila.querySelector(".codigoProducto");
    if (codigoProducto) codigoProducto.value = `${producto.codigo}`;

    let precioUnitarioProducto = fila.querySelector(".precioUnitarioProducto");
    if (precioUnitarioProducto)
        precioUnitarioProducto.value = `${producto.precio_unitario}`;

    let cantidadProducto = fila.querySelector(".cantidadProducto");
    if (cantidadProducto && cantidadProducto.value > 0)
        actualizarSubtotal(cantidadProducto);
}

function manejarCambioCantidad(input) {
    let fila = input.closest("tr");
    let selectProducto = $(fila).find(".selectProducto").val();

    leerJsonProductos(function (data) {
        let productoSeleccionado = data.find(
            (producto) => producto.producto_id == selectProducto
        );

        if (productoSeleccionado) {
            let maxUnidades = Math.min(
                productoSeleccionado.unidades_disponibles,
                productoSeleccionado.stock_maximo
            );
            let minUnidades = 1; // Cantidad mínima permitida

            if (input.value > maxUnidades) {
                input.value = maxUnidades; // Ajustar al máximo permitido
                //alert(`No puedes seleccionar más de ${maxUnidades} unidades para este producto.`);
            } else if (input.value < minUnidades) {
                input.value = minUnidades; // Ajustar al mínimo permitido
                //alert(`Debes seleccionar al menos ${minUnidades} unidad(es) de este producto.`);
            }
        }

        // Actualizar el subtotal después de validar la cantidad
        actualizarSubtotal(input);
    });
}

function actualizarSubtotal(inputCantidad) {
    // Obtener la fila en la que está el campo de cantidad
    let fila = inputCantidad.closest("tr");

    // Seleccionar los elementos de cantidad, precio unitario y subtotal
    let cantidadProducto = fila.querySelector(".cantidadProducto");
    let precioUnitarioProducto = fila.querySelector(".precioUnitarioProducto");
    let subtotalProducto = fila.querySelector(".subtotalProducto");

    // Asegurarse de que los campos no sean nulos y calcular el subtotal
    if (cantidadProducto && precioUnitarioProducto && subtotalProducto) {
        let cantidad = parseFloat(cantidadProducto.value) || 0;
        let precioUnitario = parseFloat(precioUnitarioProducto.value) || 0;

        // Calcular el subtotal
        let subtotal = cantidad * precioUnitario;

        // Actualizar el campo de subtotal
        subtotalProducto.value = subtotal.toFixed(2); // Mostrar con 2 decimales
    }

    // Calcular el total del pedido
    calcularTotalPedido();
}

function calcularTotalPedido() {
    let total = 0;

    // Obtener todas las filas del detalle del pedido
    let filas = document.querySelectorAll("#detallePedido tr");

    // Recorrer las filas y sumar los subtotales
    filas.forEach((fila) => {
        let subtotalProducto = fila.querySelector(".subtotalProducto");
        if (subtotalProducto) {
            let subtotal = parseFloat(subtotalProducto.value) || 0;
            total += subtotal;
        }
    });

    // Actualizar el campo total del pedido
    let totalPedido = document.getElementById("totalPedido");
    if (totalPedido) {
        totalPedido.value = total.toFixed(2); // Mostrar con 2 decimales
    }
}

function limpiarFilas() {
    let tablaPedido = document.getElementById("detallePedido");
    let listaFilas = [...tablaPedido.getElementsByTagName("tr")];

    listaFilas.forEach((fila) => {
        fila.remove();
    });
}

function eliminarFila(boton) {
    let fila = boton.closest("tr");
    let selectFilaProducto = fila.querySelector(".selectProducto");

    // Obtener el ID del producto seleccionado en la fila (si existe)
    let idProductoSeleccionado = $(selectFilaProducto).val();
    if (idProductoSeleccionado) {
        // Eliminar el producto de la lista global de seleccionados
        productosSeleccionados = productosSeleccionados.filter(
            (id) => id != idProductoSeleccionado
        );
    }

    // Eliminar la fila del DOM
    fila.remove();

    // Actualizar todos los Select2 para reflejar los cambios
    actualizarTodosLosSelects();

    // Recalcular el total del pedido
    calcularTotalPedido();
}


function leerJsonProveedores(callback) {
    //Usamos callback para manejar la asincronia, tambien se puede usar promesas pero esta manera me parece mas facil
    $.ajax({
        url: "/js/datosProveedores.json", // Ruta al archivo JSON
        method: "GET",
        dataType: "json",
        success: function (data) {
            callback(data); // Llamar al callback con los datos obtenidos
        },
        error: function (xhr, status, error) {
            console.error("Error al cargar el JSON proveedores:", error);
        },
    });
}

function leerJsonProductos(callback) {
    $.ajax({
        url: "/js/datosProductos.json", // Ruta al archivo JSON
        method: "GET",
        dataType: "json",
        success: function (data) {
            callback(data); // Llamar al callback con los datos obtenidos
        },
        error: function (xhr, status, error) {
            console.error("Error al cargar el JSON productos:", error);
        },
    });
}

function leerJsonProvinciasEspania(callback) {
    $.ajax({
        url: "/js/provinciasEspania.json", // Ruta al archivo JSON
        method: "GET",
        dataType: "json",
        success: function (data) {
            callback(data); // Llamar al callback con los datos obtenidos
        },
        error: function (xhr, status, error) {
            console.error("Error al cargar el JSON provincias:", error);
        },
    });
}

function inicializarSelect2Provincias() {
    leerJsonProvinciasEspania(function (data) {
        let provincias = data.map((provincia) => ({
            id: provincia.id, // ID para referencia
            text: provincia.nombre, // Nombre visible en el dropdown
        }));

        // Inicializa Select2
        $("#selectProvincias").select2({
            placeholder: "Seleccione provincia",
            data: provincias,
        });
    });
}

//--------------------- FUNCIONES MODAL TARJETA Y TRANSFERENCIA --------------------------------------

document.addEventListener("DOMContentLoaded", function () {
    const metodoPago = document.getElementById("metodoPago");
    const tarjetaModal = new bootstrap.Modal(
        document.getElementById("tarjetaModal")
    );
    const transferenciaModal = new bootstrap.Modal(
        document.getElementById("transferenciaModal")
    );
    const datosTarjetaDiv = document.getElementById("datosTarjeta");
    const datosTransferenciaDiv = document.getElementById("datosTransferencia");

    // Inicia los eventos
    function initEvents() {
        metodoPago.addEventListener("change", mostrarModalPago);
        document
            .getElementById("numeroTarjeta")
            .addEventListener("input", formatearNumeroTarjeta);
        document
            .getElementById("fechaExpiracion")
            .addEventListener("input", formatearFechaExpiracion);
        document
            .getElementById("formTarjeta")
            .addEventListener("submit", guardarDatosTarjeta);
        document
            .getElementById("formTransferencia")
            .addEventListener("submit", guardarDatosTransferencia);
    }

    // Muestra el modal de pago según la opción seleccionada
    function mostrarModalPago() {
        // Ocultar datos previos si cambian de método de pago
        datosTarjetaDiv.innerHTML = "";
        datosTransferenciaDiv.innerHTML = "";

        if (metodoPago.value === "tarjeta") {
            tarjetaModal.show();
        } else if (metodoPago.value === "transferencia") {
            transferenciaModal.show();
        }
    }

    // Formateo automático del número de tarjeta
    function formatearNumeroTarjeta(e) {
        let valor = e.target.value.replace(/\D/g, ""); // Elimina caracteres no numéricos
        valor = valor.match(/.{1,4}/g)?.join(" ") || valor; // Agrupa cada 4 números con espacios
        e.target.value = valor;
    }

    // Formateo automático de la fecha de expiración
    function formatearFechaExpiracion(e) {
        let valor = e.target.value.replace(/\D/g, ""); // Elimina caracteres no numéricos
        if (valor.length > 2) {
            valor = valor.slice(0, 2) + "/" + valor.slice(2, 4); // Formato MM/YY
        }
        e.target.value = valor;
    }

    // Manejar el envío del formulario de tarjeta
    function guardarDatosTarjeta(e) {
        e.preventDefault();

        const nombreTitular = document.getElementById("nombreTitular").value.trim();
        const numeroTarjeta = document.getElementById("numeroTarjeta").value;
        const fechaExpiracion = document.getElementById("fechaExpiracion").value;
        const cvv = document.getElementById("cvv").value;

        // Validaciones
        if (!validarNombreTarjeta(nombreTitular)) {
            alert(
                "Por favor, introduce un nombre válido (sin números ni caracteres especiales, 2-50 caracteres)."
            );
            return;
        }

        if (!validarNumeroTarjeta(numeroTarjeta)) {
            alert("El número de tarjeta debe contener solo 16 dígitos.");
            return;
        }

        if (!validarFechaExpiracion(fechaExpiracion)) {
            alert("La tarjeta está caducada.");
            return;
        }

        // Mostrar los datos debajo del formulario
        datosTarjetaDiv.innerHTML =
            `<div class="card mt-3">
                <div class="card-body">
                  <h5 class="card-title">Datos de la Tarjeta</h5>
                  <p><strong>Nombre del Titular:</strong> ${nombreTitular}</p>
                  <p><strong>Número de Tarjeta:</strong> ${numeroTarjeta}</p>
                  <p><strong>Fecha de Expiración:</strong> ${fechaExpiracion}</p>
                  <p><strong>CVV:</strong> ${"*".repeat(cvv.length)}</p>
                </div>
            </div>`;

        tarjetaModal.hide(); // Cierra el modal
    }

    // Manejar el envío del formulario de transferencia bancaria
    function guardarDatosTransferencia(e) {
        e.preventDefault();

        const cuentaOrigen = document.getElementById("cuentaOrigen").value.trim();
        const titularCuenta = document.getElementById("titularCuenta").value.trim();
        const codigoBic = document.getElementById("codigoBic").value.trim();
        const bancoBeneficiario = document
            .getElementById("bancoBeneficiario")
            .value.trim();
        const cuentaDestino = document.getElementById("cuentaDestino").value.trim();
        const montoTransferencia =
            document.getElementById("montoTransferencia").value;

        // Validaciones
        if (!validarIBAN(cuentaOrigen)) {
            alert("Por favor, introduce un IBAN de origen válido.");
            return;
        }

        if (!validarNombreTarjeta(titularCuenta)) {
            alert(
                "Por favor, introduce un titular válido (sin números ni caracteres especiales, 2-50 caracteres)."
            );
            return;
        }

        if (!validarIBAN(cuentaDestino)) {
            alert("Por favor, introduce un IBAN de destino válido.");
            return;
        }

        if (codigoBic && !validarBIC(codigoBic)) {
            alert(
                "El código BIC debe tener el formato correcto (8 o 11 caracteres)."
            );
            return;
        }

        if (!validarNombreBanco(bancoBeneficiario)) {
            alert(
                "Por favor, introduce un nombre de banco válido (sin números, entre 2 y 100 caracteres)."
            );
            return;
        }

        if (cuentaDestino === "") {
            alert("Por favor, introduce el número de cuenta de destino.");
            return;
        }

        if (!validarMontoTransferencia(montoTransferencia)) {
            alert(
                "El monto debe ser un número positivo y válido (puede incluir decimales)."
            );
            return;
        }

        let cantidadTotalSolicitud = document.getElementById("totalPedido");
        // Validar que el monto no sea menor al total de la solicitud
        if (
            parseFloat(montoTransferencia) != cantidadTotalSolicitud.value ||
            cantidadTotalSolicitud.value <= 0
        ) {
            alert(
                "El monto de la transferencia debe ser igual al total de la solicitud de pedido."
            );
            return;
        }

        // Mostrar los datos debajo del formulario
        datosTransferenciaDiv.innerHTML =
            `<div class="card mt-3">
                <div class="card-body">
                  <h5 class="card-title">Datos de la Transferencia</h5>
                  <p><strong>Cuenta Origen:</strong> ${cuentaOrigen}</p>
                  <p><strong>Titular Cuenta:</strong> ${titularCuenta}</p>
                  <p><strong>Código BIC:</strong> ${codigoBic}</p>
                  <p><strong>Banco Beneficiario:</strong> ${bancoBeneficiario}</p>
                  <p><strong>Cuenta Destino:</strong> ${cuentaDestino}</p>
                  <p><strong>Monto de la Transferencia:</strong> ${montoTransferencia}</p>
                </div>
            </div>`;
        transferenciaModal.hide(); // Cierra el modal
    }

    // Llama a la función para iniciar los eventos
    initEvents();
});
