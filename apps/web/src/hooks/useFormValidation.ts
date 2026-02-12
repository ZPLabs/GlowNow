"use client";

import { useState, useCallback, type ChangeEvent } from "react";
import type { ValidationRule } from "@/lib/validation/rules";

type FieldRules<T extends Record<string, string>> = {
  [K in keyof T]?: ValidationRule<string>[];
};

type FormErrors<T extends Record<string, string>> = {
  [K in keyof T]?: string;
};

type FormTouched<T extends Record<string, string>> = {
  [K in keyof T]?: boolean;
};

interface UseFormValidationOptions<T extends Record<string, string>> {
  initialValues: T;
  rules: FieldRules<T>;
  onSubmit?: (values: T) => void | Promise<void>;
}

export function useFormValidation<T extends Record<string, string>>({
  initialValues,
  rules,
  onSubmit,
}: UseFormValidationOptions<T>) {
  const [values, setValues] = useState<T>(initialValues);
  const [errors, setErrors] = useState<FormErrors<T>>({});
  const [touched, setTouched] = useState<FormTouched<T>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  const validateField = useCallback(
    (name: keyof T, value: string): string | undefined => {
      const fieldRules = rules[name];
      if (!fieldRules) return undefined;

      for (const rule of fieldRules) {
        const error = rule(value);
        if (error) return error;
      }
      return undefined;
    },
    [rules]
  );

  const validateAll = useCallback((): boolean => {
    const newErrors: FormErrors<T> = {};
    let isValid = true;

    for (const key of Object.keys(values) as (keyof T)[]) {
      const value = values[key] ?? "";
      const error = validateField(key, value);
      if (error) {
        newErrors[key] = error;
        isValid = false;
      }
    }

    setErrors(newErrors);
    return isValid;
  }, [values, validateField]);

  const handleChange = useCallback(
    (e: ChangeEvent<HTMLInputElement>) => {
      const { name, value } = e.target;
      setValues((prev) => ({ ...prev, [name]: value }));

      // Clear error on change if field was touched
      if (touched[name as keyof T]) {
        const error = validateField(name as keyof T, value);
        setErrors((prev) => ({ ...prev, [name]: error }));
      }
    },
    [touched, validateField]
  );

  const handleBlur = useCallback(
    (e: ChangeEvent<HTMLInputElement>) => {
      const { name, value } = e.target;
      setTouched((prev) => ({ ...prev, [name]: true }));

      const error = validateField(name as keyof T, value);
      setErrors((prev) => ({ ...prev, [name]: error }));
    },
    [validateField]
  );

  const handleSubmit = useCallback(
    async (e: React.FormEvent) => {
      e.preventDefault();

      // Mark all fields as touched
      const allTouched: FormTouched<T> = {};
      for (const key of Object.keys(values) as (keyof T)[]) {
        allTouched[key] = true;
      }
      setTouched(allTouched);

      if (!validateAll()) {
        return;
      }

      if (onSubmit) {
        setIsSubmitting(true);
        try {
          await onSubmit(values);
        } finally {
          setIsSubmitting(false);
        }
      }
    },
    [values, validateAll, onSubmit]
  );

  const setFieldValue = useCallback((name: keyof T, value: string) => {
    setValues((prev) => ({ ...prev, [name]: value }));
  }, []);

  const setFieldError = useCallback((name: keyof T, error: string | undefined) => {
    setErrors((prev) => ({ ...prev, [name]: error }));
  }, []);

  const resetForm = useCallback(() => {
    setValues(initialValues);
    setErrors({});
    setTouched({});
    setIsSubmitting(false);
  }, [initialValues]);

  const getFieldProps = useCallback(
    (name: keyof T) => ({
      name: name as string,
      value: values[name] ?? "",
      onChange: handleChange,
      onBlur: handleBlur,
    }),
    [values, handleChange, handleBlur]
  );

  const getFieldError = useCallback(
    (name: keyof T) => (touched[name] ? errors[name] : undefined),
    [errors, touched]
  );

  return {
    values,
    errors,
    touched,
    isSubmitting,
    handleChange,
    handleBlur,
    handleSubmit,
    validateAll,
    setFieldValue,
    setFieldError,
    resetForm,
    getFieldProps,
    getFieldError,
  };
}
