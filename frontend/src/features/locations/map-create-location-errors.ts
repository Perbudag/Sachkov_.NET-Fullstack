import type { UseFormSetError } from "react-hook-form";
import type { ApiError } from "@/shared/types/error.type";
import type { CreateLocationFormValues } from "./create-location-schema";

const fieldByInvalidField: Record<string, keyof CreateLocationFormValues | `address.${keyof CreateLocationFormValues["address"]}`> = {
  name: "name",
  postalcode: "address.postalCode",
  country: "address.country",
  region: "address.region",
  city: "address.city",
  street: "address.street",
  house: "address.house",
  apartment: "address.apartment",
};

function normalizeInvalidField(invalidField: string) {
  return invalidField.replace(/[^a-z]/gi, "").toLowerCase().replace(/^address/, "");
}

export function mapCreateLocationErrorsToForm(
  apiErrors: ApiError[],
  setError: UseFormSetError<CreateLocationFormValues>,
) {
  const unhandledErrors: ApiError[] = [];

  for (const apiError of apiErrors) {
    const formField = apiError.invalidField
      ? fieldByInvalidField[normalizeInvalidField(apiError.invalidField)]
      : undefined;

    if (formField) {
      setError(formField, { type: "server", message: apiError.message });
    } else {
      unhandledErrors.push(apiError);
    }
  }

  return unhandledErrors;
}
