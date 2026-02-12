import type { ReactNode } from "react";
import styles from "./Alert.module.css";

export interface AlertProps {
  variant?: "error" | "success" | "info" | "warning";
  children: ReactNode;
  className?: string;
}

export function Alert({ variant = "info", children, className }: AlertProps) {
  const classNames = [styles.alert, styles[variant], className]
    .filter(Boolean)
    .join(" ");

  return (
    <div className={classNames} role="alert">
      {children}
    </div>
  );
}
