import { forwardRef, type InputHTMLAttributes } from "react";
import styles from "./Input.module.css";

export interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  error?: boolean;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ className, error, ...props }, ref) => {
    const classNames = [styles.input, error && styles.error, className]
      .filter(Boolean)
      .join(" ");

    return <input ref={ref} className={classNames} {...props} />;
  }
);

Input.displayName = "Input";
