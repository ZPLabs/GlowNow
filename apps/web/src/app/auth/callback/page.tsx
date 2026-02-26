"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { Hub } from "aws-amplify/utils";
import { getCurrentUser } from "aws-amplify/auth";

export default function AuthCallbackPage() {
  const router = useRouter();

  useEffect(() => {
    const unsubscribe = Hub.listen("auth", ({ payload }) => {
      if (payload.event === "signedIn") {
        router.push("/dashboard");
      } else if (payload.event === "signInWithRedirect_failure") {
        router.push("/login?error=google_auth_failed");
      }
    });

    // Handle case where Amplify already processed the callback before this component mounted
    getCurrentUser()
      .then(() => router.push("/dashboard"))
      .catch(() => {
        // Not signed in yet — wait for Hub signedIn event
      });

    return () => unsubscribe();
  }, [router]);

  return (
    <div style={{ display: "flex", justifyContent: "center", alignItems: "center", height: "100vh", flexDirection: "column", gap: "1rem" }}>
      <h2>Completing sign in...</h2>
      <p>You will be redirected automatically.</p>
    </div>
  );
}
