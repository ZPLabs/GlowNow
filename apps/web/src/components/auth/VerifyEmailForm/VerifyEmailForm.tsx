"use client";

import { useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { FormField } from "@/components/ui/FormField";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { useFormValidation } from "@/hooks/useFormValidation";
import { useAuth } from "@/hooks/useAuth";
import { required, minLength, compose } from "@/lib/validation/rules";
import styles from "./VerifyEmailForm.module.css";

const validationRules = {
  code: [
    compose(
      required("Verification code is required"),
      minLength(6, "Code must be 6 characters")
    ),
  ],
};

export function VerifyEmailForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const email = searchParams.get("email") || "";
  const { confirmSignUpCode } = useAuth();
  const [error, setError] = useState<string | null>(null);

  const { getFieldProps, getFieldError, handleSubmit, isSubmitting } =
    useFormValidation({
      initialValues: { code: "" },
      rules: validationRules,
      onSubmit: async (values) => {
        setError(null);
        try {
          await confirmSignUpCode(email, values.code);
          router.push("/login?verified=true");
        } catch (err: any) {
          setError(err.message || "Invalid verification code. Please try again.");
        }
      },
    });

  if (!email) {
    return (
      <Alert variant="error">
        Missing email address. Please go back to the registration page.
      </Alert>
    );
  }

  return (
    <form onSubmit={handleSubmit} className={styles.form}>
      <div className={styles.header}>
        <h1 className={styles.title}>Verify your email</h1>
        <p className={styles.subtitle}>
          We sent a verification code to <strong>{email}</strong>
        </p>
      </div>

      {error && <Alert variant="error">{error}</Alert>}

      <div className={styles.fields}>
        <FormField
          label="Verification Code"
          type="text"
          placeholder="123456"
          errorMessage={getFieldError("code")}
          {...getFieldProps("code")}
        />
      </div>

      <Button
        type="submit"
        isLoading={isSubmitting}
        className={styles.submitButton}
      >
        Verify Email
      </Button>

      <p className={styles.footer}>
        Didn&apos;t receive a code?{" "}
        <button type="button" className={styles.resendButton}>
          Resend code
        </button>
      </p>
    </form>
  );
}
