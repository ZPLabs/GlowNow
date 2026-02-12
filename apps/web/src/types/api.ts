export interface ApiResponse<T> {
  data: T;
  meta?: {
    timestamp: string;
    requestId: string;
  };
}

export interface ApiErrorDetail {
  field?: string;
  message: string;
}

export interface ApiError {
  error: {
    code: string;
    message: string;
    details?: ApiErrorDetail[];
  };
}

export class ApiException extends Error {
  public readonly code: string;
  public readonly details?: ApiErrorDetail[];
  public readonly status: number;

  constructor(
    message: string,
    code: string,
    status: number,
    details?: ApiErrorDetail[]
  ) {
    super(message);
    this.name = "ApiException";
    this.code = code;
    this.status = status;
    this.details = details;
  }
}
