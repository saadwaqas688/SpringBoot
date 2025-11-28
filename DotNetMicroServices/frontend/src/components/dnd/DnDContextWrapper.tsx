"use client";

import React from "react";
import { DndContext, DndContextProps } from "@dnd-kit/core";

export interface DnDContextWrapperProps
  extends Omit<DndContextProps, "children"> {
  children: React.ReactNode;
}

const DnDContextWrapper = ({
  children,
  ...dndProps
}: DnDContextWrapperProps) => {
  return <DndContext {...dndProps}>{children}</DndContext>;
};

export { DnDContextWrapper };
export default DnDContextWrapper;
