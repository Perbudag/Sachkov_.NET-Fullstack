import axios, { AxiosError } from "axios";
import { ApiError } from "../types/error.type";
import { Envelope } from "../types/envelope.type";
import { ErrorType } from "../types/error-type.type";

export const apiClient = axios.create({
  baseURL: "http://localhost:5047/api",
  headers: { "Content-Type": "application/json; charset=utf-8" },
});

apiClient.interceptors.response.use(
  (response) => {
    const envelope = response.data as Envelope;

    if (envelope && envelope.errors && envelope.errors.length > 0) {
      throw AxiosError.from({ response: response });
    }

    response.data = envelope.result;

    return response;
  },
  (error) => {
    if (axios.isAxiosError(error) && error.response?.data) {
      const envelope = error.response.data as Envelope;

      if (envelope && envelope.errors && envelope.errors.length > 0) {
        throw new EnvelopeError(envelope.errors);
      }
    }

    return Promise.reject(error);
  },
);

export class EnvelopeError extends Error {
  public readonly apiErrors: ApiError[];
  public readonly type: ErrorType;

  constructor(apiErrors: ApiError[]) {
    const firstMessage = apiErrors[0].message ?? "Неизвестная ошибка";

    super(firstMessage);

    this.name = "EnvelopeError";
    this.apiErrors = apiErrors;
    this.type = apiErrors[0].type;

    Object.setPrototypeOf(this, EnvelopeError.prototype);
  }
}

export function isEnvelopeError(error: unknown): error is EnvelopeError {
  return error instanceof EnvelopeError;
}
