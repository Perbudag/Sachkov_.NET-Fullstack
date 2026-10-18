"use client";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Input } from "@/components/ui/input";
import React from "react";
import { Trash2 } from "lucide-react";
import { useTodo } from "@/hooks/useTodo";
import { ToDo } from "@/types/todo.type";

export default function Home() {
  const [todoList, setTodoList, uncompletedTasks] = useTodo([]);
  const [titleText, setTitleText] = React.useState("");
  const [contentText, setContentText] = React.useState("");

  const addTodo = () => {
    if (!titleText.trim()) return;

    const newTodo: ToDo = {
      id: crypto.randomUUID(),
      title: titleText,
      content: contentText,
      isCompleted: false,
    };

    setTodoList([...todoList, newTodo]);
    setTitleText("");
    setContentText("");
  };

  const toggleTodo = (id: string) => {
    setTodoList(
      todoList.map((todo) =>
        todo.id === id ? { ...todo, isCompleted: !todo.isCompleted } : todo,
      ),
    );
  };

  const deleteTodo = (id: string) => {
    setTodoList(todoList.filter((todo) => todo.id !== id));
  };

  return (
    <div className="flex flex-col flex-1 items-center justify-center bg-zinc-50 font-sans min-h-screen">
      <main className="flex flex-1 w-full max-w-md flex-col gap-8 py-8 px-8 bg-white sm:items-center shadow-sm">
        <h1 className="text-2xl font-bold tracking-tight">
          Невыполненных задач: {uncompletedTasks}
        </h1>
        <div className="flex flex-col gap-2 w-full">
          <Input
            type="text"
            value={titleText}
            onChange={(e) => setTitleText(e.target.value)}
            placeholder="Заголовок"
          />
          <Input
            type="text"
            value={contentText}
            onChange={(e) => setContentText(e.target.value)}
            placeholder="Описание"
          />
          <Button onClick={addTodo}>Добавить</Button>
        </div>

        <div className="flex flex-col w-full gap-3">
          {todoList.map((todo) => (
            <Card
              key={todo.id}
              className={`transition-all duration-200 ${
                todo.isCompleted ? "opacity-60 bg-zinc-50" : "bg-transparent"
              }`}
            >
              <CardHeader className="flex flex-row items-start justify-between space-y-0 pb-2">
                <div className="space-y-1 pr-4 flex-1">
                  <CardTitle
                    className={`text-lg font-medium break-all ${
                      todo.isCompleted
                        ? "line-through text-muted-foreground"
                        : ""
                    }`}
                  >
                    {todo.title}
                  </CardTitle>
                </div>

                <div className="flex items-center gap-2">
                  <Checkbox
                    checked={todo.isCompleted}
                    onCheckedChange={() => toggleTodo(todo.id)}
                    className="h-5 w-5 data-[state=checked]:bg-primary"
                  />

                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => deleteTodo(todo.id)}
                    className="h-8 w-8 text-muted-foreground hover:text-destructive hover:bg-destructive/10 transition-colors"
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <p
                  className={`text-sm text-muted-foreground break-all ${
                    todo.isCompleted ? "line-through" : ""
                  }`}
                >
                  {todo.content}
                </p>
              </CardContent>
            </Card>
          ))}
        </div>
      </main>
    </div>
  );
}
