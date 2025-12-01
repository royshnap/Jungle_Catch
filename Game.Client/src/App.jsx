import { useState } from "react";
import { createGame } from "./api";

function cellDisplay(cell) {
  if (!cell) return "·";

  // first letter of the piece type, E, T, M
  const letter = cell.type[0];

  // Player1 uppercase, Player2 lowercase
  return cell.owner === "Player1" ? letter.toUpperCase() : letter.toLowerCase();
}

function App() {
  const [game, setGame] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  async function handleCreateGame() {
    try {
      setLoading(true);
      setError("");
      const data = await createGame();
      setGame(data);
    } catch (err) {
      console.error(err);
      setError(err.message || "Unknown error");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div
      style={{
        minHeight: "100vh",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "flex-start",
        gap: "1rem",
        padding: "1.5rem",
        fontFamily: "system-ui, sans-serif",
        background: "#0f172a",
        color: "white",
      }}
    >
      <h1>Roy s Flag Game</h1>

      <button
        onClick={handleCreateGame}
        disabled={loading}
        style={{
          padding: "0.5rem 1rem",
          borderRadius: "0.5rem",
          border: "none",
          fontSize: "1rem",
          cursor: "pointer",
          background: "#22c55e",
        }}
      >
        {loading ? "Creating..." : "Create new game"}
      </button>

      {error && (
        <div
          style={{
            background: "#b91c1c",
            padding: "0.5rem 1rem",
            borderRadius: "0.5rem",
            marginTop: "0.5rem",
          }}
        >
          {error}
        </div>
      )}

      {game && (
        <div style={{ marginTop: "1rem", width: "fit-content" }}>
          <div style={{ marginBottom: "0.25rem" }}>
            <strong>Game id, </strong> {game.id}
          </div>
          <div style={{ marginBottom: "0.25rem" }}>
            <strong>Current player, </strong> {game.currentPlayer}
          </div>
          <div style={{ marginBottom: "0.75rem" }}>
            <strong>Status, </strong> {game.status}
          </div>

          <BoardView cells={game.cells} />
        </div>
      )}
    </div>
  );
}

function BoardView({ cells }) {
  if (!cells) return null;

  const size = cells.length;

  return (
    <div
      style={{
        display: "grid",
        gridTemplateColumns: `repeat(${size}, 40px)`,
        gridTemplateRows: `repeat(${size}, 40px)`,
        gap: "4px",
        background: "#1e293b",
        padding: "6px",
        borderRadius: "0.75rem",
      }}
    >
      {cells.map((row, r) =>
        row.map((cell, c) => {
          const text = cellDisplay(cell);
          const isP1 = cell && cell.owner === "Player1";
          const bg = cell
            ? isP1
              ? "#334155"
              : "#1d4ed8"
            : "#020617";

          return (
            <div
              key={`${r}-${c}`}
              style={{
                width: "40px",
                height: "40px",
                borderRadius: "0.5rem",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                fontSize: "1.25rem",
                border: "1px solid #1f2937",
                background: bg,
                boxShadow: cell
                  ? "0 0 6px rgba(15,23,42,0.6)"
                  : "0 0 2px rgba(15,23,42,0.4)",
              }}
            >
              {text}
            </div>
          );
        })
      )}
    </div>
  );
}

export default App;
