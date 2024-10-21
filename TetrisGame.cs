using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace TetrisGame
{
    public class TetrisGame : GameWindow
    {
        private int _vao, _vbo;
        private Shader _shader;
        private Vector2[] vertices = new Vector2[]
        {
            new Vector2(-0.5f,-0.5f), // Vertice 1
            new Vector2(0.5f,-0.5f),  // Vertice 2
            new Vector2(0.5f,0.5f),   // Vertice 3
            new Vector2(0.5f,0.5f),   // Vertice 4
            new Vector2(-0.5f,0.5f),  // Vertice 5
            new Vector2(-0.5f,-0.5f), // Vertice 6
        };

        Vector2 posicionPiezaActual = new Vector2(3, 0);
        int[,] tablero;
        int[,] piezaActual;
        int altoTablero = 20;
        int anchoTablero = 10;

        private double tiempoDesdeUltimaCaida = 0;
        private double intervaloCaida = 0.5;

        private bool juegoEnCurso = true;

        private static readonly Random random = new Random();
        private static readonly int[][,] tetrominos = new int[][,]
        {
            new int[,]
            {   //Pieza I
                {0,0,0,0},
                {1,1,1,1},
                {0,0,0,0},
                {0,0,0,0}
            },
            new int[,]
            {   //Pieza J
                {1,0,0,0},
                {1,1,1,0},
                {0,0,0,0},
                {0,0,0,0}
            },
            new int[,]
            {   //Pieza L
                {0,0,1,0},
                {1,1,1,0},
                {0,0,0,0},
                {0,0,0,0}
            },
            new int[,]
            {   //Pieza O
                {1,1,0,0},
                {1,1,0,0},
                {0,0,0,0},
                {0,0,0,0}
            },
            new int[,]
            {   //Pieza S
                {0,1,1,0},
                {1,1,0,0},
                {0,0,0,0},
                {0,0,0,0}
            },
            new int[,]
            {   //Pieza T
                {0,1,0,0},
                {1,1,1,0},
                {0,0,0,0},
                {0,0,0,0}
            },
            new int[,]
            {   //Pieza Z
                {1,1,0,0},
                {0,1,1,0},
                {0,0,0,0},
                {0,0,0,0}
            }
        };

        private Matrix4 _projectionMatrix;

        public TetrisGame(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                Size = (width, height),
                Title = title
            })
        {
        }
        
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            _shader = new Shader("vertex_shader.glsl", "fragment_shader.glsl");

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vector2.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            _projectionMatrix = Matrix4.CreateOrthographicOffCenter(0f, 10f, 20f, 0f, -1f, 1f);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            tablero = new int[altoTablero, anchoTablero];
            GenerarNuevaPieza();
            Console.WriteLine("¡ OnLoas FIN TetrisGame !");
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            Console.WriteLine("¡ OnReaderFrame TetrisGame !");
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();
            DibujarTablero();
            DibujarPiezaActual();

            SwapBuffers();
        }

        private void DibujarTablero()
        {
            _shader.SetMatrix4("projection", _projectionMatrix);

            for (int y = 0; y < altoTablero; y++)
            {
                for (int x = 0; x < anchoTablero; x++)
                {
                    if (tablero[y, x] != 0)
                    {
                        DibujarCuadrado(new Vector2(x, y), Color4.White);
                    }
                }
            }
        }

        private void DibujarCuadrado(Vector2 position, Color4 color)
        {
            _shader.SetVector4("uColor", new Vector4(color.R, color.G, color.B, color.A));
            Matrix4 model = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0.0f));
            _shader.SetMatrix4("model", model);
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Thread.Sleep(100);
            base.OnUpdateFrame(e);
            ActualizarCaidaPieza(e.Time);

            var input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape)) { Close(); }
            if (input.IsKeyDown(Keys.Left)) { MoverPieza(-1); }
            if (input.IsKeyDown(Keys.Right)) { MoverPieza(1); }
            if (input.IsKeyDown(Keys.Space)) { RotarPieza(); }
        }


        private void RotarPieza()
        {
            Thread.Sleep(100);
            int[,] piezaRotada = new int[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    piezaRotada[j, 3 - i] = piezaActual[i, j];
                }
            }

            if (EsPosicionValida((int)posicionPiezaActual.X, (int)posicionPiezaActual.Y, piezaRotada))
            {
                piezaActual = piezaRotada;
            }
        }

        private void ActualizarCaidaPieza(double tiempoDelta)
        {
            tiempoDesdeUltimaCaida += tiempoDelta;
            if (tiempoDesdeUltimaCaida >= intervaloCaida)
            {
                tiempoDesdeUltimaCaida = 0;
                Vector2 nuevaPosicion = new Vector2(posicionPiezaActual.X, posicionPiezaActual.Y + 1);
                if (EsPosicionValida((int)nuevaPosicion.X, (int)nuevaPosicion.Y, piezaActual))
                {
                    posicionPiezaActual = nuevaPosicion;
                }
                else
                {
                    FijarPiezaEnTablero();
                    GenerarNuevaPieza();
                    VerificarLineasCompletas();
                    if (!EsPosicionValida((int)posicionPiezaActual.X, (int)posicionPiezaActual.Y, piezaActual))
                    {
                        FinDelJuego();
                    }
                }
            }
        }

        private void FijarPiezaEnTablero()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (piezaActual[i, j] != 0)
                    {
                        tablero[(int)posicionPiezaActual.Y + i, (int)posicionPiezaActual.X + j] = piezaActual[i, j];
                    }
                }
            }
        }

        private void VerificarLineasCompletas()
        {
            for (int y = 0; y < altoTablero; y++)
            {
                bool lineaCompleta = true;
                for (int x = 0; x < anchoTablero; x++)
                {
                    if (tablero[y, x] == 0)
                    {
                        lineaCompleta = false;
                        break;
                    }
                }
                if (lineaCompleta)
                {
                    EliminarLinea(y);
                }
            }
        }

        private void EliminarLinea(int fila)
        {
            for (int y = fila; y > 0; y--)
            {
                for (int x = 0; x < anchoTablero; x++)
                {
                    tablero[y, x] = tablero[y - 1, x];
                }
            }
            for (int x = 0; x < anchoTablero; x++)
            {
                tablero[0, x] = 0;
            }
        }

        private void MoverPieza(int direccion)
        {
            Vector2 nuevaPosicion = new Vector2(posicionPiezaActual.X + direccion, posicionPiezaActual.Y);
            if (EsPosicionValida((int)nuevaPosicion.X, (int)nuevaPosicion.Y, piezaActual))
            {
                posicionPiezaActual = nuevaPosicion;
            }
        }

        private bool EsPosicionValida(int nuevaX, int nuevaY, int[,] pieza)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (pieza[i, j] != 0)
                    {
                        int tableroX = nuevaX + j;
                        int tableroY = nuevaY + i;

                        if (tableroX < 0 || tableroX >= anchoTablero || tableroY >= altoTablero || tablero[tableroY, tableroX] != 0)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void GenerarNuevaPieza()
        {
            piezaActual = ObtenerPiezaAleatoria();
            posicionPiezaActual = new Vector2(3, 0);
        }

        private int[,] ObtenerPiezaAleatoria()
        {
            return tetrominos[random.Next(tetrominos.Length)];
        }

        private void DibujarPiezaActual()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (piezaActual[i, j] != 0)
                    {
                        Vector2 posicionCuadro = new Vector2(posicionPiezaActual.X + j, posicionPiezaActual.Y + i);
                        DibujarCuadrado(posicionCuadro, Color4.Blue);
                    }
                }
            }
        }

        private void FinDelJuego()
        {
            juegoEnCurso = false;
            Close();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);

            _shader.Dispose();
        }
    }
}