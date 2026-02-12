import type { ReactNode } from "react";
import { Input, type InputProps } from "../Input";
import styles from "./FormField.module.css";

export interface FormFieldProps extends InputProps {
  label: string;
  errorMessage?: string;
  hint?: string;
  children?: ReactNode;
}

export function FormField({
  label,
  errorMessage,
  hint,
  id,
  children,
  ...inputProps
}: FormFieldProps) {
  const inputId = id ?? label.toLowerCase().replace(/\s+/g, "-");
  const hasError = !!errorMessage;

  return (
    <div className={styles.field}>
      <label htmlFor={inputId} className={styles.label}>
        {label}
      </label>
      {children ?? <Input id={inputId} error={hasError} {...inputProps} />}
      {hint && !hasError && <p className={styles.hint}>{hint}</p>}
      {hasError && <p className={styles.error}>{errorMessage}</p>}
    </div>
  );
}
