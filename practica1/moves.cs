using System;
using Microsoft.Kinect;
/*using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;*/

namespace Microsoft.Samples.Kinect.SkeletonBasics{
    
    // el margenErrorSup es el margen de error de la aplicacion al comparar la postura del usuario superiormente
    // el margenErrorInf es el margen de error de la aplicacion al comparar la postura del usuario inferiormente
    public class Moves{
        private float margenErrorSup, margenErrorInf;
        private posturas posturaActual;

        //enum de las distintas posturas que realizara el usuario
        public enum posturas{
            Mal,            //la postura es erronea
            Inicial,        //el usuario esta en la postura inicial
            Brazos_En_Cruz, //el usuario tiene los brazos levantados a la altura de los hombros
            Brazos_Arriba  //el usuario tiene los brazos estirados y por encima de la cabeza
        };

        //comenzamos en la fase inicial, reposo
        //el margen de error tanto superior como inferior es del 10%
        public Moves(float sup = 0.1f, float inf = 0.1f){
            margenErrorSup = sup;
            margenErrorInf = inf;
            posturaActual = posturas.Mal;
        }

        //nos devuelve la fase en la que se encuentra el usuario
        public int getPostura(){
            return posturaActual;
        }

        //metodo con el que actualizamos la postura del usuario
        public void actualizarPostura(posturas nuevaPostura){
            posturaActual = nuevaPostura;
        }

        //metodo con el que comparamos si dos partes del cuerpo estan a la misma altura
        public bool compararCoordenadas(Joint a, Joint b, char coord){
            bool respuesta = false;

            switch (coord) {
                case X:
                    if (a.Position.X < (b.Position.X * margenErrorSup) && (a.Position.X > (b.Position.X * margenErrorInf)))
                        respuesta = true;
                        break;
                case Y:
                    if (a.Position.Y < (b.Position.Y * margenErrorSup) && (a.Position.Y > (b.Position.Y * margenErrorInf)))
                        respuesta = true;
                    break;
                case Z:
                    if (a.Position.Z < (b.Position.Z * margenErrorSup) && (a.Position.Z > (b.Position.Z * margenErrorInf)))
                        respuesta = true;
                    break;
            }

            return respuesta;
        }
        
        //comprobamos que el usuario esta en el estado de reposo (con las manos a los lados del cuerpo y hacia abajo)
        private bool estadoReposo(Skeleton esqueleto){
            //cogemos los puntos de referencia que vamos a usar a la hora de ver la postura del usuario
            Joint hombroI = esqueleto.Joints[JointType.ShoulderLeft];
            Joint codoI = esqueleto.Joints[JointType.ElbowLeft];
            Joint manoI = esqueleto.Joints[JointType.WristLeft];
            Joint hombroD = esqueleto.Joints[JointType.ShoulderRight];
            Joint codoD = esqueleto.Joints[JointType.ElbowRight];
            Joint manoD = esqueleto.Joints[JointType.WristRight];

            //comprobamos que los hombros estan correctamente alineados
            if (compararCoordenadas(hombroI, hombroD, "Y")){
                //comprobamos que los codos estan alineados con los hombros en el eje x
                if (compararCoordenadas(codoI, hombroI, "X")){
                    //vemos que esta correcto el otro codo
                    if (compararCoordenadas(codoD, hombroD, "X")){
                        //ahora comprobamos las manos
                        if (compararCoordenadas(manoI, codoI, "X")){
                            //comprobamos la ultima mano finalmente
                            if (compararCoordenadas(manoD, codoD, "X")){
                                actualizarPostura(posturas.Inicial);//indicamos que el usuario esta en la postura correcta de inicio
                                return true;
                            }
                            else{
                                return false;
                            }
                        }
                        else{
                            return false;
                        }
                    }
                    else{
                        return false;
                    }
                }
                else{
                    return false;
                }
            }
            else{
                return false;
            }
        }

        //vamos comprobando que el usuario va realizando el movimiento correctamente, este consiste en levantar y bajar los brazos
        public void movimiento(SkeletonBasics esqueleto) {
            Joint hombroI = esqueleto.Joints[JointType.ShoulderLeft];
            Joint codoI = esqueleto.Joints[JointType.ElbowLeft];
            Joint manoI = esqueleto.Joints[JointType.WristLeft];
            Joint hombroD = esqueleto.Joints[JointType.ShoulderRight];
            Joint codoD = esqueleto.Joints[JointType.ElbowRight];
            Joint manoD = esqueleto.Joints[JointType.WristRight];
            //añadimos dos variables nuevas con las posiciones nuevas de las manos para ver si se mueven
            Joint newManoI = manoI;
            Joint newManoD = manoD;
            //y con las posiciones nuevas de los codos para asegurarnos que se realiza correctamente el movimiento
            Join newCodoI = codoI;
            Join newCodoD = codoD;
            //inicializamos la variable postura, en caso de que el usuario este en la inicial comenzaremos el movimiento
            postura pasoPostura = getPostura();

            //caso en el que postura=Inicial, podemos comenzar el movimiento
            switch(pasoPostura) {
                //vamos subiendo los brazos poco a poco
                case posturas.Inicial:
                    newManoI = esqueleto.Joints[JointType.WristLeft];
                    newManoD = esqueleto.Joints[JointType.WristRight];
                    
                    //comprobamos si el usuario tiene los brazos en cruz
                    if (compararCoordenadas(manoI, codoI, "Y") && compararCoordenadas(codoI, hombroI, "Y")
                        && compararCoordenadas(manoD, codoD, "Y") && compararCoordenadas(codoD, hombroD, "Y"))
                        actualizarPostura(posturas.Brazos_En_Cruz);
                    else {
                        //si no estamos en la segunda parte del movimiento, comprobamos que este en el transito del primero al segundo
                        if (compararCoordenadas(newManoD, manoD, "X") && compararCoordenadas(newManoI, manoI, "X")){
                            //como esta realizando el movimiento ascendente, actualizamos los valores de las manos
                            manoD = newManoD;
                            manoI = newManoI;
                        }//si no se ha hecho bien el movimiento
                        else {
                            actualizarPostura(posturas.Mal);
                        }
                    }
                    break;
                
                //comprobamos si el usuario tiene los brazos en cruz y si es asi procederemos a ver si los sube
                case posturas.Brazos_En_Cruz:
                    //volvemos a ver donde estan las manos del usuario
                    newManoI = esqueleto.Joints[JointType.WristLeft];
                    newManoD = esqueleto.Joints[JointType.WristRight];
                    newCodoI = esqueleto.Joints[JointType.ElbowLeft];
                    newCodoD = esqueleto.Joints[JointType.ElbowRight];
                    
                    //comprobamos si el usuario tiene los brazos subidos
                    if (compararCoordenadas(newCodoD, hombroD, "X") && (compararCoordenadas(newManoD, hombroD,"X")) &&
                        compararCoordenadas(newCodoI, hombroI, "X") && (compararCoordenadas(newManoI, hombroI, "X")) ) {
                        actualizarPostura(posturas.Brazos_Arriba);
                    }
                    else {
                        //si no estamos en la tercera parte del movimiento, comprobamos que se van subiendo los brazos
                        if(compararCoordenadas(newManoD, manoD, "Y") && (compararCoordenadas(newCodoD, codoD, "Y")) &&
                            compararCoordenadas(newManoI, manoI, "Y") && (compararCoordenadas(newCodoI, codoI, "Y") ) {
                            manoD = newManoD;
                            manoI = newManoI;
                            codoD = newCodoD;
                            codoI = newCodoI;
                        }
                        else {
                            actualizarPostura(posturas.Mal);
                        }
                    }
                break;
              /*  default:
                    actualizarPostura(posturas.Mal);*/
            }//end switch
        }//end method
    }//end clasee
}//end namespace
