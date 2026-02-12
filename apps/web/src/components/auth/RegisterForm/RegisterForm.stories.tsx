import type { Meta, StoryObj } from "@storybook/react";
import { RegisterForm } from "./RegisterForm";

const meta: Meta<typeof RegisterForm> = {
  title: "Auth/RegisterForm",
  component: RegisterForm,
  parameters: {
    layout: "centered",
    nextjs: {
      appDirectory: true,
      navigation: {
        pathname: "/register",
      },
    },
  },
  decorators: [
    (Story) => (
      <div style={{ padding: "2rem", maxHeight: "90vh", overflowY: "auto" }}>
        <Story />
      </div>
    ),
  ],
};

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {};
