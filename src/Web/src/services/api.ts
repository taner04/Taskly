import type { Todo } from "../types/todo";

const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5226";

export async function fetchTodos(
  getAccessToken: () => Promise<string>
): Promise<Todo[]> {
  try {
    const token = await getAccessToken();

    const response = await fetch(`${API_BASE_URL}/api/todos`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      credentials: "include",
    });

    if (!response.ok) {
      if (response.status === 401) {
        throw new Error("Unauthorized. Please log in again.");
      }
      throw new Error(`Failed to fetch todos: ${response.statusText}`);
    }

    const data = await response.json();
    return data.value || data; // Handle both ErrorOr format and direct array
  } catch (error) {
    console.error("Error fetching todos:", error);
    throw error;
  }
}
