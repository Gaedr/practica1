## Practica1 MS-Kinect Detección de posiciones y movimientos básicos.

### Descripción del problema.

Esta primera práctica ha consistido en crear una aplicación que detecta una posición fija y un gesto realizado por un usuario ante la cámara de Kinect.
La posición fija escogida ha sido la del usuario de pie y con los brazos a los lados del cuerpo; por otro lado el gesto ha consistido en subir los dos brazos a la vez hasta tenerlos estirados por encima de la cabeza. 

Para ello, en primer lugar se le indica al usuario que tiene que estar en un lugar concreto de la habitación, algo alejado de la cámara para que al subir los brazos estos no se salgan de la pantalla dando lugar a error, y tras esto se le especifica la posición que debe tener desde un principio para poder realizar el ejercicio completo.
Una vez el usuario está listo para comenzar, se le indica el gesto que debe realizar y se va monitorizando durante el tiempo que tarde el usuario en subir los brazos. Si por algún motivo se equivoca o hace algo mal, se le indica y deberá comenzar al realizar el gesto desde el principio. El usuario podrá activar la opción de "Ayuda" siempre que lo vea necesario de forma que se le mostrará como debe realizar el gesto y se le pondrá una guía. También podrá ver el esqueleto creado por la aplicación si así lo desea aunque por defecto esa opción estará desactivada. 

### Descripción de la solución. (subapartados: codigo y hud)

A la hora de realizar la aplicación, se ha creado una clase: "Moves.cs"en la que se han declarado:
    ..*Las variables _margenErrorSup_ y _margenErrorInf_ en las que almacenamos el valor que hemos establecido de margen de error de forma que el usuario no tenga que realizar las acciones con una gran precisión. Este marjen está establecido como el 10% tanto para el superior como para el inferior creando así una franja de seguridad con la que dar un mínimo de movilidad al usuario.
    ..


### Errores y aspectos destacados.

### Lecturas recomendadas. ??

### Bibliografía.