## Practica1 
## MS-Kinect - Detección de posiciones y movimientos básicos.

### Autores
* Samuel Peregrina Morillas
* Nieves Victoria Velásquez Díaz

### Duración de la práctica.
Desde 6-Oct-2015 hasta 30-Oct-2015

### Descripción del problema.

Esta primera práctica ha consistido en crear una aplicación que detecta una posición fija y un gesto realizado por un usuario ante la cámara de Kinect.
La posición fija escogida ha sido la del usuario de pie y con los brazos a los lados del cuerpo; por otro lado el gesto ha consistido en subir los dos brazos a la vez hasta tenerlos estirados por encima de la cabeza. 

Para ello, en primer lugar se le indica al usuario que tiene que estar en un lugar concreto de la habitación, algo alejado de la cámara para que al subir los brazos estos no se salgan de la pantalla dando lugar a error, y tras esto se le especifica la posición que debe tener desde un principio para poder realizar el ejercicio completo.
Una vez el usuario está listo para comenzar, se le indica el gesto que debe realizar y se va monitorizando durante el tiempo que tarde el usuario en subir los brazos. Si por algún motivo se equivoca o hace algo mal, se le indica y deberá comenzar al realizar el gesto desde el principio. El usuario podrá activar la opción de "Ayuda" siempre que lo vea necesario de forma que se le mostrará como debe realizar el gesto y se le pondrá una guía. También podrá ver el esqueleto creado por la aplicación si así lo desea aunque por defecto esa opción estará desactivada. 

### Descripción de la solución.

A la hora de realizar la aplicación, se ha creado una clase: **"Moves.cs"** en la que se han declarado:
* Las variables _margenErrorSup_ y _margenErrorInf_ en las que almacenamos el valor que hemos establecido de margen de error de forma que el usuario no tenga que realizar las acciones con una gran precisión. Este marjen está establecido como el 10% tanto para el superior como para el inferior creando así una franja de seguridad con la que dar un mínimo de movilidad al usuario. 
* La variable _posturaActual_ de tipon enum, donde se almacena la postura actual del usuario.
* Las variables _manoIzqIni_ y _manoDerIni_ donde se almacenan las coordenadas de ambas manos a partir de la postura inicial.
*Las variables _manoIzqAct_, _manoDerAct_, _codoIzqAct_ y _codoDerAct_ donde almacenamos las coordenadas de la última posición tomada.

---
    
* Un struct llamado _posturas_ en el que tenemos cuatro estados distintos. Con ellos vamos almacenando la fase del movimiento en el que se encuentra el usuario. Estos pueden ser:
    1. _Inicial_ indicando que el usuario está en la posición inicial.
    2. *Brazos_En_Cruz*, con esto vemos si se ha llegado a la mitad del movimiento, en el que el usuario tiene los brazos totalmente estirados a la altura de los hombros.
    3. *Brazos_Arriba* con esto establecemos que el usuario ha llegado a la fase final del movimiento de forma correcta.
    4. *Mal* en caso de que el usuario se haya equivocado en alguno de los pasos se establecerá que lo ha realizado mal y deberá empezar de nuevo.

---
    
* El constructor de la clase en el que establecemos el valor predeterminado de _margenErrorSup_ y _margenErrorInf_ a 0.1 como se ha explicado antes, y también establecemos que se parte de que la postura del usuario al principio es _Mal_.
    
--- 
    
* Los métodos:
    1. **getManoDerInicial()** este método nos devolverá las coordenadas de la mano derecha a partir de la postura _Inicio_.
    2. **getManoDerFinal(Skeleton esqueleto)** a partir de este método calcularemos donde debe terminar la mano derecha para que se considere el ejercicio como correcto.
    1. **getPostura()** este método nos permite consutar la postura actual del usuario.
    2. **actualizarPostura(posturas nuevaPostura)** con este método actualizamos el valor de la variable _posturaActual_ con el valor pasado: _nuevaPostura_
    3. **compararCoordenadas(Joint a, Joint b, char coord)** este método se encarga de, dadas dos partes del cuerpo del usuario (indicadas con los *Join*, y el eje de coordenadas donde queremos hacer la comprobación, ver si ambas están alineadas. De forma que, tendremos un *switch case* dependiente de la coordenada donde se realizará la siguiente comprobación:
    
     ```C#
     case $:
        if (a.Position.$ < (b.Position.$ * margenErrorSup) && (a.Position.$ > (b.Position.$ * margenErrorInf)))
            respuesta = true;
      break;
     ```
     
     Una vez comprobado el eje en el que vamos a trabajar, pasaremos a comprobar si el primer elemento a está comprendido entre la franja de b por el margen de error superior e inferior de forma que nos quede _posicion de b * margen superior > posicion de a > posicion de b * margen superior_, en caso de que se cumpla la condición, establecerá la variable _respuesta_ a _true_. Variable establecida por defecto a _false_. 
     Como la comprobación es la misma en los tres casos del _switch_ para la explicación se ha puesto un ejemplo genérico, de forma que en el caso de cada eje sustituiremos en '$' por dicho eje, 'X', 'Y' y 'Z'. El método devolverá si están alineados (true) o no (false).
    4. **estadoReposo(Skeleton esqueleto)** con este método comprobamos que el usuario se encuentra en estado de reposo, para ello creamos 6 variables de forma que cada una corresponde a una de las distintas partes del cuerpo que vamos a supervisar durante el movimiento; estas son los hombros, los codos y las muñecas, tanto las del lado derecho como las del izquierdo. Para obtener esta información necesitamos acceder a la clase Skeleton de forma que podamos consultar los datos del esqueleto proporcionados por el Kinect. Una vez las hemos creado pasamos a comprobar la postura, de forma que primeramente veremos que tenga los hombros alineados, para ello nos apoyaremos en el método anterior, si estan correctos, pasaremos a comprobar que los codos estén alineados en el eje Y con su respectivos hombros, y si estan correctos se procederá a hacer lo mismo con las muñecas. Si todo estaba correcto se actualiza el valor de _posturaActual_ a _Inicial_, y en caso de que alguno de los puntos no este correctamente alineado se establecerá a _Mal_.
    5. **movimiento(SkeletonBasics esqueleto)** este método tiene un funcionamiento similar al anterior, pero en este caso, nos encargamos de monitorizar el gesto que realiza el usuario para asegurarnos de que lo está haciendo correctamente, como necesitamos saber las coordenadas de las distintas partes a tener en cuenta, debemos pasarle al algoritmo de nuevo el esqueleto mediante la clase _SkeletonBasics_, además de los seis puntos usados previamente, necesitamos cuatro más, estos son _newManoI_, _newManoD_, _newCodoI_ y _newCodoD_, en estas cuatro nuevas variables guardaremos la nueva posición de las manos o codos para poder compararlas con las anteriores y asegurar el correcto comportamiento. Para esto tendremos un _switch case_ donde dependiendo de la fase del movimiento realizaremos unas comprobaciones u otras.

     * Primero confirmaremos que el usuario esté en la posición inicial, de ser así este comenzará el gesto, de forma que tomaremos la nueva posición de las manos y veremos si están por encima de la posición anterior o no. En caso de que estén por encima veremos si el usuario ya tiene los brazos en forma de cruz, de ser así actualizaremos el valor de _posturalActual_ a *Brazos_en_cruz*, si no los ha subido aún, comprobamos que, al menos, los está subiendo mediante el método _compararCoordenadas_, si los está subiendo actualizamos el valor de las coordenadas de las manos, pero si no las está subiendo o ha realizado un movimiento distinto, diremos que está realizando mal el gesto y deberá comenzar de nuevo.
     * En el segundo caso partiremos de que el usuario  ya tiene los brazos en cruz y el procedmiento será igual que el anterior, tomará las nuevas posiciones de los brazos y los codos y comprobará si ya los tiene por encima de la cabeza. De ser así actualizará la variable _posturaActual_ a *Brazos_Arriba*, de no ser así veremos si los está subiendo, de forma que compararemos la nueva posición de los codos con la anterior y en caso de que haya aumentado actualizaremos los valores de las manos y los codos. Si no se cumple nada de lo anterior el usuario no está realizando el gesto correctamente por lo que se le indicará teniendo que comenzar el movimiento de nuevo.


### Errores y aspectos destacados.
A la hora de calcular si las extremidades están correctamente alineadas, debemos de trabajar con el valor absoluto. Esto se debe a que, aunque con el lado derecho del cuerpo no es necesario, con el lado izquierdo se obtienen valores negativos, lo que induce a error y no es capaz de ver si los miembros están alineados o no; es por eso que debemos usar valor absoluto, de modo que se evita dicho error y se asegura de que se realizan las comprobaciones de la forma correcta.

### Bibliografía.
