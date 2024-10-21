//using OpenTK.Mathematics;
//using OpenTK.Windowing.Desktop;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using OpenTK.Graphics.OpenGL;
//using OpenTK.Windowing.Common;
//using OpenTK.Windowing.GraphicsLibraryFramework;
//using System.Security.Cryptography.X509Certificates;
//namespace TetrisGame
//{

//    public class TetrisGame : GameWindow
//    {
//        private int _vao, _vbo;
//        private Shader _shader;
//        private Vector2[] vertices = new Vector2[]
//        {
//            //new Vector2 (0.0f, 0.0f),
//            //new Vector2 (0.0f, 1.0f),
//            //new Vector2 (1.0f, 1.0f),
//            //new Vector2 (1.0f, 0.0f)

//            new Vector2(-0.5f,-0.5f), //vertice 1
//            new Vector2(0.5f,-0.5f), //vertice 2
//            new Vector2(0.5f,0.5f), //vertice 3

//            //Segundo triangulo
//            new Vector2(0.5f,0.5f), //vertice 4
//            new Vector2(-0.5f,0.5f), //vertice 5
//            new Vector2(-0.5f,-0.5f), //vertice 6


//        };
//        Vector2 posicionPiezaActual = new Vector2(3, 0); // Posicion inicial de la pieza
//        int[,] tablero;
//        int[,] piezaActual;

//        int altoTablero = 20;
//        int anchoTablero = 10;

//        private double tiempoDesdeUltimaCaida = 0;

//        private double intervaloCaida = 5.0;

//        private bool juegoEnCurso = true;

//        private static readonly Random random = new Random();

//        private static readonly int[][,] tetrominos = new int[][,]
//        {
//            new int[,]
//            {   //Pieza I
//                {0,0,0,0},
//                {1,1,1,1},
//                {0,0,0,0},
//                {0,0,0,0}
//            },
//            new int[,]
//            {   //Pieza J
//                {1,0,0,0},
//                {1,1,1,0},
//                {0,0,0,0},
//                {0,0,0,0}
//            },
//            new int[,]
//            {   //Pieza L
//                {0,0,1,0},
//                {1,1,1,0},
//                {0,0,0,0},
//                {0,0,0,0}
//            },
//            new int[,]
//            {   //Pieza O
//                {1,1,0,0},
//                {1,1,0,0},
//                {0,0,0,0},
//                {0,0,0,0}
//            },
//            new int[,]
//            {   //Pieza S
//                {0,1,1,0},
//                {1,1,0,0},
//                {0,0,0,0},
//                {0,0,0,0}
//            },
//            new int[,]
//            {   //Pieza T
//                {0,1,0,0},
//                {1,1,1,0},
//                {0,0,0,0},
//                {0,0,0,0}
//            },
//            new int[,]
//            {   //Pieza Z
//                {1,1,0,0},
//                {0,1,1,0},
//                {0,0,0,0},
//                {0,0,0,0}
//            }



//        };

//        private Matrix4 _projectionMatrix;

//        public TetrisGame(int width, int height, string title)
//            : base(GameWindowSettings.Default, new NativeWindowSettings()
//            { Size = (width, height), Title = title })
//        {

//            Console.WriteLine("¡ Constructor TetrisGame !");

//        }

//        protected override void OnLoad()
//        {
//            base.OnLoad();
//            Console.WriteLine("¡ OnLoad TetrisGame !");
//            // Color de fondo
//            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
//            // Inicializacion de Shader
//            _shader = new Shader("vertex_shader.glsl", "fragment_shader.glsl");
//            // Configuracion VAO y VBO
//            _vao = GL.GenVertexArray();
//            _vbo = GL.GenBuffer();
//            GL.BindVertexArray(_vao);
//            // Cargar datos de vertices
//            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
//            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vector2.SizeInBytes,
//                vertices, BufferUsageHint.StaticDraw);
//            // Espesificamos el layout del buffer de vertices
//            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
//            GL.EnableVertexAttribArray(0);
//            // Proyeccion ortogonal (para ajustar el tablero en la ventana)
//            _projectionMatrix = Matrix4.CreateOrthographicOffCenter(0f, 10f, 20f, 0f, -1f, 1f);
//            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
//            GL.BindVertexArray(0);

//            //Inicializar tablero

//            tablero = new int[altoTablero, anchoTablero];
//            for (int fila = 0; fila < 20; fila++)
//            {
//                for (int columna = 0; columna < 10; columna++)
//                {
//                    tablero[fila, columna] = 0;
//                }
//            }
//            Console.WriteLine("¡ OnLoad Fin TetrisGame !");
//        }

//        protected override void OnRenderFrame(FrameEventArgs e)
//        {
//            base.OnRenderFrame(e);
//            Console.WriteLine("¡ OnRenderFrame TetrisGame !");
//            GL.Clear(ClearBufferMask.ColorBufferBit);

//            _shader.Use();

//            // Dibujar el tablero y las piezas
//            DibujarTablero();
//            GenerarNuevaPieza();
//            DibujarPiezaActual();

//            SwapBuffers();
//            // DibujarPiezaActual();
//        }

//        private void DibujarTablero()
//        {
//            _shader.SetMatrix4("projection", _projectionMatrix);
//            Console.WriteLine("¡ Dibujando Tablero TetrisGame !");
//            for (int y = 0; y < altoTablero; y++)
//            {
//                for (int x = 0; x < anchoTablero; x++)
//                {
//                    if (!(tablero[x, y] == 0))
//                        DibujarCuadrado(new Vector2(x, y), Color4.White);
//                }
//            }
//        }

//        private void DibujarCuadrado(Vector2 position, Color4 color)
//        {

//            _shader.SetVector4("uColor", new Vector4(color.R, color.G, color.B, color.A));
//            Matrix4 model = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0.0f));
//            _shader.SetMatrix4("model", model);
//            GL.BindVertexArray(_vao);
//            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

//            Console.WriteLine("¡ 6 Dibujando Cuadrado TetrisGame !" + GL.GetError().ToString());


//        }

//        protected override void OnUnload()
//        {
//            base.OnUnload();
//            Console.WriteLine("¡ OnUnload TetrisGame !");
//            // Eliminar VBO, VAO y shaders
//            GL.DeleteBuffer(_vbo);
//            GL.DeleteVertexArray(_vao);
//            _shader.Dispose();
//        }

//        protected override void OnUpdateFrame(FrameEventArgs e)
//        {
//            base.OnUpdateFrame(e);
//            Console.WriteLine("¡ onUpdateFrame TetrisGame !");

//            ActualizarCaidaPieza(e.Time);

//            var input = KeyboardState;

//            if (input.IsKeyDown(Keys.Escape))
//            {
//                Close();
//            }

//            if (KeyboardState.IsKeyDown(Keys.Left))
//            {
//                MoverPieza(-1);
//            }

//            if (KeyboardState.IsKeyDown(Keys.Right))
//            {
//                MoverPieza(1);
//            }

//            if (KeyboardState.IsKeyDown(Keys.Down))
//            {
//                //CaerPiezaRapido();
//            }

//            if (KeyboardState.IsKeyDown(Keys.Space))
//            {
//                RotarPieza();
//            }

//        }

//        private void RotarPieza()
//        {
//            int[,] piezaRotada = new int[4, 4];

//            for (int i = 0; i < 4; i++)
//            {

//                for (int j = 0; j < 4; j++)
//                {
//                    piezaRotada[j, 3 - i] = piezaActual[i, j]; // Rotacion de 90 grados
//                }
//            }
//        }

//        bool HayColision(int[,] tablero, int[,] pieza, Vector2 posicion)
//        {

//            for (int i = 0; i < 4; i++)
//            {
//                for (int j = 0; j < 4; j++)
//                {
//                    if (pieza[i, j] != 0)
//                    {
//                        int x = (int)posicion.X + j;
//                        int y = (int)posicion.Y + i;

//                        // Verificar si esta fuera de los limietes del tablero
//                        if (x < 0 || x >= tablero.GetLength(1) || y >= tablero.GetLength(0))
//                        {
//                            return true;
//                        }

//                        // Verificar colision con piesas ya colocadas en el tablero
//                        if (tablero[y, x] != 0)
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }

//            return false;
//        }

//        void FijarPieza(int[,] tablero, int[,] pieza, Vector2 posicion)
//        {
//            for (int i = 0; i < 4; i++)
//            {
//                for (int j = 0; j < 4; j++)
//                {
//                    int x = (int)posicion.X + j;
//                    int y = (int)posicion.Y + i;
//                    tablero[y, x] = pieza[i, j]; //Fijar la pieza en el tablero
//                }
//            }
//        }

//        //void EliminarLineasCompletadas(int[,] tablero)
//        //{
//        //    int anchoTablero = tablero.GetLength(1);
//        //    int altoTablero = tablero.GetLength(0);

//        //    for(int y = altoTablero - 1; y >= 0; y--)
//        //    {
//        //        bool lineaCompletada = true;

//        //        for (int x = 0;x < anchoTablero; x++)
//        //        {
//        //            if (tablero[x, y] == 0)
//        //            {
//        //                lineaCompletada = false;
//        //                break;
//        //            }
//        //        }

//        //        // si la linea esta completa, eliminarla y bajar las lineas superiores
//        //        if(lineaCompletada)
//        //        {
//        //            EliminarLinea(tablero, y);
//        //            y++; // Repetir la misma fila ya que ha sido desplazada
//        //        }
//        //    }
//        //}

//        private void EliminarLinea(int fila)
//        {
//            // Mover todas las filas por encima hacia abajo
//            for (int i = fila; i > 0; i--)
//            {
//                for (int j = 0; j < anchoTablero; j++)
//                {
//                    tablero[i, j] = tablero[i - 1, j];
//                }
//            }

//            //Vaciar la fila superior
//            for (int j = 0; j < anchoTablero; j++)
//            {
//                tablero[0, j] = 0;
//            }
//        }

//        private void FinDelJuego()
//        {
//            //Detener el bucle del juego o reiniciar el juego
//            Close();
//        }

//        private void MoverPieza(int direccion)
//        {
//            Vector2 nuevaPosicion = new Vector2(posicionPiezaActual.X + direccion, posicionPiezaActual.Y);

//            if (EsPosicionValida((int)nuevaPosicion.X, (int)nuevaPosicion.Y, piezaActual))
//            {
//                posicionPiezaActual = nuevaPosicion;
//            }
//        }

//        private void ActualizarCaidaPieza(double tiempoDelta)
//        {
//            tiempoDesdeUltimaCaida += tiempoDelta;
//            if (tiempoDesdeUltimaCaida >= tiempoDelta)
//            {
//                tiempoDesdeUltimaCaida = 0;
//                Vector2 nuevaPosicion = new Vector2(posicionPiezaActual.X, posicionPiezaActual.Y + 1);
//                //Verificar si la posicion es valida
//                if (EsPosicionValida((int)nuevaPosicion.X, (int)nuevaPosicion.Y, piezaActual))
//                {
//                    posicionPiezaActual = nuevaPosicion;
//                }
//                else
//                {
//                    //Si no es valida, la pieza ha llegado al fondo o ha colisionado con otra pieza
//                    FijarPiezaEnTablero();
//                    GenerarNuevaPieza();
//                    if (!EsPosicionValida((int)posicionPiezaActual.X, (int)posicionPiezaActual.Y, piezaActual))
//                    {
//                        //juego Terminado
//                        FinDelJuego();
//                    }
//                }
//            }
//        }

//        private void FijarPiezaEnTablero()
//        {
//            for (int i = 0; i < 4; i++)
//            {

//                for (int j = 0; j < 4; j++)
//                {
//                    if (piezaActual[i, j] != 0)
//                    {
//                        tablero[(int)posicionPiezaActual.Y + i, (int)posicionPiezaActual.X + j] = piezaActual[i, j];
//                    }
//                }
//            }
//        }

//        public int[,] ObtenerPiezaAleatoria()
//        {
//            //Seleccionar aleatoriamente una de las matrices tetrominos
//            int indiceAleatorio = random.Next(tetrominos.Length);
//            return tetrominos[indiceAleatorio];
//        }


//        private void GenerarNuevaPieza()
//        {
//            piezaActual = ObtenerPiezaAleatoria();
//            posicionPiezaActual = new Vector2(3, 0); //Comienza en la parte superior del tablero

//            if (!EsPosicionValida((int)posicionPiezaActual.X, (int)posicionPiezaActual.Y, piezaActual))
//            {
//                //Si no es valida la nueva pieza, el juego ha terminado.
//                FinDelJuego();
//            }
//        }

//        private void VerificarLineasCompletas()
//        {
//            for (int i = 0; i < altoTablero; i++)
//            {
//                bool lineaCompleta = true;

//                for (int j = 0; j < anchoTablero; j++)
//                {
//                    if (tablero[i, j] == 0)
//                    {
//                        lineaCompleta = false;
//                        break;
//                    }
//                }

//                if (lineaCompleta)
//                {
//                    EliminarLinea(i);
//                }

//            }
//        }
//        private bool EsPosicionValida(int x, int y, int[,] pieza)
//        {
//            for (int i = 0; i < 4; i++)
//            {
//                for (int j = 0; j < 4; j++)
//                {
//                    if (pieza[i, j] != 0)
//                    {
//                        //Verificar limites del tablero
//                        if (x + j < 0 || x + j >= anchoTablero || y + i >= altoTablero)
//                        {
//                            return false; //Colision con el borde del tablero
//                        }
//                        //Verificar colision con piezas fijas
//                        if (y + i >= 0 && tablero[y + i, x + j] != 0)
//                        {
//                            return false; //Colision con otra pieza
//                        }
//                    }
//                }
//            }
//            return true;
//        }

//        private void DibujarPiezaActual()
//        {
//            // Recorrer la matriz 4x4 de la pieza actual
//            for (int i = 0; i < 4; i++)
//            {
//                for (int j = 0; j < 4; j++)
//                {

//                    // Si la celda en la matriz de la pieza actual es diferente de 0, significa que es parte de la pieza
//                    if (piezaActual[i, j] != 0)
//                    {

//                        // Calcular la posicion en el tablero en base a la posicion actual de la pieza y el indice de la celda
//                        int x = (int)posicionPiezaActual.X + j;
//                        int y = (int)posicionPiezaActual.Y + i;

//                        // Llamer al metodo DibujarCuadrado para dibujar la celda de la pieza
//                        Vector2 dibujar = new Vector2(x, y);
//                        DibujarCuadrado(dibujar, Color4.Blue);
//                    }
//                }
//            }
//        }
//    }
//}