import type { Meta, StoryObj } from "@storybook/react";
import { FormField } from "./FormField";

const meta: Meta<typeof FormField> = {
  title: "UI/FormField",
  component: FormField,
  parameters: {
    layout: "centered",
  },
  tags: ["autodocs"],
  decorators: [
    (Story) => (
      <div style={{ width: "320px" }}>
        <Story />
      </div>
    ),
  ],
};

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    label: "Email",
    type: "email",
    placeholder: "you@example.com",
  },
};

export const WithHint: Story = {
  args: {
    label: "Phone Number",
    type: "tel",
    placeholder: "+593 9X XXX XXXX",
    hint: "Optional - Ecuador format",
  },
};

export const WithError: Story = {
  args: {
    label: "Email",
    type: "email",
    placeholder: "you@example.com",
    value: "invalid-email",
    errorMessage: "Please enter a valid email address",
    onChange: () => {},
  },
};

export const Password: Story = {
  args: {
    label: "Password",
    type: "password",
    placeholder: "At least 8 characters",
  },
};

export const Required: Story = {
  args: {
    label: "Business Name",
    type: "text",
    placeholder: "Your Business Name",
    required: true,
  },
};
