const API_BASE = "http://localhost:5000/api";

export async function createGame() {
  console.log("Calling, ", `${API_BASE}/games`);

  const res = await fetch(`${API_BASE}/games`, {
    method: "POST"
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(`CreateGame failed, ${res.status} ${text}`);
  }

  return await res.json();
}
