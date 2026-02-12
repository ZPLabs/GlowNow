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
import { rucValidationRule } from "@/lib/validation/ecuadorRuc";
import { ApiException } from "@/types/api";
import styles from "./RegisterForm.module.css";

const initialValues = {
  email: "",
  password: "",
  firstName: "",
  lastName: "",
  phoneNumber: "",
  businessName: "",
  businessRuc: "",
  businessAddress: "",
  businessPhoneNumber: "",
  businessEmail: "",
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
  businessName: [required("Business name is required")],
  businessRuc: [compose(required("RUC is required"), rucValidationRule())],
  businessAddress: [required("Business address is required")],
  businessPhoneNumber: [phone()],
  businessEmail: [email()],
};

export function RegisterForm() {
  const router = useRouter();
  const { register, login } = useAuth();
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const { getFieldProps, getFieldError, handleSubmit, isSubmitting } =
    useFormValidation({
      initialValues,
      rules: validationRules,
      onSubmit: async (formValues) => {
        setError(null);
        try {
          await register({
            email: formValues.email,
            password: formValues.password,
            firstName: formValues.firstName,
            lastName: formValues.lastName,
            phoneNumber: formValues.phoneNumber || undefined,
            businessName: formValues.businessName,
            businessRuc: formValues.businessRuc,
            businessAddress: formValues.businessAddress,
            businessPhoneNumber: formValues.businessPhoneNumber || undefined,
            businessEmail: formValues.businessEmail || undefined,
          });

          setSuccess(true);

          // Auto-login after registration
          await login({
            email: formValues.email,
            password: formValues.password,
          });

          router.push("/dashboard");
        } catch (err) {
          if (err instanceof ApiException) {
            setError(err.message);
          } else {
            setError("An unexpected error occurred. Please try again.");
          }
        }
      },
    });

  return (
    <form onSubmit={handleSubmit} className={styles.form}>
      <div className={styles.header}>
        <h1 className={styles.title}>Create your account</h1>
        <p className={styles.subtitle}>
          Register your business on GlowNow
        </p>
      </div>

      {error && <Alert variant="error">{error}</Alert>}
      {success && (
        <Alert variant="success">
          Account created successfully! Redirecting to dashboard...
        </Alert>
      )}

      <div className={styles.section}>
        <h2 className={styles.sectionTitle}>Account Information</h2>
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

      <div className={styles.section}>
        <h2 className={styles.sectionTitle}>Business Information</h2>
        <div className={styles.fields}>
          <FormField
            label="Business Name"
            type="text"
            placeholder="Your Business Name"
            errorMessage={getFieldError("businessName")}
            {...getFieldProps("businessName")}
          />

          <FormField
            label="RUC"
            type="text"
            placeholder="1234567890001"
            hint="13-digit tax identification number"
            errorMessage={getFieldError("businessRuc")}
            {...getFieldProps("businessRuc")}
          />

          <FormField
            label="Business Address"
            type="text"
            placeholder="Av. Example 123, Cuenca"
            errorMessage={getFieldError("businessAddress")}
            {...getFieldProps("businessAddress")}
          />

          <FormField
            label="Business Phone"
            type="tel"
            placeholder="+593 9X XXX XXXX"
            hint="Optional"
            errorMessage={getFieldError("businessPhoneNumber")}
            {...getFieldProps("businessPhoneNumber")}
          />

          <FormField
            label="Business Email"
            type="email"
            placeholder="contact@business.com"
            hint="Optional - defaults to your email"
            errorMessage={getFieldError("businessEmail")}
            {...getFieldProps("businessEmail")}
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
