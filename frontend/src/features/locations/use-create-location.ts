import { locationsApi } from "@/entities/locations";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

export function useCreateLocation() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: locationsApi.create,
    onSettled: async () => {
      await queryClient.invalidateQueries({ queryKey: ["LocationItems"] });
    },
    onSuccess: () => {
      toast.success("Локация создана");
    },
  });

  return {
    createLocation: mutation.mutate,
    error: mutation.error,
    isPending: mutation.isPending,
  };
}
