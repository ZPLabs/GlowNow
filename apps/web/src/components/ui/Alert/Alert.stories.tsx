import type { Meta, StoryObj } from "@storybook/react";
import { Alert } from "./Alert";

const meta: Meta<typeof Alert> = {
  title: "UI/Alert",
  component: Alert,
  parameters: {
    layout: "centered",
  },
  tags: ["autodocs"],
  argTypes: {
    variant: {
      control: "select",
      options: ["error", "success", "info", "warning"],
    },
  },
  decorators: [
    (Story) => (
      <div style={{ width: "400px" }}>
        <Story />
      </div>
    ),
  ],
};

export default meta;
type Story = StoryObj<typeof meta>;

export const Error: Story = {
  args: {
    variant: "error",
    children: "Invalid email or password. Please try again.",
  },
};

export const Success: Story = {
  args: {
    variant: "success",
    children: "Account created successfully! Redirecting to dashboard...",
  },
};

export const Info: Story = {
  args: {
    variant: "info",
    children: "Your session will expire in 5 minutes.",
  },
};

export const Warning: Story = {
  args: {
    variant: "warning",
    children: "Please verify your email address to access all features.",
  },
};

export const LongContent: Story = {
  args: {
    variant: "error",
    children:
      "We encountered an issue while processing your request. Please check your connection and try again. If the problem persists, contact support.",
  },
};
