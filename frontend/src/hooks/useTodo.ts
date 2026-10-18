import { ToDo } from "@/types/todo.type";
import React from "react";

export const useTodo = (todos: Array<ToDo>) => {
  const [todoList, setTodoList] = React.useState<Array<ToDo>>(todos);
  const [uncompletedTasks, setUncompletedTasks] = React.useState<number>(0);

  React.useEffect(() => {
    setUncompletedTasks(
      todoList.filter((todo) => todo.isCompleted === false).length,
    );
  }, [todoList]);

  return [todoList, setTodoList, uncompletedTasks] as const;
};
