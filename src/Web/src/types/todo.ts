export const TodoPriority = {
  Low: 0,
  Normal: 1,
  High: 2,
} as const;

export type TodoPriority = (typeof TodoPriority)[keyof typeof TodoPriority];

export interface Todo {
  id: string;
  title: string;
  description: string | null;
  priority: TodoPriority;
  isCompleted: boolean;
  userId: string;
}
