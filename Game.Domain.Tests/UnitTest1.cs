using Game.Domain;
using Game.Domain.Models;
using Xunit;

namespace Game.Domain.Tests
{
    public class GameLogicTests
    {
        [Fact]
        public void MouseBeatsElephant_PredicateIsTrue()
        {
            var result = GameLogic.AttackerBeatsDefender(PieceType.Mouse, PieceType.Elephant);

            Assert.True(result);
        }

        [Fact]
        public void IsAdjacent_OneStepDiagonal_IsTrue()
        {
            var from = new Position(3, 3);
            var to = new Position(4, 4);

            var adjacent = GameLogic.IsAdjacent(from, to);

            Assert.True(adjacent);
        }

        [Fact]
        public void IsAdjacent_TwoStepsAway_IsFalse()
        {
            var from = new Position(0, 0);
            var to = new Position(0, 2);

            var adjacent = GameLogic.IsAdjacent(from, to);

            Assert.False(adjacent);
        }
        [Fact]
        public void ElephantKillsLowLifeTiger_AndMovesIntoTargetSquare()
        {
            var state = new GameState
            {
                Status = GameStatus.InProgress,
                CurrentPlayer = Player.Player1
            };

            // Player1 Elephant at (3,3)
            var elephant = new Piece(Player.Player1, PieceType.Elephant);
            state.Board.SetPiece(3, 3, elephant);

            // Player2 Tiger at (3,4) with only 1 life left
            var tiger = new Piece(Player.Player2, PieceType.Tiger);
            tiger.TakeHit(); // 3 -> 2
            tiger.TakeHit(); // 2 -> 1
            state.Board.SetPiece(3, 4, tiger);

            var from = new Position(3, 3);
            var to = new Position(3, 4);

            var ok = GameLogic.TryMove(state, from, to, out var error);

            Assert.True(ok, error);

            // Tiger should be dead
            Assert.True(tiger.IsDead);

            // From is now empty
            Assert.Null(state.Board.GetPiece(3, 3));

            // Elephant moved into target square
            var pieceAtTo = state.Board.GetPiece(3, 4);
            Assert.NotNull(pieceAtTo);
            Assert.Equal(Player.Player1, pieceAtTo.Owner);
            Assert.Equal(PieceType.Elephant, pieceAtTo.Type);
            Assert.Equal(Piece.MaxLives, pieceAtTo.Lives);

            // Turn should switch to Player2
            Assert.Equal(Player.Player2, state.CurrentPlayer);
        }

        [Fact]
        public void TryMove_Fails_WhenMovingTooFar()
        {
            var state = new GameState
            {
                Status = GameStatus.InProgress,
                CurrentPlayer = Player.Player1
            };

            var mouse = new Piece(Player.Player1, PieceType.Mouse);
            state.Board.SetPiece(0, 0, mouse);

            var from = new Position(0, 0);
            var to = new Position(0, 2);

            var ok = GameLogic.TryMove(state, from, to, out var error);

            Assert.False(ok);
            Assert.Equal("Pieces can move only one step in any direction", error);
        }
        [Fact]
        public void TigerHitsMouse_MouseLosesOneLife_DefenderStaysIfAlive()
        {
            var state = new GameState
            {
                Status = GameStatus.InProgress,
                CurrentPlayer = Player.Player1
            };

            // Player1 Tiger at (1,1)
            var tiger = new Piece(Player.Player1, PieceType.Tiger);
            state.Board.SetPiece(1, 1, tiger);

            // Player2 Mouse at (1,2) full life
            var mouse = new Piece(Player.Player2, PieceType.Mouse);
            state.Board.SetPiece(1, 2, mouse);

            var from = new Position(1, 1);
            var to = new Position(1, 2);

            var ok = GameLogic.TryMove(state, from, to, out var error);

            Assert.True(ok, error);

            // Tiger beats Mouse, so Mouse loses one life
            Assert.Equal(Piece.MaxLives - 1, mouse.Lives);
            Assert.Equal(Piece.MaxLives, tiger.Lives);

            // In our current rules, attacker only moves in if defender dies
            // Mouse is still alive, so Tiger stays at (1,1), Mouse stays at (1,2)
            var pieceAtFrom = state.Board.GetPiece(1, 1);
            var pieceAtTo = state.Board.GetPiece(1, 2);

            Assert.NotNull(pieceAtFrom);
            Assert.NotNull(pieceAtTo);

            Assert.Equal(Player.Player1, pieceAtFrom.Owner);
            Assert.Equal(PieceType.Tiger, pieceAtFrom.Type);

            Assert.Equal(Player.Player2, pieceAtTo.Owner);
            Assert.Equal(PieceType.Mouse, pieceAtTo.Type);

            // Turn should switch to Player2
            Assert.Equal(Player.Player2, state.CurrentPlayer);
        }
        [Fact]
        public void Player1Wins_WhenCarrierBringsEnemyFlagToHomeMiddle()
        {
            var state = new GameState
            {
                Status = GameStatus.InProgress,
                CurrentPlayer = Player.Player1
            };

            // Place Player1 piece that already carries Player2 flag
            // Put it just above Player1FlagHome so one move reaches goal
            var carrier = new Piece(Player.Player1, PieceType.Tiger)
            {
                HasEnemyFlag = true
            };

            var goal = state.Player1FlagHome;        // usually (6,3)
            var start = new Position(goal.Row - 1, goal.Col);  // (5,3)

            state.Board.SetPiece(start.Row, start.Col, carrier);

            // Since carrier holds Player2 flag, that flag is not on the board
            state.IsPlayer2FlagOnBoard = false;

            var from = start;
            var to = goal; // moving into home middle

            var ok = GameLogic.TryMove(state, from, to, out var error);

            Assert.True(ok, error);

            // Game should be finished and Player1 should be winner
            Assert.Equal(GameStatus.Finished, state.Status);
            Assert.Equal(Player.Player1, state.Winner);

            // Carrier should now be on the goal square
            var pieceAtGoal = state.Board.GetPiece(goal.Row, goal.Col);
            Assert.NotNull(pieceAtGoal);
            Assert.Equal(Player.Player1, pieceAtGoal.Owner);
            Assert.Equal(PieceType.Tiger, pieceAtGoal.Type);
        }


    }
}
