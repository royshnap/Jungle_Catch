using System;
using Game.Domain;
using Game.Domain.Models;

class Program
{
    static void Main()
    {
        var state = new GameState
        {
            Status = GameStatus.Placement,
            CurrentPlayer = Player.Player1
        };

        // Player1 places an Elephant at bottom row
        var pos1 = new Position(Board.Size - 1, 0); // row 6, col 0
        var ok1 = GameLogic.TryPlacePiece(state, pos1, PieceType.Elephant, out var err1);

        Console.WriteLine($"P1 place Elephant ok: {ok1}, error: {err1}");
        Console.WriteLine($"Current player is now: {state.CurrentPlayer}");

        // Player2 places a Tiger at top row
        var pos2 = new Position(0, 0); // row 0, col 0
        var ok2 = GameLogic.TryPlacePiece(state, pos2, PieceType.Tiger, out var err2);

        Console.WriteLine($"P2 place Tiger ok: {ok2}, error: {err2}");
        Console.WriteLine($"Current player is now: {state.CurrentPlayer}");

        Console.WriteLine();
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
