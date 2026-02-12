export type ValidationRule<T> = (value: T) => string | undefined;

export function required(message = "This field is required"): ValidationRule<string> {
  return (value) => {
    if (!value || value.trim() === "") {
      return message;
    }
    return undefined;
  };
}

export function email(message = "Please enter a valid email"): ValidationRule<string> {
  return (value) => {
    if (!value) return undefined;
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(value)) {
      return message;
    }
    return undefined;
  };
}

export function minLength(
  length: number,
  message?: string
): ValidationRule<string> {
  return (value) => {
    if (!value) return undefined;
    if (value.length < length) {
      return message ?? `Must be at least ${length} characters`;
    }
    return undefined;
  };
}

export function maxLength(
  length: number,
  message?: string
): ValidationRule<string> {
  return (value) => {
    if (!value) return undefined;
    if (value.length > length) {
      return message ?? `Must be at most ${length} characters`;
    }
    return undefined;
  };
}

export function pattern(
  regex: RegExp,
  message: string
): ValidationRule<string> {
  return (value) => {
    if (!value) return undefined;
    if (!regex.test(value)) {
      return message;
    }
    return undefined;
  };
}

export function phone(
  message = "Please enter a valid phone number"
): ValidationRule<string> {
  return (value) => {
    if (!value) return undefined;
    // Ecuador phone: +593 9X XXX XXXX or 09X XXX XXXX
    const phoneRegex = /^(\+593\s?)?0?9\d{8}$/;
    const cleanValue = value.replace(/[\s-]/g, "");
    if (!phoneRegex.test(cleanValue)) {
      return message;
    }
    return undefined;
  };
}

export function compose<T>(
  ...rules: ValidationRule<T>[]
): ValidationRule<T> {
  return (value) => {
    for (const rule of rules) {
      const error = rule(value);
      if (error) {
        return error;
      }
    }
    return undefined;
  };
}
