"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormField } from "@/components/ui/FormField";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { useFormValidation } from "@/hooks/useFormValidation";
import { useAuth } from "@/hooks/useAuth";
import { required, email, compose } from "@/lib/validation/rules";
import styles from "./LoginForm.module.css";

const initialValues = {
  email: "",
  password: "",
};

const validationRules = {
  email: [compose(required("Email is required"), email())],
  password: [required("Password is required")],
};

export function LoginForm() {
  const router = useRouter();
  const { signInWithEmail, signInWithGoogle } = useAuth();
  const [error, setError] = useState<string | null>(null);

  const { getFieldProps, getFieldError, handleSubmit, isSubmitting } =
    useFormValidation({
      initialValues,
      rules: validationRules,
      onSubmit: async (values) => {
        setError(null);
        try {
          await signInWithEmail(values.email, values.password);
          router.push("/dashboard");
        } catch (err: any) {
          if (err.name === 'UserNotConfirmedException') {
            router.push(`/verify-email?email=${encodeURIComponent(values.email)}`);
          } else {
            setError(err.message || "An unexpected error occurred. Please try again.");
          }
        }
      },
    });

  return (
    <form onSubmit={handleSubmit} className={styles.form}>
      <div className={styles.header}>
        <h1 className={styles.title}>Welcome back</h1>
        <p className={styles.subtitle}>Sign in to your GlowNow account</p>
      </div>

      {error && <Alert variant="error">{error}</Alert>}

      <div className={styles.fields}>
        <FormField
          label="Email"
          type="email"
          placeholder="you@example.com"
          autoComplete="email"
          errorMessage={getFieldError("email")}
          {...getFieldProps("email")}
        />

        <FormField
          label="Password"
          type="password"
          placeholder="Enter your password"
          autoComplete="current-password"
          errorMessage={getFieldError("password")}
          {...getFieldProps("password")}
        />
      </div>

      <Button type="submit" isLoading={isSubmitting} className={styles.submitButton}>
        Sign in
      </Button>

      <div className={styles.divider}>
        <span>or</span>
      </div>

      <Button
        type="button"
        variant="secondary"
        onClick={async () => {
          setError(null);
          try {
            await signInWithGoogle();
            // Only reached if user was already signed in (signInWithRedirect
            // normally navigates the browser away before this line executes)
            router.push("/dashboard");
          } catch (err: unknown) {
            setError(err instanceof Error ? err.message : "Failed to sign in with Google.");
          }
        }}
        className={styles.googleButton}
      >
        Sign in with Google
      </Button>

      <p className={styles.footer}>
        Don&apos;t have an account?{" "}
        <Link href="/register" className={styles.link}>
          Create one
        </Link>
      </p>
    </form>
  );
}
