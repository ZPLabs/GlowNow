import { LoginForm } from "@/components/auth/LoginForm";
import styles from "./page.module.css";

export const metadata = {
  title: "Sign In - GlowNow",
  description: "Sign in to your GlowNow account",
};

export default function LoginPage() {
  return (
    <div className={styles.container}>
      <LoginForm />
    </div>
  );
}
