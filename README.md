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

## Descripción de la solución.

### Movimiento.

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
    2. **getManoIzqInicial()** este método nos devolverá las coordenadas de la mano izquierda a partir de la postura _Inicio_.
    3. **getPostura()** este método nos permite consutar la postura actual del usuario.
    4. **actualizarPostura(posturas nuevaPostura, Skeleton esqueleto)** con este método actualizamos el valor de la variable _posturaActual_ con el valor pasado: _nuevaPostura_: también actualizaremos los valores de _manoDerAct_, _manoIzqAct_, _codoDerAct_ y _codoIzqAct_ de forma que almacenaremos las nuevas coordenadas de dichos puntos en caso de que se haya realizado un movimiento, para comprobaciones posteriores. En el caso de que el usuario se encuentre en la posicion actual, sólo actualizará los valores de ambas manos. 
    5. **actualizarPostura(Skeleton esqueleto)** existe otra versión del método actualizarPostura donde sólo actualizamos los valores de las manos y los codos con los nuevos valores obtenidos a partir del esqueleto.
    6. **compararCoordenadas(Joint a, Joint b, char coord)** este método se encarga de, dadas dos partes del cuerpo del usuario (indicadas con los *Join*, y el eje de coordenadas donde queremos hacer la comprobación, ver si ambas están alineadas. De forma que, tendremos un *switch case* dependiente de la coordenada donde se realizará la siguiente comprobación:
    
     ```C#
     case $:
        if (Math.Abs(a.Position.$) < (Math.Abs(b.Position.$) * margenErrorSup) && (Math.Abs(a.Position.$) > (Math.Abs(b.Position.$) * margenErrorInf)))
            respuesta = true;
      break;
     ```
     
     Una vez comprobado el eje en el que vamos a trabajar, pasaremos a comprobar si el primer elemento 'a' está comprendido entre la franja de 'b' por el margen de error superior e inferior de forma que nos quede _posicion de b * margen superior > posicion de a > posicion de b * margen superior_, el calculo debe ser en valor absoluto para evitar valores negativos; en caso de que se cumpla la condición, establecerá la variable _respuesta_ a _true_. Variable establecida por defecto a _false_. 
     Como la comprobación es la misma en los tres casos del _switch_ para la explicación se ha puesto un ejemplo genérico, de forma que en el caso de cada eje sustituiremos en '$' por dicho eje, 'X', 'Y' y 'Z'. El método devolverá si están alineados (true) o no (false).
    9. **estadoReposo(Skeleton esqueleto)** con este método comprobamos que el usuario se encuentra en estado de reposo, para ello tenemos 4 variables, de forma que cada una corresponde a una de las distintas partes del cuerpo que vamos a supervisar durante el movimiento; estas son las muñecas y las caderas. Para obtener esta información necesitamos acceder a la clase Skeleton de forma que podamos consultar los datos del esqueleto proporcionados por el Kinect. Una vez las hemos creado pasamos a comprobar la postura, de forma que nos aseguraremos que las muñecas están por debajo de la cadera. Es decir, que la muñeca derecha está por debajo de la cadera derecha y la muñeca izquierda está por debajo de la cadera izquierda. Si todo está correcto se actualiza el valor de _posturaActual_ a _Inicial_, y en caso de que alguno de los puntos no este correctamente alineado se establecerá a _Mal_.
    10. **movimiento(SkeletonBasics esqueleto)** este método tiene un funcionamiento similar al anterior, pero en este caso, nos encargamos de monitorizar el gesto que realiza el usuario para asegurarnos de que lo está haciendo correctamente, como necesitamos saber las coordenadas de las distintas partes a tener en cuenta, debemos pasarle al algoritmo de nuevo el esqueleto mediante la clase _SkeletonBasics_, además de los puntos de las muñecas usados previamente, necesitamos cinco más, estos son _hombroI_, _hombroD_, _codoI_, _codoD_ y _cabeza_, en estas nuevas variables guardaremos la nueva posición de los codos, los hombros y cabeza para poder compararlas con las anteriores y asegurar el correcto comportamiento. Para esto tendremos un _switch case_ donde dependiendo de la fase del movimiento realizaremos unas comprobaciones u otras.

     * Primero confirmaremos que el usuario esté en la posición inicial, de ser así este comenzará el gesto, de forma que tomaremos la nueva posición de las manos y veremos si están por encima de la posición anterior o no. En caso de que estén por encima veremos si el usuario ya tiene los brazos en forma de cruz, de ser así actualizaremos el valor de _posturalActual_ a *Brazos_en_cruz*, si no los ha subido aún, comprobamos que, al menos, los está subiendo comparando la coordenada y nueva con la antigua, de forma que si aumenta los está subiendo, si los está subiendo actualizamos el valor de las coordenadas de las manos, pero si no las está subiendo, no está en reposo ni en cruz ha realizado un movimiento distinto, por lo que diremos que está realizando mal el gesto y deberá comenzar de nuevo.
     * En el segundo caso partiremos de que el usuario  ya tiene los brazos en cruz y el procedmiento será igual que el anterior, tomará las nuevas posiciones de los brazos y los codos y comprobará si ya los tiene por encima de la cabeza. De ser así actualizará la variable _posturaActual_ a *Brazos_Arriba*, de no ser así veremos si los está subiendo, de forma que compararemos la nueva posición de los codos con la anterior y en caso de que haya aumentado actualizaremos los valores de las manos y los codos. Si no se cumple nada de lo anterior el usuario no está realizando el gesto correctamente por lo que se le indicará, teniendo que comenzar el movimiento de nuevo.
    
--- 

#### Interfaz gráfica
En este apartado se procederá a explicar la interfaz con la que interactuará el usuario. En la ventana principal tendremos, en la parte más baja un mensaje que nos informará de si el dispositivo está conectado o no. 
Por otra parte, en la parte inferior de lo que es la ventana de interactuación tenemos una serie de mensajes que hacen de guía para el usuario, de modo que el primero es *Póngase en estado de Reposo*, que consiste en colocar las manos a los lados, una vez en este estado el mensaje cambiará a *Levante los brazos hasta estar en cruz* y además aparecerán dos puntos azules indicando la posición que deberá tener el usuario para que se considere que tiene los brazos en cruz. Una vez se tienen los brazos en cruz cambia el color de las esferas a verde y aparecerá un nuevo mensaje *Continúe el movimiento de los brazos hasta arriba* y de nuevo nos mostrará una guía de forma que volverán a aparecer dos nuevos círculos por encima de la cabeza, indicando que es la posición final. En cuanto el usuario alcance dicha posición, se pondrán los círculos en verde y aparecerá el siguiente mensaje: *¡¡Perfecto!!*. En caso de que el usuario se equivoque en algun punto del movimiento aparecerá el mensaje *Pongase en estado de Reposo* de nuevo obligando al usuario a comenzar el movimiento desde el principio.

### Errores y aspectos destacados.
A la hora de calcular si las extremidades están correctamente alineadas, debemos de trabajar con el valor absoluto. Esto se debe a que, aunque con el lado derecho del cuerpo no es necesario, con el lado izquierdo se obtienen valores negativos, lo que induce a error y no es capaz de ver si los miembros están alineados o no; es por eso que debemos usar valor absoluto, de modo que se evita dicho error y se asegura de que se realizan las comprobaciones de la forma correcta. Esto también es necesario a tener en cuenta a la hora de calcular la posición final de cada uno de las partes del movimiento.
Han surjido problemas a la hora de calcular la posición donde deberían estar las muñecas en los brazos en cruz y con los brazos arriba, ya que no se realizaba bien la conversión de las coordenadas, lo que daba lugar a que se dibujase el punto en la esquina superior izquierda, por lo que hemos tenido que realizar los calculos en la misma clase de la interfaz en vez de en la de moves para evitar dicho error.
La idea principal era comparar las muñecas con los codos y los hombros de ambos brazos para ir monitorizando el movimiento según se iba realizando, pero han surjido varios problemas a la hora de tratar con esto ya que era necesario que se subiesen ambos brazos a la vez de forma muy exacta lo que daba lugar a que el usuario fallase en repetidas ocasiones, llegando a ser excesivo, por lo que se ha simplificado la monitorización del movimiento.
También se ha terminado modificando el valor del margen de error aumentándolo de un 10% a un 40%.

### Bibliografía.
* http://www.codeproject.com/Articles/213034/Kinect-Getting-Started-Become-The-Incredible-Hulk
* http://stackoverflow.com/questions/27236134/kinect-transform-from-skeleton-data-to-depth-data-and-back
* https://msdn.microsoft.com/en-us/library/hh855377.aspx
* https://msdn.microsoft.com/en-us/library/jj131025.aspx
* https://www.youtube.com/user/NarutoZeroner?&ab_channel=YazirSolis
