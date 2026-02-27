"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormField } from "@/components/ui/FormField";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { useFormValidation } from "@/hooks/useFormValidation";
import { useAuth } from "@/hooks/useAuth";
import { required, email, minLength, compose, phone } from "@/lib/validation/rules";
import styles from "./RegisterForm.module.css";

const initialValues = {
  email: "",
  password: "",
  firstName: "",
  lastName: "",
  phoneNumber: "",
};

const validationRules = {
  email: [compose(required("Email is required"), email())],
  password: [
    compose(
      required("Password is required"),
      minLength(8, "Password must be at least 8 characters")
    ),
  ],
  firstName: [required("First name is required")],
  lastName: [required("Last name is required")],
  phoneNumber: [phone()],
};

export function RegisterForm() {
  const router = useRouter();
  const { signUpWithEmail } = useAuth();
  const [error, setError] = useState<string | null>(null);

  const { getFieldProps, getFieldError, handleSubmit, isSubmitting } =
    useFormValidation({
      initialValues,
      rules: validationRules,
      onSubmit: async (formValues) => {
        setError(null);
        try {
          await signUpWithEmail(
            formValues.email,
            formValues.password,
            formValues.firstName,
            formValues.lastName,
            formValues.phoneNumber || undefined
          );

          router.push(`/verify-email?email=${encodeURIComponent(formValues.email)}`);
        } catch (err: any) {
          setError(err.message || "An unexpected error occurred. Please try again.");
        }
      },
    });

  return (
    <form onSubmit={handleSubmit} className={styles.form}>
      <div className={styles.header}>
        <h1 className={styles.title}>Create your account</h1>
        <p className={styles.subtitle}>
          Join the GlowNow community
        </p>
      </div>

      {error && <Alert variant="error">{error}</Alert>}

      <div className={styles.section}>
        <div className={styles.fields}>
          <div className={styles.row}>
            <FormField
              label="First Name"
              type="text"
              placeholder="John"
              autoComplete="given-name"
              errorMessage={getFieldError("firstName")}
              {...getFieldProps("firstName")}
            />

            <FormField
              label="Last Name"
              type="text"
              placeholder="Doe"
              autoComplete="family-name"
              errorMessage={getFieldError("lastName")}
              {...getFieldProps("lastName")}
            />
          </div>

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
            placeholder="At least 8 characters"
            autoComplete="new-password"
            errorMessage={getFieldError("password")}
            {...getFieldProps("password")}
          />

          <FormField
            label="Phone Number"
            type="tel"
            placeholder="+593 9X XXX XXXX"
            autoComplete="tel"
            hint="Optional"
            errorMessage={getFieldError("phoneNumber")}
            {...getFieldProps("phoneNumber")}
          />
        </div>
      </div>

      <Button
        type="submit"
        isLoading={isSubmitting}
        className={styles.submitButton}
      >
        Create Account
      </Button>

      <p className={styles.footer}>
        Already have an account?{" "}
        <Link href="/login" className={styles.link}>
          Sign in
        </Link>
      </p>
    </form>
  );
}
