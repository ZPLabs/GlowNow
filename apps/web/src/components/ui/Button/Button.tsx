import { forwardRef, type ButtonHTMLAttributes } from "react";
import styles from "./Button.module.css";

export interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: "primary" | "secondary";
  isLoading?: boolean;
}

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  (
    { className, variant = "primary", isLoading, disabled, children, ...props },
    ref
  ) => {
    const classNames = [
      styles.button,
      styles[variant],
      isLoading && styles.loading,
      className,
    ]
      .filter(Boolean)
      .join(" ");

    return (
      <button
        ref={ref}
        className={classNames}
        disabled={disabled || isLoading}
        {...props}
      >
        {isLoading ? <span className={styles.spinner} /> : children}
      </button>
    );
  }
);

Button.displayName = "Button";
