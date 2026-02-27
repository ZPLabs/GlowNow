"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { FormField } from "@/components/ui/FormField";
import { Button } from "@/components/ui/Button";
import { Alert } from "@/components/ui/Alert";
import { useFormValidation } from "@/hooks/useFormValidation";
import { useAuth } from "@/hooks/useAuth";
import { required, email, compose, phone } from "@/lib/validation/rules";
import { rucValidationRule } from "@/lib/validation/ecuadorRuc";
import styles from "./BusinessRegistrationForm.module.css";

const initialValues = {
  businessName: "",
  businessRuc: "",
  businessAddress: "",
  businessPhoneNumber: "",
  businessEmail: "",
};

const validationRules = {
  businessName: [required("Business name is required")],
  businessRuc: [compose(required("RUC is required"), rucValidationRule())],
  businessAddress: [required("Business address is required")],
  businessPhoneNumber: [phone()],
  businessEmail: [email()],
};

export function BusinessRegistrationForm() {
  const router = useRouter();
  const { registerBusiness } = useAuth();
  const [error, setError] = useState<string | null>(null);

  const { getFieldProps, getFieldError, handleSubmit, isSubmitting } =
    useFormValidation({
      initialValues,
      rules: validationRules,
      onSubmit: async (values) => {
        setError(null);
        try {
          await registerBusiness({
            businessName: values.businessName,
            businessRuc: values.businessRuc,
            businessAddress: values.businessAddress,
            businessPhoneNumber: values.businessPhoneNumber || undefined,
            businessEmail: values.businessEmail || undefined,
          });
          router.push("/dashboard");
        } catch (err: any) {
          setError(err.message || "Failed to register business. Please try again.");
        }
      },
    });

  return (
    <form onSubmit={handleSubmit} className={styles.form}>
      <div className={styles.header}>
        <h1 className={styles.title}>Register your business</h1>
        <p className={styles.subtitle}>
          One last step to set up your GlowNow account
        </p>
      </div>

      {error && <Alert variant="error">{error}</Alert>}

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

      <Button
        type="submit"
        isLoading={isSubmitting}
        className={styles.submitButton}
      >
        Complete Registration
      </Button>
    </form>
  );
}
