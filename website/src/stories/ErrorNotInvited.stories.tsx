import React from "react";
import { Meta, Story } from "@storybook/react";
import { ErrorNotInvited } from "../components/errors/ErrorNotInvited";

export default {
  title: "Explorer/Errors/ErrorNotInvited",
  component: ErrorNotInvited,
} as Meta;

export const Template: Story = () => <ErrorNotInvited />;
