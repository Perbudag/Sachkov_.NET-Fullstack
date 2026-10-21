import { Error } from "./error.type";

export type Envelope<T> = {
  result: T | null;
  errors: Error[] | null;
  timeGenerated: Date;
};
