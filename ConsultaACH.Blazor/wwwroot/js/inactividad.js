(() => {
    const TIEMPO_INACTIVIDAD = 5 * 60 * 1000; //po 5 min

    let temporizador;

    function iniciarDetector() {

        //buscar el formulario de cierre de sesión
        const formulario = document.querySelector('form[action="/logout"]');

        //si no existe el formulario, no iniciar el detector
        if (!formulario) return;

        function cerrarSesion() {
            formulario.requestSubmit();
        }

        function reiniciarTemporizador() {
            clearTimeout(temporizador);

            temporizador = setTimeout(
                cerrarSesion,
                TIEMPO_INACTIVIDAD
            );
        }

        //detectar actividad del usuario
        const eventos = [
            "mousemove",
            "mousedown",
            "keydown",
            "scroll",
            "touchstart",
            "click"
        ];

        eventos.forEach(evento => {
            document.addEventListener(
                evento,
                reiniciarTemporizador,
                { passive: true }
            );
        });

        //contador
        reiniciarTemporizador();
    }

    iniciarDetector();
})();