using System;
using Game.Domain;
using Game.Domain.Models;

class Program
{
    static void Main()
    {
        var state = new GameState
        {
            Status = GameStatus.InProgress,
            CurrentPlayer = Player.Player1
        };

        // Player1 Mouse at (2,2)
        var mouse = new Piece(Player.Player1, PieceType.Mouse);
        state.Board.SetPiece(2, 2, mouse);

        // Player2 Elephant at (2,3)
        var elephant = new Piece(Player.Player2, PieceType.Elephant);
        state.Board.SetPiece(2, 3, elephant);

        Console.WriteLine("Before move:");
        PrintBoard(state);

        var from = new Position(2, 2);
        var to = new Position(2, 3);

        var ok = GameLogic.TryMove(state, from, to, out var error);

        Console.WriteLine();
        Console.WriteLine($"Move ok: {ok}");
        Console.WriteLine($"Error: {error}");
        Console.WriteLine($"Mouse lives: {mouse.Lives}");
        Console.WriteLine($"Elephant lives: {elephant.Lives}");
        Console.WriteLine($"Current player: {state.CurrentPlayer}");
        Console.WriteLine($"Status: {state.Status}, Winner: {state.Winner}");

        Console.WriteLine();
        Console.WriteLine("After move:");
        PrintBoard(state);
    }

    static void PrintBoard(GameState state)
    {
        for (int r = 0; r < Board.Size; r++)
        {
            for (int c = 0; c < Board.Size; c++)
            {
                var piece = state.Board.GetPiece(r, c);
                char ch = '.';

                if (piece != null)
                {
                    char typeChar = piece.Type switch
                    {
                        PieceType.Elephant => 'E',
                        PieceType.Tiger => 'T',
                        PieceType.Mouse => 'M',
                        _ => '?'
                    };

                    ch = piece.Owner == Player.Player1
                        ? char.ToUpper(typeChar)
                        : char.ToLower(typeChar);
                }

                Console.Write(ch + " ");
            }

            Console.WriteLine();
        }
    }
}
