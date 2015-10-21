## Practica1 MS-Kinect Detección de posiciones y movimientos básicos.

###Autores
Samuel Peregrina Morillas
Nieves Victoria Velásquez Díaz

###Duración de la práctica.
Desde 6-Oct-2015 hasta 23-Oct-2015

### Descripción del problema.

Esta primera práctica ha consistido en crear una aplicación que detecta una posición fija y un gesto realizado por un usuario ante la cámara de Kinect.
La posición fija escogida ha sido la del usuario de pie y con los brazos a los lados del cuerpo; por otro lado el gesto ha consistido en subir los dos brazos a la vez hasta tenerlos estirados por encima de la cabeza. 

Para ello, en primer lugar se le indica al usuario que tiene que estar en un lugar concreto de la habitación, algo alejado de la cámara para que al subir los brazos estos no se salgan de la pantalla dando lugar a error, y tras esto se le especifica la posición que debe tener desde un principio para poder realizar el ejercicio completo.
Una vez el usuario está listo para comenzar, se le indica el gesto que debe realizar y se va monitorizando durante el tiempo que tarde el usuario en subir los brazos. Si por algún motivo se equivoca o hace algo mal, se le indica y deberá comenzar al realizar el gesto desde el principio. El usuario podrá activar la opción de "Ayuda" siempre que lo vea necesario de forma que se le mostrará como debe realizar el gesto y se le pondrá una guía. También podrá ver el esqueleto creado por la aplicación si así lo desea aunque por defecto esa opción estará desactivada. 

### Descripción de la solución. (subapartados: codigo y hud)

A la hora de realizar la aplicación, se ha creado una clase: "Moves.cs"en la que se han declarado:
    ··*Las variables _margenErrorSup_ y _margenErrorInf_ en las que almacenamos el valor que hemos establecido de margen de error de forma que el usuario no tenga que realizar las acciones con una gran precisión. Este marjen está establecido como el 10% tanto para el superior como para el inferior creando así una franja de seguridad con la que dar un mínimo de movilidad al usuario. Y la variable _posturaActual_ donde se almacena la postura actual del usuario.
    ---
    ··* Un struct llamado _posturas_ en el que tenemos cuatro estados distintos. Con ellos vamos almacenando la fase del movimiento en el que se encuentra el usuario. Estos pueden ser:
        ···1._Inicial_ indicando que el usuario está en la posición inicial.
        ···2.*Brazos_En_Cruz*, con esto vemos si se ha llegado a la mitad del movimiento, en el que el usuario tiene los brazos totalmente estirados a la altura de los hombros.
        ···3.*Brazos_Arriba* con esto establecemos que el usuario ha llegado a la fase final del movimiento de forma correcta.
        ···4.*Mal* en caso de que el usuario se haya equivocado en alguno de los pasos se establecerá que lo ha realizado mal y deberá empezar de nuevo.
    ---
    ··*El constructor de la clase en el que establecemos el valor predeterminado de _margenErrorSup_ y _margenErrorInf_ a 0.1 como se ha explicado antes, y también establecemos que se parte de que la postura del usuario al principio es _Mal_.
    --- 
    ··*Los métodos:
        ···1.*getPostura()* este método nos permite consutar la postura actual del usuario.
        ···2.*actualizarPostura(posturas nuevaPostura)* con este método actualizamos el valor de la variable _posturaActual_ con el valor pasado: _nuevaPostura_
        ···3.*compararCoordenadas(Joint a, Joint b, char coord)* este método se encarga de, dadas dos partes del cuerpo del usuario (indicadas con los *Join*, y el eje de coordenadas donde queremos hacer la comprobación, ver si ambas están alineadas. De forma que, tendremos un *switch case* dependiente de la coordenada donde se realizará la siguiente comprobación:
        ```C#
        case $:
                if (a.Position.$ < (b.Position.$ * margenErrorSup) && (a.Position.$ > (b.Position.$ * margenErrorInf)))
                    respuesta = true;
        break;
        ```
        Una vez comprobado el eje en el que vamos a trabajar, pasaremos a comprobar si el primer elemento a está comprendido entre la franja de b por el margen de error superior e inferior de forma que nos quede _posicion de b * margen superior > posicion de a > posicion de b * margen superior_



### Errores y aspectos destacados.

### Lecturas recomendadas. ??

### Bibliografía.