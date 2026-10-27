import { ErrorType } from "./error-type.type";

export type ApiError = {
  code: string;
  message: string;
  type: ErrorType;
  invalidField?: string | null;
};
