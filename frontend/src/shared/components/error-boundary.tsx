"use client";

import { Component, type ErrorInfo, type ReactNode } from "react";
import { Button } from "@/shared/components/ui/button";

type Props = {
  children: ReactNode;
};

type State = {
  hasError: boolean;
};

export class ErrorBoundary extends Component<Props, State> {
  state: State = {
    hasError: false,
  };

  static getDerivedStateFromError(): State {
    return { hasError: true };
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error("ErrorBoundary caught an error:", error, errorInfo);
  }

  private reset = () => {
    this.setState({ hasError: false });
  };

  render() {
    if (this.state.hasError) {
      return (
        <div
          className="flex min-h-64 flex-col items-center justify-center gap-4 p-6 text-center"
          role="alert"
        >
          <div>
            <h2 className="text-lg font-semibold">Произошла ошибка</h2>
            <p className="text-muted-foreground">
              Не удалось отобразить страницу.
            </p>
          </div>
          <Button onClick={this.reset}>Попробовать снова</Button>
        </div>
      );
    }

    return this.props.children;
  }
}
