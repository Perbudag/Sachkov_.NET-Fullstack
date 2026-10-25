import { ApiError } from "./error.type";

export type Envelope<T = unknown> = {
  result: T | null;
  errors: ApiError[] | null;
  timeGenerated: Date;
};
