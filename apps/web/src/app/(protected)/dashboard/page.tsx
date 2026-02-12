"use client";

import { useAuth } from "@/hooks/useAuth";
import styles from "./page.module.css";

export default function DashboardPage() {
  const { user } = useAuth();

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Welcome, {user?.firstName}!</h1>
      <p className={styles.subtitle}>
        You are now signed in to GlowNow. This is your dashboard.
      </p>

      {user?.memberships && user.memberships.length > 0 && (
        <div className={styles.memberships}>
          <h2 className={styles.sectionTitle}>Your Businesses</h2>
          <ul className={styles.businessList}>
            {user.memberships.map((membership) => (
              <li key={membership.businessId} className={styles.businessItem}>
                <span className={styles.businessName}>
                  {membership.businessName}
                </span>
                <span className={styles.businessRole}>{membership.role}</span>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
