import { Spinner } from "@/shared/components/ui/spinner";

type QueryStateProps = {
  isLoading: boolean;
  error: Error | null;
  isEmpty: boolean;
};

export function QueryState({ isLoading, error, isEmpty }: QueryStateProps) {
  if (!isLoading && !error && !isEmpty) {
    return null;
  }

  return (
    <div className="flex h-50 flex-col items-center justify-center">
      <div className="flex flex-row items-center justify-center text-2xl font-semibold">
        {isLoading ? (
          <Spinner className="size-10" />
        ) : error ? (
          <h1>{error.message}</h1>
        ) : (
          isEmpty && <>Список пуст</>
        )}
      </div>
    </div>
  );
}
