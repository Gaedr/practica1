using Microsoft.Kinect;
using System;
using System.Windows;

namespace Microsoft.Samples.Kinect.SkeletonBasics {

    // el margenErrorSup es el margen de error de la aplicacion al comparar la postura del usuario superiormente
    // el margenErrorInf es el margen de error de la aplicacion al comparar la postura del usuario inferiormente
    public class Moves {
        private float margenErrorSup, margenErrorInf;
        private posturas posturaActual;
        private Joint manoIzqIni, manoDerIni;
        private Joint manoIzqAct, manoDerAct, codoIzqAct, codoDerAct;

        //enum de las distintas posturas que realizara el usuario
        public enum posturas {
            Mal,            //la postura es erronea
            Inicial,        //el usuario esta en la postura inicial
            Brazos_En_Cruz, //el usuario tiene los brazos levantados a la altura de los hombros
            Brazos_Arriba   //el usuario tiene los brazos estirados y por encima de la cabeza
        };

        //comenzamos en la fase inicial, reposo
        //el margen de error tanto superior como inferior es del 10%
        public Moves(float sup = 1.4f, float inf = 0.6f) {
            margenErrorSup = sup;
            margenErrorInf = inf;
            posturaActual = posturas.Mal;
        }

        //obtenemos la posicion inicial de la mano derecha 
        public Point getManoDerInicial() {
            return new Point(manoDerIni.Position.X, manoDerIni.Position.Y);
        }

        //obtenemos la que será la posicion final de la mano derecha
        public Point getManoDerFinal(Skeleton esqueleto) {
            double hombroY = esqueleto.Joints[JointType.ShoulderRight].Position.Y;
            return new Point(manoDerIni.Position.X, hombroY + (hombroY - manoDerIni.Position.Y));
        }

        //obtenemos la posicion inicial de la mano izquierda 
        public Point getManoIzqInicial() {
            return new Point(manoIzqIni.Position.X, manoIzqIni.Position.Y);
        }

        //obtenemos la que sera la posicion final de la mano izquierda
        public Point getManoIzqFinal(Skeleton esqueleto) {
            double hombroY = esqueleto.Joints[JointType.ShoulderLeft].Position.Y;
            return new Point(manoIzqIni.Position.X, hombroY + (hombroY - manoIzqIni.Position.Y));
        }

        //nos devuelve la fase en la que se encuentra el usuario
        public posturas getPostura() {
            return posturaActual;
        }

        //metodo con el que actualizamos la postura del usuario
        public void actualizarPostura(posturas nuevaPostura, Skeleton esqueleto) {
            posturaActual = nuevaPostura;
            manoDerAct = esqueleto.Joints[JointType.WristRight];
            manoIzqAct = esqueleto.Joints[JointType.WristLeft];
            codoDerAct = esqueleto.Joints[JointType.ElbowRight];
            codoIzqAct = esqueleto.Joints[JointType.ElbowLeft];
            if(nuevaPostura == posturas.Inicial) {
                manoIzqIni = esqueleto.Joints[JointType.WristLeft];
                manoDerIni = esqueleto.Joints[JointType.WristRight];
            }
        }

        public void actualizarPostura(Skeleton esqueleto) {
            manoDerAct = esqueleto.Joints[JointType.WristRight];
            manoIzqAct = esqueleto.Joints[JointType.WristLeft];
            codoDerAct = esqueleto.Joints[JointType.ElbowRight];
            codoIzqAct = esqueleto.Joints[JointType.ElbowLeft];
        }

        //metodo con el que comparamos si dos partes del cuerpo estan alineados en el mismo eje
        public bool compararCoordenadas(Joint a, Joint b, char coord) {
            bool respuesta = false;

            switch (coord) {
                case 'X':
                    if (Math.Abs(a.Position.X) < (Math.Abs(b.Position.X) * margenErrorSup) && (Math.Abs(a.Position.X) > (Math.Abs(b.Position.X) * margenErrorInf)))
                        respuesta = true;
                    break;
                case 'Y':
                    if (Math.Abs(a.Position.Y) < (Math.Abs(b.Position.Y) * margenErrorSup) && (Math.Abs(a.Position.Y) > (Math.Abs(b.Position.Y) * margenErrorInf)))
                    respuesta = true;
                    break;
                case 'Z':
                    if (Math.Abs(a.Position.Z) < (Math.Abs(b.Position.Z) * margenErrorSup) && (Math.Abs(a.Position.Z) > (Math.Abs(b.Position.Z) * margenErrorInf)))
                    respuesta = true;
                    break;
            }

            return respuesta;
        }

        //comprobamos que el usuario esta en el estado de reposo (con las manos a los lados del cuerpo y hacia abajo)
        private void estadoReposo(Skeleton esqueleto) {
            //cogemos los puntos de referencia que vamos a usar a la hora de ver la postura del usuario
            Joint hombroI = esqueleto.Joints[JointType.ShoulderLeft];
            Joint codoI = esqueleto.Joints[JointType.ElbowLeft];
            Joint manoI = esqueleto.Joints[JointType.WristLeft];
            Joint hombroD = esqueleto.Joints[JointType.ShoulderRight];
            Joint codoD = esqueleto.Joints[JointType.ElbowRight];
            Joint manoD = esqueleto.Joints[JointType.WristRight];

            //comprobamos que los hombros estan correctamente alineados
            if (compararCoordenadas(hombroI, hombroD, 'Y')) {
                //comprobamos que los codos estan alineados con los hombros en el eje x
                if (compararCoordenadas(codoI, hombroI, 'X')) {
                    //vemos que esta correcto el otro codo
                    if (compararCoordenadas(codoD, hombroD, 'X')) {
                        //ahora comprobamos las manos
                        if (compararCoordenadas(manoI, codoI, 'X')) {
                            //comprobamos la ultima mano finalmente
                            if (compararCoordenadas(manoD, codoD, 'X')) {
                                actualizarPostura(posturas.Inicial, esqueleto);//indicamos que el usuario esta en la postura correcta de inicio
                            }
                            else {
                                actualizarPostura(posturas.Mal, esqueleto);
                            }
                        }
                        else {
                            actualizarPostura(posturas.Mal, esqueleto);
                        }
                    }
                    else {
                        actualizarPostura(posturas.Mal, esqueleto);
                    }
                }
                else {
                    actualizarPostura(posturas.Mal, esqueleto);
                }
            }
            else {
                actualizarPostura(posturas.Mal,esqueleto);
            }
        }

        //vamos comprobando que el usuario va realizando el movimiento correctamente, este consiste en levantar y bajar los brazos
        public void movimiento(Skeleton esqueleto) {
            Joint hombroI = esqueleto.Joints[JointType.ShoulderLeft];
            Joint codoI = esqueleto.Joints[JointType.ElbowLeft];
            Joint manoI = esqueleto.Joints[JointType.WristLeft];
            Joint hombroD = esqueleto.Joints[JointType.ShoulderRight];
            Joint codoD = esqueleto.Joints[JointType.ElbowRight];
            Joint manoD = esqueleto.Joints[JointType.WristRight];
            //Comprobamos la postura, en caso de que el usuario este en la inicial comenzaremos el movimiento
            //caso en el que postura=Inicial, podemos comenzar el movimiento
            switch (getPostura()) {
                //vamos subiendo los brazos poco a poco
                case posturas.Inicial:
                    //comprobamos si el usuario tiene los brazos en cruz
                    if (compararCoordenadas(manoI, codoI, 'Y') && compararCoordenadas(codoI, hombroI, 'Y')
                        && compararCoordenadas(manoD, codoD, 'Y') && compararCoordenadas(codoD, hombroD, 'Y')) {
                        actualizarPostura(posturas.Brazos_En_Cruz, esqueleto);
                    } //si no estamos en la segunda parte del movimiento, comprobamos que este en el transito del primero al segundo
                    else if ((codoDerAct.Position.X <= codoD.Position.X) && (manoDerAct.Position.X <= manoD.Position.X) &&
                            (codoIzqAct.Position.X >= codoI.Position.X) && (manoIzqAct.Position.X >= manoI.Position.X)) {
                        //como esta realizando el movimiento ascendente, actualizamos los valores de las manos
                        actualizarPostura(esqueleto);
                    }//si no se ha hecho bien el movimiento
                    else { actualizarPostura(posturas.Mal,esqueleto); }
                    break;

                //comprobamos si el usuario tiene los brazos en cruz y si es asi procederemos a ver si los sube
                case posturas.Brazos_En_Cruz:
                    //comprobamos si el usuario tiene los brazos subidos
                    if (compararCoordenadas(codoDerAct, hombroD, 'X') && (compararCoordenadas(manoDerAct, hombroD, 'X')) &&
                        compararCoordenadas(codoIzqAct, hombroI, 'X') && (compararCoordenadas(manoIzqAct, hombroI, 'X'))) {
                        actualizarPostura(posturas.Brazos_Arriba,esqueleto);
                    }//si no estamos en la tercera parte del movimiento, comprobamos que se van subiendo los brazos
                    else if (compararCoordenadas(manoDerAct, manoD, 'Y') && (compararCoordenadas(codoDerAct, codoD, 'Y')) &&
                            compararCoordenadas(manoIzqAct, manoI, 'Y') && (compararCoordenadas(codoIzqAct, codoI, 'Y'))) {
                        actualizarPostura(esqueleto);
                    }//si no se ha hecho bien el movimiento
                    else { actualizarPostura(posturas.Mal, esqueleto); }
                    break;

                case posturas.Mal:
                    estadoReposo(esqueleto);
                    break;
            }//end switch
        }//end method
    }//end clase
}//end namespace
