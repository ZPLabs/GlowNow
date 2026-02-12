import { RegisterForm } from "@/components/auth/RegisterForm";
import styles from "./page.module.css";

export const metadata = {
  title: "Create Account - GlowNow",
  description: "Register your business on GlowNow",
};

export default function RegisterPage() {
  return (
    <div className={styles.container}>
      <RegisterForm />
    </div>
  );
}
