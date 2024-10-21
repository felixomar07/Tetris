namespace TetrisGame
{
     class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("¡ Inicia el juego Tetris !");
            using (TetrisGame game = new TetrisGame(800, 600, "Tetris 2D"))
            {
                game.Run();
            }
        }
    }
}
