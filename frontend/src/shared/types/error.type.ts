import { ErrorType } from "./error-type.type";

export type Error = {
  code: string;
  message: string;
  type: ErrorType;
  invalidField: string | null;
};
