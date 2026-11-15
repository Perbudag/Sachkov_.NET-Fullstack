import { locationsApi } from "@/entities/location";
import { isEnvelopeError } from "@/shared/api/axios-instance";
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
    onError: (error) => {
      if (isEnvelopeError(error)) {
        error.apiErrors.map((err) => {
          toast.error(err.message);
        });
      } else {
        toast.error(error.message);
      }
    },
  });

  return {
    createLocation: mutation.mutate,
    error: mutation.error,
    isPending: mutation.isPending,
  };
}
